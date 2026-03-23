using System;
using System.Collections.Generic;

namespace QuanLyQuayThuoc.Models;

public partial class ChiTietKiemKe
{
    public int MaPhieu { get; set; }

    public int MaLo { get; set; }

    public int? SoLuongHeThong { get; set; }

    public int? SoLuongThucTe { get; set; }

    public string? LyDoLech { get; set; }

    public virtual LoHang MaLoNavigation { get; set; } = null!;

    public virtual PhieuKiemKe MaPhieuNavigation { get; set; } = null!;
}
