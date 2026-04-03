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
        // PUT: api/DanhMuc/5 (Cập nhật danh mục)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] DanhMucDTO dto, IFormFile? icon)
        {
            var dm = await _context.DanhMucs.FindAsync(id);
            if (dm == null) return NotFound();

            dm.TenDanhMuc = dto.TenDanhMuc;
            dm.MaDanhMucCha = dto.MaDanhMucCha == 0 ? null : dto.MaDanhMucCha;
            dm.MoTa = dto.MoTa;
            dm.Slug = dto.Slug;
            dm.TrangThai = dto.TrangThai;

            if (icon != null)
            {
                // Xử lý lưu file tương tự như hàm Create
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(icon.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/icons", fileName);
                using (var stream = new FileStream(path, FileMode.Create)) { await icon.CopyToAsync(stream); }
                dm.Icon = "/uploads/icons/" + fileName;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thành công" });
        }

        // PUT: api/DanhMuc/5/thu-tu (Đổi thứ tự lên/xuống)
        [HttpPut("{id}/thu-tu")]
        public async Task<IActionResult> ChangeOrder(int id, [FromBody] dynamic body)
        {
            string huong = body.GetProperty("huong").ToString();
            var currentDm = await _context.DanhMucs.FindAsync(id);
            if (currentDm == null) return NotFound();

            // Logic đổi thứ tự: Tìm danh mục lân cận cùng cấp (cùng MaDanhMucCha)
            var listCungCap = await _context.DanhMucs
                .Where(x => x.MaDanhMucCha == currentDm.MaDanhMucCha)
                .OrderBy(x => x.ThuTu)
                .ToListAsync();

            int index = listCungCap.FindIndex(x => x.MaDanhMuc == id);

            if (huong == "len" && index > 0)
            {
                var target = listCungCap[index - 1];
                (currentDm.ThuTu, target.ThuTu) = (target.ThuTu, currentDm.ThuTu);
            }
            else if (huong == "xuong" && index < listCungCap.Count - 1)
            {
                var target = listCungCap[index + 1];
                (currentDm.ThuTu, target.ThuTu) = (target.ThuTu, currentDm.ThuTu);
            }

            await _context.SaveChangesAsync();
            return Ok();
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
            var dm = new DanhMuc
            {
                TenDanhMuc = dto.TenDanhMuc,
                MaDanhMucCha = dto.MaDanhMucCha == 0 ? null : dto.MaDanhMucCha,
                MoTa = dto.MoTa,
                Slug = dto.Slug,
                TrangThai = dto.TrangThai ?? "hien"
            };

            // Xử lý lưu File Icon nếu có
            if (icon != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(icon.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/icons", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await icon.CopyToAsync(stream);
                }
                dm.Icon = "/uploads/icons/" + fileName;
            }

            _context.DanhMucs.Add(dm);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Thêm thành công" });
        }

        // DELETE: api/DanhMuc/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dm = await _context.DanhMucs.FindAsync(id);
            if (dm == null) return NotFound();

            // Kiểm tra xem có sản phẩm nào đang dùng danh mục này không
            var hasProducts = await _context.Thuocs.AnyAsync(t => t.MaDanhMuc == id);
            if (hasProducts)
                return BadRequest(new { message = "Danh mục đang có sản phẩm, không thể xóa!" });

            _context.DanhMucs.Remove(dm);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã xóa danh mục" });
        }
    }
}