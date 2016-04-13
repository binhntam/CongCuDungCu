using System.Web.Mvc;
using System.Collections.Generic;
using DevExpress.Web.Mvc;
using CongCuDungCu.Demos;

namespace CongCuDungCu.Controllers
{
    public partial class DanhSachController : DemoController
    {
        public override string Name
        {
            get
            {
                return "DanhSach";
            }
        }

        public ActionResult Index()
        {
            return RedirectToAction("DanhSach");
        }
    }
    public class DanhSachController_Helper
    {
        public static string getDMNoiSX()
        {
            return DevExpressHelper.GetUrl(new
            { Controller = "DanhSach", Action = "DMNoiSX" });
        }
        public static string getDMNhaCungCap()
        {
            return DevExpressHelper.GetUrl(new
            { Controller = "DanhSach", Action = "DMNhaCungCap" });
        }
        public static string getDonViQL()
        {
            return DevExpressHelper.GetUrl(new
            { Controller = "DanhSach", Action = "DMDonViQL" });
        }
        public static string getDMDonVi()
        {
            return DevExpressHelper.GetUrl(new
            { Controller = "DanhSach", Action = "DMDonVi" });
        }
        public static string getDMLoaiphieu()
        {
            return DevExpressHelper.GetUrl(new
            { Controller = "DanhSach", Action = "DMLoaiphieu" });
        }
        public static string getDMKho()
        {
            return DevExpressHelper.GetUrl(new
            { Controller = "DanhSach", Action = "DMKho" });
        }
        public static string getDMTinhtrang()
        {
            return DevExpressHelper.GetUrl(new
            { Controller = "DanhSach", Action = "DMTinhtrang" });
        }
        public static string getDMNguoidung()
        {
            return DevExpressHelper.GetUrl(new
            { Controller = "DanhSach", Action = "DMNguoidung" });
        }
        public static string getDMNhomnguoidung()
        {
            return DevExpressHelper.GetUrl(new
            { Controller = "DanhSach", Action = "DMNhomnguoidung" });
        }
        public static string getDMDvt()
        {
            return DevExpressHelper.GetUrl(new
            { Controller = "DanhSach", Action = "DMDvt" });
        }
        public static string getDMbaocao()
        {
            return DevExpressHelper.GetUrl(new
            { Controller = "DanhSach", Action = "DMbaocao" });
        }
    }
}
