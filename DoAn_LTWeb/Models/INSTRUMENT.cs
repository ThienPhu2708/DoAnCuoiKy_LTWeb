namespace DoAn_LTWeb.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class INSTRUMENT : DbContext
    {
        public INSTRUMENT()
            : base("name=INSTRUMENT")
        {
        }

        public virtual DbSet<CHITIETDONDATHANG> CHITIETDONDATHANGs { get; set; }
        public virtual DbSet<CHUYENMUC_TINTUC> CHUYENMUC_TINTUC { get; set; }
        public virtual DbSet<DANHGIA> DANHGIAs { get; set; }
        public virtual DbSet<DONDATHANG> DONDATHANGs { get; set; }
        public virtual DbSet<KHACHHANG> KHACHHANGs { get; set; }
        public virtual DbSet<LIST_ANHSP> LIST_ANHSP { get; set; }
        public virtual DbSet<LOAISANPHAM> LOAISANPHAMs { get; set; }
        public virtual DbSet<SANPHAM> SANPHAMs { get; set; }
        public virtual DbSet<SANPHAM_BIENTHE> SANPHAM_BIENTHE { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<TINTUC> TINTUCs { get; set; }
        public virtual DbSet<THUONGHIEU> THUONGHIEUs { get; set; }
        public virtual DbSet<VAITRO> VAITROes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CHITIETDONDATHANG>()
                .Property(e => e.DONGIA)
                .HasPrecision(18, 0);

            modelBuilder.Entity<DANHGIA>()
                .Property(e => e.MASP)
                .IsUnicode(false);

            modelBuilder.Entity<DONDATHANG>()
                .Property(e => e.TONGTIEN)
                .HasPrecision(18, 0);

            modelBuilder.Entity<DONDATHANG>()
                .Property(e => e.SDT_GIAO)
                .IsUnicode(false);

            modelBuilder.Entity<DONDATHANG>()
                .Property(e => e.EMAIL)
                .IsUnicode(false);

            modelBuilder.Entity<KHACHHANG>()
                .Property(e => e.SDT)
                .IsUnicode(false);

            modelBuilder.Entity<KHACHHANG>()
                .Property(e => e.EMAIL)
                .IsUnicode(false);

            modelBuilder.Entity<LIST_ANHSP>()
                .Property(e => e.MASP)
                .IsUnicode(false);

            modelBuilder.Entity<LOAISANPHAM>()
                .HasMany(e => e.LOAISANPHAM1)
                .WithOptional(e => e.LOAISANPHAM2)
                .HasForeignKey(e => e.MALOAICHA);

            modelBuilder.Entity<SANPHAM>()
                .Property(e => e.MASP)
                .IsUnicode(false);

            modelBuilder.Entity<SANPHAM>()
                .HasMany(e => e.DANHGIAs)
                .WithOptional(e => e.SANPHAM)
                .WillCascadeOnDelete();

            modelBuilder.Entity<SANPHAM>()
                .HasMany(e => e.LIST_ANHSP)
                .WithOptional(e => e.SANPHAM)
                .WillCascadeOnDelete();

            modelBuilder.Entity<SANPHAM>()
                .HasMany(e => e.SANPHAM_BIENTHE)
                .WithOptional(e => e.SANPHAM)
                .WillCascadeOnDelete();

            modelBuilder.Entity<SANPHAM_BIENTHE>()
                .Property(e => e.MASP)
                .IsUnicode(false);

            modelBuilder.Entity<SANPHAM_BIENTHE>()
                .HasMany(e => e.CHITIETDONDATHANGs)
                .WithRequired(e => e.SANPHAM_BIENTHE)
                .WillCascadeOnDelete(false);
        }
    }
}
