using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.DTOs.SanPham;

namespace QuanLyQuayThuoc.Controllers.KhachHang
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChuDeSucKhoeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChuDeSucKhoeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách các tab chủ đề (Cúm, Sốt xuất huyết...)
        [HttpGet]
        public async Task<IActionResult> GetChuDe()
        {
            var chuDe = await _context.ChuDeSucKhoes
                .Where(c => c.TrangThai == true)
                .Select(c => new {
                    c.MaChuDe,
                    c.TenChuDe
                })
                .ToListAsync();
            return Ok(chuDe);
        }

        // 2. Lấy Full thông tin bao gồm Card giới thiệu + Danh sách sản phẩm
        [HttpGet("{id}/san-pham")]
        public async Task<IActionResult> GetSanPhamTheoChuDe(int id)
        {
            // Lấy thông tin chi tiết của chủ đề để hiển thị card bên trái
            var infoChuDe = await _context.ChuDeSucKhoes
                .Where(c => c.MaChuDe == id)
                .Select(c => new {
                    c.MaChuDe,
                    c.TenChuDe,
                    c.TieuDePhu,
                    c.NoiDungGiaiPhap,
                     // Đường dẫn ảnh nền hoặc icon virus như trong mẫu
                })
                .FirstOrDefaultAsync();

            if (infoChuDe == null) return NotFound();

            // Lấy danh sách sản phẩm thuộc chủ đề đó
            var sanPhams = await _context.Thuocs
                .Where(t => t.MaChuDes.Any(c => c.MaChuDe == id))
                .Select(t => new SanPhamChuDeDTO
                {
                    MaThuoc = t.MaThuoc,
                    TenThuoc = t.TenThuoc,
                    HinhAnhChinh = t.HinhAnhChinh,
                    GiaBan = t.DonViTinhs.Select(d => d.GiaBan).FirstOrDefault() ?? 0,
                    TenDonVi = t.DonViTinhs.Select(d => d.TenDonVi).FirstOrDefault() ?? "",
                    QuyCach = t.QuyCach,
                    NuocSanXuat = t.NuocSanXuat
                })
                .ToListAsync();

            // Trả về object gộp cả 2 phần dữ liệu
            return Ok(new
            {
                Info = infoChuDe,
                Products = sanPhams
            });
        }
    }
}