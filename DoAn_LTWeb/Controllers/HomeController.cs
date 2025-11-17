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

            viewModel.ParentCategories = data.LOAISANPHAMs.Where(c => c.MALOAICHA == null).ToList();

            viewModel.BestSeller = data.SANPHAM_BIENTHE.Include("SANPHAM")
                                                        .OrderBy(b => b.GIABAN) 
                                                        .Take(9) 
                                                        .ToList();
            //tin tức
            viewModel.News = data.TINTUCs.OrderByDescending(n => n.NGAYDANG).Take(6).ToList();

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

            var headerViewModel = new HeaderViewModel
            {
                Categories = viewModel, // Gán list 1
                Brands = allBrands              // Gán list 2
            };
            return PartialView("_HeaderMenu",headerViewModel);
        }



    }
}