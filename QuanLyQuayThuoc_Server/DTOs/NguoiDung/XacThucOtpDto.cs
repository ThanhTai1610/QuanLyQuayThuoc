using System.Text.Json.Serialization; // Nhớ thêm thư viện này nhé

namespace QuanLyQuayThuoc.DTOs.NguoiDung
{
    public class XacThucOtpDto
    {
        [JsonPropertyName("email")] // Ép Backend hiểu key "email" từ Vue
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("otp")] // Ép Backend hiểu key "otp" từ Vue
        public string Otp { get; set; } = string.Empty;
    }
}