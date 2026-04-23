using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyQuayThuoc.Services.Interfaces;
using QuanLyQuayThuoc.Services.Models;

namespace QuanLyQuayThuoc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LienHeController : ControllerBase
    {
        private readonly IEmailQueueService _emailQueueService;

        public LienHeController(IEmailQueueService emailQueueService)
        {
            _emailQueueService = emailQueueService;
        }

        [HttpPost("GuiDonThuoc")]
        public async Task<IActionResult> GuiDonThuoc([FromForm] DonThuocRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.HoTen) || string.IsNullOrWhiteSpace(request.SoDienThoai))
                return BadRequest(new { message = "Vui lòng nhập họ tên và số điện thoại." });

            if (request.Files == null || request.Files.Count == 0)
                return BadRequest(new { message = "Vui lòng tải ảnh đơn thuốc." });

            string adminEmail = "taiptpk04158@gmail.com";
            string subject = $"[ĐƠN THUỐC MỚI] - Khách hàng: {request.HoTen}";

            string body = $@"
                <h3>Yêu cầu tư vấn đơn thuốc mới</h3>
                <p><b>Khách hàng:</b> {request.HoTen}</p>
                <p><b>Số điện thoại:</b> {request.SoDienThoai}</p>
                <p><b>Sản phẩm cần mua:</b> {request.TenThuoc} (Số lượng: {request.SoLuong})</p>
                <p><b>Ghi chú:</b> {request.GhiChu}</p>
                <hr/>
                <p><i>Vui lòng kiểm tra ảnh đính kèm để xem chi tiết đơn thuốc.</i></p>";

            var attachments = new List<EmailAttachmentData>();

            foreach (var file in request.Files)
            {
                await using var stream = file.OpenReadStream();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);

                attachments.Add(new EmailAttachmentData
                {
                    FileName = file.FileName,
                    ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                    Content = memoryStream.ToArray()
                });
            }

            await _emailQueueService.QueueEmailAsync(new EmailQueueItem
            {
                ToEmail = adminEmail,
                Subject = subject,
                Body = body,
                Attachments = attachments
            });

            return Ok(new { message = "Đã ghi nhận yêu cầu tư vấn. Dược sĩ sẽ sớm liên hệ với bạn." });
        }
    }

    public class DonThuocRequest
    {
        public string HoTen { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
        public string TenThuoc { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
