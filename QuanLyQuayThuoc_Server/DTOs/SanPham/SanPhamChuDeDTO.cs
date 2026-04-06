namespace QuanLyQuayThuoc.DTOs.SanPham
{
    public class SanPhamChuDeDTO
    {
        public int MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string? HinhAnhChinh { get; set; }
        public decimal GiaBan { get; set; }
        public decimal? GiaCu { get; set; }
        public int? PhanTramGiam { get; set; }
        public string? TenDonVi { get; set; }      // Lấy từ bảng DonViTinh
        public string? QuyCach { get; set; }
        public string? NuocSanXuat { get; set; }
    }
}
