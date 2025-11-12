namespace DoAn_LTWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LIST_ANHSP
    {
        [Key]
        public int MA_HINHANH { get; set; }

        [StringLength(150)]
        public string MASP { get; set; }

        [Required]
        [StringLength(255)]
        public string URL_ANH { get; set; }

        public virtual SANPHAM SANPHAM { get; set; }
    }
}
