using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.Models.Momo;
using QuanLyQuayThuoc.Services.Momo;

namespace QuanLyQuayThuoc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThanhToanController : ControllerBase
    {
        private readonly IMomoService _momoService;
        private readonly ApplicationDbContext _context;

        public ThanhToanController(IMomoService momoService, ApplicationDbContext context)
        {
            _momoService = momoService;
            _context = context;
        }

        [HttpPost("tao-thanh-toan")]
        public async Task<IActionResult> CreatePayment([FromBody] OrderInfoModel request)
        {
            if (request == null)
                return BadRequest(new { message = "Dữ liệu yêu cầu không hợp lệ." });

            try
            {
                var result = await _momoService.CreatePaymentAsync(request);

                if (result == null || (result.ResultCode != 0 && string.IsNullOrEmpty(result.PayUrl)))
                {
                    var errorMsg = result?.Message ?? "Lỗi không xác định từ MoMo";
                    return BadRequest(new { message = "MoMo từ chối tạo thanh toán.", detail = errorMsg });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi gọi MoMo: " + ex.Message });
            }
        }

        /// <summary>
        /// SỬA ĐỔI: Điều hướng trực tiếp về trang đặt hàng/bán hàng thay vì trang hoan-tat riêng biệt
        /// </summary>
        [HttpGet("ket-qua-momo")]
        public async Task<IActionResult> KetQuaMoMo()
        {
            var query = HttpContext.Request.Query;

            if (query == null || !query.ContainsKey("resultCode"))
                return Content("Lỗi phản hồi MoMo: Không tìm thấy resultCode");

            string resultCode = query["resultCode"].ToString();
            string orderId = query["orderId"].ToString();
            // extraData được gán là "NhanVien" hoặc "KhachHang" từ Frontend gửi lên
            string userType = query.ContainsKey("extraData") ? query["extraData"].ToString() : "KhachHang";

            // Cập nhật Database nếu thành công
            if (resultCode == "0")
            {
                if (int.TryParse(orderId, out int maDonHang))
                {
                    var donHang = await _context.DonHangs.FindAsync(maDonHang);
                    if (donHang != null)
                    {
                        donHang.TrangThai = "Đã thanh toán";
                        donHang.PhuongThucThanhToan = "Momo";
                        await _context.SaveChangesAsync();
                    }
                }
            }

            string status = (resultCode == "0") ? "success" : "error";

            // --- PHẦN SỬA ĐỔI ĐƯỜNG DẪN REDIRECT ---

            // Nếu là Nhân viên: Quay về trang Bán hàng tại quầy
            if (userType == "NhanVien")
            {
                // Giả sử route của trang bán hàng là /ban-hang hoặc /nhan-vien/ban-hang
                return Redirect($"http://localhost:5173/ban-hang?orderId={orderId}&status={status}");
            }

            // Nếu là Khách hàng: Quay về chính trang Đặt hàng (để hiện Swal thông báo)
            // Giả sử route trang đặt hàng của khách là /dat-hang hoặc /checkout
            return Redirect($"http://localhost:5173/dat-hang?orderId={orderId}&status={status}");
        }
    }
}