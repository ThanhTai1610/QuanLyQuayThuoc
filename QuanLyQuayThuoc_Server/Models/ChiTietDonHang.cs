using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyQuayThuoc.Models;

public partial class ChiTietDonHang
{
    [Key]
    public int MaChiTiet { get; set; }
    public int? MaDonHang { get; set; }
    public int? MaLo { get; set; }
    public int? MaDvt { get; set; }
    public int? SoLuong { get; set; }
    public decimal? GiaBanTaiThoiDiem { get; set; }

    // --- GIỮ LẠI TÊN CŨ CHO EF QUẢN LÝ ---
    [ForeignKey("MaDonHang")]
    public virtual DonHang? MaDonHangNavigation { get; set; }

    [ForeignKey("MaLo")]
    public virtual LoHang? MaLoNavigation { get; set; }

    [ForeignKey("MaDvt")]
    public virtual DonViTinh? MaDvtNavigation { get; set; }

    // --- TẠO BÍ DANH CHO BÁO CÁO (Dùng [NotMapped] để tránh lỗi 500) ---
    [NotMapped]
    public virtual DonHang? DonHang => MaDonHangNavigation;

    [NotMapped]
    public virtual LoHang? LoHang => MaLoNavigation;
}