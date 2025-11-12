namespace DoAn_LTWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DONDATHANG")]
    public partial class DONDATHANG
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DONDATHANG()
        {
            CHITIETDONDATHANGs = new HashSet<CHITIETDONDATHANG>();
        }

        [Key]
        public int MADON { get; set; }

        public int? MAKH { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NGAYDAT { get; set; }

        public decimal? TONGTIEN { get; set; }

        [StringLength(50)]
        public string TRANGTHAIDON { get; set; }

        [StringLength(50)]
        public string PHUONGTHUCTHANHTOAN { get; set; }

        [StringLength(50)]
        public string MATHANHTOAN { get; set; }

        [StringLength(200)]
        public string DIACHI_GIAO { get; set; }

        [Required]
        [StringLength(100)]
        public string TENNGUOINHAN { get; set; }

        [Required]
        [StringLength(15)]
        public string SDT_GIAO { get; set; }

        [Required]
        [StringLength(100)]
        public string EMAIL { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHITIETDONDATHANG> CHITIETDONDATHANGs { get; set; }

        public virtual KHACHHANG KHACHHANG { get; set; }
    }
}
