using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoAn_LTWeb.Models;
using System.IO;
using System;

namespace DoAn_LTWeb.Controllers
{
    public class AdminController : Controller
    {
        INSTRUMENT db = new INSTRUMENT();
        public ActionResult Index()
        {
            ViewBag.Title = "Tổng quan";

            ViewBag.ProductCount = db.SANPHAMs.Count();

            ViewBag.CategoryCount = db.LOAISANPHAMs.Count();

            ViewBag.OrderCount = db.DONDATHANGs.Count();

            ViewBag.CustomerCount = db.KHACHHANGs.Count(k => k.MAVAITRO == 2);

            ViewBag.AccountCount = db.KHACHHANGs.Count(k => k.MAVAITRO == 1);

            ViewBag.NewReportCount = db.DONDATHANGs.Count(d => d.TRANGTHAIDON == "Chờ xử lý");

            return View();
        }
        public ActionResult Products(string searchQuery, int? categoryId, int? brandId)
        {
            var sANPHAMs = db.SANPHAMs.Include(s => s.LOAISANPHAM).Include(s => s.THUONGHIEU).AsQueryable();

            if (!String.IsNullOrEmpty(searchQuery))
            {
                sANPHAMs = sANPHAMs.Where(s => s.TENSP.Contains(searchQuery));
            }
            if (categoryId.HasValue)
            {
                sANPHAMs = sANPHAMs.Where(s => s.MALOAI == categoryId);
            }
            if (brandId.HasValue)
            {
                sANPHAMs = sANPHAMs.Where(s => s.MATHUONGHIEU == brandId);
            }

            ViewBag.MALOAI = new SelectList(db.LOAISANPHAMs, "MALOAI", "TENLOAI", categoryId);
            ViewBag.MATHUONGHIEU = new SelectList(db.THUONGHIEUs, "MATHUONGHIEU", "TENTHUONGHIEU", brandId);
            ViewBag.CurrentSearch = searchQuery;

            return View(sANPHAMs.ToList());
        }

        public ActionResult CreateProduct()
        {
            ViewBag.Title = "Thêm sản phẩm mới";
            ViewBag.MALOAI = new SelectList(db.LOAISANPHAMs, "MALOAI", "TENLOAI");
            ViewBag.MATHUONGHIEU = new SelectList(db.THUONGHIEUs, "MATHUONGHIEU", "TENTHUONGHIEU");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct([Bind(Include = "TENSP,MALOAI,MATHUONGHIEU,MOTA,DANHGIA")] SANPHAM product, HttpPostedFileBase fileUpload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (fileUpload != null && fileUpload.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(fileUpload.FileName);
                        _FileName = Guid.NewGuid().ToString().Substring(0, 8) + "_" + _FileName;

                        string _path = Path.Combine(Server.MapPath("~/Content/Assets/Images"), _FileName);

                        fileUpload.SaveAs(_path);

                        product.ANHBIA = _FileName;
                    }
                    else
                    {
                        product.ANHBIA = null;
                    }

                    product.MASP = Guid.NewGuid().ToString();

                    db.SANPHAMs.Add(product);
                    db.SaveChanges();
                    return RedirectToAction("Products");
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                var errorMessages = new System.Text.StringBuilder();
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string propertyName = validationError.PropertyName;
                        string errorMessage = validationError.ErrorMessage;
                        errorMessages.AppendFormat("Thuộc tính: {0}, Lỗi: {1}\n", propertyName, errorMessage);
                    }
                }
                throw new Exception("Lỗi Validation Chi Tiết: \n" + errorMessages.ToString(), dbEx);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
            }

