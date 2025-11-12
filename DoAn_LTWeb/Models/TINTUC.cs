namespace DoAn_LTWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TINTUC")]
    public partial class TINTUC
    {
        [Key]
        public int MATINTUC { get; set; }

        [Required]
        [StringLength(200)]
        public string TIEUDE { get; set; }

        public string NOIDUNG { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NGAYDANG { get; set; }

        [StringLength(150)]
        public string ANHTINTUC { get; set; }

        public int? MACHUYENMUC { get; set; }

        public int? MAVAITRO { get; set; }

        public virtual CHUYENMUC_TINTUC CHUYENMUC_TINTUC { get; set; }

        public virtual VAITRO VAITRO { get; set; }
    }
}
