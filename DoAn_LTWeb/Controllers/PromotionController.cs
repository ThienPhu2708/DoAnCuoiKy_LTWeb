using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn_LTWeb.Models;
using DoAn_LTWeb.ViewModels;

namespace DoAn_LTWeb.Controllers
{
    public class PromotionController : Controller
    {
       INSTRUMENT data = new INSTRUMENT();

        public ActionResult Index(List<int> brandIds, string priceRange)
        {
            ViewBag.Title = "Sản phẩm Khuyến mãi";
            // Fix the problematic line by replacing the lambda expression with a string path
            IQueryable<SANPHAM_BIENTHE> query = data.SANPHAM_BIENTHE
                .Include("SANPHAM")
                .Include("SANPHAM.LIST_ANHSP") // Use string-based Include for navigation properties
                .Where(b => b.GIAGOC.HasValue && b.GIABAN.HasValue && b.GIAGOC > b.GIABAN);

            // Lọc theo Hãng
            if (brandIds != null && brandIds.Any())
            {
                query = query.Where(b => brandIds.Contains(b.SANPHAM.MATHUONGHIEU ?? 0));
            }

            // Lọc theo Giá
            if (!string.IsNullOrEmpty(priceRange))
            {
                switch (priceRange)
                {
                    case "1": 
                        query = query.Where(b => b.GIABAN < 10000000);
                        break;
                    case "2": 
                        query = query.Where(b => b.GIABAN >= 10000000 && b.GIABAN <= 20000000);
                        break;
                    case "3":
                        query = query.Where(b => b.GIABAN >= 20000000 && b.GIABAN <= 40000000);
                        break;
                    case "4":
                        query = query.Where(b => b.GIABAN >= 40000000 && b.GIABAN <= 100000000);
                        break;
                    case "5":
                        query = query.Where(b => b.GIABAN >= 100000000 && b.GIABAN <= 300000000);
                        break;
                    case "6":
                        query = query.Where(b => b.GIABAN > 300000000);
                        break;
                }
            }


            var brands = data.THUONGHIEUs.OrderBy(t => t.TENTHUONGHIEU).ToList();
            var viewModel = new ProductFilterViewModel
            {
                Products = query.OrderByDescending(b => (b.GIAGOC - b.GIABAN)).ToList(),
                Brands = brands
            };

            //gửi các giá trị lọc về ViewBag
            ViewBag.ActiveBrandIds = brandIds;
            ViewBag.ActivePriceRange = priceRange;


            return View(viewModel);
        }





    }
}