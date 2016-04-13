using DevExpress.Web.Mvc;
using CongCuDungCu.Models;
using System.Web.Mvc;
using CongCuDungCu.Demos;

namespace CongCuDungCu.Controllers
{
    public partial class ReportController : DemoController
    {
        public ActionResult rpThuHoi()
        {
            return DemoView("rpThuHoi");
        }
        public ActionResult rpThuHoiPartial()
        {
            var report = new CongCuDungCu.Reports.rpThuHoi();
            report.getIDDonVi(System.Web.HttpContext.Current.User.Identity.Name);
            report.Fill(AccountDataProvider.GetSystemDate());
            ViewData["Report"] = report;
            return PartialView("rpThuHoiPartial");
        }
        public ActionResult DocumentViewerExportToThuHoi()
        {
            var report = new CongCuDungCu.Reports.rpThuHoi();
            report.getIDDonVi(System.Web.HttpContext.Current.User.Identity.Name);
            report.Fill(AccountDataProvider.GetSystemDate());
            return DocumentViewerExtension.ExportTo(report);
        }
    }
}
