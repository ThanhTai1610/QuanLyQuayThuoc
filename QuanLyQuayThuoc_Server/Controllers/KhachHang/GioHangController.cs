// GioHangController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyQuayThuoc.DTOs.DonHang;
using QuanLyQuayThuoc.Services.Interfaces;
using System.Security.Claims;

namespace QuanLyQuayThuoc.Controllers.KhachHang
{
    [Route("api/GioHang")]
    [ApiController]
    [Authorize] 
    public class GioHangController : ControllerBase
    {
        private readonly IGioHangService _gioHangService;

        public GioHangController(IGioHangService gioHangService)
        {
            _gioHangService = gioHangService;
        }

        private int GetMaKhachHangFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("Không tìm thấy thông tin định danh trong Token.");

            return int.Parse(userIdClaim.Value);
        }

        [HttpGet]
        public async Task<IActionResult> LayGioHang()
        {
            try
            {
                int maKhachHang = GetMaKhachHangFromToken();
                var ketQua = await _gioHangService.LayDanhSachGioHangAsync(maKhachHang);
                return Ok(ketQua);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("them")]
        public async Task<IActionResult> ThemVaoGio([FromBody] ThemVaoGioDto dto)
        {
            try
            {
                int maKhachHang = GetMaKhachHangFromToken();
                var thanhCong = await _gioHangService.ThemVaoGioHangAsync(
                    maKhachHang, dto.MaThuoc, dto.MaDvt, dto.SoLuong);

                if (thanhCong) return Ok(new { message = "Đã thêm vào giỏ hàng" });
                return BadRequest(new { message = "Không thể thêm vào giỏ hàng" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPut("cap-nhat")]
        public async Task<IActionResult> CapNhatGio([FromBody] List<CapNhatGioHangDto> danhSach)
        {
            if (danhSach == null || !danhSach.Any())
                return BadRequest(new { message = "Danh sách cập nhật trống" });

            var thanhCong = await _gioHangService.CapNhatGioHangAsync(danhSach);
            if (thanhCong) return Ok(new { message = "Cập nhật thành công" });
            return BadRequest(new { message = "Cập nhật thất bại" });
        }

        [HttpDelete("xoa/{id}")]
        public async Task<IActionResult> XoaSanPham(int id)
        {
            var thanhCong = await _gioHangService.XoaKhoiGioHangAsync(id);
            if (thanhCong) return Ok(new { message = "Đã xóa sản phẩm" });
            return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ" });
        }

        [HttpDelete("xoa-tat-ca")]
        public async Task<IActionResult> XoaTatCa()
        {
            try
            {
                int maKhachHang = GetMaKhachHangFromToken();
                await _gioHangService.XoaToanBoGioHangAsync(maKhachHang);
                return Ok(new { message = "Giỏ hàng đã được làm trống" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("dat-hang")]
        public async Task<IActionResult> DatHang([FromBody] DatHangKhachHangDto dto)
        {
            try
            {
                if (dto == null || dto.ChiTiet == null || !dto.ChiTiet.Any())
                    return BadRequest(new { success = false, message = "Giỏ hàng trống." });

                int maKhachHang = GetMaKhachHangFromToken();

                var maDonHang = await _gioHangService.DatHangAsync(dto, maKhachHang);

                return Ok(new { success = true, maDonHang = maDonHang });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    public class ThemVaoGioDto
    {
        public int MaThuoc { get; set; }
        public int MaDvt { get; set; }
        public int SoLuong { get; set; }
    }
}