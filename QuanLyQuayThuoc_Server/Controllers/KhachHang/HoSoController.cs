using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyQuayThuoc.DTOs.NguoiDung;
using QuanLyQuayThuoc.Repositories.Interfaces;
using System.Security.Claims;

namespace QuanLyQuayThuoc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu Token hợp lệ
    public class HoSoController : ControllerBase
    {
        private readonly INguoiDungRepository _repo;
        public HoSoController(INguoiDungRepository repo) => _repo = repo;

        [HttpGet("thong-tin")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _repo.LayHoSoCaNhan(userId);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPut("cap-nhat")]
        public async Task<IActionResult> UpdateProfile([FromBody] CapNhatHoSoDto data)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _repo.LuuCapNhatHoSo(userId, data);
            return success ? Ok(new { message = "Thành công" }) : BadRequest();
        }
    }
}