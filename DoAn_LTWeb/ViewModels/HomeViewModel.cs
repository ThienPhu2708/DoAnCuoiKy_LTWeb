using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DoAn_LTWeb.Models;

namespace DoAn_LTWeb.ViewModels
{
	public class HomeViewModel
	{

		public List<LOAISANPHAM> ParentCategories { get; set; }
		public List<SANPHAM_BIENTHE> BestSeller { get; set; }
		public List<TINTUC> News { get; set; }

		public List<THUONGHIEU> Brands { get; set; }

    }
}