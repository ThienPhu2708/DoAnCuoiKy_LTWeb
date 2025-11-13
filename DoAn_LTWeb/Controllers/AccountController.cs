using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn_LTWeb.Models;
using DoAn_LTWeb.ViewModels;
using Microsoft.Ajax.Utilities;

namespace DoAn_LTWeb.Controllers
{
    public class AccountController : Controller
    {
       
        INSTRUMENT data = new INSTRUMENT();
       
        [HttpPost]
        public ActionResult Login(FormCollection col)
        {
            KHACHHANG kh = data.KHACHHANGs.FirstOrDefault(k => k.EMAIL == col["EMAIL"] && k.MATKHAU == col["MATKHAU"]);

            if (kh !=null)
            {
                Session["kh"] = kh;
                return RedirectToAction("", ""); //chuyển hướng đến trang quản trị
            }
            return View();
        }



        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }



        





    }
}