using System;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using CongCuDungCu.Models;
using CongCuDungCu.Demos;
using DevExpress.Web.Mvc;
using DevExpress.Web.ASPxEditors;
using System.Web.UI.WebControls;

namespace CongCuDungCu.Controllers
{
    [Authorize]

    public partial class AccountController : DemoController
    {
        public override string Name
        {
            get
            {
                return "Account";
            }
        }
        public IMembershipService MembershipService { get; set; }
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            if (MembershipService == null)
            {
                MembershipService = new AccountMembershipService();
            }
            base.Initialize(requestContext);
        }



        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }




        [HttpPost]
        [AllowAnonymous]

        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false );
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The user name or password provided is incorrect.");
                }
            }
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }




        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }




        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }




        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError(string.Empty, ErrorCodeToString(e.StatusCode));
                }
            }


            return View(model);
        }














        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
    public class CommonDemoHelper
    {
        private static Action<TextBoxSettings> textBoxSettingsMethod;
        private static Action<DateEditSettings> dateEditSettingsMethod;
        private static Action<LabelSettings> labelSettingsMethod;
        private static Action<LabelSettings> wideLabelSettingsMethod;
        private static Action<SpinEditSettings> spinEditSettingsMethod;
        private static Action<MemoSettings> memoSettingsMethod;
        private static Action<MVCxFormLayoutItem> formLayoutItemSettingsMethod;
        public static Action<TextBoxSettings> TextBoxSettingsMethod
        {
            get
            {
                if (textBoxSettingsMethod == null)
                {
                    textBoxSettingsMethod = CreateTextBoxSettingsMethod();
                }
                return textBoxSettingsMethod;
            }
        }
        public static Action<DateEditSettings> DateEditSettingsMethod
        {
            get
            {
                if (dateEditSettingsMethod == null)
                {
                    dateEditSettingsMethod = CreateDateEditSettingsMethod();
                }
                return dateEditSettingsMethod;
            }
        }
        public static Action<LabelSettings> LabelSettingsMethod
        {
            get
            {
                if (labelSettingsMethod == null)
                {
                    labelSettingsMethod = CreateLabelSettingsMethod();
                }
                return labelSettingsMethod;
            }
        }
        public static Action<LabelSettings> WideLabelSettingsMethod
        {
            get
            {
                if (wideLabelSettingsMethod == null)
                {
                    wideLabelSettingsMethod = CreateWideLabelSettingsMethod();
                }
                return wideLabelSettingsMethod;
            }
        }
        public static Action<SpinEditSettings> SpinEditSettingsMethod
        {
            get
            {
                if (spinEditSettingsMethod == null)
                {
                    spinEditSettingsMethod = CreateSpinEditSettingsMethod();
                }
                return spinEditSettingsMethod;
            }
        }
        public static Action<MemoSettings> MemoSettingsMethod
        {
            get
            {
                if (memoSettingsMethod == null)
                {
                    memoSettingsMethod = CreateMemoSettingsMethod();
                }
                return memoSettingsMethod;
            }
        }

        private static Action<TextBoxSettings> CreateTextBoxSettingsMethod()
        {
            return settings =>
            {
                settings.ControlStyle.CssClass = "editor";
                settings.ShowModelErrors = true;
                settings.Properties.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithText;
            };
        }
        private static Action<DateEditSettings> CreateDateEditSettingsMethod()
        {
            return settings =>
            {
                settings.ControlStyle.CssClass = "editor";
                settings.ShowModelErrors = true;
                settings.Properties.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithText;
            };
        }
        private static Action<LabelSettings> CreateLabelSettingsMethod()
        {
            return settings =>
            {
                settings.ControlStyle.CssClass = "label";
            };
        }
        private static Action<LabelSettings> CreateWideLabelSettingsMethod()
        {
            return settings =>
            {
                settings.ControlStyle.CssClass = "wideLabel";
            };
        }
        private static Action<SpinEditSettings> CreateSpinEditSettingsMethod()
        {
            return settings =>
            {
                settings.ControlStyle.CssClass = "editor";
                settings.ShowModelErrors = true;
                settings.Properties.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithText;

                settings.Properties.NumberType = SpinEditNumberType.Float;
                settings.Properties.Increment = 0.1M;
                settings.Properties.LargeIncrement = 1;
                settings.Properties.SpinButtons.ShowLargeIncrementButtons = true;
            };
        }
        private static Action<MemoSettings> CreateMemoSettingsMethod()
        {
            return settings =>
            {
                settings.Height = 50;
                settings.ControlStyle.CssClass = "editor";
                settings.ShowModelErrors = true;
                settings.Properties.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithText;
            };
        }
        public static Action<MVCxFormLayoutItem> FormLayoutItemSettingsMethod
        {
            get
            {
                if (formLayoutItemSettingsMethod == null)
                {
                    formLayoutItemSettingsMethod = CreateFormLayoutItemSettingsMethod();
                }
                return formLayoutItemSettingsMethod;
            }
        }
        private static Action<MVCxFormLayoutItem> CreateFormLayoutItemSettingsMethod()
        {
            return itemSettings =>
            {
                dynamic editorSettings = itemSettings.NestedExtensionSettings;
                editorSettings.Properties.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithText;
                editorSettings.ShowModelErrors = true;
                editorSettings.Width = Unit.Pixel(230);
            };
        }
    }
}
