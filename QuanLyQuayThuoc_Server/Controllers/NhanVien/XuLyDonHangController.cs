using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.DTOs.DonHang;

namespace QuanLyQuayThuoc.Controllers.NhanVien
{
    [Route("api/[controller]")]
    [ApiController]
    public class XuLyDonHangController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private const string TrangThaiChoXuLy = "Chờ xử lý";

        public XuLyDonHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        private static string ChuanHoaTrangThaiDonHang(string? trangThai)
        {
            if (string.IsNullOrWhiteSpace(trangThai))
            {
                return TrangThaiChoXuLy;
            }

            var value = trangThai.Trim();
            return value switch
            {
                "Chờ xác nhận" => TrangThaiChoXuLy,
                "Chờ thanh toán" => TrangThaiChoXuLy,
                "Đã thanh toán" => TrangThaiChoXuLy,
                _ => value
            };
        }

        // 1. Lấy danh sách đơn hàng xử lý
        [HttpGet("danh-sach")]
        public async Task<IActionResult> LayDanhSachDonHang()
        {
            var bayGio = DateTime.Now;

            var ketQua = await _context.DonHangs
                .OrderByDescending(d => d.NgayDat) // Đổi NgayTao -> NgayDat
                .Select(d => new
                {
                    MaDonHang = d.MaDonHang,
                    NgayDat = d.NgayDat.HasValue ? d.NgayDat.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    // Lấy tên khách hàng từ bảng NguoiDung liên kết
                    TenKhachHang = d.MaKhachHangNavigation != null ? d.MaKhachHangNavigation.HoTen : "Khách vãng lai",
                    SoDienThoaiNhan = d.SoDienThoaiNhan, // Đúng tên trong Model
                    TongTien = d.TongTien ?? 0,
                    TrangThai = d.TrangThai == "Chờ xác nhận" || d.TrangThai == "Chờ thanh toán" || d.TrangThai == "Đã thanh toán"
                        ? TrangThaiChoXuLy
                        : d.TrangThai,
                    LaThuocKeDon = d.ChiTietDonHangs.Any(ct =>
                        ct.MaLoNavigation != null &&
                        ct.MaLoNavigation.MaThuocNavigation != null &&
                        ct.MaLoNavigation.MaThuocNavigation.LaThuocKeDon == true),

                    CapDoTre = d.NgayDat.HasValue && (bayGio - d.NgayDat.Value).TotalMinutes > 45 ? "urgent" :
                               d.NgayDat.HasValue && (bayGio - d.NgayDat.Value).TotalMinutes > 20 ? "warn" : "normal"
                })
                .ToListAsync();

            return Ok(ketQua);
        }

        [HttpGet("chi-tiet/{id}")]
        public async Task<IActionResult> LayChiTiet(int id)
        {
            var donHang = await _context.DonHangs
                .Where(d => d.MaDonHang == id)
                .Select(d => new
                {
                    MaDonHang = d.MaDonHang,
                    NgayDat = d.NgayDat.HasValue ? d.NgayDat.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    TenKhachHang = d.MaKhachHangNavigation != null ? d.MaKhachHangNavigation.HoTen : "",
                    SoDienThoaiNhan = d.SoDienThoaiNhan,
                    DiaChiGiaoHang = d.DiaChiGiaoHang, // Đã sửa đúng tên DiaChiGiaoHang

                    // VÌ MODEL KHÔNG CÓ GhiChu, MÌNH SẼ ĐỂ TRỐNG HOẶC DÙNG PhuongThucThanhToan ĐỂ HIỆN THỊ TẠM
                    GhiChu = "Phương thức: " + d.PhuongThucThanhToan,

                    TongTien = d.TongTien ?? 0,
                    TrangThai = d.TrangThai == "Chờ xác nhận" || d.TrangThai == "Chờ thanh toán" || d.TrangThai == "Đã thanh toán"
                        ? TrangThaiChoXuLy
                        : d.TrangThai,
                    AnhDonThuoc = d.AnhDonThuoc,
                    ChiTietSanPham = d.ChiTietDonHangs.Select(ct => new
                    {
                        MaChiTiet = ct.MaChiTiet,
                        TenThuoc = ct.MaLoNavigation != null && ct.MaLoNavigation.MaThuocNavigation != null
                                   ? ct.MaLoNavigation.MaThuocNavigation.TenThuoc
                                   : "Sản phẩm không xác định",
                        TenDonVi = ct.MaDvtNavigation != null ? ct.MaDvtNavigation.TenDonVi : "Đơn vị",
                        SoLuong = ct.SoLuong ?? 0,
                        GiaBan = ct.GiaBanTaiThoiDiem ?? 0,
                        ThanhTien = (ct.SoLuong ?? 0) * (ct.GiaBanTaiThoiDiem ?? 0)
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (donHang == null) return NotFound("Không tìm thấy đơn hàng");
            return Ok(donHang);
        }

        [HttpPut("cap-nhat-trang-thai/{id}")]
        public async Task<IActionResult> CapNhatTrangThai(int id, [FromBody] YeuCauCapNhatTrangThai yeuCau)
        {
            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null) return NotFound("Đơn hàng không tồn tại");

            donHang.TrangThai = ChuanHoaTrangThaiDonHang(yeuCau.TrangThaiMoi);

            // VÌ KHÔNG CÓ CỘT GhiChu TRONG DB, NÊN TẠM THỜI BỎ QUA VIỆC LƯU LÝ DO HỦY
            // HOẶC BẠN PHẢI THÊM CỘT GhiChu VÀO SQL SERVER RỒI SCAFFOLD LẠI
            /* if (yeuCau.TrangThaiMoi == "da-huy")
            {
                donHang.GhiChu = yeuCau.LyDoHuy; // Dòng này sẽ báo lỗi nếu bạn không có cột GhiChu
            }
            */

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { tinNhan = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi hệ thống: " + ex.Message);
            }
        }
    }
}
