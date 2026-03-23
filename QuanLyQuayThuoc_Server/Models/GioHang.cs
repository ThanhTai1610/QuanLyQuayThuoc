using System;
using System.Collections.Generic;

namespace QuanLyQuayThuoc.Models;

public partial class GioHang
{
    public int MaGioHang { get; set; }

    public int? MaKhachHang { get; set; }

    public int? MaThuoc { get; set; }

    public int? MaDvt { get; set; }

    public int? SoLuong { get; set; }

    public virtual DonViTinh? MaDvtNavigation { get; set; }

    public virtual NguoiDung? MaKhachHangNavigation { get; set; }

    public virtual Thuoc? MaThuocNavigation { get; set; }
}
