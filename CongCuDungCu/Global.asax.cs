using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace CongCuDungCu
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AuthConfig.RegisterAuth();

            ModelBinders.Binders.DefaultBinder = new DevExpress.Web.Mvc.DevExpressEditorsBinder();
        }
    }
}
