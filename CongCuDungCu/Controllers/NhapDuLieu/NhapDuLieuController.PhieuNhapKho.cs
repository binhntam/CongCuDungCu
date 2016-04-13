using System;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using CongCuDungCu.Models;
using CongCuDungCu.Demos;

namespace CongCuDungCu.Controllers
{
    public partial class NhapDuLieuController : DemoController
    {
        public ActionResult PhieuNhapKho()
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            ViewData["Madvql"] = Madvql;
            return DemoView("PhieuNhapKho", CCDCDataProvider.PhieuNhapKhos(Madvql, "N00"));
        }
        public ActionResult PhieuNhapKhoPartial()
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            return PartialView("PhieuNhapKhoPartial", CCDCDataProvider.PhieuNhapKhos(Madvql, "N00"));
        }
        public ActionResult GridLookupPartial(string MaCCDC)
        {
            ViewData["MaCCDC"] = MaCCDC;
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            string tenDonVi = CCDCDataProvider.getTenDvi_BySTTCT((int)Session["STTCT"]);
            string maDonvi = CCDCDataProvider.DMDonVis_byMaDonVi(Madvql, tenDonVi);
          
            object result= new object();
            if (Session["STTCT"]==null)
            {
                ViewData["CCDCs"] = CCDCDataProvider.GetEditableCCDC2s(Madvql);
                result = CCDCDataProvider.GetCCDC_ByMaCCDC(MaCCDC);
            }
           else
            {
               // ViewData["CCDCs"] = CCDCDataProvider.GetEditableCCDCs3(Madvql, maDonvi);
                ViewData["CCDCs"] = CCDCDataProvider.GetEditableCCDC2s(Madvql);
                result = CCDCDataProvider.GetCCDC_ByMaCCDC(MaCCDC);
            }

            return PartialView("GridLookupPartial", result);
        }

        [HttpGet]
        public ActionResult FormEdit_PhieuNhapKho(int STTCT)
        {
            Session["STTCT"] = STTCT;
            var editCT = CCDCDataProvider.GetEditableChungtus(STTCT);
            if (editCT == null)
            {
                editCT = new EditableChungTu();
                editCT.STTCT = -1;
            }
            return DemoView("PhieuNhapKho", "FormEdit_Nhap", editCT);
        }
        [HttpPost,
        ValidateInput(false)]
        public ActionResult FormEdit_PhieuNhapKho([ModelBinder(typeof(DevExpressEditorsBinder))] EditableChungTu chungtu)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            chungtu.Maloaiphieu = "N00";
            chungtu.Madvql = Madvql;
            if (!ModelState.IsValid)
            {
                return DemoView("PhieuNhapKho", "FormEdit_Nhap", CCDCDataProvider.GetEditableChungtus(chungtu.STTCT));
            }
            if (chungtu.STTCT == -1)
            {
                CCDCDataProvider.InsertChungTuNhap(chungtu);
            }
            else
            {
                CCDCDataProvider.UpdateChungTuNhap(chungtu);
            }
            chungtu.STTCT=(int)Session["STTCT"];          
            return DemoView("PhieuNhapKho", "FormEdit_Nhap", CCDCDataProvider.GetEditableChungtus(chungtu.STTCT));
        }
        public ActionResult FormDelete_PhieuNhapKho(int STTCT)
        {
          //  return RedirectToAction("PhieuNhapKho");
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

            return DemoView("PhieuNhapKho", CCDCDataProvider.PhieuNhapKhos(Madvql, "N00"));
        }
        public ActionResult menuNhap()
        {
            return PartialView("menuNhap");
        }
        public ActionResult NhapDetailPartial()
        {
            ViewData["STTCT"] = Session["STTCT"];
            return PartialView("NhapDetailPartial", CCDCDataProvider.listCTChungtus((int)Session["STTCT"]));
        }
        [HttpPost,
        ValidateInput(false)]
        public ActionResult UpdateNhapPartial(EditableCTChungTu chitiet)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            var maDonVi = CCDCDataProvider.getMaDonVi_fromChiTietCT(Madvql, (int)Session["STTCT"]);
            chitiet.Madonvi = maDonVi;
            chitiet.STTCT = (int)Session["STTCT"];
            chitiet.Madvql = Madvql;
            if (chitiet.NguonVon == "XDCB") chitiet.MaCCDC = chitiet.TenCCDC.Split('-')[0] + "_XDCB";
            else chitiet.MaCCDC = chitiet.TenCCDC.Split('-')[0];
            chitiet.TenCCDC = chitiet.TenCCDC.Split('-')[1];
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
            return PartialView("NhapDetailPartial", CCDCDataProvider.listCTChungtus(chitiet.STTCT));
        }

        [HttpPost,
        ValidateInput(false)]
        public ActionResult NewNhapPartial(EditableCTChungTu chitiet)
        {
            
            var userName = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            var maDonVi = CCDCDataProvider.getMaDonVi_fromChiTietCT(Madvql, (int)Session["STTCT"]);
            chitiet.Madonvi = maDonVi;
            chitiet.STTCT = (int)Session["STTCT"];
            chitiet.SoCT = CCDCDataProvider.getSoCT(chitiet.STTCT);
            chitiet.Madvql = Madvql;
            if (chitiet.NguonVon == "XDCB") chitiet.MaCCDC = chitiet.TenCCDC.Split('-')[0]+"_XDCB";
            else chitiet.MaCCDC = chitiet.TenCCDC.Split('-')[0];
            chitiet.TenCCDC = chitiet.TenCCDC.Split('-')[1];
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
            return PartialView("NhapDetailPartial", CCDCDataProvider.listCTChungtus(chitiet.STTCT));
        }
        [HttpPost,
        ValidateInput(false)]
        public ActionResult DeleteNhapPartial(int STTCTCT)
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
            return PartialView("NhapDetailPartial", CCDCDataProvider.listCTChungtus(STTCT));
        }
    }
    
}
