using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn_LTWeb.Models;
using DoAn_LTWeb.ViewModels;

namespace DoAn_LTWeb.Controllers
{
    public class HomeController : Controller
    {
        INSTRUMENT data = new INSTRUMENT();
        //page index
        public ActionResult Index()
        {
            var viewModel = new HomeViewModel();

            //sản phẩm loại cha
            viewModel.ParentCategories = data.LOAISANPHAMs.Where(c => c.MALOAICHA == null).ToList();

            //sản phẩm bán chạy
            viewModel.BestSeller = data.SANPHAM_BIENTHE.Include("SANPHAM")
                                                        .OrderBy(b => b.GIABAN) 
                                                        .Take(8) 
                                                        .ToList();
            //tin tức
            viewModel.News = data.TINTUCs.OrderByDescending(n => n.NGAYDANG).Take(6).ToList();



            viewModel.Brands = data.THUONGHIEUs.OrderBy(b => b.TENTHUONGHIEU).ToList();

            return View(viewModel);
        }



        [ChildActionOnly]
        public ActionResult _HeaderMenu()
        {
            var allCategory = data.LOAISANPHAMs.ToList();
            var parentCategory = allCategory.Where(c => c.MALOAICHA == null).ToList();

            var viewModel = new List<MenuCategoryViewModel>();

            foreach (var parent in parentCategory)
            {
                var menuCategory = new MenuCategoryViewModel
                {
                    Parent = parent,
                    Children = allCategory.Where(c => c.MALOAICHA == parent.MALOAI).ToList()
                };
                viewModel.Add(menuCategory);
            }

            var allBrands = data.THUONGHIEUs.OrderBy(b => b.TENTHUONGHIEU).ToList(); // List 2

            // --- 3. [MỚI] Tạo ViewModel tổng để gói 2 list lại ---
            var headerViewModel = new HeaderViewModel
            {
                Categories = viewModel, // Gán list 1
                Brands = allBrands              // Gán list 2
            };
            return PartialView("_HeaderMenu",headerViewModel);  
        }



        //tìm kiếm sản phẩm gần đúng
        [HttpGet]
        public ActionResult SearchSuggestion(string keyword)
        {
            if (string.IsNullOrEmpty(keyword) || keyword.Length <2)
            {
                return Content(""); //chuỗi rỗng
            }

            var products = data.SANPHAMs
                .Include("SANPHAM_BIENTHE")
                .Where(p => p.TENSP.Contains(keyword))
                .Take(5).ToList();

            return PartialView("_SearchSuggestion", products);
        }






        //About
        public ActionResult About()
        {
            return View();
        }











    }
}