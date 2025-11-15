using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DoAn_LTWeb.Models;

namespace DoAn_LTWeb.ViewModels
{
	public class ProductFilterViewModel
	{

		public List<SANPHAM_BIENTHE> Products { get; set; }
		public List<THUONGHIEU> Brands { get; set; }

    }
}