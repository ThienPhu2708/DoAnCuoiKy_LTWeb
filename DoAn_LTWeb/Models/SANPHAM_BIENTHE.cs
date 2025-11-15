namespace DoAn_LTWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SANPHAM_BIENTHE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SANPHAM_BIENTHE()
        {
            CHITIETDONDATHANGs = new HashSet<CHITIETDONDATHANG>();
        }

        [Key]
        public int MABIENTHE { get; set; }

        [StringLength(150)]
        public string MASP { get; set; }

        [StringLength(150)]
        public string TENBIENTHE { get; set; }

        public decimal? GIAGOC { get; set; }    

        public decimal? GIABAN { get; set; }

        public int? SOLUONGTON { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHITIETDONDATHANG> CHITIETDONDATHANGs { get; set; }

        public virtual SANPHAM SANPHAM { get; set; }


        public string ANH_BIENTHE { get; set; }


    }
}
