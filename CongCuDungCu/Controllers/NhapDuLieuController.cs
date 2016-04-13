﻿using CongCuDungCu.Demos;
using DevExpress.Web.Mvc;
using System;

using System.Web.Mvc;
namespace CongCuDungCu.Controllers
{
    public partial class NhapDuLieuController : DemoController
    {
        public override string Name
        {
            get
            {
                return "NhapDuLieu";
            }
        }

        public ActionResult Index()
        {
            return RedirectToAction("NhapDuLieu");
        }

    }

    public class NhapDuLieuController_Helper
    {
        public static string rpNhapKho1Phieu()
        {
           //var sttct = !string.IsNullOrEmpty(Request.Params["idSuCo"]) ? int.Parse(Request.Params["idSuCo"]) : 0;
            return DevExpressHelper.GetUrl(new { Controller = "Report", Action = "rpNhapKho1Phieu" });
        }
        public static string rpXuatKho1Phieu()
        {
            //var sttct = !string.IsNullOrEmpty(Request.Params["idSuCo"]) ? int.Parse(Request.Params["idSuCo"]) : 0;
            return DevExpressHelper.GetUrl(new { Controller = "Report", Action = "rpXuatKho1Phieu" });
        }
       
    }
}

