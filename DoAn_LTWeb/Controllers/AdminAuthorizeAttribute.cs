using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DoAn_LTWeb.Controllers
{
	public class AdminAuthorizeAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 1. Kiểm tra Session: Nếu chưa đăng nhập
            if (HttpContext.Current.Session["Admin_MaKH"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Account",
                            action = "Login",
                            area = ""
                        }
                    )
                );
            }
            else
            {
                var vaitro = HttpContext.Current.Session["Admin_VaiTro"] as String;
                if (vaitro !="Quản trị viên"&& vaitro!="Admin")
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new { controller = "Home", action = "Index", area = "" }
                        ));
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}