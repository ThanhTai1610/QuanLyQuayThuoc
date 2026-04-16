using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyQuayThuoc.Helpers;

namespace QuanLyQuayThuoc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class LienHeController : ControllerBase
    {
        [HttpPost("GuiDonThuoc")]
        public async Task<IActionResult> GuiDonThuoc([FromForm] DonThuocRequest request)
        {
            // 1. Tạo nội dung Email
            string adminEmail = "taiptpk04158@gmail.com"; // Email nhận thông báo
            string subject = $"[ĐƠN THUỐC MỚI] - Khách hàng: {request.HoTen}";

            string body = $@"
                <h3>Yêu cầu tư vấn đơn thuốc mới</h3>
                <p><b>Khách hàng:</b> {request.HoTen}</p>
                <p><b>Số điện thoại:</b> {request.SoDienThoai}</p>
                <p><b>Sản phẩm cần mua:</b> {request.TenThuoc} (Số lượng: {request.SoLuong})</p>
                <p><b>Ghi chú:</b> {request.GhiChu}</p>
                <hr/>
                <p><i>Vui lòng kiểm tra ảnh đính kèm để xem chi tiết đơn thuốc.</i></p>";

            // 2. Gọi Helper gửi Mail
            bool isSent = await EmailHelper.SendEmailWithAttachmentsAsync(adminEmail, subject, body, request.Files);

            if (isSent)
                return Ok(new { message = "Gửi yêu cầu thành công!" });

            return BadRequest(new { message = "Gửi mail thất bại, Tài kiểm tra lại cấu hình SMTP nhé!" });
        }
    }

    // Class hứng dữ liệu từ Form
    public class DonThuocRequest
    {
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public string? GhiChu { get; set; }
        public string TenThuoc { get; set; }
        public int SoLuong { get; set; }
        public List<IFormFile>? Files { get; set; } // Hứng mảng ảnh từ Frontend
    }
}