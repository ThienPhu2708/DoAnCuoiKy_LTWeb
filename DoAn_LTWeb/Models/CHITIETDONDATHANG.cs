namespace DoAn_LTWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CHITIETDONDATHANG")]
    public partial class CHITIETDONDATHANG
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MADON { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MABIENTHE { get; set; }

        public int? SOLUONG { get; set; }

        public decimal? DONGIA { get; set; }

        public virtual SANPHAM_BIENTHE SANPHAM_BIENTHE { get; set; }

        public virtual DONDATHANG DONDATHANG { get; set; }
    }
}
