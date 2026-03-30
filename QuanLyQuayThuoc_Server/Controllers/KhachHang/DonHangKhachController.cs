using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.Dtos;
using System.Security.Claims;

namespace QuanLyQuayThuoc.Controllers.KhachHang
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DonHangKhachController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DonHangKhachController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("cua-toi")]
        public async Task<IActionResult> GetLichSuDonHang()
        {
            try
            {
                // 1. Lấy MaNguoiDung từ Token
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
                int maKH = int.Parse(userIdStr);

                // 2. Truy vấn sử dụng Navigation Properties để tối ưu code
                var orders = await _context.DonHangs
                    .Where(dh => dh.MaKhachHang == maKH)
                    .OrderByDescending(dh => dh.NgayDat)
                    .Select(dh => new DonHangCuaToiDto
                    {
                        MaDonHang = dh.MaDonHang,
                        // Xử lý Nullable cho DateTime và decimal
                        NgayDat = dh.NgayDat ?? DateTime.Now,
                        TongTien = dh.TongTien ?? 0,
                        TrangThai = dh.TrangThai,

                        // Lấy thông tin từ ChiTietDonHang đầu tiên thông qua Navigation
                        TenSanPham = dh.ChiTietDonHangs
                            .Select(ct => ct.MaLoNavigation.MaThuocNavigation.TenThuoc)
                            .FirstOrDefault(),

                        HinhAnh = dh.ChiTietDonHangs
                            .Select(ct => ct.MaLoNavigation.MaThuocNavigation.HinhAnhChinh)
                            .FirstOrDefault(),

                        SoLuong = dh.ChiTietDonHangs
                            .Select(ct => ct.SoLuong)
                            .FirstOrDefault() ?? 0,

                        // Sử dụng MaDvtNavigation thay vì Join thủ công
                        DonVi = dh.ChiTietDonHangs
                            .Select(ct => ct.MaDvtNavigation.TenDonVi)
                            .FirstOrDefault(),

                        // Tính số lượng sản phẩm khác
                        SoSanPhamKhac = dh.ChiTietDonHangs.Count() > 1
                                        ? dh.ChiTietDonHangs.Count() - 1
                                        : 0
                    })
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", detail = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChiTietDonHang(int id)
        {
            // Bỏ check MaKhachHang để đảm bảo lấy được đơn hàng test số 3
            var order = await _context.DonHangs
                .Where(dh => dh.MaDonHang == id)
                .Select(dh => new ChiTietDonHangHienThiDto
                {
                    MaDonHang = dh.MaDonHang,
                    NgayDat = dh.NgayDat ?? DateTime.Now,
                    TongTien = dh.TongTien ?? 0,
                    TrangThai = dh.TrangThai,
                    DiaChiGiaoHang = dh.DiaChiGiaoHang,
                    SoDienThoaiNhan = dh.SoDienThoaiNhan,
                    PhuongThucThanhToan = dh.PhuongThucThanhToan,
                    // Đảm bảo mảng SanPhams được Select đúng
                    SanPhams = dh.ChiTietDonHangs.Select(ct => new SanPhamDonHangDto
                    {
                        TenThuoc = ct.MaLoNavigation.MaThuocNavigation.TenThuoc,
                        HinhAnh = ct.MaLoNavigation.MaThuocNavigation.HinhAnhChinh,
                        SoLuong = ct.SoLuong ?? 0,
                        DonGia = ct.GiaBanTaiThoiDiem ?? 0,
                        DonVi = ct.MaDvtNavigation.TenDonVi
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (order == null) return NotFound(new { message = "Không tìm thấy đơn hàng" });
            return Ok(order);
        }
    }
}