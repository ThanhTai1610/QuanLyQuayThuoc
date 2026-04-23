using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.DTOs.NguoiDung;
using QuanLyQuayThuoc.Helpers;
using QuanLyQuayThuoc.Models;
using QuanLyQuayThuoc.Services.Interfaces;
using System.Security.Claims;

namespace QuanLyQuayThuoc.Controllers.KhachHang
{
    [Route("api/[controller]")]
    [ApiController]
    public class NguoiDungController : ControllerBase
    {
        private readonly INguoiDungService _nguoiDungService;
        private readonly JwtHelper _jwtHelper;
        private readonly ApplicationDbContext _context;

        public NguoiDungController(INguoiDungService nguoiDungService, JwtHelper jwtHelper, ApplicationDbContext context)
        {
            _nguoiDungService = nguoiDungService;
            _jwtHelper = jwtHelper;
            _context = context;
        }

        [HttpPost("dang-nhap")]
        public async Task<IActionResult> Login([FromBody] DangNhapDto duLieu)
        {
            var userAuth = await _nguoiDungService.DangNhap(duLieu);

            if (userAuth == null)
                return Unauthorized(new { message = "Email hoặc mật khẩu không chính xác" });

            return Ok(TaoPhanHoiDangNhap(userAuth));
        }

        [HttpPost("dang-nhap-google")]
        public async Task<IActionResult> LoginGoogle([FromBody] DangNhapGoogleDto duLieu)
        {
            try
            {
                var userAuth = await _nguoiDungService.DangNhapBangGoogle(duLieu);

                if (userAuth == null)
                    return Unauthorized(new { message = "Xác thực Google không hợp lệ." });

                return Ok(TaoPhanHoiDangNhap(userAuth));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("doi-mat-khau")]
        public async Task<IActionResult> DoiMatKhau([FromBody] DoiMatKhauDto model)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("Không tìm thấy thông tin người dùng.");

                var userId = int.Parse(userIdClaim);
                var user = await _context.NguoiDungs.FindAsync(userId);

                if (user == null)
                    return NotFound("Người dùng không tồn tại.");

                var isCorrect = BCrypt.Net.BCrypt.Verify(model.MatKhauCu, user.MatKhau);
                if (!isCorrect)
                    return BadRequest("Mật khẩu cũ không chính xác.");

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
            try
            {
                var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                    return NotFound(new { message = "Email không tồn tại trong hệ thống." });

                var otp = new Random().Next(100000, 999999).ToString();
                user.MaOtp = otp;
                user.HanOtp = DateTime.Now.AddMinutes(5);
                await _context.SaveChangesAsync();

                var subject = "Mã xác nhận quên mật khẩu - Pharmative";
                var body = $"Mã OTP của bạn là: <b>{otp}</b>. Hiệu lực 5 phút.";
                var isSent = await EmailHelper.SendEmailAsync(model.Email, subject, body);

                if (isSent)
                    return Ok(new { success = true, message = "Mã OTP đã được gửi về Email." });

                return StatusCode(500, new { message = "Gửi mail thất bại. Tài kiểm tra lại App Password Gmail nhé!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("dat-lai-mat-khau")]
        public async Task<IActionResult> DatLaiMatKhau([FromBody] DatLaiMatKhauDto model)
        {
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
                return NotFound(new { message = "Email không tồn tại." });

            if (string.IsNullOrEmpty(user.MaOtp) || user.MaOtp != model.MaOtp)
                return BadRequest(new { message = "Mã OTP không chính xác." });

            if (user.HanOtp < DateTime.Now)
                return BadRequest(new { message = "Mã OTP đã hết hạn (quá 5 phút)." });

            user.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi);
            user.MaOtp = null;
            user.HanOtp = null;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đặt lại mật khẩu thành công!" });
        }

        [AllowAnonymous]
        [HttpPost("xac-nhan-otp")]
        public async Task<IActionResult> XacNhanOtp([FromBody] XacThucOtpDto model)
        {
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == model.Email.Trim());

            if (user == null || user.MaOtp?.Trim() != model.Otp.Trim())
                return BadRequest(new { message = "Mã OTP không chính xác." });

            return Ok(new { message = "Mã OTP hợp lệ." });
        }

        [AllowAnonymous]
        [HttpPost("gui-otp-dang-ky")]
        public async Task<IActionResult> GuiOtpDangKy([FromBody] DangKyDto model)
        {
            if (await _context.NguoiDungs.AnyAsync(u => u.Email == model.Email))
                return Conflict(new { message = "Email này đã được đăng ký." });

            var otp = new Random().Next(100000, 999999).ToString();
            var subject = "Mã xác thực đăng ký tài khoản - Pharmative";
            var body = $"Mã OTP của bạn là: <b>{otp}</b>. Hiệu lực trong 5 phút.";

            var isSent = await EmailHelper.SendEmailAsync(model.Email, subject, body);

            if (!isSent)
                return StatusCode(500, "Không thể gửi email.");

            return Ok(new { otpXacThuc = otp, message = "Mã OTP đã gửi về Email." });
        }

        [AllowAnonymous]
        [HttpPost("dang-ky-otp")]
        public async Task<IActionResult> DangKyChinhThuc([FromBody] DangKyDto model)
        {
            if (await _context.NguoiDungs.AnyAsync(u => u.Email == model.Email))
                return Conflict(new { message = "Email này đã được đăng ký." });

            var user = new NguoiDung
            {
                HoTen = $"{model.Ho} {model.Ten}".Trim(),
                Email = model.Email,
                SoDienThoai = model.SoDienThoai,
                MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
                MaVaiTro = 3,
                TrangThai = "Hoạt động",
                NgayTao = DateTime.Now
            };

            _context.NguoiDungs.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đăng ký thành công!" });
        }

        private object TaoPhanHoiDangNhap(PhanQuyenDto userAuth)
        {
            var token = _jwtHelper.GenerateToken(userAuth);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddDays(1)
            };

            Response.Cookies.Append("AuthToken", token, cookieOptions);

            return new
            {
                user = userAuth,
                token,
                message = "Đăng nhập thành công"
            };
        }
    }
}