            ViewBag.MALOAI = new SelectList(db.LOAISANPHAMs, "MALOAI", "TENLOAI", product.MALOAI);
            ViewBag.MATHUONGHIEU = new SelectList(db.THUONGHIEUs, "MATHUONGHIEU", "TENTHUONGHIEU", product.MATHUONGHIEU);
            return View(product);
        }


        public ActionResult EditProduct(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SANPHAM product = db.SANPHAMs.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.MALOAI = new SelectList(db.LOAISANPHAMs, "MALOAI", "TENLOAI", product.MALOAI);
            ViewBag.MATHUONGHIEU = new SelectList(db.THUONGHIEUs, "MATHUONGHIEU", "TENTHUONGHIEU", product.MATHUONGHIEU);

            ViewBag.Title = "Chỉnh sửa sản phẩm";
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct([Bind(Include = "MASP,TENSP,MALOAI,MATHUONGHIEU,MOTA,DANHGIA,ANHBIA")] SANPHAM product, HttpPostedFileBase fileUpload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (fileUpload != null && fileUpload.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(fileUpload.FileName);
                        _FileName = Guid.NewGuid().ToString().Substring(0, 8) + "_" + _FileName;

                        string _path = Path.Combine(Server.MapPath("~/Content/Assets/Images"), _FileName);
                        fileUpload.SaveAs(_path);

                        product.ANHBIA = _FileName;
                    }

                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Products");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
            }

            ViewBag.MALOAI = new SelectList(db.LOAISANPHAMs, "MALOAI", "TENLOAI", product.MALOAI);
            ViewBag.MATHUONGHIEU = new SelectList(db.THUONGHIEUs, "MATHUONGHIEU", "TENTHUONGHIEU", product.MATHUONGHIEU);
            return View(product);
        }

        public ActionResult DeleteProduct(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SANPHAM product = db.SANPHAMs.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProductConfirmed(string id)
        {
            SANPHAM product = db.SANPHAMs.Find(id);
            db.SANPHAMs.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Products");
        }

        public ActionResult Categories(string searchQuery, int? parentCategoryId)
        {
            var lOAISANPHAMs = db.LOAISANPHAMs.Include(l => l.LOAISANPHAM2).AsQueryable();

            if (!String.IsNullOrEmpty(searchQuery))
            {
                lOAISANPHAMs = lOAISANPHAMs.Where(l => l.TENLOAI.Contains(searchQuery));
            }

            if (parentCategoryId.HasValue)
            {
                lOAISANPHAMs = lOAISANPHAMs.Where(l => l.MALOAICHA == parentCategoryId);
            }

            ViewBag.ParentCategoryList = new SelectList(db.LOAISANPHAMs, "MALOAI", "TENLOAI", parentCategoryId);

            ViewBag.CurrentSearch = searchQuery;

            ViewBag.Title = "Quản lý Loại Sản Phẩm";
            return View(lOAISANPHAMs.ToList());
        }

        public ActionResult CreateCategory()
        {
            ViewBag.Title = "Thêm loại sản phẩm mới";
            ViewBag.MALOAICHA = new SelectList(db.LOAISANPHAMs, "MALOAI", "TENLOAI");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory([Bind(Include = "TENLOAI,MALOAICHA")] LOAISANPHAM loaiSanPham, HttpPostedFileBase fileUpload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (fileUpload != null && fileUpload.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(fileUpload.FileName);
                        _FileName = Guid.NewGuid().ToString().Substring(0, 8) + "_" + _FileName;

                        string _path = Path.Combine(Server.MapPath("~/Content/Assets/Images"), _FileName);

                        fileUpload.SaveAs(_path);

                        loaiSanPham.ANHDAIDIEN = _FileName;
                    }
                    else
                    {
                        loaiSanPham.ANHDAIDIEN = null;
                    }

                    db.LOAISANPHAMs.Add(loaiSanPham);
                    db.SaveChanges();
                    return RedirectToAction("Categories");
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                var errorMessages = new System.Text.StringBuilder();
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        errorMessages.AppendFormat("Thuộc tính: {0}, Lỗi: {1}\n", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                throw new Exception("Lỗi Validation Chi Tiết: \n" + errorMessages.ToString(), dbEx);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
            }

            ViewBag.MALOAICHA = new SelectList(db.LOAISANPHAMs, "MALOAI", "TENLOAI", loaiSanPham.MALOAICHA);
            return View(loaiSanPham);
        }

        public ActionResult EditCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOAISANPHAM loaiSanPham = db.LOAISANPHAMs.Find(id);
            if (loaiSanPham == null)
            {
                return HttpNotFound();
            }

            ViewBag.MALOAICHA = new SelectList(db.LOAISANPHAMs, "MALOAI", "TENLOAI", loaiSanPham.MALOAICHA);
            ViewBag.Title = "Chỉnh sửa loại sản phẩm";
            return View(loaiSanPham);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory([Bind(Include = "MALOAI,TENLOAI,ANHDAIDIEN,MALOAICHA")] LOAISANPHAM loaiSanPham, HttpPostedFileBase fileUpload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (fileUpload != null && fileUpload.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(fileUpload.FileName);
                        _FileName = Guid.NewGuid().ToString().Substring(0, 8) + "_" + _FileName;

                        string _path = Path.Combine(Server.MapPath("~/Content/Assets/Images"), _FileName);
                        fileUpload.SaveAs(_path);

                        loaiSanPham.ANHDAIDIEN = _FileName;
                    }

                    db.Entry(loaiSanPham).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Categories");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
            }

            ViewBag.MALOAICHA = new SelectList(db.LOAISANPHAMs, "MALOAI", "TENLOAI", loaiSanPham.MALOAICHA);
            return View(loaiSanPham);
        }

        public ActionResult DeleteCategory(int? id) 
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOAISANPHAM loaiSanPham = db.LOAISANPHAMs.Find(id);
            if (loaiSanPham == null)
            {
                return HttpNotFound();
            }
            return View(loaiSanPham);
        }

        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCategoryConfirmed(int id) 
        {
            LOAISANPHAM loaiSanPham = db.LOAISANPHAMs.Find(id);
            db.LOAISANPHAMs.Remove(loaiSanPham);
            db.SaveChanges();
            return RedirectToAction("Categories");
        }


        private SelectList GetOrderStatusList(string selectedValue = "")
        {
            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Chờ xử lý", Value = "Chờ xử lý" },
                new SelectListItem { Text = "Đã xác nhận", Value = "Đã xác nhận" },
                new SelectListItem { Text = "Đang giao hàng", Value = "Đang giao hàng" },
                new SelectListItem { Text = "Đã hoàn thành", Value = "Đã hoàn thành" },
                new SelectListItem { Text = "Đã hủy", Value = "Đã hủy" }
            };

            return new SelectList(statusList, "Value", "Text", selectedValue);
        }

        public ActionResult Orders(string searchQuery, string status)
        {
            ViewBag.Title = "Quản lý Đơn Hàng";
            var dONDATHANGs = db.DONDATHANGs.Include(d => d.KHACHHANG).AsQueryable();
            if (!String.IsNullOrEmpty(searchQuery))
            {
                int searchId = 0;
                int.TryParse(searchQuery, out searchId);

                dONDATHANGs = dONDATHANGs.Where(d =>
                    (d.MAKH.HasValue && d.MAKH == searchId) || 
                    d.TENNGUOINHAN.Contains(searchQuery) ||
                    d.SDT_GIAO.Contains(searchQuery) ||
                    d.EMAIL.Contains(searchQuery) ||
                    d.KHACHHANG.HOTEN.Contains(searchQuery)
                );
            }

            if (!String.IsNullOrEmpty(status))
            {
                dONDATHANGs = dONDATHANGs.Where(d => d.TRANGTHAIDON == status);
            }

            ViewBag.StatusList = GetOrderStatusList(status);

            ViewBag.CurrentSearch = searchQuery;

            return View(dONDATHANGs.ToList());
        }

        public ActionResult OrderDetails(int? id) 
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DONDATHANG donDatHang = db.DONDATHANGs
                .Include(d => d.KHACHHANG)
                .Include(d => d.CHITIETDONDATHANGs.Select(ct => ct.SANPHAM_BIENTHE.SANPHAM)) 
                .FirstOrDefault(d => d.MADON == id);

            if (donDatHang == null)
            {
                return HttpNotFound();
            }

            ViewBag.Title = "Chi tiết Đơn hàng";
            return View(donDatHang);
        }

        public ActionResult EditOrder(int? id) 
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DONDATHANG donDatHang = db.DONDATHANGs.Find(id);
            if (donDatHang == null)
            {
                return HttpNotFound();
            }

            ViewBag.Title = "Cập nhật Đơn hàng";
            ViewBag.TRANGTHAIDON = GetOrderStatusList(donDatHang.TRANGTHAIDON);
 
            ViewBag.MAKH = new SelectList(db.KHACHHANGs, "MAKH", "HOTEN", donDatHang.MAKH); 

            return View(donDatHang);
        }

        //---  CẬP NHẬT ĐƠN HÀNG  ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrder([Bind(Include = "MADON,MAKH,TRANGTHAIDON,PHUONGTHUCTHANHTOAN,MATHANHTOAN,DIACHI_GIAO,TENNGUOINHAN,SDT_GIAO,EMAIL")] DONDATHANG donDatHang)
        {

            if (ModelState.IsValid)
            {
                db.Entry(donDatHang).State = EntityState.Modified;
                db.Entry(donDatHang).Property(x => x.TONGTIEN).IsModified = false;
                db.Entry(donDatHang).Property(x => x.NGAYDAT).IsModified = false;

                db.SaveChanges();
                return RedirectToAction("Orders");
            }

            ViewBag.TRANGTHAIDON = GetOrderStatusList(donDatHang.TRANGTHAIDON);
            ViewBag.MAKH = new SelectList(db.KHACHHANGs, "MAKH", "HOTEN", donDatHang.MAKH);

            return View(donDatHang);
        }

        public ActionResult Customers(string searchQuery, int? roleId)
        {
            ViewBag.Title = "Quản lý Khách hàng";

            var kHACHHANGs = db.KHACHHANGs.Include(k => k.VAITRO).AsQueryable();

            if (!String.IsNullOrEmpty(searchQuery))
            {
                kHACHHANGs = kHACHHANGs.Where(k =>
                    k.HOTEN.Contains(searchQuery) ||
                    k.SDT.Contains(searchQuery) ||
                    k.EMAIL.Contains(searchQuery)
                );
            }
            if (roleId.HasValue)
            {
                kHACHHANGs = kHACHHANGs.Where(k => k.MAVAITRO == roleId);
            }

            ViewBag.RoleList = new SelectList(db.VAITROes, "MAVAITRO", "TENVAITRO", roleId);
            ViewBag.CurrentSearch = searchQuery;

            return View(kHACHHANGs.ToList());
        }

        public ActionResult CustomerDetails(int? id) 
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            KHACHHANG khachHang = db.KHACHHANGs
                .Include(k => k.VAITRO)
                .Include(k => k.DONDATHANGs) 
                .FirstOrDefault(k => k.MAKH == id);

            if (khachHang == null)
            {
                return HttpNotFound();
            }

            ViewBag.Title = "Chi tiết Khách hàng";
            return View(khachHang);
        }
        public ActionResult EditCustomer(int? id) 
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KHACHHANG khachHang = db.KHACHHANGs.Find(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }

            ViewBag.Title = "Chỉnh sửa Khách hàng";
            ViewBag.MAVAITRO = new SelectList(db.VAITROes, "MAVAITRO", "TENVAITRO", khachHang.MAVAITRO);
            return View(khachHang);
        }

        //--- HỈNH SỬA KHÁCH HÀNG  ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCustomer([Bind(Include = "MAKH,HOTEN,SDT,EMAIL,DIACHI_MACDINH,MAVAITRO")] KHACHHANG khachHang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(khachHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Customers");
            }

            ViewBag.MAVAITRO = new SelectList(db.VAITROes, "MAVAITRO", "TENVAITRO", khachHang.MAVAITRO);
            return View(khachHang);
        }


        public ActionResult Statistics()
        {
            ViewBag.Title = "Bảng điều khiển Thống kê";

            var completedOrders = db.DONDATHANGs.Where(d => d.TRANGTHAIDON == "Đã hoàn thành" && d.NGAYDAT.HasValue);

            DateTime today = DateTime.Today;
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
            DateTime startOfYear = new DateTime(today.Year, 1, 1);

            ViewBag.RevenueToday = completedOrders
                .Where(d => DbFunctions.TruncateTime(d.NGAYDAT) == today)
                .Sum(d => d.TONGTIEN) ?? 0;

            ViewBag.RevenueWeek = completedOrders
                .Where(d => d.NGAYDAT >= startOfWeek)
                .Sum(d => d.TONGTIEN) ?? 0;

            ViewBag.RevenueMonth = completedOrders
                .Where(d => d.NGAYDAT >= startOfMonth)
                .Sum(d => d.TONGTIEN) ?? 0;

            ViewBag.RevenueYear = completedOrders
                .Where(d => d.NGAYDAT >= startOfYear)
                .Sum(d => d.TONGTIEN) ?? 0;

            //--- 2. THỐNG KÊ ĐƠN HÀNG ---
            var ordersThisMonth = db.DONDATHANGs.Where(d => d.NGAYDAT >= startOfMonth);
            int totalOrdersMonth = ordersThisMonth.Count();
            ViewBag.TotalOrdersMonth = totalOrdersMonth; //

            ViewBag.NewOrdersMonth = ordersThisMonth
                .Count(d => d.TRANGTHAIDON == "Chờ xử lý"); // 

            ViewBag.CompletedOrdersMonth = ordersThisMonth
                .Count(d => d.TRANGTHAIDON == "Đã hoàn thành");

            int cancelledOrdersMonth = ordersThisMonth
                .Count(d => d.TRANGTHAIDON == "Đã hủy"); 
            ViewBag.CancelledOrdersMonth = cancelledOrdersMonth; 
            ViewBag.CancellationRateMonth = (totalOrdersMonth > 0)
                ? (double)cancelledOrdersMonth / totalOrdersMonth
                : 0;

            //---  BÁO CÁO SẢN PHẨM ---

            ViewBag.BestSellingProducts = db.CHITIETDONDATHANGs
                .Where(ct => ct.DONDATHANG.TRANGTHAIDON == "Đã hoàn thành")
                .GroupBy(ct => ct.SANPHAM_BIENTHE) 
                .Select(g => new {
                    ProductVariant = g.Key, 
                    TotalSold = g.Sum(ct => ct.SOLUONG) 
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(5)
                .ToList(); 

            ViewBag.LowStockProducts = db.SANPHAM_BIENTHE
                .Where(b => b.SOLUONGTON < 10 && b.SOLUONGTON > 0) 
                .OrderBy(b => b.SOLUONGTON)
                .Take(5)
                .ToList(); 

            // ---THÊM ĐƠN HÀNG GẦN ĐÂY ---
            ViewBag.RecentOrders = db.DONDATHANGs
                .Include(d => d.KHACHHANG) 
                .OrderByDescending(d => d.NGAYDAT)
                .Take(5) 
                .ToList();


            return View();
        }
        [HttpGet]
        public JsonResult GetRevenueChartData()
        {
            var labels = new List<string>();
            var data = new List<decimal>();
            DateTime today = DateTime.Today;

            for (int i = 6; i >= 0; i--)
            {
                DateTime date = today.AddDays(-i);
                DateTime nextDate = date.AddDays(1);
                var revenue = db.DONDATHANGs
                    .Where(d => d.TRANGTHAIDON == "Đã hoàn thành" &&
                                d.NGAYDAT >= date &&
                                d.NGAYDAT < nextDate)
                    .Sum(d => d.TONGTIEN) ?? 0;

                labels.Add(date.ToString("dd/MM"));
                data.Add(revenue);
            }

            return Json(new { labels, data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetOrderStatusChartData()
        {
            DateTime startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var data = db.DONDATHANGs
                .Where(d => d.NGAYDAT >= startOfMonth)
                .GroupBy(d => d.TRANGTHAIDON)
                .Select(g => new {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}