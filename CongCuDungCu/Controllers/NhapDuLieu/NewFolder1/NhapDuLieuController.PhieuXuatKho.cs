using System;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using CongCuDungCu.Demos;
using CongCuDungCu.Models;
namespace CongCuDungCu.Controllers
{
    public partial class NhapDuLieuController : DemoController
    {

        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateChiTietPartial(EditableCTChungTu chitiet)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            chitiet.STTCT = (int)Session["STTCT"];
            chitiet.Madvql = Madvql;
            chitiet.MaCCDC = chitiet.TenCCDC.Split('-')[0];
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
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("CTChungTuNhapPartial", CCDCDataProvider.listCTChungtus(chitiet.STTCT));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult NewChiTietPartial(EditableCTChungTu chitiet)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            chitiet.STTCT = (int)Session["STTCT"];
            chitiet.Madvql = Madvql;
            chitiet.MaCCDC = chitiet.TenCCDC.Split('-')[0];
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
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("CTChungTuNhapPartial", CCDCDataProvider.listCTChungtus(chitiet.STTCT));
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteChiTietPartial(int STTCTCT)
        {
            int STTCT = (int)Session["STTCT"];
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
            return PartialView("CTChungTuNhapPartial", CCDCDataProvider.listCTChungtus(STTCT));
        }
        /***/     
        #region PhieuNhapKho     
        public ActionResult PhieuNhapKho()
        {
            return DemoView("PhieuNhapKho", CCDCDataProvider.PhieuNhapKhos());
        }
        public ActionResult PhieuNhapKhoPartial()
        {
            return PartialView("PhieuNhapKhoPartial", CCDCDataProvider.PhieuNhapKhos());
        }
        public ActionResult GridLookupPartial(string MaCCDC)
        {
            ViewData["MaCCDC"] = MaCCDC;
            ViewData["CCDCs"] = CCDCDataProvider.GetEditableCCDCs();
         var result = CCDCDataProvider.GetCCDC_ByMaCCDC(MaCCDC);
         return PartialView("GridLookupPartial", result);
        //    return PartialView("MultiColumnComboBoxPartial", result);
      //    return PartialView();
        }
      
        [HttpGet]
        public ActionResult FormEdit_PhieuNhapKho(int STTCT)
        {
            Session["STTCT"] = STTCT;
            EditableChungTu editCT = CCDCDataProvider.GetEditableChungtus(STTCT);
            if (editCT == null)
            {
                editCT = new EditableChungTu();
                editCT.STTCT = -1;
            }
            return DemoView("PhieuNhapKho", "FormEdit_Nhap", editCT);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEdit_PhieuNhapKho([ModelBinder(typeof(DevExpressEditorsBinder))] EditableChungTu chungtu)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            chungtu.Madvql = Madvql;
            if (!ModelState.IsValid)              
                return DemoView("PhieuNhapKho", "FormEdit_Nhap", CCDCDataProvider.GetEditableChungtus(chungtu.STTCT));
            if (chungtu.STTCT == -1)
                CCDCDataProvider.InsertChungTu(chungtu);
            else
                CCDCDataProvider.UpdateChungTu(chungtu);
          //  return RedirectToAction("PhieuNhapKho");
            return DemoView("PhieuNhapKho", "FormEdit_Nhap", CCDCDataProvider.GetEditableChungtus(chungtu.STTCT));
        }
        public ActionResult FormDelete_PhieuNhapKho(int STTCT)
        {        
            return RedirectToAction("PhieuNhapKho");
        }
        public ActionResult menuChiTietChungTu()
        {
            return PartialView("menuChiTietChungTu");
        }
        public ActionResult CTChungTuNhapPartial()
        {
            ViewData["STTCT"] = Session["STTCT"];   
            return PartialView("CTChungTuNhapPartial", CCDCDataProvider.listCTChungtus((int)Session["STTCT"]));
        }
      
        #endregion
        /****/      
        #region PhieuXuatKho
        public ActionResult PhieuXuatKho()
        {
            return DemoView("PhieuXuatKho", CCDCDataProvider.PhieuXuatKhos("X00"));
        }
        public ActionResult PhieuXuatKhoPartial()
        {
            return PartialView("PhieuXuatKhoPartial", CCDCDataProvider.PhieuXuatKhos("X00"));
        }      

        [HttpGet]
        public ActionResult FormEdit_PhieuXuatKho(int STTCT)
        {
            Session["STTCT"] = STTCT;
            EditableChungTu editCT = CCDCDataProvider.getChungtuXuats("X00",STTCT);
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
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEdit_PhieuXuatKho([ModelBinder(typeof(DevExpressEditorsBinder))] EditableChungTu chungtu)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            ViewData["Madvql"] = Madvql;
            chungtu.Madvql = Madvql;
            
            if (!ModelState.IsValid)
                return DemoView("PhieuXuatKho", "FormEdit_Xuat", CCDCDataProvider.getChungtuXuats("X01",chungtu.STTCT));
            if (chungtu.STTCT == -1)
                CCDCDataProvider.InsertChungTuXuat(chungtu);
                
            else
                CCDCDataProvider.UpdateChungTuXuat(chungtu);
            var ob = CCDCDataProvider.getChungtuXuats("X00", chungtu.STTCT);
            return DemoView("PhieuXuatKho", "FormEdit_Xuat", CCDCDataProvider.getChungtuXuats("X00", chungtu.STTCT));
           // return RedirectToAction("FormEdit_Xuat");
        }
        public ActionResult FormDelete_PhieuXuatKho(int STTCT)
        {
            return RedirectToAction("PhieuXuatKho");
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
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateXuatPartial(EditableCTChungTu chitiet)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            chitiet.STTCT = (int)Session["STTCT"];
            chitiet.Madvql = Madvql;
            chitiet.MaCCDC = chitiet.TenCCDC.Split('-')[0];
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
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("XuatDetailPartial", CCDCDataProvider.listCTChungtus(chitiet.STTCT));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult NewXuatPartial(EditableCTChungTu chitiet)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            chitiet.STTCT = (int)Session["STTCT"];
            chitiet.SoCT = (string)Session["newsoCT"];
            chitiet.Madvql = Madvql;
            chitiet.MaCCDC = chitiet.TenCCDC.Split('-')[0];
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
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("XuatDetailPartial", CCDCDataProvider.listCTChungtus(chitiet.STTCT));
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteXuatPartial(int STTCTCT)
        {
            int STTCT = (int)Session["STTCT"];
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
        #endregion
      
    }
}
