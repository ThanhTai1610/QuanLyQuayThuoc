using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Bắt buộc để dùng .Include
using Microsoft.AspNetCore.SignalR; // Bắt buộc để dùng IHubContext
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.Hubs;

namespace QuanLyQuayThuoc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<BarcodeHub> _hubContext;

        public WebhookController(ApplicationDbContext context, IHubContext<BarcodeHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost("scan")]
        public async Task<IActionResult> ReceiveScan([FromQuery(Name = "maVach")] string maVach)
        {
            // Tìm Đơn vị tính kèm theo thông tin Thuốc (Sử dụng Include)
            var dvt = await _context.DonViTinhs
                .Include(d => d.MaThuocNavigation)
                .FirstOrDefaultAsync(x => x.MaVach == maVach);

            if (dvt == null) return NotFound("Mã vạch không tồn tại");

            // Tạo Object gửi sang Vue.js (Phải khớp với cấu trúc hàm themVaoGioHang ở FE)
            var thongTinGuiDi = new
            {
                maThuoc = dvt.MaThuoc,
                tenThuoc = dvt.MaThuocNavigation.TenThuoc,
                giaBan = dvt.GiaBan,
                maDvtSelected = dvt.MaDvt,
                tenDvt = dvt.TenDonVi,
                // Lấy thêm danh sách lô để người dùng chọn trong giỏ hàng
                danhSachLo = await _context.LoHangs
                    .Where(l => l.MaThuoc == dvt.MaThuoc && l.SoLuongTon > 0)
                    .Select(l => new { maLo = l.MaLo, soLo = l.SoLo })
                    .ToListAsync()
            };

            // Bắn tín hiệu qua SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveBarcode", thongTinGuiDi);

            return Ok(new { message = "Đã tìm thấy và gửi dữ liệu!" });
        }
    }
}