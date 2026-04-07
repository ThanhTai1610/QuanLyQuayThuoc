using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.Models;

namespace QuanLyQuayThuoc.Controllers.KhachHang
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Đảm bảo người dùng đã đăng nhập
    public class SoDiaChiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SoDiaChiController(ApplicationDbContext context) { _context = context; }

        // Lấy danh sách địa chỉ của người dùng hiện tại
        [HttpGet]
        public async Task<IActionResult> GetMyAddresses()
        {
            // Sử dụng cách an toàn hơn để lấy ID người dùng
            var userIdClaim = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "Không tìm thấy thông tin người dùng trong Token" });
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest(new { message = "ID người dùng không hợp lệ" });
            }

            var list = await _context.SoDiaChis
                .Where(x => x.MaNguoiDung == userId)
                .OrderByDescending(x => x.LaMacDinh)
                .ToListAsync();
            return Ok(list);
        }

        // Thêm mới địa chỉ
        // Thêm mới địa chỉ
        [HttpPost]
        public async Task<IActionResult> Create(SoDiaChi model)
        {
            // Lấy UserId an toàn từ Claim
            var userIdClaim = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Phiên đăng nhập hết hạn hoặc không hợp lệ" });
            }

            model.MaNguoiDung = userId;

            if (model.LaMacDinh == true)
            {
                await UnsetDefaultAddresses(userId);
            }

            _context.SoDiaChis.Add(model);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Thêm thành công" });
        }

        // Cập nhật địa chỉ
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SoDiaChi model)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var existing = await _context.SoDiaChis.FirstOrDefaultAsync(x => x.MaDiaChi == id && x.MaNguoiDung == userId);
            if (existing == null) return NotFound();

            if (model.LaMacDinh == true && existing.LaMacDinh != true)
            {
                await UnsetDefaultAddresses(userId);
            }

            // Cập nhật từng trường để tránh ghi đè dữ liệu null ngoài ý muốn
            existing.HoTenNguoiNhan = model.HoTenNguoiNhan;
            existing.SoDienThoaiNhan = model.SoDienThoaiNhan;
            existing.TinhThanh = model.TinhThanh;
            existing.QuanHuyen = model.QuanHuyen;
            existing.PhuongXa = model.PhuongXa;
            existing.DiaChiChiTiet = model.DiaChiChiTiet;
            existing.LoaiDiaChi = model.LoaiDiaChi;
            existing.LaMacDinh = model.LaMacDinh;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thành công" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId)) return Unauthorized();

            var item = await _context.SoDiaChis.FirstOrDefaultAsync(x => x.MaDiaChi == id && x.MaNguoiDung == userId);

            if (item == null) return NotFound(new { message = "Không tìm thấy địa chỉ để xóa" });

            _context.SoDiaChis.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa địa chỉ thành công" });
        }

        // 5. ĐẶT LÀM MẶC ĐỊNH (Dùng cho nút "Đặt mặc định" ở ngoài danh sách)
        [HttpPut("{id}/mac-dinh")]
        public async Task<IActionResult> SetDefault(int id)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId)) return Unauthorized();

            var item = await _context.SoDiaChis.FirstOrDefaultAsync(x => x.MaDiaChi == id && x.MaNguoiDung == userId);
            if (item == null) return NotFound();

            await UnsetDefaultAddresses(userId);
            item.LaMacDinh = true;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã đặt làm địa chỉ mặc định" });
        }

        private async Task UnsetDefaultAddresses(int userId)
        {
            var defaults = await _context.SoDiaChis
                .Where(x => x.MaNguoiDung == userId && x.LaMacDinh == true)
                .ToListAsync();
            foreach (var item in defaults) item.LaMacDinh = false;
        }
    }
}
