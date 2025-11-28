using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn_LTWeb.Models;
using System.IO;
using System.Data.Entity;
using System.Net;
using DoAn_LTWeb.ViewModels;
using System.Data.SqlClient;
namespace DoAn_LTWeb.Controllers
{
    [AdminAuthorize] //form login bắt buộc 
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
                string lowerSearch = searchQuery.ToLower();
                sANPHAMs = sANPHAMs.Where(s => s.TENSP.ToLower().Contains(lowerSearch));
            }
            if (categoryId.HasValue)
            {
                var childrenIds = db.LOAISANPHAMs
                                    .Where(c => c.MALOAICHA == categoryId.Value)
                                    .Select(c => c.MALOAI)
                                    .ToList();
                childrenIds.Add(categoryId.Value);
                sANPHAMs = sANPHAMs.Where(s => s.MALOAI.HasValue && childrenIds.Contains(s.MALOAI.Value));
            }
            if (brandId.HasValue)
            {
                sANPHAMs = sANPHAMs.Where(s => s.MATHUONGHIEU == brandId);
            }

            ViewBag.MALOAI = new SelectList(db.LOAISANPHAMs.Where(l => l.MALOAICHA == null), "MALOAI", "TENLOAI", categoryId);
            ViewBag.MATHUONGHIEU = new SelectList(db.THUONGHIEUs, "MATHUONGHIEU", "TENTHUONGHIEU", brandId);
            ViewBag.CurrentSearch = searchQuery;



            return View(sANPHAMs.ToList());
        }

        public ActionResult CreateProduct()
        {
            ViewBag.Title = "Thêm sản phẩm mới";
            ViewBag.MALOAI = new SelectList(db.LOAISANPHAMs.Where(l => l.MALOAICHA != null), "MALOAI", "TENLOAI");
            ViewBag.MATHUONGHIEU = new SelectList(db.THUONGHIEUs, "MATHUONGHIEU", "TENTHUONGHIEU");

            var viewModel = new ProductCreateViewModel
            {
                SanPham = new SANPHAM(),
                BienThe = new SANPHAM_BIENTHE() 
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateProduct(ProductCreateViewModel viewModel)
        {
            ViewBag.Title = "Thêm sản phẩm mới";
            ViewBag.MALOAI = new SelectList(db.LOAISANPHAMs.Where(l => l.MALOAICHA != null), "MALOAI", "TENLOAI", viewModel.SanPham.MALOAI);
            ViewBag.MATHUONGHIEU = new SelectList(db.THUONGHIEUs, "MATHUONGHIEU", "TENTHUONGHIEU", viewModel.SanPham.MATHUONGHIEU);

            try
            {
                string tenFileAnh = null;
                if (viewModel.AnhBiaFile != null && viewModel.AnhBiaFile.ContentLength > 0)
                {
                    tenFileAnh = Path.GetFileName(viewModel.AnhBiaFile.FileName);
                    tenFileAnh = Guid.NewGuid().ToString().Substring(0, 8) + "_" + tenFileAnh;

                    string pathLuuFile = Path.Combine(Server.MapPath("~/Content/Assets/Product_Images"), tenFileAnh);
                    viewModel.AnhBiaFile.SaveAs(pathLuuFile);
                }
                //GỌI PROCEDURE THÊM SẢN PHẨM
                db.Database.ExecuteSqlCommand(
            "EXEC sp_THEMSANPHAM @MASP, @TENSP, @MALOAI, @MATHUONGHIEU, @MOTA, @ANHBIA, @TENBIENTHE, @GIAGOC, @GIABAN, @SOLUONGTON",
            new SqlParameter("@MASP", viewModel.SanPham.MASP),
            new SqlParameter("@TENSP", viewModel.SanPham.TENSP),
            new SqlParameter("@MALOAI", (object)viewModel.SanPham.MALOAI ?? DBNull.Value),
            new SqlParameter("@MATHUONGHIEU", (object)viewModel.SanPham.MATHUONGHIEU ?? DBNull.Value),
            new SqlParameter("@MOTA", (object)viewModel.SanPham.MOTA ?? DBNull.Value),
            new SqlParameter("@ANHBIA", (object)tenFileAnh ?? DBNull.Value),
            new SqlParameter("@TENBIENTHE", viewModel.BienThe.TENBIENTHE),
            new SqlParameter("@GIAGOC", viewModel.BienThe.GIAGOC),
            new SqlParameter("@GIABAN", viewModel.BienThe.GIABAN),
            new SqlParameter("@SOLUONGTON", viewModel.BienThe.SOLUONGTON)
        );


                //GỌI PROCEDURE CHO LIST_ANHSP
                if (viewModel.GalleryFiles != null && viewModel.GalleryFiles.Any())
                {
                    foreach (var file in viewModel.GalleryFiles)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            // a. Lưu file ảnh gallery
                            string tenFileGallery = Path.GetFileName(file.FileName);
                            tenFileGallery = Guid.NewGuid().ToString().Substring(0, 8) + "_" + tenFileGallery;
                            string pathLuuFileGallery = Path.Combine(Server.MapPath("~/Content/Assets/Product_Images"), tenFileGallery);
                            file.SaveAs(pathLuuFileGallery);

                            // b. GỌI SP_THEM_ANHSP cho từng ảnh
                            db.Database.ExecuteSqlCommand(
                                "EXEC SP_THEM_ANHSP @MASP, @URL_ANH",
                                new SqlParameter("@MASP", viewModel.SanPham.MASP),
                                new SqlParameter("@URL_ANH", tenFileGallery)
                            );
                        }
                    }
                }
                return RedirectToAction("Products");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
            }
            return View(viewModel);
        }

        public ActionResult EditProduct(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Tìm sản phẩm
            SANPHAM product = db.SANPHAMs.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách thông số kỹ thuật cũ để hiển thị lên bảng
            ViewBag.ListThongSo = db.THONGSO_KYTHUATs
                                    .Where(t => t.MASP == id)
                                    .OrderBy(t => t.NHOM_THONGSO) // Sắp xếp theo nhóm 
                                    .ToList();

            // Tìm biến thể (Ruột)
            SANPHAM_BIENTHE variant = db.SANPHAM_BIENTHE.FirstOrDefault(v => v.MASP == id);
            if (variant == null)
            {
                // Nếu chưa có ruột thì tạo ruột rỗng để không bị lỗi null bên View
                variant = new SANPHAM_BIENTHE { MASP = id };
            }

            // Đóng gói vào ViewModel
            var viewModel = new ProductCreateViewModel
            {
                SanPham = product,
                BienThe = variant
            };

            // Tạo Dropdown cho Loại và Thương hiệu
            ViewBag.MALOAI = new SelectList(db.LOAISANPHAMs.Where(l => l.MALOAICHA != null), "MALOAI", "TENLOAI", product.MALOAI);
            ViewBag.MATHUONGHIEU = new SelectList(db.THUONGHIEUs, "MATHUONGHIEU", "TENTHUONGHIEU", product.MATHUONGHIEU);

            ViewBag.Title = "Chỉnh sửa sản phẩm";
            return View(viewModel);
        }

        //MOI//
        [HttpPost]
        public JsonResult QuickAddBrand(string tenThuongHieu)
        {
            try
            {
                if (!string.IsNullOrEmpty(tenThuongHieu))
                {
                    // 1. Kiểm tra xem có trùng tên chưa
                    var exist = db.THUONGHIEUs.FirstOrDefault(t => t.TENTHUONGHIEU == tenThuongHieu);
                    if (exist != null)
                    {
                        return Json(new { success = false, message = "Thương hiệu này đã có rồi!" });
                    }

                    // 2. Tạo mới (Chỉ cần tên, mấy cái khác để null hoặc mặc định)
                    var brand = new THUONGHIEU();
                    brand.TENTHUONGHIEU = tenThuongHieu;
                    // brand.ANH = ... (Nếu bắt buộc ảnh thì phải gán ảnh mặc định ở đây)

                    db.THUONGHIEUs.Add(brand);
                    db.SaveChanges();

                    // 3. Trả về ID và Tên để Dropdown tự chọn
                    return Json(new { success = true, id = brand.MATHUONGHIEU, name = brand.TENTHUONGHIEU });
                }
                return Json(new { success = false, message = "Tên không được để trống" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] //  Cho phép lưu mã HTML từ CKEditor
        public ActionResult EditProduct(ProductCreateViewModel viewModel, HttpPostedFileBase fileUpload, string[] TS_Nhom, string[] TS_Ten, string[] TS_Giatri)
        {
            // Load lại Dropdown phòng khi lỗi phải trả về View
            ViewBag.MALOAI = new SelectList(db.LOAISANPHAMs.Where(l => l.MALOAICHA != null), "MALOAI", "TENLOAI", viewModel.SanPham.MALOAI);
            ViewBag.MATHUONGHIEU = new SelectList(db.THUONGHIEUs, "MATHUONGHIEU", "TENTHUONGHIEU", viewModel.SanPham.MATHUONGHIEU);

            // Kiểm tra dữ liệu đầu vào
            if (!ModelState.IsValid) { return View(viewModel); }

            // 2. LOGIC XỬ LÝ ẢNH BÌA (MỚI THÊM)
            string tenFileAnh = viewModel.SanPham.ANHBIA; // Mặc định lấy ảnh cũ

            // Kiểm tra: Nếu có chọn ảnh mới (fileUpload không null)
            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                // a. Tạo tên file mới
                string _FileName = Path.GetFileName(fileUpload.FileName);
                _FileName = Guid.NewGuid().ToString().Substring(0, 8) + "_" + _FileName;

                // b. Lưu file vào server
                string _path = Path.Combine(Server.MapPath("~/Content/Assets/Product_Images"), _FileName);
                fileUpload.SaveAs(_path);

                // c. Cập nhật tên file mới vào biến để tí nữa lưu xuống SQL
                tenFileAnh = _FileName;
            }

            try
            {
                // XỬ LÝ LƯU SẢN PHẨM CHÍNH (GỌI STORED PROCEDURE)
                db.Database.ExecuteSqlCommand(
                    "EXEC SP_SUASANPHAM @MASP, @MABIENTHE, @TENSP, @MALOAI, @MATHUONGHIEU, @MOTA, @ANHBIA, @TENBIENTHE, @GIAGOC, @GIABAN, @SOLUONGTON",
                    new SqlParameter("@MASP", viewModel.SanPham.MASP),
                    new SqlParameter("@MABIENTHE", viewModel.BienThe.MABIENTHE),

                    new SqlParameter("@TENSP", viewModel.SanPham.TENSP),
                    new SqlParameter("@MALOAI", (object)viewModel.SanPham.MALOAI ?? DBNull.Value),
                    new SqlParameter("@MATHUONGHIEU", (object)viewModel.SanPham.MATHUONGHIEU ?? DBNull.Value),
                    new SqlParameter("@MOTA", (object)viewModel.SanPham.MOTA ?? DBNull.Value),
                    new SqlParameter("@ANHBIA", (object)tenFileAnh ?? DBNull.Value),

                    // Xử lý Null cho Biến thể (Quan trọng)
                    new SqlParameter("@TENBIENTHE", (object)viewModel.BienThe.TENBIENTHE ?? DBNull.Value),
                    new SqlParameter("@GIAGOC", (object)viewModel.BienThe.GIAGOC ?? 0),
                    new SqlParameter("@GIABAN", (object)viewModel.BienThe.GIABAN ?? 0),
                    new SqlParameter("@SOLUONGTON", (object)viewModel.BienThe.SOLUONGTON ?? 0)
                );

                // XỬ LÝ ẢNH GALLERY (NẾU CÓ UP MỚI)
                if (viewModel.GalleryFiles != null && viewModel.GalleryFiles.Any())
                {
                    foreach (var file in viewModel.GalleryFiles)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            string tenFileGallery = Path.GetFileName(file.FileName);
                            tenFileGallery = Guid.NewGuid().ToString().Substring(0, 8) + "_" + tenFileGallery;
                            string pathLuuFileGallery = Path.Combine(Server.MapPath("~/Content/Assets/Product_Images"), tenFileGallery);
                            file.SaveAs(pathLuuFileGallery);

                            db.Database.ExecuteSqlCommand(
                                "EXEC SP_THEM_ANHSP @MASP, @URL_ANH",
                                new SqlParameter("@MASP", viewModel.SanPham.MASP),
                                new SqlParameter("@URL_ANH", tenFileGallery)
                            );
                        }
                    }
                }

                // XỬ LÝ LƯU THÔNG SỐ KỸ THUẬT (MỚI THÊM)
                // Bước 1: Xóa sạch thông số cũ của SP này đi
                var oldSpecs = db.THONGSO_KYTHUATs.Where(t => t.MASP == viewModel.SanPham.MASP);
                db.THONGSO_KYTHUATs.RemoveRange(oldSpecs);
                db.SaveChanges();

                // Bước 2: Thêm lại danh sách mới từ Mảng (Array) gửi lên
                if (TS_Ten != null && TS_Giatri != null)
                {
                    for (int i = 0; i < TS_Ten.Length; i++)
                    {
                        // Chỉ lưu những dòng có dữ liệu (Tên và Giá trị không rỗng)
                        if (!string.IsNullOrEmpty(TS_Ten[i]) && !string.IsNullOrEmpty(TS_Giatri[i]))
                        {
                            var spec = new THONGSO_KYTHUAT();
                            spec.MASP = viewModel.SanPham.MASP;
                            // Lấy nhóm tương ứng (nếu có)
                            spec.NHOM_THONGSO = (TS_Nhom != null && TS_Nhom.Length > i) ? TS_Nhom[i] : null;
                            spec.TEN_THONGSO = TS_Ten[i];
                            spec.GIATRI = TS_Giatri[i];

                            db.THONGSO_KYTHUATs.Add(spec);
                        }
                    }
                    db.SaveChanges(); // Lưu đợt 2 (Lưu thông số)
                }
                TempData["ThongBao"] = "Đã lưu thay đổi thành công!";
                // LƯU XONG QUAY LẠI TRANG EDIT ĐỂ SỬA TIẾP (KHÔNG VĂNG RA LIST)
                return RedirectToAction("EditProduct", new { id = viewModel.SanPham.MASP });
            }
            catch (Exception ex)
            {
                // Nếu lỗi thì hiện thông báo đỏ lên Form
                ModelState.AddModelError("", "Lỗi CSDL: " + ex.Message);
            }

            // Nếu lỗi thì trả về View cũ kèm dữ liệu để người dùng sửa lại
            return View(viewModel);
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

            if (parentCategoryId.HasValue)
            {
                // Nếu có ID (đang xem Cha), thì HIỆN CON
                lOAISANPHAMs = lOAISANPHAMs.Where(l => l.MALOAICHA == parentCategoryId.Value);

                // (Lấy thông tin của Cha để hiển thị tiêu đề)
                var parentCat = db.LOAISANPHAMs.Find(parentCategoryId.Value);
                ViewBag.Title = "Quản lý Loại (Con của: " + parentCat.TENLOAI + ")";
            }
            else
            {
                // Nếu KHÔNG có ID (mặc định), thì CHỈ HIỆN CHA
                lOAISANPHAMs = lOAISANPHAMs.Where(l => l.MALOAICHA == null);
                ViewBag.Title = "Quản lý Loại Sản Phẩm (Cha)";
            }

            ViewBag.ParentCategoryList = new SelectList(
                    db.LOAISANPHAMs.Where(l => l.MALOAICHA == null),
                    "MALOAI", "TENLOAI",
                    parentCategoryId
                );

            ViewBag.CurrentSearch = searchQuery;

            return View(lOAISANPHAMs.OrderBy(l => l.TENLOAI).ToList());
        }



        public ActionResult CreateCategory()
        {
            ViewBag.Title = "Thêm loại sản phẩm mới";
            ViewBag.MALOAICHA = new SelectList(db.LOAISANPHAMs.Where(l => l.MALOAICHA == null), "MALOAI", "TENLOAI");
            var viewModel = new CategoryCreateViewModel
            {
                LoaiSanPham = new LOAISANPHAM()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory(CategoryCreateViewModel viewModel)
        {
            ViewBag.MALOAICHA = new SelectList(db.LOAISANPHAMs.Where(l => l.MALOAICHA == null), "MALOAI", "TENLOAI", viewModel.LoaiSanPham.MALOAICHA);
            try
            {
                string tenFileAnh = null;
                if (viewModel.AnhDaiDienFile != null && viewModel.AnhDaiDienFile.ContentLength > 0)
                {
                    tenFileAnh = Path.GetFileName(viewModel.AnhDaiDienFile.FileName);
                    string pathLuuFile = Path.Combine(Server.MapPath("~/Content/Assets/Product_Images"), tenFileAnh);
                    viewModel.AnhDaiDienFile.SaveAs(pathLuuFile);
                }
                db.Database.ExecuteSqlCommand(
                    "EXEC sp_THEMLOAISANPHAM @TENLOAI, @MALOAICHA, @ANHDAIDIEN",
                    new SqlParameter("@TENLOAI", viewModel.LoaiSanPham.TENLOAI),
                    new SqlParameter("@MALOAICHA", (object)viewModel.LoaiSanPham.MALOAICHA ?? DBNull.Value),
                    new SqlParameter("@ANHDAIDIEN", (object)tenFileAnh ?? DBNull.Value)
                );
                return RedirectToAction("Categories");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi CSDL: " + ex.Message);
            }
                return View(viewModel);
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

                        string _path = Path.Combine(Server.MapPath("~/Content/Assets/Product_Images"), _FileName);
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
                searchQuery = searchQuery.Trim();
                int searchId = 0;
                bool isNumber = int.TryParse(searchQuery, out searchId);

                dONDATHANGs = dONDATHANGs.Where(d =>
                    (isNumber && d.MADON == searchId) ||
                    (d.MAKH.HasValue && isNumber && d.MAKH == searchId) ||
                    d.TENNGUOINHAN.Contains(searchQuery) ||
                    d.SDT_GIAO.Contains(searchQuery) ||
                    d.EMAIL.Contains(searchQuery) ||
                    (d.KHACHHANG != null && d.KHACHHANG.HOTEN.Contains(searchQuery))
                );
            }

            if (!String.IsNullOrEmpty(status))
            {
                dONDATHANGs = dONDATHANGs.Where(d => d.TRANGTHAIDON == status);
            }

            // Thông báo khi lọc không có trạng thái
            if (!dONDATHANGs.Any() && !String.IsNullOrEmpty(status))
            {
                ViewBag.NotifyMessage = "Không tìm thấy đơn hàng nào với trạng thái: " + status;
            }

            ViewBag.StatusList = GetOrderStatusList(status);
            ViewBag.CurrentSearch = searchQuery;
            ViewBag.CurrentStatus = status;

            return View(dONDATHANGs.OrderByDescending(d => d.NGAYDAT).ToList());
        }

        public ActionResult OrderDetails(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            DONDATHANG donDatHang = db.DONDATHANGs
                .Include(d => d.KHACHHANG)
                .Include(d => d.CHITIETDONDATHANGs.Select(ct => ct.SANPHAM_BIENTHE.SANPHAM))
                .FirstOrDefault(d => d.MADON == id);

            if (donDatHang == null) return HttpNotFound();

            ViewBag.Title = "Chi tiết Đơn hàng";
            return View(donDatHang);
        }


        public ActionResult EditOrder(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            DONDATHANG donDatHang = db.DONDATHANGs.Find(id);
            if (donDatHang == null) return HttpNotFound();

            ViewBag.Title = "Cập nhật Đơn hàng";
            ViewBag.TRANGTHAIDON = GetOrderStatusList(donDatHang.TRANGTHAIDON);

            ViewBag.MAKH = new SelectList(
                db.KHACHHANGs.Where(k => k.MAVAITRO == 2),
                "MAKH", "HOTEN", donDatHang.MAKH
            );

            return View(donDatHang);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrder([Bind(Include = "MADON,MAKH,TRANGTHAIDON,PHUONGTHUCTHANHTOAN,MATHANHTOAN,DIACHI_GIAO,TENNGUOINHAN,SDT_GIAO,EMAIL")] DONDATHANG donDatHang)
        {
            if (ModelState.IsValid)
            {
                // Lấy trạng thái cũ và dữ liệu cũ để so sánh
                var oldOrder = db.DONDATHANGs.AsNoTracking().FirstOrDefault(d => d.MADON == donDatHang.MADON);

                // [UPDATE] Cập nhật thông tin người nhận nếu thay đổi Khách hàng
                if (donDatHang.MAKH != oldOrder.MAKH && donDatHang.MAKH.HasValue)
                {
                    var khachHangMoi = db.KHACHHANGs.Find(donDatHang.MAKH);
                    if (khachHangMoi != null)
                    {
                        donDatHang.TENNGUOINHAN = khachHangMoi.HOTEN;
                        donDatHang.SDT_GIAO = khachHangMoi.SDT;
                        donDatHang.EMAIL = khachHangMoi.EMAIL;
                        if (!string.IsNullOrEmpty(khachHangMoi.DIACHI_MACDINH))
                        {
                            donDatHang.DIACHI_GIAO = khachHangMoi.DIACHI_MACDINH;
                        }
                    }
                }

                db.Entry(donDatHang).State = EntityState.Modified;
                db.Entry(donDatHang).Property(x => x.TONGTIEN).IsModified = false;
                db.Entry(donDatHang).Property(x => x.NGAYDAT).IsModified = false;

                if (donDatHang.TRANGTHAIDON == "Đã hủy" && oldOrder.TRANGTHAIDON != "Đã hủy")
                {
                    var chiTietDon = db.CHITIETDONDATHANGs.Where(ct => ct.MADON == donDatHang.MADON).ToList();
                    foreach (var item in chiTietDon)
                    {
                        var bienThe = db.SANPHAM_BIENTHE.Find(item.MABIENTHE);
                        if (bienThe != null)
                        {
                            bienThe.SOLUONGTON += item.SOLUONG;
                            db.Entry(bienThe).State = EntityState.Modified;
                        }
                    }
                }
                else if (oldOrder.TRANGTHAIDON == "Đã hủy" && donDatHang.TRANGTHAIDON != "Đã hủy")
                {
                    var chiTietDon = db.CHITIETDONDATHANGs.Where(ct => ct.MADON == donDatHang.MADON).ToList();
                    foreach (var item in chiTietDon)
                    {
                        var bienThe = db.SANPHAM_BIENTHE.Find(item.MABIENTHE);
                        if (bienThe != null)
                        {
                            bienThe.SOLUONGTON -= item.SOLUONG;
                            db.Entry(bienThe).State = EntityState.Modified;
                        }
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Orders");
            }

            ViewBag.TRANGTHAIDON = GetOrderStatusList(donDatHang.TRANGTHAIDON);
            ViewBag.MAKH = new SelectList(db.KHACHHANGs.Where(k => k.MAVAITRO == 2), "MAKH", "HOTEN", donDatHang.MAKH);

            return View(donDatHang);
        }




        //QUẢN LÝ KHÁCH HÀNG

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCustomer([Bind(Include = "MAKH,HOTEN,SDT,EMAIL,DIACHI_MACDINH")] KHACHHANG khachHang)
        {
            if (ModelState.IsValid)
            {
                var existingCustomer = db.KHACHHANGs.Find(khachHang.MAKH);
                if (existingCustomer != null)
                {
                    existingCustomer.HOTEN = khachHang.HOTEN;
                    existingCustomer.SDT = khachHang.SDT;
                    existingCustomer.EMAIL = khachHang.EMAIL;
                    existingCustomer.DIACHI_MACDINH = khachHang.DIACHI_MACDINH;

                    db.SaveChanges();
                }
                return RedirectToAction("Customers");
            }

            ViewBag.MAVAITRO = new SelectList(db.VAITROes, "MAVAITRO", "TENVAITRO", 1);
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemThongSo(string masp, string nhom, string ten, string giatri)
        {
            if (!string.IsNullOrEmpty(masp) && !string.IsNullOrEmpty(ten))
            {
                var ts = new THONGSO_KYTHUAT();
                ts.MASP = masp;
                ts.NHOM_THONGSO = nhom;
                ts.TEN_THONGSO = ten;
                ts.GIATRI = giatri;

                db.THONGSO_KYTHUATs.Add(ts);
                db.SaveChanges();
            }
            // Load lại trang Edit
            return RedirectToAction("EditProduct", new { id = masp });
        }

        public ActionResult XoaThongSo(int id)
        {
            var ts = db.THONGSO_KYTHUATs.Find(id);
            if (ts != null)
            {
                string masp = ts.MASP;
                db.THONGSO_KYTHUATs.Remove(ts);
                db.SaveChanges();
                return RedirectToAction("EditProduct", new { id = masp });
            }
            return RedirectToAction("Products");
        }
        public ActionResult SuaThongSo(int id)
        {
            var ts = db.THONGSO_KYTHUATs.Find(id);
            if (ts == null) return HttpNotFound();

            ViewBag.Title = "Cập nhật thông số";
            return View(ts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaThongSo(THONGSO_KYTHUAT model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("EditProduct", new { id = model.MASP });
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult ProcessUpload(HttpPostedFileBase upload)
        {
            if (upload != null && upload.ContentLength > 0)
            {
                // 1. Đặt tên file ảnh (Dùng Guid để không bị trùng tên)
                string tenFile = Guid.NewGuid().ToString() + "_" + Path.GetFileName(upload.FileName);

                // 2. Lưu vào thư mục ảnh sản phẩm cũ của bạn
                string path = Path.Combine(Server.MapPath("~/Content/Assets/Product_Images"), tenFile);
                upload.SaveAs(path);

                // 3. Trả về JSON theo đúng chuẩn mà CKEditor yêu cầu
                // (Nó bắt buộc phải trả về: uploaded = 1 và url = đường dẫn ảnh)
                return Json(new
                {
                    uploaded = 1,
                    fileName = tenFile,
                    url = "/Content/Assets/Product_Images/" + tenFile
                });
            }

            // Nếu lỗi
            return Json(new { uploaded = 0, error = new { message = "Lỗi tải ảnh lên server!" } });
        }










    }
}