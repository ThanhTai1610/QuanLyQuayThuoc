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

        //[Authorize(Roles = "Admin")]
        [HttpPost("nhap-kho")]
        public async Task<IActionResult> NhapKho([FromBody] PhieuNhapKhoDto phieuNhap)
        {
            if (phieuNhap == null || phieuNhap.ChiTiet == null || !phieuNhap.ChiTiet.Any())
                return BadRequest("Dữ liệu nhập kho không được để trống.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ketQua = await _khoService.NhapKhoAsync(phieuNhap);

            if (ketQua)
            {
                return Ok(new
                {
                    status = "success",
                    message = "Nhập kho thành công",
                    data = phieuNhap
                });
            }

            return StatusCode(500, "Có lỗi xảy ra trong quá trình lưu dữ liệu kho.");
        }
        [HttpGet("ma-vach/{maThuoc}")]
        public async Task<IActionResult> GetMaVach(int maThuoc)
        {
            var result = await _khoService.GetMaVachTheoThuocAsync(maThuoc);
            return Ok(result);
        }
        [HttpPost("nhap-kho-thuoc-moi")]
        public async Task<IActionResult> NhapKhoThuocMoi([FromBody] ThemThuocMoiVaNhapKhoDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.TenThuoc))
                return BadRequest("Tên thuốc không được để trống.");

            if (dto.ChiTiet == null || !dto.ChiTiet.Any())
                return BadRequest("Phải có ít nhất một đơn vị tính và lô nhập.");

            if (!dto.ChiTiet.Any(x => x.LaDonViCoBan))
                return BadRequest("Phải có ít nhất một đơn vị tính là đơn vị cơ bản.");

            var ketQua = await _khoService.ThemThuocMoiVaNhapKhoAsync(dto);

            if (ketQua)
            {
                return Ok(new
                {
                    status = "success",
                    data = new { chiTiet = dto.ChiTiet }
                });
            }

            return StatusCode(500, new { status = "error", message = "Có lỗi xảy ra khi lưu." });
        }
    }
}