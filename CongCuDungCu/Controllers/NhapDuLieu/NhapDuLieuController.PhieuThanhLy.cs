using System;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using CongCuDungCu.Models;
using CongCuDungCu.Demos;

namespace CongCuDungCu.Controllers
{
    public partial class NhapDuLieuController : DemoController
    {
        public ActionResult PhieuThanhLy()
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            ViewData["Madvql"] = Madvql;
            return DemoView("PhieuThanhLy", CCDCDataProvider.PhieuThanhLys(Madvql, "X01"));
        }
        public ActionResult PhieuThanhLyPartial()
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            ViewData["Madvql"] = Madvql;
            return PartialView("PhieuThanhLyPartial", CCDCDataProvider.PhieuThanhLys(Madvql, "X01"));
        }

        [HttpGet]
        public ActionResult FormEdit_ThanhLy(int STTCT)
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
            return DemoView("PhieuThanhLy", "FormEdit_ThanhLy", editCT);
        }
        [HttpPost,
        ValidateInput(false)]
        public ActionResult FormEdit_ThanhLy([ModelBinder(typeof(DevExpressEditorsBinder))] EditableChungTu chungtu)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            ViewData["Madvql"] = Madvql;
            chungtu.Madvql = Madvql;
            chungtu.Maloaiphieu = "X01";
            if (!ModelState.IsValid)
            {
                return DemoView("PhieuThanhLy", "FormEdit_ThanhLy", CCDCDataProvider.GetEditableChungtus(chungtu.STTCT));
            }
            if (chungtu.STTCT == -1)
            {
                CCDCDataProvider.InsertChungTuThanhLy(chungtu);
            }
            else
            {
                CCDCDataProvider.UpdateChungTuThanhLy(chungtu);
            }
            chungtu.STTCT = (int)Session["STTCT"];          
            return DemoView("PhieuThanhLy", "FormEdit_ThanhLy", CCDCDataProvider.GetEditableChungtus(chungtu.STTCT));
        }
        public ActionResult FormDelete_PhieuThanhLy(int STTCT)
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

            return DemoView("PhieuThanhLy", CCDCDataProvider.PhieuThanhLys(Madvql, "X01"));
        }
        public ActionResult menuThanhLy()
        {
            return PartialView("menuThanhLy");
        }
        public ActionResult ThanhLyDetailPartial()
        {
            ViewData["STTCT"] = Session["STTCT"];
            return PartialView("ThanhLyDetailPartial", CCDCDataProvider.listCTChungtus((int)Session["STTCT"]));
        }
        [HttpPost,
        ValidateInput(false)]
        public ActionResult UpdateThanhLyPartial(EditableCTChungTu chitiet)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            var maDonVi = CCDCDataProvider.getMaDonVi_fromChiTietCT(Madvql, (int)Session["STTCT"]);
            chitiet.Madonvi = maDonVi;
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
            {
                ViewData["EditError"] = "Please, correct all errors.";
            }
            return PartialView("ThanhLyDetailPartial", CCDCDataProvider.listCTChungtus(chitiet.STTCT));
        }

        [HttpPost,
        ValidateInput(false)]
        public ActionResult NewThanhLyPartial(EditableCTChungTu chitiet)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            var maDonVi = CCDCDataProvider.getMaDonVi_fromChiTietCT(Madvql, (int)Session["STTCT"]);
            chitiet.Madonvi = maDonVi;
            chitiet.STTCT = (int)Session["STTCT"];
            chitiet.SoCT = CCDCDataProvider.getSoCT(chitiet.STTCT);
            chitiet.Madvql = Madvql;
            if (chitiet.NguonVon == "XDCB") chitiet.MaCCDC = chitiet.TenCCDC.Split('-')[0] + "_XDCB";
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
                ViewData["EditError"] = "Please, correct all errors.";
            }
            return PartialView("ThanhLyDetailPartial", CCDCDataProvider.listCTChungtus(chitiet.STTCT));
        }
        [HttpPost,
        ValidateInput(false)]
        public ActionResult DeleteThanhLyPartial(int STTCTCT)
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
            return PartialView("ThanhLyDetailPartial", CCDCDataProvider.listCTChungtus(STTCT));
        }
    }
}
