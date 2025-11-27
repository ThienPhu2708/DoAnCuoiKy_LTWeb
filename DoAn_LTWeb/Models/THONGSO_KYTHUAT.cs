using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DoAn_LTWeb.Models
{
    [Table("THONGSO_KYTHUAT")]
    public partial class THONGSO_KYTHUAT
    {
        [Key]
        public int ID { get; set; }

        [StringLength(150)]
        public string MASP { get; set; }

        [StringLength(100)]
        public string NHOM_THONGSO { get; set; }

        [StringLength(255)]
        public string TEN_THONGSO { get; set; }

        [StringLength(255)]
        public string GIATRI { get; set; }

        public virtual SANPHAM SANPHAM { get; set; }
    }
}