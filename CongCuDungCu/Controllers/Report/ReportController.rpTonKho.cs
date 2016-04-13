using DevExpress.Web.Mvc;
using CongCuDungCu.Models;
using System.Web.Mvc;
using CongCuDungCu.Demos;

namespace CongCuDungCu.Controllers
{
    public partial class ReportController : DemoController
    {
        public ActionResult rpTonKho()
        {
            return DemoView("rpTonKho");
        }
        public ActionResult rpTonKhoPartial()
        {
            var report = new CongCuDungCu.Reports.rpTonKho();
            report.getIDDonVi(System.Web.HttpContext.Current.User.Identity.Name);
            report.Fill(AccountDataProvider.GetSystemDate());
            ViewData["Report"] = report;
            return PartialView("rpTonKhoPartial");
        }
        public ActionResult DocumentViewerExportToTonKho()
        {
            var report = new CongCuDungCu.Reports.rpTonKho();
            report.getIDDonVi(System.Web.HttpContext.Current.User.Identity.Name);
            report.Fill(AccountDataProvider.GetSystemDate());
            return DocumentViewerExtension.ExportTo(report);
        }
    }
}
