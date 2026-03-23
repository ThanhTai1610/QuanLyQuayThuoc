using System;
using System.Collections.Generic;

namespace QuanLyQuayThuoc.Models;

public partial class LoHang
{
    public int MaLo { get; set; }

    public int? MaThuoc { get; set; }

    public string SoLo { get; set; } = null!;

    public DateOnly? NgaySanXuat { get; set; }

    public DateOnly HanSuDung { get; set; }

    public decimal? GiaNhap { get; set; }

    public int SoLuongTon { get; set; }

    public int? MaNhaCungCap { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<ChiTietKiemKe> ChiTietKiemKes { get; set; } = new List<ChiTietKiemKe>();

    public virtual Thuoc? MaThuocNavigation { get; set; }
}
