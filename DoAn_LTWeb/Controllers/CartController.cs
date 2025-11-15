using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn_LTWeb.ViewModels;
using DoAn_LTWeb.Models;

namespace DoAn_LTWeb.Controllers
{
    public class CartController : Controller
    {

        INSTRUMENT data = new INSTRUMENT();

       private List<CartItemViewModel> GetCart()
        {
            var cart = Session["GioHang"] as List<CartItemViewModel>;
            if (cart == null)
            {
                cart = new List<CartItemViewModel>();
                Session["GioHang"] = cart;
            }
            return cart;
        }


        //giỏ hàng
        public ActionResult Index()
        {
            var cart = GetCart();
            ViewBag.TongTien = cart.Sum(item => item.ThanhTien);
            return View(cart);
        }


        //thêm vào giỏ hàng
        [HttpPost]
        public ActionResult AddToCart(int variantID, int quantity, string type)
        {

            //lấy thông tin biến thể sản phẩm 
            var variant = data.SANPHAM_BIENTHE.Find(variantID);
            if (variant == null)
            {
                return HttpNotFound();
            }
            //Lấy thông tin sản phẩm cha
            var product = data.SANPHAMs.Find(variant.MASP);
            string images = !string.IsNullOrEmpty(variant.ANH_BIENTHE)
                                                    ? variant.ANH_BIENTHE
                                                    :product.ANHBIA;
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.MaBienThe == variantID);
            if (item !=null)
            {
                //nếu có rồi thì tăng số lượng
                item.SoLuong += quantity;
            }
            else
            {
                cart.Add(new CartItemViewModel
                {
                    MaBienThe = variant.MABIENTHE,
                    TenSanPham = product.TENSP,
                    TenBienThe = variant.TENBIENTHE,
                    HinhAnh = images,
                    DonGia = variant.GIABAN ?? 0,
                    SoLuong = quantity
                });

            }


            Session["GioHang"] = cart;
            if (type == "buynow")
            {
                return RedirectToAction("Checkout"); //đến trang thanh toán
            }
            else
            {
                TempData["SuccessMessage"] = "Đã thêm vào giỏ hàng thành công!";
                if (Request.UrlReferrer != null)
                {
                    return Redirect(Request.UrlReferrer.ToString());
                }
                return RedirectToAction("Details", "Store", new { id = product.MASP });
            }
        }


        //THÔNG BÁO thành công khi đặt hàng 
        public ActionResult OrderSuccess()
        {
            // Trả về View OrderSuccess.cshtml
            return View();
        }




        //cập nhật số lượng
        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.MaBienThe == id);
            if (item != null)
            {
                item.SoLuong = quantity;
            }
            return RedirectToAction("Index");
        }







        //xóa sản phẩm
        public ActionResult Remove(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.MaBienThe == id);
            if (item != null)
            {
                cart.Remove(item);
            }
            return RedirectToAction("Index");
        }
        //thanh toán
        public ActionResult Checkout()
        {
            var cart = GetCart();
            if (cart.Count == 0) return RedirectToAction("Index", "Home");

            ViewBag.TongTien = cart.Sum(x => x.ThanhTien);
            return View(cart); // Trả về View để điền thông tin
        }




    }
}