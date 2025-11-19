using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DoAn_LTWeb.Models;



namespace DoAn_LTWeb.ViewModels
{
	public class ProductCreateViewModel
	{
        public SANPHAM SanPham { get; set; }
        public SANPHAM_BIENTHE BienThe { get; set; }
        public HttpPostedFileBase AnhBiaFile { get; set; }


        //THÊM LIST_ẢNH SẢN PHẨM
        public IEnumerable<HttpPostedFileBase> GalleryFiles { get; set; }


    

    }
}