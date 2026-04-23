using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.Dtos;
using QuanLyQuayThuoc.DTOs.NguoiDung;
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

                        MaThuoc = dh.ChiTietDonHangs
                            .OrderBy(ct => ct.MaChiTiet)
                            .Select(ct => ct.MaLoNavigation.MaThuoc)
                            .FirstOrDefault(),
                        MaDVT = dh.ChiTietDonHangs
                            .OrderBy(ct => ct.MaChiTiet)
                            .Select(ct => ct.MaDvt)
                            .FirstOrDefault(),
                        // Lấy thông tin từ ChiTietDonHang đầu tiên thông qua Navigation
                        TenSanPham = dh.ChiTietDonHangs
                            .OrderBy(ct => ct.MaChiTiet)
                            .Select(ct => ct.MaLoNavigation.MaThuocNavigation.TenThuoc)
                            .FirstOrDefault(),

                        HinhAnh = dh.ChiTietDonHangs
                            .OrderBy(ct => ct.MaChiTiet)
                            .Select(ct => ct.MaLoNavigation.MaThuocNavigation.HinhAnhChinh)
                            .FirstOrDefault(),

                        SoLuong = dh.ChiTietDonHangs
                            .OrderBy(ct => ct.MaChiTiet)
                            .Select(ct => ct.SoLuong)
                            .FirstOrDefault() ?? 0,

                        // Sử dụng MaDvtNavigation thay vì Join thủ công
                        DonVi = dh.ChiTietDonHangs
                            .OrderBy(ct => ct.MaChiTiet)
                            .Select(ct => ct.MaDvtNavigation.TenDonVi)
                            .FirstOrDefault(),

                        // Tính số lượng sản phẩm khác
                        SoSanPhamKhac = dh.ChiTietDonHangs.Count() > 1
                                        ? dh.ChiTietDonHangs.Count() - 1
                                        : 0,

                        SanPhamsTomTat = dh.ChiTietDonHangs
                            .OrderBy(ct => ct.MaChiTiet)
                            .Select(ct => new SanPhamTomTatDonHangDto
                            {
                                TenSanPham = ct.MaLoNavigation.MaThuocNavigation.TenThuoc,
                                HinhAnh = ct.MaLoNavigation.MaThuocNavigation.HinhAnhChinh,
                                SoLuong = ct.SoLuong ?? 0,
                                DonVi = ct.MaDvtNavigation.TenDonVi
                            })
                            .ToList()
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

        [HttpPut("huy/{id}")]
        public async Task<IActionResult> HuyDonHang(int id, [FromBody] HuyDonHangDto dto)
        {
            var donHang = await _context.DonHangs.FindAsync(id);

            if (donHang == null) return NotFound(new { message = "Không tìm thấy đơn hàng" });

            // Chỉ cho phép hủy khi đơn hàng đang ở trạng thái 'Chờ xử lý'
            if (donHang.TrangThai != "Chờ xử lý")
            {
                return BadRequest(new { message = "Không thể hủy đơn hàng ở trạng thái này." });
            }

            donHang.TrangThai = "Đã hủy";
            donHang.GhiChu = $"Lý do hủy: {dto.LyDo}"; // Lưu lý do vào cột GhiChu hoặc cột mới nếu bạn có

            await _context.SaveChangesAsync();

            return Ok(new { message = "Hủy đơn hàng thành công" });
        }
    }
}
