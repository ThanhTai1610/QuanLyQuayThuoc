using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization; 
using System.Security.Claims; 
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.DTOs.NguoiDung;
using QuanLyQuayThuoc.Helpers;
using QuanLyQuayThuoc.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Models;

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
            // 1. Kiểm tra Email
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null) return NotFound(new { message = "Email không tồn tại." });

            // 2. Tạo OTP và lưu DB
            string otp = new Random().Next(100000, 999999).ToString();
            user.MaOtp = otp;
            user.HanOtp = DateTime.Now.AddMinutes(5);
            await _context.SaveChangesAsync();

            // 3. GỬI MAIL CHẠY NGẦM (Fire and Forget)
            // Không dùng await ở đây để trả về Ok ngay lập tức
            _ = Task.Run(async () => {
                try
                {
                    string subject = "Mã xác nhận quên mật khẩu - Pharmative";
                    string body = $"Mã OTP của bạn là: <b>{otp}</b>. Hiệu lực 5 phút.";
                    await EmailHelper.SendEmailAsync(model.Email, subject, body);
                }
                catch (Exception ex)
                {
                    // Log lỗi ra file nếu cần để kiểm tra sau
                }
            });

            return Ok(new { message = "Mã OTP đang được gửi." });
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
        [AllowAnonymous]
        [HttpPost("gui-otp-dang-ky")]
        public async Task<IActionResult> GuiOtpDangKy([FromBody] DangKyDto model)
        {
            // 1. Kiểm tra email đã tồn tại chưa
            if (await _context.NguoiDungs.AnyAsync(u => u.Email == model.Email))
                return Conflict(new { message = "Email này đã được đăng ký." });

            // 2. Tạo OTP
            string otp = new Random().Next(100000, 999999).ToString();

            // 3. Gửi Email (Dùng EmailHelper Tài đã có)
            string subject = "Mã xác thực đăng ký tài khoản - Pharmative";
            string body = $"Mã OTP của bạn là: <b>{otp}</b>. Hiệu lực trong 5 phút.";

            bool isSent = await EmailHelper.SendEmailAsync(model.Email, subject, body);

            if (!isSent) return StatusCode(500, "Không thể gửi email.");

            return Ok(new { otpXacThuc = otp, message = "Mã OTP đã gửi về Email." });
        }
        [AllowAnonymous]
        [HttpPost("dang-ky-otp")]
        public async Task<IActionResult> DangKyChinhThuc([FromBody] DangKyDto model)
        {
            // 1. Kiểm tra lại email một lần nữa cho chắc
            if (await _context.NguoiDungs.AnyAsync(u => u.Email == model.Email))
                return Conflict(new { message = "Email này đã được đăng ký." });

            // 2. Logic kiểm tra OTP (Nếu Tài dùng cách gửi OTP về Client ở bước GuiOtp)
            // model.MaOtp này phải khớp với mã đã gửi qua Email
            // Lưu ý: Tài cần thêm trường 'MaOtp' vào trong DangKyDto nhé

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
    }
}