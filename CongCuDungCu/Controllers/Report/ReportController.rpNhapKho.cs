using DevExpress.Web.Mvc;
using CongCuDungCu.Models;
using System.Web.Mvc;
using CongCuDungCu.Demos;

namespace CongCuDungCu.Controllers
{
    public partial class ReportController : DemoController
    {
        public ActionResult rpNhapKho()
        {
            return DemoView("rpNhapKho");
        }
        public ActionResult rpNhapKhoPartial()
        {
            var report = new CongCuDungCu.Reports.rpNhapKho();
            report.getIDDonVi(System.Web.HttpContext.Current.User.Identity.Name);
            report.Fill(AccountDataProvider.GetSystemDate());
            ViewData["Report"] = report;
            return PartialView("rpNhapKhoPartial");
        }
        public ActionResult DocumentViewerExportToNhapKho()
        {
            var report = new CongCuDungCu.Reports.rpNhapKho();
            report.getIDDonVi(System.Web.HttpContext.Current.User.Identity.Name);
            report.Fill(AccountDataProvider.GetSystemDate());
            return DocumentViewerExtension.ExportTo(report);
        }
    }
}
