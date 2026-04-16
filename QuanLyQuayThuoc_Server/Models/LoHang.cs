using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

    // EF quản lý cái này, trang Xử lý đơn hàng dùng cái này -> GIỮ NGUYÊN
    [ForeignKey("MaThuoc")]
    public virtual Thuoc? MaThuocNavigation { get; set; }

    // Dùng cái này cho Báo cáo, EF sẽ bỏ qua không check lỗi trùng lặp nhờ [NotMapped]
    [NotMapped]
    public virtual Thuoc? Thuoc => MaThuocNavigation;
}