using DevExpress.Web.Mvc;
using CongCuDungCu.Models;
using System.Web.Mvc;
using CongCuDungCu.Demos;

namespace CongCuDungCu.Controllers
{
    public partial class ReportController : DemoController
    {
        public ActionResult rpXuatKho1Phieu()
        {
            return DemoView("rpXuatKho1Phieu");
        }
        public ActionResult rpXuatKho1PhieuPartial()
        {
            var report = new CongCuDungCu.Reports.rpXuatKho1Phieu();
            int sttct = (int)Session["STTCT"];
            report.Fill(sttct);
            ViewData["Report"] = report;
            return PartialView("rpXuatKho1PhieuPartial");
        }
        public ActionResult DocumentViewerExportToXuatKho1Phieu()
        {
            var report = new CongCuDungCu.Reports.rpXuatKho1Phieu();          
            int sttct = (int)Session["STTCT"];
            report.Fill(sttct);
            return DocumentViewerExtension.ExportTo(report);
        }
    }
}
