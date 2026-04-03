using Microsoft.AspNetCore.Mvc;
using QuanLyQuayThuoc.Models.Momo;
using QuanLyQuayThuoc.Services.Momo;

namespace QuanLyQuayThuoc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThanhToanController : ControllerBase
    {
        private readonly IMomoService _momoService;

        public ThanhToanController(IMomoService momoService)
        {
            _momoService = momoService;
        }

        [HttpPost("tao-thanh-toan")]
        public async Task<IActionResult> CreatePayment([FromBody] OrderInfoModel request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Dữ liệu yêu cầu không hợp lệ." });
            }

            try
            {
                // Gọi đến Service để tạo link thanh toán MoMo
                var result = await _momoService.CreatePaymentAsync(request);

                // --- ĐOẠN DEBUG QUAN TRỌNG ---
                if (result == null || (result.ResultCode != 0 && string.IsNullOrEmpty(result.PayUrl)))
                {
                    // Nếu MoMo trả về lỗi (ResultCode khác 0), in ra để Tài thấy
                    var errorMsg = result?.Message ?? "Lỗi không xác định từ MoMo";
                    Console.WriteLine($"[MoMo Error] ResultCode: {result?.ResultCode}, Message: {errorMsg}");

                    return BadRequest(new
                    {
                        message = "MoMo từ chối tạo thanh toán.",
                        detail = errorMsg,
                        momoCode = result?.ResultCode
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[System Error] {ex.Message}");
                return StatusCode(500, new { message = "Lỗi hệ thống khi gọi MoMo: " + ex.Message });
            }
        }

        [HttpGet("ket-qua-momo")]
        public IActionResult PaymentCallback()
        {
            // Lấy dữ liệu MoMo trả về từ URL
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);

            // Kiểm tra trạng thái thanh toán (ResultCode "0" là thành công)
            if (response.ResultCode == "0")
            {
                // Sau này Tài thêm code cập nhật database ở đây nhé
                // Ví dụ: _donHangRepository.CapNhatTrangThai(response.OrderId, "DaThanhToan");

                // Thay vì return Ok, Tài nên Redirect về trang thành công của Vue
                return Redirect($"http://localhost:5173/hoan-tat?orderId={response.OrderId}&status=success");
            }

            // Nếu thất bại, redirect về trang lỗi
            return Redirect($"http://localhost:5173/hoan-tat?orderId={response.OrderId}&status=error");
        }
    }
}