using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.DTOs.NguoiDung;
using QuanLyQuayThuoc.Repositories.Interfaces;
using System.Security.Claims;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace QuanLyQuayThuoc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HoSoController : ControllerBase
    {
        private readonly INguoiDungRepository _repo;
        private readonly IWebHostEnvironment _env;

        public HoSoController(INguoiDungRepository repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }

        [HttpGet("thong-tin")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var userId = int.Parse(userIdStr);
            var result = await _repo.LayHoSoCaNhan(userId);

            return result != null ? Ok(result) : NotFound();
        }

        [HttpPut("cap-nhat")]
        public async Task<IActionResult> UpdateProfile([FromBody] CapNhatHoSoDto data)
        {
            if (!ModelState.IsValid)
            {
                var errorMsg = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault();
                return BadRequest(new { message = errorMsg });
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var userId = int.Parse(userIdStr);
            var success = await _repo.LuuCapNhatHoSo(userId, data);

            return success ? Ok(new { message = "Thành công" }) : BadRequest(new { message = "Cập nhật thất bại" });
        }

        [HttpPut("cap-nhat-avatar")]
        public async Task<IActionResult> CapNhatAvatar(IFormFile File)
        {
            if (File == null || File.Length == 0)
                return BadRequest("File không hợp lệ");

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            var userId = int.Parse(userIdStr);

            try
            {
                string rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var avatarsPath = Path.Combine(rootPath, "uploads", "avatars");

                if (!Directory.Exists(avatarsPath)) Directory.CreateDirectory(avatarsPath);

                var fileName = $"avatar_{userId}_{DateTime.Now:yyyyMMddHHmmss}.jpg";
                var filePath = Path.Combine(avatarsPath, fileName);

                using (var inputStream = File.OpenReadStream())
                {
                    using (var image = await Image.LoadAsync(inputStream))
                    {
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(256, 256),
                            Mode = ResizeMode.Crop
                        }));

                        var encoder = new JpegEncoder { Quality = 75 };
                        await image.SaveAsync(filePath, encoder);
                    }
                }

                string relativePath = $"/uploads/avatars/{fileName}";
                var success = await _repo.CapNhatDuongDanAvatar(userId, relativePath);

                return success ? Ok(new { url = relativePath }) : BadRequest("Lỗi cập nhật CSDL");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        [HttpDelete("xoa-avatar/{maNguoiDung}")]
        public async Task<IActionResult> XoaAvatar(int maNguoiDung)
        {
            var success = await _repo.CapNhatDuongDanAvatar(maNguoiDung, "default-avatar.png");
            return success ? Ok(new { message = "Đã xóa ảnh đại diện thành công" }) : NotFound(new { message = "Lỗi hệ thống" });
        }
    }
}