using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DoAn_LTWeb.Models;

namespace DoAn_LTWeb.ViewModels
{
	public class CategoryCreateViewModel
	{
        public LOAISANPHAM LoaiSanPham { get; set; }
        public HttpPostedFileBase AnhDaiDienFile { get; set; }
    }
}