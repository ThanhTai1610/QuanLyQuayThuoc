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
                if (string.IsNullOrWhiteSpace(request.Message))
                    return BadRequest(new { message = "Nội dung câu hỏi không được để trống." });

                var prompt = $@"Bạn là dược sĩ tư vấn trực tuyến của nhà thuốc Pharmative.

Nhiệm vụ:
- Trả lời đúng trọng tâm câu hỏi khách hàng, không lan man.
- Nếu thông tin chưa đủ để tư vấn an toàn, chỉ hỏi tối đa 2 câu hỏi làm rõ quan trọng nhất.
- Ưu tiên tư vấn về: công dụng, cách dùng, lưu ý an toàn, đối tượng phù hợp, tương tác cơ bản, khi nào nên đi khám.
- Không khẳng định chẩn đoán chắc chắn.
- Không bịa thông tin không có trong câu hỏi.
- Nếu có dấu hiệu nguy hiểm như sốt cao kéo dài, khó thở, đau ngực, co giật, chảy máu nhiều, phát ban nặng hoặc triệu chứng nặng lên nhanh, phải khuyên đi khám/cấp cứu ngay.
- Không dùng giọng quảng cáo. Xưng hô là ""bạn"".

Ngữ cảnh sản phẩm đang xem:
{(string.IsNullOrWhiteSpace(request.TenThuoc) ? "Không có." : request.TenThuoc)}

Câu hỏi của khách hàng:
{request.Message}

Yêu cầu định dạng trả lời:
- Viết bằng tiếng Việt.
- Tối đa 3 đoạn ngắn.
- Ưu tiên câu trả lời trực tiếp trước, rồi mới đến lưu ý quan trọng.
- Nếu cần hỏi thêm, hỏi ngắn gọn ở cuối.
- Không dùng markdown, không mở đầu bằng lời chào dài.";

                string rawJson = await _gemini.GenerateAsync(prompt);
                string reply = ChuanHoaGeminiHelper.LayText(rawJson).Trim();

                return Ok(new { reply });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Lỗi hệ thống", message = ex.Message });
            }
        }
    }
}
