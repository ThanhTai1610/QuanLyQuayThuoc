using System;
using System.Collections.Generic;

namespace QuanLyQuayThuoc.Models;

public partial class DonHang
{
    public int MaDonHang { get; set; }

    public int? MaKhachHang { get; set; }

    public int? MaNhanVien { get; set; }

    public DateTime? NgayDat { get; set; }

    public decimal? TongTien { get; set; }

    public string? PhuongThucThanhToan { get; set; }

    public string? TrangThai { get; set; }

    public string? AnhDonThuoc { get; set; }

    public string? DiaChiGiaoHang { get; set; }

    public string? SoDienThoaiNhan { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual NguoiDung? MaKhachHangNavigation { get; set; }

    public virtual NguoiDung? MaNhanVienNavigation { get; set; }
}
