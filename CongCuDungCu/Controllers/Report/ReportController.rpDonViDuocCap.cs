using DevExpress.Web.Mvc;
using System.Web.Mvc;
using System;
using CongCuDungCu.Demos;

namespace CongCuDungCu.Controllers
{
    public partial class ReportController : DemoController
    {
        public ActionResult rpDonViDuocCap()
        {
            return DemoView("rpDonViDuocCap");
        }

        public ActionResult DonViDuocCapPartial([Bind] Chart_Helper.ChartViewTypeDemoOptions options)
        {
            ViewData[Chart_Helper.OptionsKey] = options;
            return DemoView("rpDonViDuocCap");
        }
        [HttpPost]
        public ActionResult rpDonViDuocCapPartial([Bind] Chart_Helper.ChartViewTypeDemoOptions options)
        {
            ViewData[Chart_Helper.OptionsKey] = options;

            if (string.IsNullOrEmpty(options.Tendonvi))
            {
                return PartialView("rpDonViDuocCapPartial");
            }
            else
            {
                var report = new CongCuDungCu.Reports.rpDonViDuocCap();
                report.getIDDonVi(System.Web.HttpContext.Current.User.Identity.Name);
                report.Fill(options.Tendonvi);
                Session["Tendonvi"] = options.Tendonvi;
                ViewData["Report"] = report;
                return PartialView("rpDonViDuocCapPartial");
            }
            return PartialView("rpDonViDuocCapPartial");
        }
        public ActionResult DocumentViewerExportToDonViDuocCap()
        {
            var report = new CongCuDungCu.Reports.rpDonViDuocCap();
            report.getIDDonVi(System.Web.HttpContext.Current.User.Identity.Name);
            report.Fill((string)Session["Tendonvi"]);
            return DocumentViewerExtension.ExportTo(report);
        }
    }
}
