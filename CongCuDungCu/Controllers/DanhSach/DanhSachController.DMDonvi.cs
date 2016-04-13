using CongCuDungCu.Demos;
using CongCuDungCu.Models;
using System;
using System.Web.Mvc;

namespace CongCuDungCu.Controllers
{
    public partial class DanhSachController : DemoController
    {
        public ActionResult DMDonvi()
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            return DemoView("DMDonvi", CCDCDataProvider.GetEditableDMDonVis(Madvql));
        }
        public ActionResult DMDonviPartial()
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            return PartialView("DMDonviPartial", CCDCDataProvider.GetEditableDMDonVis(Madvql));
        }
        [HttpPost,
        ValidateInput(false)]
        public ActionResult UpdateDonViPartial(EditableDonVi chitiet)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            chitiet.Madvql = Madvql;
            if (ModelState.IsValid)
            {
                try
                {
                    CCDCDataProvider.UpdateDonVi(chitiet);
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
            return PartialView("DMDonviPartial", CCDCDataProvider.GetEditableDMDonVis(Madvql));
        }
        [HttpPost,
       ValidateInput(false)]
        public ActionResult NewDonViPartial(EditableDonVi chitiet)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            chitiet.Madvql = Madvql;
            if (ModelState.IsValid)
            {
                try
                {
                    CCDCDataProvider.InsertDonVi(chitiet);
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
            return PartialView("DMDonviPartial", CCDCDataProvider.GetEditableDMDonVis(Madvql));
        }
        [HttpPost,
       ValidateInput(false)]
        public ActionResult DeleteDonViPartial(int STTDMDV)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
           
            if (STTDMDV > 0)
            {
                try
                {
                    CCDCDataProvider.DeleteDonVi(STTDMDV,Madvql);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("DMDonviPartial", CCDCDataProvider.GetEditableDMDonVis(Madvql));
        }
    }
}
