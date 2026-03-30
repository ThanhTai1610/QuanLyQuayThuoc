namespace QuanLyQuayThuoc.Dtos
{
    public class DonHangCuaToiDto
    {
        public int MaDonHang { get; set; }
        public DateTime NgayDat { get; set; }
        public decimal TongTien { get; set; }
        public string? TrangThai { get; set; }

        // Thông tin sản phẩm đầu tiên để hiển thị làm đại diện
        public string? TenSanPham { get; set; }
        public string? HinhAnh { get; set; }
        public int SoLuong { get; set; }
        public string? DonVi { get; set; }

        // Số lượng các loại thuốc khác trong cùng đơn hàng
        public int SoSanPhamKhac { get; set; }
    }
}