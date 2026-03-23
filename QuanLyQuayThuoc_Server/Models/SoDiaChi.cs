using System;
using System.Collections.Generic;

namespace QuanLyQuayThuoc.Models;

public partial class SoDiaChi
{
    public int MaDiaChi { get; set; }

    public int? MaNguoiDung { get; set; }

    public string HoTenNguoiNhan { get; set; } = null!;

    public string SoDienThoaiNhan { get; set; } = null!;

    public string? TinhThanh { get; set; }

    public string? QuanHuyen { get; set; }

    public string? PhuongXa { get; set; }

    public string? DiaChiChiTiet { get; set; }

    public bool? LaMacDinh { get; set; }

    public string? LoaiDiaChi { get; set; }

    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }
}
