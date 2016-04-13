using DevExpress.Web.Mvc;
using CongCuDungCu.Demos;
using System.Web.Mvc;
using System;

namespace CongCuDungCu.Controllers
{
    public partial class ReportController : DemoController
    {
        public ActionResult rpBieuMau9KK()
        {
            return DemoView("rpBieuMau9KK");
        }
        public ActionResult rpBieuMau9KKPartial()
        {
            var report = new CongCuDungCu.Reports.rpBieuMau9KK();
            report.getMaDVQL(System.Web.HttpContext.Current.User.Identity.Name);
            report.Fill();          
          
            ViewData["Report"] = report;
            return PartialView("rpBieuMau9KKPartial");
        }
        public ActionResult DocumentViewerExportToBieuMau9KK()
        {
            var report = new CongCuDungCu.Reports.rpBieuMau9KK();
            report.getMaDVQL(System.Web.HttpContext.Current.User.Identity.Name);
           report.Fill();
            return DocumentViewerExtension.ExportTo(report);
        }
   
      
    }
}
