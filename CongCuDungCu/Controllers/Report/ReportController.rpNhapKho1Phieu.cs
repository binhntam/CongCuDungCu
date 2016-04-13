using DevExpress.Web.Mvc;
using CongCuDungCu.Models;
using System.Web.Mvc;
using CongCuDungCu.Demos;

namespace CongCuDungCu.Controllers
{
    public partial class ReportController : DemoController
    {
        public ActionResult rpNhapKho1Phieu()
        {
            return DemoView("rpNhapKho1Phieu");
        }
        public ActionResult rpNhapKho1PhieuPartial()
        {
            var report = new CongCuDungCu.Reports.rpNhapKho1Phieu();
            int sttct = (int)Session["STTCT"];
            report.Fill(sttct);
            ViewData["Report"] = report;
            return PartialView("rpNhapKho1PhieuPartial");
        }
        public ActionResult DocumentViewerExportToNhapKho1Phieu()
        {
            var report = new CongCuDungCu.Reports.rpNhapKho1Phieu();          
            int sttct = (int)Session["STTCT"];
            report.Fill(sttct);
            return DocumentViewerExtension.ExportTo(report);
        }
    }
}
