using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn_LTWeb.Models;
using DoAn_LTWeb.ViewModels;
using System.Data.Entity;

namespace DoAn_LTWeb.Controllers
{
    public class StoreController : Controller
    {
        private INSTRUMENT db = new INSTRUMENT();

        public ActionResult Index(int? id, string search, decimal? minPrice, decimal? maxPrice, int? brandId)
        {
            if (id == null && string.IsNullOrEmpty(search))
            {
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new StoreViewModel();

            // Khởi tạo query lấy tất cả sản phẩm trước
            var productsQuery = db.SANPHAMs.Include(s => s.SANPHAM_BIENTHE)
                                           .Include(s => s.THUONGHIEU)
                                           .AsQueryable();

            // 2. Xử lý hiển thị Danh mục (nếu có chọn danh mục)
            if (id.HasValue)
            {
                viewModel.CurrentCategory = db.LOAISANPHAMs.Include(l => l.LOAISANPHAM2).FirstOrDefault(l => l.MALOAI == id);
                viewModel.SubCategories = db.LOAISANPHAMs.Where(l => l.MALOAICHA == id).ToList();

                // Lọc sản phẩm theo danh mục cha/con
                productsQuery = productsQuery.Where(p => p.MALOAI == id || p.LOAISANPHAM.MALOAICHA == id);
            }
            else
            {
                // Nếu tìm kiếm toàn cục (không có ID), tạo một danh mục ảo để hiển thị tiêu đề
                viewModel.CurrentCategory = new LOAISANPHAM { TENLOAI = $"Kết quả tìm kiếm: '{search}'" };
            }

            // 3. Áp dụng bộ lọc tìm kiếm (Keyword)
            if (!string.IsNullOrEmpty(search))
            {
                productsQuery = productsQuery.Where(p => p.TENSP.Contains(search) || p.MOTA.Contains(search));
                viewModel.SearchKeyword = search;
            }

            // 4. Áp dụng bộ lọc Thương hiệu
            if (brandId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.MATHUONGHIEU == brandId);
                viewModel.BrandId = brandId;
            }

            // 5. Áp dụng bộ lọc Giá
            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.SANPHAM_BIENTHE.Any(bt => bt.GIABAN >= minPrice.Value));
                viewModel.MinPrice = minPrice;
            }
            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.SANPHAM_BIENTHE.Any(bt => bt.GIABAN <= maxPrice.Value));
                viewModel.MaxPrice = maxPrice;
            }

            // Thực thi truy vấn
            viewModel.Products = productsQuery.ToList();
            viewModel.Brands = db.THUONGHIEUs.ToList();

            return View(viewModel);
        }
        public ActionResult Details(string masp)
        {
            if (string.IsNullOrEmpty(masp))
            {
                return RedirectToAction("Index", "Store");
            }

            var sanpham = db.SANPHAMs.Include(s => s.THUONGHIEU)
                                     .Include(s => s.LIST_ANHSP)
                                     .Include(s => s.SANPHAM_BIENTHE)
                                     .Include(s => s.LOAISANPHAM.LOAISANPHAM2)
                                     .FirstOrDefault(s => s.MASP == masp);

            if (sanpham == null)
            {
                return HttpNotFound();
            }
            ViewBag.RelatedProducts = db.SANPHAMs.Where(s => s.MALOAI == sanpham.MALOAI && s.MASP != masp)
                                                 .Take(4)
                                                 .ToList();

            return View(sanpham);
        }
    }
}