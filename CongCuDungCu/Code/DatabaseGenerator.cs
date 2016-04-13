using DevExpress.DemoData.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace CongCuDungCu.Code
{
    public static class DatabaseGenerator
    {
        public class TableData
        {
            public string ConnectionStringName;
            public string Name;
            public List<FieldData> Fields = new List<FieldData>();
            public int RecordCount;

            public string ConnectionString
            {
                get
                {
                    var sqlExpressString = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
                    return DbEngineDetector.PatchConnectionString(sqlExpressString);
                }
            }
        }

        public class FieldData
        {
            private static Random rnd = new Random();
            public delegate object ValueGeneratorDelegate(Random rnd);

            protected FieldData(string name)
            {
                Name = name;
            }
            public FieldData(string name, IList possibleValues)
                :
                this(name)
            {
                PossibleValues = possibleValues;
            }
            public FieldData(string name, ValueGeneratorDelegate valueGenerator)
                :
                this(name)
            {
                ValueGenerator = valueGenerator;
            }

            public string Name;
            public IList PossibleValues;
            public ValueGeneratorDelegate ValueGenerator;

            public object GenerateValue()
            {
                if (PossibleValues != null)
                {
                    return PossibleValues[rnd.Next(PossibleValues.Count - 1)];
                }
                if (ValueGenerator != null)
                {
                    return ValueGenerator(rnd);
                }
                return null;
            }
        }

        private static readonly Dictionary<string, object> _lockers = new Dictionary<string, object>();
        private static readonly object _lockersLock = new object();

        private static Dictionary<string, int> _recordCountTable = new Dictionary<string, int>();
        private static Dictionary<string, TableData> _tables = new Dictionary<string, TableData>();

        public static void RegisterTable(string key, TableData table)
        {
            _tables[key] = table;
        }

        public static TableData GetTable(string key)
        {
            return _tables[key];
        }

        public static bool IsReady(string tableKey)
        {
            return !IsDatabaseCreating(tableKey) && IsDatabaseCreated(tableKey);
        }

        public static bool TryCreateDatabase(string tableKey)
        {
            if (IsDatabaseCreating(tableKey))
            {
                return false;
            }
            if (!_lockers.ContainsKey(tableKey))
            {
                lock (_lockersLock)
                {
                    _lockers[tableKey] = new object();
                }
            }

            lock (_lockers[tableKey])
            {
                if (IsDatabaseCreated(tableKey))
                {
                    return true;
                }
                _recordCountTable.Add(tableKey, 0);
                try
                {
                    GenerateDatabase(tableKey);
                }
                finally
                {
                    _recordCountTable.Remove(tableKey);
                }
                return false;
            }
        }

        public static int GetCreatingDatabaseRecordCount(string tableKey)
        {
            if (_recordCountTable.ContainsKey(tableKey))
            {
                return _recordCountTable[tableKey];
            }
            return GetRecordCount(tableKey);
        }

        private static bool IsDatabaseCreated(string tableKey)
        {
            return GetRecordCount(tableKey) > 0;
        }

        private static bool IsDatabaseCreating(string tableKey)
        {
            return _recordCountTable.ContainsKey(tableKey);
        }

        private static void GenerateDatabase(string tableKey)
        {
            var table = _tables[tableKey];

            using (var connection = new SqlConnection(table.ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = null;

                for (var i = 0; i < table.RecordCount; i++)
                {
                    if (i == 1 || i % 1000 == 0)
                    {
                        CommitTransaction(transaction);
                        _recordCountTable[tableKey] = i;
                        transaction = connection.BeginTransaction();
                    }
                    using (var cmd = CreateInsertCommand(table.Name, table.Fields))
                    {
                        cmd.Connection = connection;
                        cmd.Transaction = transaction;
                        cmd.ExecuteNonQuery();
                    }
                }
                CommitTransaction(transaction);
            }
        }


        private static void CommitTransaction(SqlTransaction transaction)
        {
            if (transaction == null)
            {
                return;
            }
            try
            {
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private static SqlCommand CreateInsertCommand(string tableName, List<FieldData> fields)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("insert into [{0}] (", tableName);
            for (var i = 0; i < fields.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }
                builder.AppendFormat("[{0}]", fields[i].Name);
            }
            builder.Append(") values (");
            for (var i = 0; i < fields.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }
                builder.Append("@p" + i);
            }
            builder.Append(")");
            var result = new SqlCommand(builder.ToString());
            for (var i = 0; i < fields.Count; i++)
            {
                result.Parameters.Add(new SqlParameter("@p" + i, fields[i].GenerateValue()));
            }
            return result;
        }

        private static int GetRecordCount(string tableKey)
        {
            try
            {
                var table = _tables[tableKey];
                using (var connection = new SqlConnection(table.ConnectionString))
                {
                    connection.Open();
                    var cmd = new SqlCommand("select count(*) from " + table.Name, connection);
                    return (int)cmd.ExecuteScalar();
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}
