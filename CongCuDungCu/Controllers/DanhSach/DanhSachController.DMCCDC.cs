using CongCuDungCu.Demos;
using CongCuDungCu.Models;
using System;
using System.Web.Mvc;

namespace CongCuDungCu.Controllers
{
    public partial class DanhSachController : DemoController
    {
        public ActionResult DMCCDC()
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            return DemoView("DMCCDC", CCDCDataProvider.listCCDCs(Madvql));
        }
        public ActionResult DMCCDCPartial()
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            return PartialView("DMCCDCPartial", CCDCDataProvider.listCCDCs(Madvql));
        }
        [HttpPost,
        ValidateInput(false)]
        public ActionResult UpdateCCDCPartial(EditableCCDC ccdc)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            ccdc.Madvql = Madvql;

            if (ModelState.IsValid)
            {
                try
                {
                    CCDCDataProvider.UpdateCCDC(ccdc);
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
            return PartialView("DMCCDCPartial", CCDCDataProvider.listCCDCs(Madvql));
        }
        [HttpPost,
        ValidateInput(false)]
        public ActionResult NewCCDCPartial(EditableCCDC ccdc)
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userName);
            ccdc.Madvql = Madvql;
            if (ModelState.IsValid)
            {
                try
                {
                    CCDCDataProvider.InsertCCDC(ccdc);
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
            return PartialView("DMCCDCPartial", CCDCDataProvider.listCCDCs(Madvql));
        }
    }
}
