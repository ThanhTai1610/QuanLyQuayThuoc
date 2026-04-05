namespace QuanLyQuayThuoc.DTOs.DonHang
{
    public class DatHangKhachHangDto
    {
        public int MaKhachHang { get; set; }
        public string PhuongThucThanhToan { get; set; } = "COD";
        public string DiaChiGiaoHang { get; set; } = "";
        public string SoDienThoaiNhan { get; set; } = "";
        public string? GhiChu { get; set; }
        public decimal GiamGia { get; set; } = 0;
        public List<ChiTietDatHangDto> ChiTiet { get; set; } = new();
    }

    public class ChiTietDatHangDto
    {
        public int MaLo { get; set; }
        public int MaDVT { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaBan { get; set; }
    }
}