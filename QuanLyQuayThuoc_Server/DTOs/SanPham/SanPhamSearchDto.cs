namespace QuanLyQuayThuoc.DTOs.SanPham
{
    public class SanPhamSearchDto
    {
        public int Id { get; set; }
        public string TenThuoc { get; set; }
        public string HinhAnhChinh { get; set; }
        public decimal GiaBan { get; set; }
        public decimal? GiaCu { get; set; }
        public string TenDanhMuc { get; set; }
    }
}
