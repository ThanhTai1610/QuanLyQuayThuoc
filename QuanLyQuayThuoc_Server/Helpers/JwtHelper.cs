using Microsoft.IdentityModel.Tokens;
using QuanLyQuayThuoc.DTOs.NguoiDung;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuanLyQuayThuoc.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _config;

        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(PhanQuyenDto user)
        {
            // 1. Lấy giá trị và kiểm tra Null ngay lập tức
            var key = _config["Jwt:Key"];
            var durationStr = _config["Jwt:DurationInMinutes"];

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(durationStr))
            {
                // Trả về lỗi rõ ràng để Tài biết thiếu cái gì trong appsettings.json
                throw new InvalidOperationException("Jwt:Key hoặc Jwt:DurationInMinutes chưa được cấu hình!");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // 2. Tạo danh sách Claims
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.MaNguoiDung.ToString()),
        new Claim(ClaimTypes.Name, user.HoTen),
        new Claim(ClaimTypes.Role, user.MaVaiTro.ToString())
    };

            // 3. Khởi tạo Token
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                // Dùng TryParse hoặc ép kiểu an toàn
                expires: DateTime.Now.AddMinutes(double.Parse(durationStr)),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}