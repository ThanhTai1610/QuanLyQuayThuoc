namespace QuanLyQuayThuoc.Dtos
{
    public class ChiTietDonHangHienThiDto
    {
        public int MaDonHang { get; set; }
        public DateTime NgayDat { get; set; }
        public decimal TongTien { get; set; }
        public string? TrangThai { get; set; }
        public string? DiaChiGiaoHang { get; set; }
        public string? SoDienThoaiNhan { get; set; }
        public string? PhuongThucThanhToan { get; set; }

        // Danh sách các món trong đơn
        public List<SanPhamDonHangDto> SanPhams { get; set; } = new();
    }

    public class SanPhamDonHangDto
    {
        public string? TenThuoc { get; set; }
        public string? HinhAnh { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public string? DonVi { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }
}