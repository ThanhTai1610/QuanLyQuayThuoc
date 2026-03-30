using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data; // Thay bằng namespace DbContext của Tài
using QuanLyQuayThuoc.Models; // Thay bằng namespace Model của Tài
using BCrypt.Net;

namespace QuanLyQuayThuoc.Controllers.QuanTriVien
{
    [Route("api/admin/[controller]")]
    [ApiController]
    // [Authorize(Roles = "1")] // Nếu MaVaiTro = 1 là Admin
    public class AdminNguoiDungController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminNguoiDungController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách người dùng kèm tên vai trò
        [HttpGet]
        public async Task<IActionResult> GetDanhSach()
        {
            try
            {
                // Chỉ lấy những người có MaVaiTro là 2 (Nhân viên) hoặc 3 (Khách hàng)
                var users = await _context.NguoiDungs
                    .Include(u => u.MaVaiTroNavigation)
                    .Where(u => u.MaVaiTro == 2 || u.MaVaiTro == 3) // Thêm dòng lọc này
                    .Select(u => new {
                        u.MaNguoiDung,
                        u.HoTen,
                        u.Email,
                        u.SoDienThoai,
                        u.TrangThai,
                        u.MaVaiTro,
                        TenVaiTro = u.MaVaiTroNavigation.TenVaiTro,
                        u.AnhDaiDien
                    })
                    .OrderByDescending(u => u.MaNguoiDung)
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("them-nhan-vien")]
        public async Task<IActionResult> ThemNhanVien([FromBody] NguoiDung model)
        {
            try
            {
                // 1. Kiểm tra xem Email đã tồn tại chưa (Tránh lỗi trùng Unique trong DB)
                var exists = await _context.NguoiDungs.AnyAsync(u => u.Email == model.Email);
                if (exists) return BadRequest(new { message = "Email này đã được sử dụng!" });

                // 2. Tạo đối tượng mới hoàn toàn "sạch"
                var nhanVienMoi = new NguoiDung
                {
                    HoTen = model.HoTen ?? "Nhân viên mới",
                    Email = model.Email,
                    SoDienThoai = model.SoDienThoai,
                    // Sử dụng BCrypt để băm mật khẩu (Nhớ cài gói NuGet BCrypt.Net-Next)
                    MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau ?? "123456"),
                    MaVaiTro = 2, // Mặc định là nhân viên
                    TrangThai = "Hoạt động",
                    NgayTao = DateTime.Now,
                    GioiTinh = model.GioiTinh ?? "Nam",
                    // Đảm bảo MaNguoiDung không được gán để DB tự tăng
                };

                _context.NguoiDungs.Add(nhanVienMoi);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Thêm nhân viên thành công!" });
            }
            catch (Exception ex)
            {
                // Lấy lỗi chi tiết nhất từ SQL Server
                var error = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { message = "Lỗi Database: " + error });
            }
        }
        // 2. Lấy chi tiết 1 người dùng
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChiTiet(int id)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user == null) return NotFound("Không tìm thấy người dùng");
            return Ok(user);
        }

        // 3. Thêm mới hoặc Cập nhật người dùng
        [HttpPost("luu")]
        public async Task<IActionResult> LuuNguoiDung([FromBody] NguoiDung model)
        {
            if (model.MaNguoiDung == 0)
            {
                model.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);
                model.NgayTao = DateTime.Now;
                _context.NguoiDungs.Add(model);
            }
            else
            {
                var user = await _context.NguoiDungs.FindAsync(model.MaNguoiDung);
                user.HoTen = model.HoTen;
                user.SoDienThoai = model.SoDienThoai;
                user.MaVaiTro = model.MaVaiTro;
                user.TrangThai = model.TrangThai;
                if (!string.IsNullOrEmpty(model.MatKhau) && model.MatKhau.Length < 30)
                    user.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Lưu thành công" });
        }

        public class DoiTrangThaiRequest
        {
            public string LyDo { get; set; }
        }

        [HttpPut("doi-trang-thai/{id}")]
        public async Task<IActionResult> DoiTrangThai(int id, [FromBody] DoiTrangThaiRequest request)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user == null) return NotFound(new { message = "Không tìm thấy người dùng" });

            // Đảo trạng thái
            if (user.TrangThai == "Hoạt động")
            {
                user.TrangThai = "Bị khóa";
                // user.GhiChu = "Bị khóa vì: " + request.LyDo;
            }
            else
            {
                user.TrangThai = "Hoạt động";
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                trangThaiMoi = user.TrangThai,
                message = "Cập nhật thành công"
            });
        }


    }
}