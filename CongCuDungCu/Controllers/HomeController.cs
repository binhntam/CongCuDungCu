using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CongCuDungCu.Demos;
using CongCuDungCu.Code;
using DevExpress.Web.Mvc;
using System.Threading;

namespace CongCuDungCu.Controllers
{
    [HandleError]
    public class HomeController : DemoController
    {
        public override string Name
        {
            get
            {
                return "Home";
            }
        }

        public ActionResult Index()
        {
            return DemoView("Index");
        }
        public ActionResult DemoTabsPartial(string group, string demo)
        {
            Utils.RegisterCurrentMvcDemo(group, demo);
            return PartialView("DemoTabsPartial");
        }
        public ActionResult SearchListPartial(string text)
        {
            if (DevExpressHelper.IsCallback)
            {
                Thread.Sleep(500);
            }
            ViewData["RequestText"] = text;
            return PartialView("SearchListPartial", SearchUtils.DoSearch(text));
        }
    }
}
