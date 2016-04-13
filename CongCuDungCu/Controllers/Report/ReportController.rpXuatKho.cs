using DevExpress.Web.Mvc;
using CongCuDungCu.Models;
using System.Web.Mvc;
using CongCuDungCu.Demos;

namespace CongCuDungCu.Controllers
{
    public partial class ReportController : DemoController
    {
        public ActionResult rpXuatKho()
        {
            return DemoView("rpXuatKho");
        }
        public ActionResult rpXuatKhoPartial()
        {
            var report = new CongCuDungCu.Reports.rpXuatKho();
            report.getIDDonVi(System.Web.HttpContext.Current.User.Identity.Name);
            report.Fill(AccountDataProvider.GetSystemDate());
            ViewData["Report"] = report;
            return PartialView("rpXuatKhoPartial");
        }
        public ActionResult DocumentViewerExportToXuatKho()
        {
            var report = new CongCuDungCu.Reports.rpXuatKho();
            report.getIDDonVi(System.Web.HttpContext.Current.User.Identity.Name);
            report.Fill( AccountDataProvider.GetSystemDate());
            return DocumentViewerExtension.ExportTo(report);
        }
    }
}
