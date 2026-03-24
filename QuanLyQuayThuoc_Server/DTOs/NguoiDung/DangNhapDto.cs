namespace QuanLyQuayThuoc.DTOs.NguoiDung
{
    public class DangNhapDto
    {
        public string Email { get; set; }
        public string MatKhau { get; set; }
    }
    public class PhanQuyenDto
    {
        public int MaNguoiDung { get; set; }
        public string HoTen { get; set; }
        public int MaVaiTro { get; set; }
    }
}
