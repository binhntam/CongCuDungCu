using System;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using CongCuDungCu.Demos;
using CongCuDungCu.Models;

namespace CongCuDungCu.Controllers
{
    public partial class NhapDuLieuController : DemoController
    {
        public ActionResult PhieuXuatKho()
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            ViewData["Madvql"] = Madvql;
            return DemoView("PhieuXuatKho", CCDCDataProvider.PhieuXuatKhos(Madvql, "X00"));
        }
        public ActionResult PhieuXuatKhoPartial()
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            return PartialView("PhieuXuatKhoPartial", CCDCDataProvider.PhieuXuatKhos(Madvql, "X00"));
        }
        public ActionResult GridLookup_XuatPartial(string MaCCDC)
        {
            ViewData["MaCCDC"] = MaCCDC;
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            string tenDonVi = CCDCDataProvider.getTenDvi_BySTTCT((int)Session["STTCT"]);
            string maDonvi = CCDCDataProvider.DMDonVis_byMaDonVi(Madvql, tenDonVi);
            string _newVariable = Request.Params["newVariable"];
            if (string.IsNullOrEmpty(_newVariable))
            {
                Session["newVariable"] = CCDCDataProvider.GetEditableCCDCs5(MaCCDC, Madvql, maDonvi); 
            }
            else
            {               
                Session["newVariable"] = _newVariable;
            }
            object result = new object();
            if (Session["STTCT"] == null)
            {
                ViewData["CCDCs"] = CCDCDataProvider.GetEditableCCDC2s(Madvql);
                result = CCDCDataProvider.GetCCDC_ByMaCCDC(MaCCDC);
            }
            else
            {
                ViewData["CCDCs"] = CCDCDataProvider.GetEditableCCDCs4(Madvql, maDonvi);
              //  result = CCDCDataProvider.GetCCDC_ByMaCCDCofDVi(Madvql, maDonvi);
                result = CCDCDataProvider.GetCCDC_ByMaCCDC(MaCCDC);
            }

            return PartialView("GridLookup_XuatPartial", result);
        }
        [HttpGet]
        public ActionResult FormEdit_PhieuXuatKho(int STTCT)
        {
            Session["STTCT"] = STTCT;
            var editCT = CCDCDataProvider.GetEditableChungtus(STTCT);
            var userName = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            ViewData["Madvql"] = Madvql;
            if (editCT == null)
            {
                editCT = new EditableChungTu();
                editCT.STTCT = -1;
            }
            return DemoView("PhieuXuatKho", "FormEdit_Xuat", editCT);
        }
        [HttpPost,
        ValidateInput(false)]
        public ActionResult FormEdit_PhieuXuatKho([ModelBinder(typeof(DevExpressEditorsBinder))] EditableChungTu chungtu)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            ViewData["Madvql"] = Madvql;
            chungtu.Madvql = Madvql;
            chungtu.Maloaiphieu = "X00";
            if (!ModelState.IsValid)
            {
                return DemoView("PhieuXuatKho", "FormEdit_Xuat", CCDCDataProvider.GetEditableChungtus(chungtu.STTCT));
            }
            if (chungtu.STTCT == -1)
            {
                CCDCDataProvider.InsertChungTuXuat(chungtu);
            }
            else
            {
                CCDCDataProvider.UpdateChungTuXuat(chungtu);
            }
            chungtu.STTCT = (int)Session["STTCT"];          
            return DemoView("PhieuXuatKho", "FormEdit_Xuat", CCDCDataProvider.GetEditableChungtus(chungtu.STTCT));
        }
        public ActionResult FormDelete_PhieuXuatKho(int STTCT)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            if (STTCT >= 0)
            {
                try
                {
                    CCDCDataProvider.DeleteChungTu(STTCT);
                    // CCDCDataProvider.DeleteChitietCT(STTCT);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            return DemoView("PhieuXuatKho", CCDCDataProvider.PhieuXuatKhos(Madvql, "X00"));
        }
        public ActionResult menuXuat()
        {
            return PartialView("menuXuat");
        }
        public ActionResult XuatDetailPartial()
        {
            ViewData["STTCT"] = Session["STTCT"];
            return PartialView("XuatDetailPartial", CCDCDataProvider.listCTChungtus((int)Session["STTCT"]));
        }
        [HttpPost,
        ValidateInput(false)]
        public ActionResult UpdateXuatPartial(EditableCTChungTu chitiet)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            var maDonVi = CCDCDataProvider.getMaDonVi_fromChiTietCT(Madvql, (int)Session["STTCT"]);
            chitiet.Madonvi = maDonVi;
            chitiet.STTCT = (int)Session["STTCT"];
            chitiet.Madvql = Madvql;
            if (chitiet.NguonVon == "XDCB") chitiet.MaCCDC = chitiet.TenCCDC.Split('-')[0].Substring(0, 8) + "_XDCB";
            else chitiet.MaCCDC = chitiet.TenCCDC.Split('-')[0];
            chitiet.TenCCDC = chitiet.TenCCDC.Split('-')[1];

            int _SL = Convert.ToInt32(Session["newVariable"]);
            if (chitiet.Soluong > _SL)
            {
                ViewData["EditError"] = "Không được nhập quá số lượng hiện có trong kho. SL đơn vị hiện có: " + Convert.ToString(Session["newVariable"]);
                return PartialView("XuatDetailPartial", CCDCDataProvider.listCTChungtus(chitiet.STTCT));
            }
            if (ModelState.IsValid)
            {
                try
                {
                    CCDCDataProvider.UpdateDetail(chitiet);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "Vui lòng nhập đầy nội dung trong (*).";
            }
            return PartialView("XuatDetailPartial", CCDCDataProvider.listCTChungtus(chitiet.STTCT));
        }

        [HttpPost,
        ValidateInput(false)]
        public ActionResult NewXuatPartial(EditableCTChungTu chitiet)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            var maDonVi = CCDCDataProvider.getMaDonVi_fromChiTietCT(Madvql, (int)Session["STTCT"]);
            chitiet.Madonvi = maDonVi;
            chitiet.STTCT = (int)Session["STTCT"];
            chitiet.SoCT =  CCDCDataProvider.getSoCT(chitiet.STTCT);
            chitiet.Madvql = Madvql;
            if (chitiet.NguonVon == "XDCB") chitiet.MaCCDC = chitiet.TenCCDC.Split('-')[0].Substring(0, 8) + "_XDCB";
            else chitiet.MaCCDC = chitiet.TenCCDC.Split('-')[0];
            chitiet.TenCCDC = chitiet.TenCCDC.Split('-')[1];

            int _SL = Convert.ToInt32(Session["newVariable"]);
            if (chitiet.Soluong > _SL)
            {
                ViewData["EditError"] = "Không được nhập quá số lượng hiện có trong kho. SL đơn vị hiện có: " + Convert.ToString(Session["newVariable"]);
                return PartialView("XuatDetailPartial", CCDCDataProvider.listCTChungtus(chitiet.STTCT));
            }
            if (ModelState.IsValid)
            {
                try
                {
                    CCDCDataProvider.InsertDetail(chitiet);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "Vui lòng nhập đầy nội dung trong (*).";
            }
            return PartialView("XuatDetailPartial", CCDCDataProvider.listCTChungtus(chitiet.STTCT));
        }
        [HttpPost,
        ValidateInput(false)]
        public ActionResult DeleteXuatPartial(int STTCTCT)
        {
            var STTCT = (int)Session["STTCT"];
            if (STTCTCT >= 0)
            {
                try
                {
                    CCDCDataProvider.DeleteDetail(STTCT, STTCTCT);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("XuatDetailPartial", CCDCDataProvider.listCTChungtus(STTCT));
        }
    }
}
