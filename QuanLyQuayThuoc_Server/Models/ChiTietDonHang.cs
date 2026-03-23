using System;
using System.Collections.Generic;

namespace QuanLyQuayThuoc.Models;

public partial class ChiTietDonHang
{
    public int MaChiTiet { get; set; }

    public int? MaDonHang { get; set; }

    public int? MaLo { get; set; }

    public int? MaDvt { get; set; }

    public int? SoLuong { get; set; }

    public decimal? GiaBanTaiThoiDiem { get; set; }

    public virtual DonHang? MaDonHangNavigation { get; set; }

    public virtual DonViTinh? MaDvtNavigation { get; set; }

    public virtual LoHang? MaLoNavigation { get; set; }
}
