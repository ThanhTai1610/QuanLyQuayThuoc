using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Bắt buộc phải có để dùng CookieOptions và Cookies
using QuanLyQuayThuoc.DTOs.NguoiDung;
using QuanLyQuayThuoc.Helpers;
using QuanLyQuayThuoc.Services.Interfaces; // Namespace chứa INguoiDungService của Tài

namespace QuanLyQuayThuoc.Controllers.KhachHang
{
    [Route("api/[controller]")]
    [ApiController]
    // Tài kiểm tra xem đã có ": ControllerBase" chưa nhé
    public class NguoiDungController : ControllerBase
    {
        private readonly INguoiDungService _nguoiDungService;
        private readonly JwtHelper _jwtHelper;

        public NguoiDungController(INguoiDungService nguoiDungService, JwtHelper jwtHelper)
        {
            _nguoiDungService = nguoiDungService;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("dang-nhap")]
        public async Task<IActionResult> Login([FromBody] DangNhapDto duLieu)
        {
            var userAuth = await _nguoiDungService.DangNhap(duLieu);

            if (userAuth == null)
                return Unauthorized(new { message = "Email hoặc mật khẩu không chính xác" });

            var token = _jwtHelper.GenerateToken(userAuth);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddDays(1)
            };

            // Sau khi thêm using Microsoft.AspNetCore.Http, dòng này sẽ hết lỗi
            Response.Cookies.Append("AuthToken", token, cookieOptions);

            return Ok(new
            {
                user = userAuth,
                token = token,
                message = "Đăng nhập thành công"
            });
        }
    }
}