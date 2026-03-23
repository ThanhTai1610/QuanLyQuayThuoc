using System;
using System.Collections.Generic;

namespace QuanLyQuayThuoc.Models;

public partial class PhieuKiemKe
{
    public int MaPhieu { get; set; }

    public DateTime? NgayKiem { get; set; }

    public int? MaNhanVien { get; set; }

    public string? GhiChu { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<ChiTietKiemKe> ChiTietKiemKes { get; set; } = new List<ChiTietKiemKe>();

    public virtual NguoiDung? MaNhanVienNavigation { get; set; }
}
