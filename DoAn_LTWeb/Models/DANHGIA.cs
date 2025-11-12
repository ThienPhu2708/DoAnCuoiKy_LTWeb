namespace DoAn_LTWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DANHGIA")]
    public partial class DANHGIA
    {
        [Key]
        public int MADANHGIA { get; set; }

        [StringLength(150)]
        public string MASP { get; set; }

        public int? MAKH { get; set; }

        public int? SOSAO { get; set; }

        [StringLength(200)]
        public string NOIDUNG_DG { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NGAY_DG { get; set; }

        public virtual KHACHHANG KHACHHANG { get; set; }

        public virtual SANPHAM SANPHAM { get; set; }
    }
}
