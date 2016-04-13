using System;
using System.Web.Mvc;
using CongCuDungCu.Models;
using CongCuDungCu.Demos;

namespace CongCuDungCu.Controllers
{
    public partial class DanhSachController : DemoController
    {
        public ActionResult DanhMucKhac()
        {
            return DemoView("DanhMucKhac");
        }
        [ValidateInput(false)]
        public ActionResult DMNoiSX()
        {
            return DemoView("DMNoiSX", CCDCDataProvider.DMNoiSXs());
        }
        [ValidateInput(false)]
        public ActionResult DMDonViQL()
        {
            return DemoView("DMDonViQL", CCDCDataProvider.DMDonViQLs());
        }
        [ValidateInput(false)]
        public ActionResult DMNhaCungCap()
        {
            return DemoView("DMNhaCungCap", CCDCDataProvider.DMNhaCungCaps());
        }
        [ValidateInput(false)]
        public ActionResult DMDonVi1()
        {
            return DemoView("DMDonVi1", CCDCDataProvider.DMDonVis());
        }
        [ValidateInput(false)]
        public ActionResult DMLoaiphieu()
        {
            return DemoView("DMLoaiphieu", CCDCDataProvider.DMLoaiphieus());
        }
        [ValidateInput(false)]
        public ActionResult DMKho()
        {
            return DemoView("DMKho", CCDCDataProvider.DMKhos());
        }
        [ValidateInput(false)]
        public ActionResult DMNguoidung()
        {
            return DemoView("DMNguoidung", CCDCDataProvider.DMNguoidungs());
        }
        [ValidateInput(false)]
        public ActionResult DMNhomnguoidung()
        {
            return DemoView("DMNhomnguoidung", CCDCDataProvider.DMNhomnguoidungs());
        }
        [ValidateInput(false)]
        public ActionResult DMDvt()
        {
            return DemoView("DMDvt", CCDCDataProvider.DMDvts());
        }
        [ValidateInput(false)]
        public ActionResult DMbaocao()
        {
            return DemoView("DMbaocao", CCDCDataProvider.DMbaocaos());
        }
        [ValidateInput(false)]
        public ActionResult DMTinhtrang()
        {
            return DemoView("DMTinhtrang", CCDCDataProvider.DMTinhtrangs());
        }
    }
}
