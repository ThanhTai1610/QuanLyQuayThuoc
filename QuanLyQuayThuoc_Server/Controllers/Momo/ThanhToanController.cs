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

        [HttpGet("ket-qua-momo")]
        public async Task<IActionResult> KetQuaMoMo()
        {
            var query = HttpContext.Request.Query;

            if (query == null || !query.ContainsKey("resultCode"))
                return Content("Lỗi phản hồi MoMo: Không tìm thấy resultCode");

            string resultCode = query["resultCode"].ToString();
            string momoOrderId = query["orderId"].ToString(); // Ví dụ: "37_1712712345"
            string userType = query.ContainsKey("extraData") ? query["extraData"].ToString() : "KhachHang";

            // ✅ BƯỚC 1: Tách chuỗi để lấy ID gốc
            // Nếu có dấu '_', lấy phần tử đầu tiên. Nếu không, giữ nguyên mã.
            string originalIdStr = momoOrderId.Contains("_") ? momoOrderId.Split('_')[0] : momoOrderId;

            // Cập nhật Database nếu thành công
            if (resultCode == "0")
            {
                // ✅ BƯỚC 2: Dùng originalIdStr đã tách để ép kiểu sang int
                if (int.TryParse(originalIdStr, out int maDonHang))
                {
                    var donHang = await _context.DonHangs.FindAsync(maDonHang);
                    if (donHang != null)
                    {
                        // Sau khi thanh toán thành công, đơn vẫn cần nhân viên xác nhận xử lý.
                        donHang.TrangThai = "Chờ xử lý";
                        donHang.PhuongThucThanhToan = "Momo";
                        await _context.SaveChangesAsync();
                    }
                }
            }

            string status = (resultCode == "0") ? "success" : "error";

            // Trả về Frontend: 
            // Bạn nên trả về momoOrderId (để Frontend tự tách) hoặc originalIdStr tùy ý. 
            // Ở đây mình trả về momoOrderId để khớp với code Frontend mình đã hướng dẫn bạn split.
            if (userType == "NhanVien")
            {
                return Redirect($"https://quan-ly-quay-thuoc-client.vercel.app/nhan-vien/ban-hang?orderId={momoOrderId}&status={status}");
            }

            return Redirect($"https://quan-ly-quay-thuoc-client.vercel.app/dat-hang?orderId={momoOrderId}&status={status}");
        }
    }
}
