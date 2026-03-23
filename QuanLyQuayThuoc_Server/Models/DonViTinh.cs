using System;
using System.Collections.Generic;

namespace QuanLyQuayThuoc.Models;

public partial class DonViTinh
{
    public int MaDvt { get; set; }

    public int? MaThuoc { get; set; }

    public string? TenDonVi { get; set; }

    public int? GiaTriQuyDoi { get; set; }

    public decimal? GiaBan { get; set; }

    public bool? LaDonViCoBan { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual Thuoc? MaThuocNavigation { get; set; }
}
