using System.Web.Mvc;
using System;
using CongCuDungCu.Demos;

namespace CongCuDungCu.Controllers
{
    public partial class ReportController : DemoController
    {
        public override string Name
        {
            get
            {
                return "Report";
            }
        }

        public ActionResult Index()
        {
            return RedirectToAction("Report");
        }
    }
    public class Chart_Helper
    {
        public const string OptionsKey = "Options";
        public class ChartViewTypeDemoOptions
        {
            public DevExpress.XtraCharts.ViewType View { get; set; }

            public ChartViewTypeDemoOptions()
            {
            }

            public bool ShowRadio { get; set; }
            public string Tendonvi { get; set; }
        }
    }
}
