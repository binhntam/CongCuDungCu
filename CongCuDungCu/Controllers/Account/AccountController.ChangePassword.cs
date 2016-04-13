using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CongCuDungCu.Demos;
using CongCuDungCu.Models;

namespace CongCuDungCu.Controllers
{
    public partial class AccountController : DemoController
    {
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return DemoView("ChangePassword", new ChangePasswordModel());
        }
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (Request.Params["btnUpdate"] == null)
            {
                ModelState.Clear();
                return DemoView("ChangePassword", model);
            }
            if (ModelState.IsValid)
            {
                bool changePasswordSucceeded;
                try
                {
                    changePasswordSucceeded = MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    AccountDataProvider.UpdateNewPassword(User.Identity.Name, model.NewPassword);

                    AccountDataProvider.UpdateLastPasswordChangeDate(User.Identity.Name);

                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Mật khẩu cũ không đúng hoặc mật khẩu mới bị sai!");
                    return DemoView("ChangePassword", model);
                }
            }


            return View(model);
        }
        public ActionResult ChangePasswordSuccess()
        {
            return DemoView("ChangePasswordSuccess");
        }
    }
}
