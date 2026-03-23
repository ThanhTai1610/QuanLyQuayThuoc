using System;
using System.Collections.Generic;

namespace QuanLyQuayThuoc.Models;

public partial class Thuoc
{
    public int MaThuoc { get; set; }

    public string TenThuoc { get; set; } = null!;

    public int? MaDanhMuc { get; set; }

    public string? SoDangKy { get; set; }

    public string? QuyCach { get; set; }

    public string? DangBaoChe { get; set; }

    public string? NhaSanXuat { get; set; }

    public string? NuocSanXuat { get; set; }

    public int? HanSuDungThang { get; set; }

    public bool? LaThuocKeDon { get; set; }

    public string? HinhAnhChinh { get; set; }

    public string? MoTaNgan { get; set; }

    public string? ThanhPhan { get; set; }

    public string? CongDung { get; set; }

    public string? CachDung { get; set; }

    public string? DoiTuongSuDung { get; set; }

    public string? TacDungPhu { get; set; }

    public string? LuuY { get; set; }

    public string? BaoQuan { get; set; }

    public string? ChongChiDinh { get; set; }

    public int? LuotXem { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<DonViTinh> DonViTinhs { get; set; } = new List<DonViTinh>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual ICollection<HinhAnhThuoc> HinhAnhThuocs { get; set; } = new List<HinhAnhThuoc>();

    public virtual ICollection<LoHang> LoHangs { get; set; } = new List<LoHang>();

    public virtual DanhMuc? MaDanhMucNavigation { get; set; }

    public virtual ICollection<ChuDeSucKhoe> MaChuDes { get; set; } = new List<ChuDeSucKhoe>();
}
