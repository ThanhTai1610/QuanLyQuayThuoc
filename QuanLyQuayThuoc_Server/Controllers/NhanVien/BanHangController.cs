// BanHangController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyQuayThuoc.DTOs.DonHang;
using QuanLyQuayThuoc.Services.Interfaces;
using System.Security.Claims;

namespace QuanLyQuayThuoc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanHangController : ControllerBase
    {
        private readonly IBanHangService _banHangService;

        public BanHangController(IBanHangService banHangService)
        {
            _banHangService = banHangService;
        }

        private int GetMaNhanVienFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("Không tìm thấy thông tin nhân viên trong Token. Vui lòng đăng nhập lại.");

            return int.Parse(userIdClaim.Value);
        }

        [HttpGet("tim-kiem")]
        public async Task<IActionResult> TimKiem(string tenThuoc)
        {
            if (string.IsNullOrEmpty(tenThuoc))
                return BadRequest("Từ khóa tìm kiếm không được để trống.");

            var result = await _banHangService.TimKiemThuocNhanhAsync(tenThuoc);
            return Ok(result);
        }
        [HttpGet("tim-thuoc-barcode/{barcode}")]
        public async Task<IActionResult> TimThuocTheoBarcode(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
                return BadRequest("Mã vạch không được để trống.");

            var result = await _banHangService.TimThuocTheoBarcodeAsync(barcode);

            if (result == null)
                return NotFound(new { message = $"Không tìm thấy thuốc với mã vạch: {barcode}" });

            return Ok(result);
        }

        [HttpGet("lo-hang/{maThuoc}")]
        public async Task<IActionResult> GetLoHang(int maThuoc)
        {
            var result = await _banHangService.LayDanhSachLoCuaThuocAsync(maThuoc);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("thanh-toan")]
        public async Task<IActionResult> ThanhToan([FromBody] TaoDonHangDto dto)
        {
            try
            {
                if (dto == null || dto.ChiTiet == null || dto.ChiTiet.Count == 0)
                    return BadRequest("Danh sách hàng hóa không được để trống.");

                int maNhanVien = GetMaNhanVienFromToken();

                var maDonHang = await _banHangService.ThanhToanTaiQuayAsync(dto, maNhanVien);

                return Ok(new
                {
                    Success = true,
                    Message = "Thanh toán thành công!",
                    MaDonHang = maDonHang
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }
}