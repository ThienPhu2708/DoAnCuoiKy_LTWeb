using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn_LTWeb.ViewModels
{


	[Serializable]

    public class CartItemViewModel
	{
		public int MaBienThe { get; set; }
        public string TenSanPham { get; set; }
        public string TenBienThe { get; set; }
        public string HinhAnh { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }



        public decimal ThanhTien
        {
            get
            {
                return DonGia * SoLuong;
            }
        }




    }
}