using Microsoft.AspNetCore.Mvc;
using QuanLyQuayThuoc.DTOs.DonHang;
using QuanLyQuayThuoc.Services.Interfaces;
using System;

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

        [HttpGet("tim-kiem")]
        public async Task<IActionResult> TimKiem(string tenThuoc)
        {
            if (string.IsNullOrEmpty(tenThuoc)) return BadRequest("Từ khóa tìm kiếm không được để trống.");

            var result = await _banHangService.TimKiemThuocNhanhAsync(tenThuoc);
            return Ok(result);
        }

        [HttpGet("lo-hang/{maThuoc}")]
        public async Task<IActionResult> GetLoHang(int maThuoc)
        {
            var result = await _banHangService.LayDanhSachLoCuaThuocAsync(maThuoc);
            return Ok(result);
        }

        [HttpPost("thanh-toan")]
        public async Task<IActionResult> ThanhToan([FromBody] TaoDonHangDto dto)
        {
            try
            {
                if (dto == null || dto.ChiTiet == null || dto.ChiTiet.Count == 0)
                {
                    return BadRequest("Danh sách hàng hóa không được để trống.");
                }
                int maNhanVien = 5;
                var maDonHang = await _banHangService.ThanhToanTaiQuayAsync(dto, maNhanVien);
                return Ok(new
                {
                    Success = true,
                    Message = "Thanh toán thành công!",
                    MaDonHang = maDonHang
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }
}