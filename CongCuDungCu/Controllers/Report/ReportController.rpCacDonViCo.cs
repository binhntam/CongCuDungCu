using DevExpress.Web.Mvc;
using CongCuDungCu.Models;
using System.Web.Mvc;
using CongCuDungCu.Demos;

namespace CongCuDungCu.Controllers
{
    public partial class ReportController : DemoController
    {
        public ActionResult rpCacDonViCo()
        {
            return DemoView("rpCacDonViCo");
        }
        public ActionResult rpCacDonViCoPartial()
        {
            var report = new CongCuDungCu.Reports.rpCacDonViCo();
            report.getIDDonViCo(System.Web.HttpContext.Current.User.Identity.Name);
            report.Fill(AccountDataProvider.GetSystemDate());
            ViewData["Report"] = report;
            return PartialView("rpCacDonViCoPartial");
        }
        public ActionResult DocumentViewerExportToCacDonViCo()
        {
            var report = new CongCuDungCu.Reports.rpCacDonViCo();
            report.getIDDonViCo(System.Web.HttpContext.Current.User.Identity.Name);
            report.Fill(AccountDataProvider.GetSystemDate());
            return DocumentViewerExtension.ExportTo(report);
        }
    }
}
