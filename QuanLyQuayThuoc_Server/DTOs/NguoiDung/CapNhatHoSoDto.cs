using System.ComponentModel.DataAnnotations;

namespace QuanLyQuayThuoc.DTOs.NguoiDung
{
    public class CapNhatHoSoDto
    {

        [Required(ErrorMessage = "Họ tên không được để trống")]
        // Regex: Chỉ cho phép chữ và khoảng trắng (Unicode)
        [RegularExpression(@"^[a-zA-Z\s\p{L}]+$", ErrorMessage = "Họ tên không được chứa số hoặc ký tự đặc biệt")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ tên từ 2 đến 100 ký tự")]
        public string HoTen { get; set; }

        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        // EmailAddress chỉ kiểm tra cơ bản, RegularExpression này sẽ kiểm tra chặt chẽ hơn
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        public string? GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        // Có thể thêm Email nếu cho phép sửa
    }
}