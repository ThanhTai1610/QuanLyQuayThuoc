namespace QuanLyQuayThuoc.DTOs.NguoiDung
{
    public class CapNhatHoSoDto
    {
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public string? Email { get; set; }
        public string? GioiTinh { get; set; } // Thêm trường này
        public DateTime? NgaySinh { get; set; }
        // Có thể thêm Email nếu cho phép sửa
    }
}