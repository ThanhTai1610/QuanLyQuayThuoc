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
                // 1. Tạo đối tượng mới, chỉ gán các trường cần thiết
                var nhanVienMoi = new NguoiDung
                {
                    HoTen = model.HoTen ?? "Chưa đặt tên", // Tránh lỗi null HoTen
                    Email = model.Email,
                    SoDienThoai = model.SoDienThoai,
                    MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau), // Mã hóa mật khẩu
                    MaVaiTro = 2, // Gán cứng là Nhân viên
                    TrangThai = "Hoạt động",
                    NgayTao = DateTime.Now,
                    GioiTinh = model.GioiTinh ?? "Nam"
                };

                // 2. Chỉ add đối tượng sạch này vào DB
                _context.NguoiDungs.Add(nhanVienMoi);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Thêm nhân viên thành công!" });
            }
            catch (Exception ex)
            {
                // Trả về lỗi chi tiết để debug dễ hơn
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = "Lỗi DB: " + innerException });
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
            if (model.MaNguoiDung == 0) // Trường hợp Thêm mới
            {
                if (await _context.NguoiDungs.AnyAsync(u => u.Email == model.Email))
                    return BadRequest("Email này đã tồn tại trên hệ thống.");

                // Hash mật khẩu bảo mật
                model.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);
                model.NgayTao = DateTime.Now;
                _context.NguoiDungs.Add(model);
            }
            else // Trường hợp Chỉnh sửa
            {
                var existingUser = await _context.NguoiDungs.FindAsync(model.MaNguoiDung);
                if (existingUser == null) return NotFound();

                existingUser.HoTen = model.HoTen;
                existingUser.SoDienThoai = model.SoDienThoai;
                existingUser.MaVaiTro = model.MaVaiTro;
                existingUser.TrangThai = model.TrangThai;
                existingUser.AnhDaiDien = model.AnhDaiDien;

                // Nếu admin nhập mật khẩu mới thì mới hash và cập nhật
                if (!string.IsNullOrEmpty(model.MatKhau) && model.MatKhau.Length < 30)
                {
                    existingUser.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Lưu dữ liệu thành công!" });
        }

        // 4. Khóa hoặc Mở khóa tài khoản
        [HttpPut("doi-trang-thai/{id}")]
        public async Task<IActionResult> DoiTrangThai(int id)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user == null) return NotFound();

            user.TrangThai = (user.TrangThai == "Hoạt động") ? "Bị khóa" : "Hoạt động";
            await _context.SaveChangesAsync();

            return Ok(new { trangThaiMoi = user.TrangThai });
        }

        // 5. Xóa người dùng (Cẩn thận: Thường nên dùng Khóa thay vì Xóa nếu đã có đơn hàng)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Xoa(int id)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user == null) return NotFound();

            // Kiểm tra nếu là Admin cuối cùng thì không cho xóa (tùy logic shop Tài)
            _context.NguoiDungs.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã xóa người dùng khỏi hệ thống." });
        }
    }
}