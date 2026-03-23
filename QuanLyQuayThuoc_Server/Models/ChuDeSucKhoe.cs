using System;
using System.Collections.Generic;

namespace QuanLyQuayThuoc.Models;

public partial class ChuDeSucKhoe
{
    public int MaChuDe { get; set; }

    public string TenChuDe { get; set; } = null!;

    public string? TieuDePhu { get; set; }

    public string? NoiDungGiaiPhap { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<Thuoc> MaThuocs { get; set; } = new List<Thuoc>();
}
