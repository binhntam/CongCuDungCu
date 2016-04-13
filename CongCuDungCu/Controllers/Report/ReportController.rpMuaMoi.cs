using DevExpress.Web.Mvc;
using CongCuDungCu.Models;
using System.Web.Mvc;
using CongCuDungCu.Demos;

namespace CongCuDungCu.Controllers
{
    public partial class ReportController : DemoController
    {
        public ActionResult rpMuaMoi()
        {
            return DemoView("rpMuaMoi");
        }
        public ActionResult rpMuaMoiPartial()
        {
            var report = new CongCuDungCu.Reports.rpMuaMoi();
            report.getIDDonViCo(System.Web.HttpContext.Current.User.Identity.Name);
            report.Fill(AccountDataProvider.GetSystemDate());
            ViewData["Report"] = report;
            return PartialView("rpMuaMoiPartial");
        }
        public ActionResult DocumentViewerExportToMuaMoi()
        {
            var report = new CongCuDungCu.Reports.rpMuaMoi();
            report.getIDDonViCo(System.Web.HttpContext.Current.User.Identity.Name);
            report.Fill(AccountDataProvider.GetSystemDate());
            return DocumentViewerExtension.ExportTo(report);
        }
    }
}
