using System;
using System.Collections.Generic;

namespace QuanLyQuayThuoc.Models;

public partial class NguoiDung
{
    public int MaNguoiDung { get; set; }

    public string HoTen { get; set; } = null!;

    public string? Email { get; set; }

    public string? SoDienThoai { get; set; }

    public string MatKhau { get; set; } = null!;

    public string? AnhDaiDien { get; set; }

    public int? MaVaiTro { get; set; }

    public string? TrangThai { get; set; }

    public DateTime? NgayTao { get; set; }



    public virtual ICollection<DonHang> DonHangMaKhachHangNavigations { get; set; } = new List<DonHang>();

    public virtual ICollection<DonHang> DonHangMaNhanVienNavigations { get; set; } = new List<DonHang>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual ICollection<LichSuChatbot> LichSuChatbots { get; set; } = new List<LichSuChatbot>();

    public virtual VaiTro? MaVaiTroNavigation { get; set; }

    public virtual ICollection<PhieuKiemKe> PhieuKiemKes { get; set; } = new List<PhieuKiemKe>();

    public virtual ICollection<SoDiaChi> SoDiaChis { get; set; } = new List<SoDiaChi>();
}
