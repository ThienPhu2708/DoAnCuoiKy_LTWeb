using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DoAn_LTWeb.Models;

namespace DoAn_LTWeb.ViewModels
{
	public class MenuCategoryViewModel
	{
		public LOAISANPHAM Parent { get; set; }
        public List<LOAISANPHAM> Children { get; set; }

    }
}