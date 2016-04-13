using DevExpress.Web.Mvc;
using PhuongTienDo.Demos;
using System.Web.Mvc;

namespace PhuongTienDo.Controllers
{
    public partial class ReportController: DemoController {
        public ActionResult DienLucReport()
        {
          //  var model = ReportDemoHelper.CreateModel("Table");
            return DemoView("DienLucReport");
        }
        public ActionResult DienLucReportPartial()
        {
            var report = new PhuongTienDo.Reports.rpDienLuc();
            report.Fill();
            ViewData["Report"] = report;
            return PartialView("DienLucReportPartial");
        }
        public ActionResult DocumentViewerExportToDienLuc()
        {
            var report = new PhuongTienDo.Reports.rpDienLuc();
            report.Fill();
            return DocumentViewerExtension.ExportTo(report);
        }
    }
}
