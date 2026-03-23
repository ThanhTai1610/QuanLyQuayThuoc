using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Models;

namespace QuanLyQuayThuoc.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<ChiTietKiemKe> ChiTietKiemKes { get; set; }

    public virtual DbSet<ChuDeSucKhoe> ChuDeSucKhoes { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<DonViTinh> DonViTinhs { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<HinhAnhThuoc> HinhAnhThuocs { get; set; }

    public virtual DbSet<LichSuChatbot> LichSuChatbots { get; set; }

    public virtual DbSet<LoHang> LoHangs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<PhieuKiemKe> PhieuKiemKes { get; set; }

    public virtual DbSet<SoDiaChi> SoDiaChis { get; set; }

    public virtual DbSet<Thuoc> Thuocs { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.MaChiTiet).HasName("PK__ChiTietD__CDF0A114511AB443");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.GiaBanTaiThoiDiem).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaDvt).HasColumnName("MaDVT");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK__ChiTietDo__MaDon__68487DD7");

            entity.HasOne(d => d.MaDvtNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDvt)
                .HasConstraintName("FK__ChiTietDo__MaDVT__6A30C649");

            entity.HasOne(d => d.MaLoNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaLo)
                .HasConstraintName("FK__ChiTietDon__MaLo__693CA210");
        });

        modelBuilder.Entity<ChiTietKiemKe>(entity =>
        {
            entity.HasKey(e => new { e.MaPhieu, e.MaLo }).HasName("PK__ChiTietK__5412E395EA49E629");

            entity.ToTable("ChiTietKiemKe");

            entity.Property(e => e.LyDoLech).HasMaxLength(255);

            entity.HasOne(d => d.MaLoNavigation).WithMany(p => p.ChiTietKiemKes)
                .HasForeignKey(d => d.MaLo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietKie__MaLo__60A75C0F");

            entity.HasOne(d => d.MaPhieuNavigation).WithMany(p => p.ChiTietKiemKes)
                .HasForeignKey(d => d.MaPhieu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietKi__MaPhi__5FB337D6");
        });

        modelBuilder.Entity<ChuDeSucKhoe>(entity =>
        {
            entity.HasKey(e => e.MaChuDe).HasName("PK__ChuDeSuc__3585451132964912");

            entity.ToTable("ChuDeSucKhoe");

            entity.Property(e => e.TenChuDe).HasMaxLength(100);
            entity.Property(e => e.TieuDePhu).HasMaxLength(200);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.MaDanhMuc).HasName("PK__DanhMuc__B3750887319BDF1A");

            entity.ToTable("DanhMuc");

            entity.Property(e => e.Icon)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenDanhMuc).HasMaxLength(100);
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang).HasName("PK__DonHang__129584AD23635ABC");

            entity.ToTable("DonHang");

            entity.Property(e => e.AnhDonThuoc)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DiaChiGiaoHang).HasMaxLength(500);
            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhuongThucThanhToan).HasMaxLength(50);
            entity.Property(e => e.SoDienThoaiNhan)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.DonHangMaKhachHangNavigations)
                .HasForeignKey(d => d.MaKhachHang)
                .HasConstraintName("FK__DonHang__MaKhach__6383C8BA");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.DonHangMaNhanVienNavigations)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("FK__DonHang__MaNhanV__6477ECF3");
        });

        modelBuilder.Entity<DonViTinh>(entity =>
        {
            entity.HasKey(e => e.MaDvt).HasName("PK__DonViTin__3D895AFE1BB90994");

            entity.ToTable("DonViTinh");

            entity.Property(e => e.MaDvt).HasColumnName("MaDVT");
            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TenDonVi).HasMaxLength(20);

            entity.HasOne(d => d.MaThuocNavigation).WithMany(p => p.DonViTinhs)
                .HasForeignKey(d => d.MaThuoc)
                .HasConstraintName("FK__DonViTinh__MaThu__5535A963");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.MaGioHang).HasName("PK__GioHang__F5001DA37BAF0A6A");

            entity.ToTable("GioHang");

            entity.Property(e => e.MaDvt).HasColumnName("MaDVT");

            entity.HasOne(d => d.MaDvtNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaDvt)
                .HasConstraintName("FK__GioHang__MaDVT__6EF57B66");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaKhachHang)
                .HasConstraintName("FK__GioHang__MaKhach__6D0D32F4");

            entity.HasOne(d => d.MaThuocNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaThuoc)
                .HasConstraintName("FK__GioHang__MaThuoc__6E01572D");
        });

        modelBuilder.Entity<HinhAnhThuoc>(entity =>
        {
            entity.HasKey(e => e.MaHinh).HasName("PK__HinhAnhT__13EE10841389A792");

            entity.ToTable("HinhAnhThuoc");

            entity.Property(e => e.DuongDan)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.MaThuocNavigation).WithMany(p => p.HinhAnhThuocs)
                .HasForeignKey(d => d.MaThuoc)
                .HasConstraintName("FK__HinhAnhTh__MaThu__52593CB8");
        });

        modelBuilder.Entity<LichSuChatbot>(entity =>
        {
            entity.HasKey(e => e.MaChat).HasName("PK__LichSuCh__1B56CB6B505800A4");

            entity.ToTable("LichSuChatbot");

            entity.Property(e => e.ThoiGian)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.LichSuChatbots)
                .HasForeignKey(d => d.MaKhachHang)
                .HasConstraintName("FK__LichSuCha__MaKha__71D1E811");
        });

        modelBuilder.Entity<LoHang>(entity =>
        {
            entity.HasKey(e => e.MaLo).HasName("PK__LoHang__2725C75608560B53");

            entity.ToTable("LoHang");

            entity.Property(e => e.GiaNhap).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SoLo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.MaThuocNavigation).WithMany(p => p.LoHangs)
                .HasForeignKey(d => d.MaThuoc)
                .HasConstraintName("FK__LoHang__MaThuoc__5812160E");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D762AE933BA2");

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.SoDienThoai, "UQ__NguoiDun__0389B7BD927BD88F").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__NguoiDun__A9D1053484A3F442").IsUnique();

            entity.Property(e => e.AnhDaiDien)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("Hoạt động");

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.MaVaiTro)
                .HasConstraintName("FK__NguoiDung__MaVai__3B75D760");
        });

        modelBuilder.Entity<PhieuKiemKe>(entity =>
        {
            entity.HasKey(e => e.MaPhieu).HasName("PK__PhieuKie__2660BFE0E83E705F");

            entity.ToTable("PhieuKiemKe");

            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.NgayKiem)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Hoàn tất");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.PhieuKiemKes)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("FK__PhieuKiem__MaNha__5BE2A6F2");
        });

        modelBuilder.Entity<SoDiaChi>(entity =>
        {
            entity.HasKey(e => e.MaDiaChi).HasName("PK__SoDiaChi__EB61213E0D918E59");

            entity.ToTable("SoDiaChi");

            entity.Property(e => e.DiaChiChiTiet).HasMaxLength(255);
            entity.Property(e => e.HoTenNguoiNhan).HasMaxLength(100);
            entity.Property(e => e.LaMacDinh).HasDefaultValue(false);
            entity.Property(e => e.LoaiDiaChi).HasMaxLength(20);
            entity.Property(e => e.PhuongXa).HasMaxLength(100);
            entity.Property(e => e.QuanHuyen).HasMaxLength(100);
            entity.Property(e => e.SoDienThoaiNhan)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TinhThanh).HasMaxLength(100);

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.SoDiaChis)
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK__SoDiaChi__MaNguo__403A8C7D");
        });

        modelBuilder.Entity<Thuoc>(entity =>
        {
            entity.HasKey(e => e.MaThuoc).HasName("PK__Thuoc__4BB1F6204ED9346A");

            entity.ToTable("Thuoc");

            entity.Property(e => e.DangBaoChe).HasMaxLength(100);
            entity.Property(e => e.HinhAnhChinh)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LaThuocKeDon).HasDefaultValue(false);
            entity.Property(e => e.LuotXem).HasDefaultValue(0);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NhaSanXuat).HasMaxLength(100);
            entity.Property(e => e.NuocSanXuat).HasMaxLength(100);
            entity.Property(e => e.QuyCach).HasMaxLength(100);
            entity.Property(e => e.SoDangKy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TenThuoc).HasMaxLength(200);

            entity.HasOne(d => d.MaDanhMucNavigation).WithMany(p => p.Thuocs)
                .HasForeignKey(d => d.MaDanhMuc)
                .HasConstraintName("FK__Thuoc__MaDanhMuc__48CFD27E");

            entity.HasMany(d => d.MaChuDes).WithMany(p => p.MaThuocs)
                .UsingEntity<Dictionary<string, object>>(
                    "ThuocChuDe",
                    r => r.HasOne<ChuDeSucKhoe>().WithMany()
                        .HasForeignKey("MaChuDe")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Thuoc_Chu__MaChu__4F7CD00D"),
                    l => l.HasOne<Thuoc>().WithMany()
                        .HasForeignKey("MaThuoc")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Thuoc_Chu__MaThu__4E88ABD4"),
                    j =>
                    {
                        j.HasKey("MaThuoc", "MaChuDe").HasName("PK__Thuoc_Ch__48E9A271E8067EEA");
                        j.ToTable("Thuoc_ChuDe");
                    });
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VaiTro__C24C41CF09F311D4");

            entity.ToTable("VaiTro");

            entity.Property(e => e.TenVaiTro).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
