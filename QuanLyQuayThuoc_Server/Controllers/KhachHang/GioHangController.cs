using Microsoft.AspNetCore.Mvc;
using QuanLyQuayThuoc.DTOs.DonHang;
using QuanLyQuayThuoc.Services.Interface;
using QuanLyQuayThuoc.Services.Interfaces;
using System.Security.Claims;

namespace QuanLyQuayThuoc.Controllers.KhachHang
{
    [Route("api/GioHang")]
    [ApiController]
    // [Authorize] // Mở ra nếu bạn đã dùng JWT Token để bảo mật
    public class GioHangController : ControllerBase
    {
        private readonly IGioHangService _gioHangService;

        public GioHangController(IGioHangService gioHangService)
        {
            _gioHangService = gioHangService;
        }

        // 1. Lấy danh sách giỏ hàng
        // GET: api/GioHang
        [HttpGet]
        public async Task<IActionResult> LayGioHang()
        {
            // Tạm thời lấy MaKhachHang = 1 để test nếu chưa có đăng nhập
            // Nếu có JWT, dùng: int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            int maKhachHang = 1;

            var ketQua = await _gioHangService.LayDanhSachGioHangAsync(maKhachHang);
            return Ok(ketQua);
        }

        // 2. Thêm sản phẩm vào giỏ
        // POST: api/GioHang/them
        [HttpPost("them")]
        public async Task<IActionResult> ThemVaoGio([FromBody] ThemVaoGioDto dto)
        {
            int maKhachHang = 1; // Giả định khách hàng ID là 1
            var thanhCong = await _gioHangService.ThemVaoGioHangAsync(
                maKhachHang, dto.MaThuoc, dto.MaDvt, dto.SoLuong);

            if (thanhCong) return Ok(new { message = "Đã thêm vào giỏ hàng" });
            return BadRequest("Không thể thêm vào giỏ hàng");
        }

        // 3. Cập nhật giỏ hàng (Khớp với nút "Cập nhật giỏ hàng" ở Vue)
        // PUT: api/GioHang/cap-nhat
        [HttpPut("cap-nhat")]
        public async Task<IActionResult> CapNhatGio([FromBody] List<CapNhatGioHangDto> danhSach)
        {
            if (danhSach == null || danhSach.Count == 0)
                return BadRequest("Danh sách cập nhật trống");

            var thanhCong = await _gioHangService.CapNhatGioHangAsync(danhSach);
            if (thanhCong) return Ok(new { message = "Cập nhật thành công" });
            return BadRequest("Cập nhật thất bại");
        }

        // 4. Xóa 1 sản phẩm
        // DELETE: api/GioHang/xoa/{id}
        [HttpDelete("xoa/{id}")]
        public async Task<IActionResult> XoaSanPham(int id)
        {
            var thanhCong = await _gioHangService.XoaKhoiGioHangAsync(id);
            if (thanhCong) return Ok(new { message = "Đã xóa sản phẩm" });
            return NotFound("Không tìm thấy sản phẩm trong giỏ");
        }

        // 5. Xóa sạch giỏ hàng
        // DELETE: api/GioHang/xoa-tat-ca
        [HttpDelete("xoa-tat-ca")]
        public async Task<IActionResult> XoaTatCa()
        {
            int maKhachHang = 1;
            var thanhCong = await _gioHangService.XoaToanBoGioHangAsync(maKhachHang);
            return Ok(new { message = "Giỏ hàng đã được làm trống" });
        }

        // 6. Đặt hàng online (dành cho khách hàng)
        // POST: api/GioHang/dat-hang
        [HttpPost("dat-hang")]
        public async Task<IActionResult> DatHang([FromBody] DatHangKhachHangDto dto)
        {
            try
            {
                if (dto == null || dto.ChiTiet == null || dto.ChiTiet.Count == 0)
                    return BadRequest(new { success = false, message = "Giỏ hàng trống." });

                // Validate MaLo - tránh trường hợp sản phẩm hết hàng
                if (dto.ChiTiet.Any(c => c.MaLo <= 0))
                    return BadRequest(new { success = false, message = "Một số sản phẩm đã hết hàng, vui lòng xóa khỏi giỏ trước khi đặt." });

                int maKhachHang = 1; // Sau này thay bằng: int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var maDonHang = await _gioHangService.DatHangAsync(dto, maKhachHang);

                return Ok(new { success = true, maDonHang = maDonHang });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    // DTO bổ trợ để nhận dữ liệu khi thêm vào giỏ
    public class ThemVaoGioDto
    {
        public int MaThuoc { get; set; }
        public int MaDvt { get; set; }
        public int SoLuong { get; set; }
    }
}