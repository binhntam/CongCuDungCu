using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DevExpress.XtraReports.UI;

namespace CongCuDungCu.Models
{
    public class ReportsModel
    {
        public string ReportID { get; set; }
        public XtraReport Report { get; set; }
        public string parameterView { get; set; }
    }
    public static class DataSource
    {
        private const string CCDCDataContextKey = "CCDCDataContext";
        public static CCDCDataContext db
        {
            get
            {
                if (HttpContext.Current.Items[CCDCDataContextKey] == null)
                {
                    HttpContext.Current.Items[CCDCDataContextKey] = new CCDCDataContext();
                }
                return (CCDCDataContext)HttpContext.Current.Items[CCDCDataContextKey];
            }
        }
    }
    public static class ReportDataProviderHelper
    {
        private static string GetValidColumnName(string columnName, DataTable table)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                return string.Empty;
            }
            if (table.Columns.Contains(columnName))
            {
                return columnName;
            }
            var words = columnName.Split('_');
            if (words == null || words.Length == 0)
            {
                return string.Empty;
            }
            var name = DevExpress.XtraPrinting.StringUtils.Join(" ", words);
            if (table.Columns.Contains(name))
            {
                return name;
            }
            return string.Empty;
        }

        public static void CopyToDataTable<T>(this IEnumerable<T> query, DataTable table)
        {
            if (query == null)
            {
                return;
            }
            var properties = (PropertyInfo[] )null;

            foreach (T item in query)
            {
                if (properties == null)
                {
                    properties = ((Type)item.GetType()).GetProperties();
                }
                var row = table.NewRow();

                foreach (PropertyInfo property in properties)
                {
                    var columnName = GetValidColumnName(property.Name, table);
                    if (string.IsNullOrEmpty(columnName))
                    {
                        continue;
                    }
                    var propertyValue = property.GetValue(item, null);
                    if (propertyValue is System.Data.Linq.Binary)
                    {
                        propertyValue = ((System.Data.Linq.Binary)propertyValue).ToArray();
                    }
                    row[columnName] = propertyValue ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }
            table.AcceptChanges();
        }

        public static ReportsModel CreateModel(string reportID)
        {
            return new ReportsModel()
            {
                ReportID = reportID,
                };
        }

        public static ReportsModel CreateModel(string reportID, Dictionary<string, string> parameter)
        {
            var model = CreateModel(reportID);

            return model;
        }
        /**/
    }
    public class ParameterDictionaryBinder : DefaultModelBinder
    {
        private static IEnumerable<string> GetValueProviderKeys1(ControllerContext context)
        {
            return context.HttpContext.Request.Params.AllKeys;
        }
        private static object ConvertType1(string stringValue, Type type)
        {
            return TypeDescriptor.GetConverter(type).ConvertFrom(stringValue);
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var modelType = bindingContext.ModelType;
            var idictType = modelType.GetInterface("System.Collections.Generic.IDictionary`2");

            if (idictType != null)
            {
                var argumetTypes = idictType.GetGenericArguments();

                object result = null;
                var valueBinder = Binders.GetBinder(argumetTypes[1]);

                foreach (string key in GetValueProviderKeys1(controllerContext))
                {
                    if (!key.StartsWith(bindingContext.ModelName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }
                    object dictKey;
                    var parameterName = key.Substring(bindingContext.ModelName.Length);
                    try
                    {
                        dictKey = ConvertType1(parameterName, argumetTypes[0]);
                    }
                    catch (NotSupportedException)
                    {
                        continue;
                    }

                    var innerBindingContext = new ModelBindingContext()
                    { ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, argumetTypes[1]),
                        ModelName = key,
                        ModelState = bindingContext.ModelState,
                        PropertyFilter = bindingContext.PropertyFilter,
                        ValueProvider = bindingContext.ValueProvider
                    };
                    var newPropertyValue = valueBinder.BindModel(controllerContext, innerBindingContext);

                    if (result == null)
                    {
                        result = CreateModel(controllerContext, bindingContext, modelType);
                    }
                    if (!(bool)idictType.GetMethod("ContainsKey").Invoke(result, new object[] { dictKey }))
                    {
                        idictType.GetProperty("Item").SetValue(result, newPropertyValue, new object[] { dictKey });
                    }
                }
                return result;
            }
            return new DefaultModelBinder().BindModel(controllerContext, bindingContext);
        }
    }
}
