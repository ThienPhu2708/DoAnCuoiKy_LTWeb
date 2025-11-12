namespace DoAn_LTWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SANPHAM")]
    public partial class SANPHAM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SANPHAM()
        {
            DANHGIAs = new HashSet<DANHGIA>();
            LIST_ANHSP = new HashSet<LIST_ANHSP>();
            SANPHAM_BIENTHE = new HashSet<SANPHAM_BIENTHE>();
        }

        [Key]
        [StringLength(150)]
        public string MASP { get; set; }

        [Required]
        [StringLength(150)]
        public string TENSP { get; set; }

        public int? MALOAI { get; set; }

        public int? MATHUONGHIEU { get; set; }

        public string MOTA { get; set; }

        public double? DANHGIA { get; set; }

        [StringLength(150)]
        public string ANHBIA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DANHGIA> DANHGIAs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LIST_ANHSP> LIST_ANHSP { get; set; }

        public virtual LOAISANPHAM LOAISANPHAM { get; set; }

        public virtual THUONGHIEU THUONGHIEU { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SANPHAM_BIENTHE> SANPHAM_BIENTHE { get; set; }
    }
}
