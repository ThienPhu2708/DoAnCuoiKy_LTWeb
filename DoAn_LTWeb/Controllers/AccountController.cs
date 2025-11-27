using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn_LTWeb.Models;
using System.Data.Entity;

namespace DoAn_LTWeb.Controllers
{
    public class AccountController : Controller
    {
        INSTRUMENT data = new INSTRUMENT();
        // GET: Account
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login (KHACHHANG model)
        {
            if (string.IsNullOrEmpty(model.EMAIL) || string.IsNullOrEmpty(model.MATKHAU))
            {
                ModelState.AddModelError("", "Vui lòng nhập Email và Mật khẩu.");
                return View(model);
            }

            var user = data.KHACHHANGs
                .Include(k=>k.VAITRO)
                .FirstOrDefault(u => u.EMAIL == model.EMAIL && u.MATKHAU == model.MATKHAU);
            if (user != null)
            {
                if (user.MAVAITRO == 1)
                {
                    Session["Admin_MaKH"] = user.MAKH;
                    Session["Admin_HoTen"] = user.HOTEN;
                    Session["Admin_VaiTro"] = user.VAITRO.TENVAITRO;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ModelState.AddModelError("", "Email hoặc Mật khẩu không đúng.");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Email hoặc Mật khẩu không đúng.");
                return View(model);
            }
           
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}