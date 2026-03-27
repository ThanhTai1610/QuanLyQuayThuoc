using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.DTOs.NguoiDung;
using QuanLyQuayThuoc.Repositories.Interfaces;
using System.Security.Claims;

namespace QuanLyQuayThuoc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu Token hợp lệ để truy cập
    public class HoSoController : ControllerBase
    {
        private readonly INguoiDungRepository _repo;
        private readonly IWebHostEnvironment _env;

        // CHỈ DÙNG 1 Constructor duy nhất để tiêm (Inject) các dịch vụ cần thiết
        public HoSoController(INguoiDungRepository repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }

        [HttpGet("thong-tin")]
        public async Task<IActionResult> GetProfile()
        {
            // Lấy ID người dùng từ NameIdentifier trong Token
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var userId = int.Parse(userIdStr);
            var result = await _repo.LayHoSoCaNhan(userId);

            return result != null ? Ok(result) : NotFound();
        }

        [HttpPut("cap-nhat")]
        public async Task<IActionResult> UpdateProfile([FromBody] CapNhatHoSoDto data)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var userId = int.Parse(userIdStr);
            var success = await _repo.LuuCapNhatHoSo(userId, data);

            return success ? Ok(new { message = "Thành công" }) : BadRequest();
        }

        [HttpPut("cap-nhat-avatar")]
        public async Task<IActionResult> CapNhatAvatar(IFormFile File)
        {
            // 1. Kiểm tra file đầu vào
            if (File == null || File.Length == 0)
                return BadRequest("File không hợp lệ");

            // 2. Lấy ID người dùng từ Token (để tránh lỗi 401)
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            var userId = int.Parse(userIdStr);

            try
            {
                // 3. Xác định đường dẫn gốc an toàn (Xử lý lỗi Object reference)
                string rootPath = _env.WebRootPath;
                if (string.IsNullOrEmpty(rootPath))
                {
                    // Nếu WebRootPath bị null, tự tìm đến thư mục wwwroot trong project
                    rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }

                // 4. Tạo đường dẫn đến thư mục lưu trữ: wwwroot/uploads/avatars
                var uploadsPath = Path.Combine(rootPath, "uploads");
                var avatarsPath = Path.Combine(uploadsPath, "avatars");

                // 5. Kiểm tra và tự động tạo từng cấp thư mục nếu chưa tồn tại
                if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
                if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);
                if (!Directory.Exists(avatarsPath)) Directory.CreateDirectory(avatarsPath);

                // 6. Đặt tên file duy nhất theo UserId và thời gian
                var fileName = $"avatar_{userId}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(File.FileName)}";
                var filePath = Path.Combine(avatarsPath, fileName);

                // 7. Lưu file vật lý vào ổ cứng
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await File.CopyToAsync(stream);
                }

                // 8. Cập nhật đường dẫn tương đối vào CSDL (Cột AnhDaiDien)
                string relativePath = $"/uploads/avatars/{fileName}";
                var success = await _repo.CapNhatDuongDanAvatar(userId, relativePath);

                if (success)
                {
                    // Trả về mã 200 kèm link ảnh để Frontend hiển thị
                    return Ok(new { url = relativePath });
                }
                else
                {
                    return BadRequest("Đã lưu file nhưng không thể cập nhật đường dẫn vào CSDL");
                }
            }
            catch (Exception ex)
            {
                // Trả về lỗi chi tiết nếu có sự cố hệ thống
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }
    }
}