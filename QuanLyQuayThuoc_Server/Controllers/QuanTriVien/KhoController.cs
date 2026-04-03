using Microsoft.AspNetCore.Mvc;
using QuanLyQuayThuoc.DTOs.Kho;
using QuanLyQuayThuoc.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace QuanLyQuayThuoc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhoController : ControllerBase
    {
        private readonly IKhoService _khoService;

        public KhoController(IKhoService khoService)
        {
            _khoService = khoService;
        }

        [HttpGet("danh-muc")]
        public async Task<IActionResult> GetDanhMuc()
        {
            var result = await _khoService.GetDanhMucAsync();
            return Ok(result);
        }
        // Tab 1: Tổng quan (Cả Nhân viên & Admin đều xem được)
        [HttpGet("tong-quan")]
        public async Task<IActionResult> GetTongQuan([FromQuery] int? maDanhMuc, [FromQuery] string? search)
        {
            var result = await _khoService.GetTongQuanAsync(maDanhMuc, search);
            return Ok(result);
        }

        // Tab 2 & 4: Lô hàng & Cảnh báo (Cả Nhân viên & Admin đều xem được)
        [HttpGet("danh-sach-lo")]
        public async Task<IActionResult> GetLoHang([FromQuery] string? search, [FromQuery] string? thang, [FromQuery] string? loai)
        {
            var result = await _khoService.GetLoHangAsync(search, thang, loai);
            return Ok(result);
        }
        [HttpPut("lo-hang/{maLo}")]
        public async Task<IActionResult> SuaLoHang(int maLo, [FromBody] SuaLoHangDto dto)
        {
            var result = await _khoService.SuaLoHangAsync(maLo, dto);
            if (!result) return NotFound("Không tìm thấy lô hàng.");
            return Ok(new { message = "Cập nhật thành công" });
        }
        // Tab 3: Nhập hàng (CHỈ ADMIN hoặc THỦ KHO)
        // [Authorize(Roles = "Admin")] // Mở comment này nếu bạn đã làm Identity
        [HttpPost("nhap-kho")]
        public async Task<IActionResult> NhapKho([FromBody] PhieuNhapKhoDto phieuNhap)
        {
            if (phieuNhap == null) return BadRequest("Dữ liệu không hợp lệ");

            var result = await _khoService.NhapKhoAsync(phieuNhap);

            if (result)
                return Ok(new { message = "Nhập kho thành công và đã sinh mã vạch", data = phieuNhap });

            return StatusCode(500, "Có lỗi xảy ra khi lưu kho");
        }
    }
}