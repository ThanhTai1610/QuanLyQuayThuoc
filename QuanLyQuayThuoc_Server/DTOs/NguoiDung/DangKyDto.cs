using System.Text.Json.Serialization;

namespace QuanLyQuayThuoc.DTOs.NguoiDung
{
    public class DangKyDto
    {
        public string Ho { get; set; } = string.Empty;
        public string Ten { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;

        // Thêm trường này để hứng OTP từ Frontend gửi lên ở bước cuối
        [JsonPropertyName("maOtp")] // <--- QUAN TRỌNG: Để nó khớp chính xác với "maOtp" từ Vue gửi lên
        public string? MaOtp { get; set; }
    }
}