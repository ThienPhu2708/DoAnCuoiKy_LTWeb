namespace DoAn_LTWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LOAISANPHAM")]
    public partial class LOAISANPHAM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LOAISANPHAM()
        {
            LOAISANPHAM1 = new HashSet<LOAISANPHAM>();
            SANPHAMs = new HashSet<SANPHAM>();
        }

        [Key]
        public int MALOAI { get; set; }

        [Required]
        [StringLength(100)]
        public string TENLOAI { get; set; }
        public string ANHDAIDIEN { get; set; }
        public int? MALOAICHA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOAISANPHAM> LOAISANPHAM1 { get; set; }

        public virtual LOAISANPHAM LOAISANPHAM2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SANPHAM> SANPHAMs { get; set; }
    }
}
