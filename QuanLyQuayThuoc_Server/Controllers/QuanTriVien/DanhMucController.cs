using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data; // Thay bằng namespace DbContext của bạn
using QuanLyQuayThuoc.DTOs.SanPham;
using QuanLyQuayThuoc.Models; // Thay bằng namespace Model của bạn

namespace QuanLyQuayThuoc.Controllers.QuanTriVien
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanhMucController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DanhMucController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("cay")]
        public async Task<ActionResult<IEnumerable<DanhMucDTO>>> GetCayDanhMuc()
        {
            // 1. Lấy toàn bộ danh sách từ DB
            var allItems = await _context.DanhMucs
                .OrderBy(d => d.ThuTu)
                .Select(d => new DanhMucDTO
                {
                    MaDanhMuc = d.MaDanhMuc,
                    TenDanhMuc = d.TenDanhMuc,
                    MaDanhMucCha = d.MaDanhMucCha,
                    Icon = d.Icon,
                    Slug = d.Slug,
                    TrangThai = d.TrangThai,
                    SoSanPham = _context.Thuocs.Count(t => t.MaDanhMuc == d.MaDanhMuc)
                })
                .ToListAsync();

            // 2. Tạo một Dictionary để truy xuất nhanh
            var lookup = allItems.ToDictionary(x => x.MaDanhMuc);
            var rootNodes = new List<DanhMucDTO>();

            // 3. Duyệt qua mảng để xây dựng cấu trúc cây
            foreach (var item in allItems)
            {
                if (item.MaDanhMucCha == null || item.MaDanhMucCha == 0)
                {
                    rootNodes.Add(item);
                }
                else if (lookup.ContainsKey(item.MaDanhMucCha.Value))
                {
                    // Nếu có cha, thì add mục hiện tại vào list Children của cha nó
                    lookup[item.MaDanhMucCha.Value].Children.Add(item);
                }
            }

            return Ok(rootNodes);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] DanhMucDTO dto, IFormFile? icon)
        {
            var dm = await _context.DanhMucs.FindAsync(id);
            if (dm == null) return NotFound();

            dm.TenDanhMuc = dto.TenDanhMuc;
            dm.MaDanhMucCha = (dto.MaDanhMucCha == 0) ? null : dto.MaDanhMucCha;
            dm.MoTa = dto.MoTa;
            dm.Slug = dto.Slug;
            dm.TrangThai = dto.TrangThai;

            // Xử lý Icon: Nếu có file thì lưu file, nếu không thì lấy chuỗi Icon từ DTO (Class FA)
            if (icon != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(icon.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/icons", fileName);
                using (var stream = new FileStream(path, FileMode.Create)) { await icon.CopyToAsync(stream); }
                dm.Icon = "/uploads/icons/" + fileName;
            }
            else
            {
                // Nếu không có file upload, thì giữ nguyên hoặc cập nhật theo Class icon từ Vue gửi về
                dm.Icon = dto.Icon;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thành công" });
        }

        // Hàm đệ quy để tìm con
        private void BuildTree(DanhMucDTO parent, List<DanhMucDTO> allItems)
        {
            var children = allItems.Where(d => d.MaDanhMucCha == parent.MaDanhMuc).ToList();
            parent.Children = children;
            foreach (var child in children)
            {
                BuildTree(child, allItems);
            }
        }

        // POST: api/DanhMuc (Thêm mới)
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] DanhMucDTO dto, IFormFile? icon)
        {
            // 1. Khởi tạo đối tượng Model từ DTO
            var dm = new DanhMuc
            {
                TenDanhMuc = dto.TenDanhMuc,
                // Nếu MaDanhMucCha là 0 hoặc null thì gán null vào DB
                MaDanhMucCha = (dto.MaDanhMucCha == 0 || dto.MaDanhMucCha == null) ? null : dto.MaDanhMucCha,
                MoTa = dto.MoTa,
                Slug = dto.Slug,
                TrangThai = dto.TrangThai ?? "hien",
                ThuTu = 0 // Mặc định thứ tự là 0 khi mới thêm
            };

            // 2. Xử lý Icon
            if (icon != null)
            {
                // Nếu có file upload (ưu tiên ảnh)
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(icon.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/icons", fileName);

                // Đảm bảo thư mục tồn tại
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await icon.CopyToAsync(stream);
                }
                dm.Icon = "/uploads/icons/" + fileName;
            }
            else
            {
                // Nếu không có file, lấy giá trị Icon từ DTO (ví dụ: "fa-capsules")
                dm.Icon = dto.Icon;
            }

            // 3. Lưu vào Database
            try
            {
                _context.DanhMucs.Add(dm);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Thêm danh mục mới thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lưu dữ liệu: " + ex.Message });
            }
        }
        // PUT: api/DanhMuc/{id}/thu-tu
        [HttpPut("{id}/thu-tu")]
        public async Task<IActionResult> ChangeOrder(int id, [FromBody] System.Text.Json.JsonElement body)
        {
            // Lấy hướng đi từ body (len hoặc xuong)
            string huong = body.GetProperty("huong").GetString();

            var currentDm = await _context.DanhMucs.FindAsync(id);
            if (currentDm == null) return NotFound();

            // Lấy danh sách các danh mục CÙNG CHA để đổi chỗ cho nhau
            var listCungCap = await _context.DanhMucs
                .Where(x => x.MaDanhMucCha == currentDm.MaDanhMucCha)
                .OrderBy(x => x.ThuTu)
                .ToListAsync();

            int index = listCungCap.FindIndex(x => x.MaDanhMuc == id);

            if (huong == "len" && index > 0)
            {
                // Đổi thứ tự với thằng đứng trước
                var target = listCungCap[index - 1];

                // Sử dụng ?? 0 để tránh lỗi null sang int
                int currentOrder = currentDm.ThuTu ?? 0;
                int targetOrder = target.ThuTu ?? 0;

                currentDm.ThuTu = targetOrder;
                target.ThuTu = currentOrder;
            }
            else if (huong == "xuong" && index < listCungCap.Count - 1)
            {
                // Đổi thứ tự với thằng đứng sau
                var target = listCungCap[index + 1];

                int currentOrder = currentDm.ThuTu ?? 0;
                int targetOrder = target.ThuTu ?? 0;

                currentDm.ThuTu = targetOrder;
                target.ThuTu = currentOrder;
            }
            else
            {
                return BadRequest("Không thể di chuyển thêm.");
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã đổi thứ tự thành công" });
        }

        // DELETE: api/DanhMuc/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dm = await _context.DanhMucs.FindAsync(id);
            if (dm == null) return NotFound();

            // 1. Chặn nếu có danh mục con
            var hasChildren = await _context.DanhMucs.AnyAsync(x => x.MaDanhMucCha == id);
            if (hasChildren)
                return BadRequest(new { message = "Danh mục này có danh mục con, không thể xóa!" });

            // 2. Chặn nếu có sản phẩm (Thuốc) đang thuộc danh mục này
            var hasProducts = await _context.Thuocs.AnyAsync(t => t.MaDanhMuc == id);
            if (hasProducts)
                return BadRequest(new { message = "Danh mục đang có sản phẩm, hãy xóa sản phẩm trước khi xóa danh mục!" });

            _context.DanhMucs.Remove(dm);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã xóa danh mục thành công" });
        }
    }
}