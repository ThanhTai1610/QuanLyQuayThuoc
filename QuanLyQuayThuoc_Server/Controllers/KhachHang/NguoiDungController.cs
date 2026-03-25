using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization; 
using System.Security.Claims; 
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.DTOs.NguoiDung;
using QuanLyQuayThuoc.Helpers;
using QuanLyQuayThuoc.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace QuanLyQuayThuoc.Controllers.KhachHang
{
    [Route("api/[controller]")]
    [ApiController]
    public class NguoiDungController : ControllerBase
    {
        private readonly INguoiDungService _nguoiDungService;
        private readonly JwtHelper _jwtHelper;
        private readonly ApplicationDbContext _context; // Thêm context vào đây

        public NguoiDungController(INguoiDungService nguoiDungService, JwtHelper jwtHelper, ApplicationDbContext context)
        {
            _nguoiDungService = nguoiDungService;
            _jwtHelper = jwtHelper;
            _context = context; // Gán giá trị cho context
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

            Response.Cookies.Append("AuthToken", token, cookieOptions);

            return Ok(new
            {
                user = userAuth,
                token = token,
                message = "Đăng nhập thành công"
            });
        }

        [Authorize]
        [HttpPost("doi-mat-khau")]
        public async Task<IActionResult> DoiMatKhau([FromBody] DoiMatKhauDto model)
        {
            try
            {
                // 1. Lấy ID người dùng từ Claim trong Token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("Không tìm thấy thông tin người dùng.");

                var userId = int.Parse(userIdClaim);
                var user = await _context.NguoiDungs.FindAsync(userId);

                if (user == null) return NotFound("Người dùng không tồn tại.");

                // 2. Kiểm tra mật khẩu cũ
                // Tài đảm bảo đã cài thư viện BCrypt.Net-Next nhé
                bool isCorrect = BCrypt.Net.BCrypt.Verify(model.MatKhauCu, user.MatKhau);
                if (!isCorrect) return BadRequest("Mật khẩu cũ không chính xác.");

                // 3. Hash mật khẩu mới và lưu
                user.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đổi mật khẩu thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi hệ thống: " + ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost("quen-mat-khau")]
        public async Task<IActionResult> QuenMatKhau([FromBody] QuenMatKhauDto model)
        {
            // 1. Kiểm tra Email có tồn tại trong hệ thống không
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                return NotFound(new { message = "Email không tồn tại trong hệ thống." });
            }

            // 2. Tạo mã OTP ngẫu nhiên 6 số
            string otp = new Random().Next(100000, 999999).ToString();

            // 3. Cập nhật mã OTP và thời gian hết hạn vào Database
            // (Tài đảm bảo bảng NguoiDung đã có 2 cột này nhé)
            user.MaOtp = otp;
            user.HanOtp = DateTime.Now.AddMinutes(5); // Mã có hiệu lực trong 5 phút

            await _context.SaveChangesAsync();

            // 4. Gửi Email thông qua Helper của Tài
            string subject = "Mã xác nhận quên mật khẩu - Pharmative";
            string body = $@"
        <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #eee;'>
            <h2 style='color: #e91e63;'>Xác thực tài khoản</h2>
            <p>Chào bạn, mã OTP để đặt lại mật khẩu của bạn là:</p>
            <div style='font-size: 32px; font-weight: bold; color: #333; letter-spacing: 5px; margin: 20px 0;'>
                {otp}
            </div>
            <p>Mã này có hiệu lực trong <b>5 phút</b>. Vui lòng không cung cấp mã này cho người khác.</p>
        </div>";

            bool checkSend = await EmailHelper.SendEmailAsync(model.Email, subject, body);

            if (checkSend)
            {
                return Ok(new { message = "Mã OTP đã được gửi thành công." });
            }
            else
            {
                return StatusCode(500, new { message = "Gửi mail thất bại, vui lòng thử lại sau." });
            }
        }
        [AllowAnonymous]
        [HttpPost("dat-lai-mat-khau")]
        public async Task<IActionResult> DatLaiMatKhau([FromBody] DatLaiMatKhauDto model)
        {
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
                return NotFound(new { message = "Email không tồn tại." });

            // Kiểm tra mã OTP
            if (string.IsNullOrEmpty(user.MaOtp) || user.MaOtp != model.MaOtp)
            {
                return BadRequest(new { message = "Mã OTP không chính xác." });
            }

            // Kiểm tra thời gian hết hạn
            if (user.HanOtp < DateTime.Now)
            {
                return BadRequest(new { message = "Mã OTP đã hết hạn (quá 5 phút)." });
            }

            // Hash mật khẩu mới
            user.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi);

            // Xóa dấu vết OTP sau khi dùng xong để bảo mật
            user.MaOtp = null;
            user.HanOtp = null;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đặt lại mật khẩu thành công!" });
        }
        [AllowAnonymous]
        [HttpPost("xac-nhan-otp")]
        public async Task<IActionResult> XacNhanOtp([FromBody] XacThucOtpDto model)
        {
            // Trong NguoiDungController.cs, hàm XacNhanOtp
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == model.Email.Trim());

            if (user == null || user.MaOtp?.Trim() != model.Otp.Trim())
            {
                return BadRequest(new { message = "Mã OTP không chính xác." });
            }
            return Ok(new { message = "Mã OTP hợp lệ." });
        }
    }
}