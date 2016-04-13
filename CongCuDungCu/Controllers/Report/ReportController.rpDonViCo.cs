using DevExpress.Web.Mvc;
using CongCuDungCu.Demos;
using System.Web.Mvc;
using System;

namespace CongCuDungCu.Controllers
{
    public partial class ReportController : DemoController
    {
        public ActionResult rpDonViCo()
        {
            return DemoView("rpDonViCo");
        }

        [HttpPost]
        public ActionResult DonViCoPartial([Bind] Chart_Helper.ChartViewTypeDemoOptions options)
        {
            ViewData[Chart_Helper.OptionsKey] = options;
            return DemoView("rpDonViCo");
        }
        [HttpPost]
        public ActionResult rpDonViCoPartial([Bind] Chart_Helper.ChartViewTypeDemoOptions options)
        {
            ViewData[Chart_Helper.OptionsKey] = options;

            if (string.IsNullOrEmpty(options.Tendonvi))
            {
                return PartialView("rpDonViCoPartial");
            }
            else
            {
                var report = new CongCuDungCu.Reports.rpDonViCo();
                report.getIDDonViCo(System.Web.HttpContext.Current.User.Identity.Name);
              report.Fill(options.Tendonvi);
               // report.Fill_NguonVon(options.Tendonvi);
                report.getTenDonViSD(options.Tendonvi);
                Session["Tendonvi"] = options.Tendonvi;
                ViewData["Report"] = report;
                return PartialView("rpDonViCoPartial");
            }
            return PartialView("rpDonViCoPartial");
        }
        public ActionResult DocumentViewerExportToDonViCo()
        {
            var report = new CongCuDungCu.Reports.rpDonViCo();
            report.getIDDonViCo(System.Web.HttpContext.Current.User.Identity.Name);
            report.getTenDonViSD((string)Session["Tendonvi"]);
            report.Fill((string)Session["Tendonvi"]);
          //  report.Fill_NguonVon((string)Session["Tendonvi"]);
            return DocumentViewerExtension.ExportTo(report);
        }
    }
}
