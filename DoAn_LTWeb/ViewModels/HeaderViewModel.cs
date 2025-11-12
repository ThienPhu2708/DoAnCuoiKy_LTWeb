using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DoAn_LTWeb.Models;

namespace DoAn_LTWeb.ViewModels
{
	public class HeaderViewModel
	{

		public List<MenuCategoryViewModel> Categories { get; set; }
		public List<THUONGHIEU> Brands { get; set; }


    }
}