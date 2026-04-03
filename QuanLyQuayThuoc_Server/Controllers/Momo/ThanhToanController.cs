using Azure.Core;
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

        // Tiêm (Inject) Service vào để sử dụng
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

                if (result == null || string.IsNullOrEmpty(result.PayUrl))
                {
                    return BadRequest(new { message = "Không thể tạo liên kết thanh toán MoMo." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("ket-qua-momo")]
        public async Task<IActionResult> PaymentCallback()
        {
            // 1. Lấy dữ liệu MoMo trả về 
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);

            // 2. Kiểm tra nếu ResultCode == "0" (Thành công) [cite: 14]
            if (response.ResultCode == "0")
            { // TẠI ĐÂY: Gọi Repository của bạn để cập nhật DB [cite: 26, 27]
                            // Ví dụ: _donHangRepository.UpdateStatus(response.OrderId, "DaThanhToan");

                return Ok(new
                {
                    Message = "Thanh toán thành công!",
                    OrderId = response.OrderId
                });
            }

            return BadRequest(new { Message = "Thanh toán thất bại hoặc bị hủy." });
        }
    }
}