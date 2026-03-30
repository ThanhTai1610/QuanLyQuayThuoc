namespace QuanLyQuayThuoc.DTOs.DonHang
{
    public class TaoDonHangDto
    {
        public int? MaKhachHang { get; set; }
        public string PhuongThucThanhToan { get; set; }
        public decimal GiamGia { get; set; }
        public List<ChiTietDonHangDto> ChiTiet { get; set; }
    }

    public class ChiTietDonHangDto
    {
        public int MaLo { get; set; }
        public int MaDVT { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaBan { get; set; }
    }
}