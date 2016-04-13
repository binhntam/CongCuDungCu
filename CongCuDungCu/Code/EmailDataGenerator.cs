using System;

namespace CongCuDungCu.Code
{
    public static class EmailDataGenerator
    {
        private static DatabaseGenerator.TableData table;
        private static readonly string[] subjects = new string[] { "Integrating Developer Express MasterView control into an Accounting System.",
            "Web Edition: Data Entry Page. There is an issue with date validation.",
            "Payables Due Calculator is ready for testing.",
            "Web Edition: Search Page is ready for testing.",
            "Main Menu: Duplicate Items. Somebody has to review all menu items in the system.",
            "Receivables Calculator. Where can I find the complete specs?",
            "Ledger: Inconsistency. Please fix it.",
            "Receivables Printing module is ready for testing.",
            "Screen Redraw. Somebody has to look at it.",
            "Email System. What library are we going to use?",
            "Cannot add new vendor. This module doesn't work!",
            "History. Will we track sales history in our system?",
            "Main Menu: Add a File menu. File menu item is missing.",
            "Currency Mask. The current currency mask in completely unusable.",
            "Drag & Drop operations are not available in the scheduler module.",
            "Data Import. What database types will we support?",
            "Reports. The list of incomplete reports.",
            "Data Archiving. We still don't have this features in our application.",
            "Email Attachments. Is it possible to add multiple attachments? I haven't found a way to do this.",
            "Check Register. We are using different paths for different modules.",
            "Data Export. Our customers asked us for export to Microsoft Excel"
        };

        private static readonly string[] names = new string[] {
            "Peter Dolan", "Ryan Fischer", "Richard Fisher",
            "Tom Hamlett", "Mark Hamilton", "Steve Lee", "Jimmy Lewis", "Jeffrey W McClain",
            "Andrew Miller", "Dave Murrel", "Bert Parkins", "Mike Roller", "Ray Shipman",
            "Paul Bailey", "Brad Barnes", "Carl Lucas", "Jerry Campbell"
        };

        public static DatabaseGenerator.TableData Table
        {
            get
            {
                return table;
            }
        }

        public static void Register()
        {
            table = new DatabaseGenerator.TableData();
            table.Name = "Emails";
            table.ConnectionStringName = "LargeDatabaseConnectionString";
            table.Fields.Add(new DatabaseGenerator.FieldData("Subject", subjects));
            table.Fields.Add(new DatabaseGenerator.FieldData("From", names));
            table.Fields.Add(new DatabaseGenerator.FieldData("Sent", GenerateSent));
            table.Fields.Add(new DatabaseGenerator.FieldData("Size", GenerateSize));
            table.Fields.Add(new DatabaseGenerator.FieldData("HasAttachment", GenerateHasAttachment));
            table.RecordCount = 300000;
            DatabaseGenerator.RegisterTable("GeneratedEmailTable", table);
        }

        private static object GenerateSent(Random rnd)
        {
            return DateTime.Now.Date.Subtract(TimeSpan.FromDays(rnd.Next(50)));
        }

        private static object GenerateSize(Random rnd)
        {
            return (long)(1000 + rnd.Next(300000));
        }

        private static object GenerateHasAttachment(Random rnd)
        {
            return rnd.Next(10) > 5;
        }
    }
}
