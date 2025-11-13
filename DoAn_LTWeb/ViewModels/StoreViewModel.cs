using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn_LTWeb.ViewModels
{
    public class StoreViewModel
    {
        // Danh mục hiện tại đang xem (Ví dụ: Đàn Piano)
        public LOAISANPHAM CurrentCategory { get; set; }

        // Danh sách các danh mục con cấp 2 (Grand, Upright, Digital...) để hiện ở trên cùng
        public List<LOAISANPHAM> SubCategories { get; set; }

        // Danh sách sản phẩm đã lọc
        public List<SANPHAM> Products { get; set; }

        // Danh sách thương hiệu để hiện checkbox bên trái
        public List<THUONGHIEU> Brands { get; set; }

        // Các tiêu chí lọc hiện tại (để giữ lại giá trị khi user bấm lọc)
        public string SearchKeyword { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? BrandId { get; set; }
    }
}