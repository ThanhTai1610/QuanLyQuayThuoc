using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyQuayThuoc.DTOs.NguoiDung;
using QuanLyQuayThuoc.Helpers;
using QuanLyQuayThuoc.Services.Interfaces;

namespace QuanLyQuayThuoc.Controllers.KhachHang
{
    [Route("api/Chatbot")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly IChatBotService _gemini;

        public ChatbotController(IChatBotService gemini)
        {
            _gemini = gemini;
        }

        [AllowAnonymous]
        [HttpPost("ask")]
        public async Task<IActionResult> AskGemini([FromBody] ChatRequest request)
        {
            try
            {
                var prompt = $@"Bạn là một dược sĩ tư vấn chuyên nghiệp tại nhà thuốc Pharmative.
Khách hàng hỏi: {request.Message}.
{(string.IsNullOrEmpty(request.TenThuoc) ? "" : $"Họ đang xem sản phẩm: {request.TenThuoc}.")}
Yêu cầu: Trả lời bằng tiếng Việt, chuyên nghiệp, ngắn gọn.";

                string rawJson = await _gemini.GenerateAsync(prompt);
                string reply = ChuanHoaGeminiHelper.LayText(rawJson);

                return Ok(new { reply });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Lỗi hệ thống", message = ex.Message });
            }
        }
    }
}