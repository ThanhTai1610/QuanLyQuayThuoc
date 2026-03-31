using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.DTOs.SanPham;

namespace QuanLyQuayThuoc.Controllers.KhachHang
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThuocController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ThuocController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetDanhSachThuoc(
            [FromQuery] string? q,
            [FromQuery] string? danhMuc,
            [FromQuery] string? gia,
            [FromQuery] string? sapXep,
            [FromQuery] int trang = 1,
            [FromQuery] int soLuong = 12)
        {
            var query = _context.Thuocs.AsQueryable();

            // 1. Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(q))
                query = query.Where(t => t.TenThuoc.Contains(q));

            // 2. Lọc theo danh mục (hỗ trợ nhiều mã cách nhau dấu phẩy)
            if (!string.IsNullOrEmpty(danhMuc))
            {
                var ids = danhMuc.Split(',').Select(int.Parse).ToList();
                query = query.Where(t => ids.Contains(t.MaDanhMuc ?? 0));
            }

            // 3. Lọc theo giá (Lấy từ bảng DonViTinh - Đơn vị cơ bản)
            var filteredQuery = query.Select(t => new ThuocHienThiDto
            {
                MaThuoc = t.MaThuoc,
                TenThuoc = t.TenThuoc,
                HinhAnhChinh = t.HinhAnhChinh,
                QuyCach = t.QuyCach,
                NuocSanXuat = t.NuocSanXuat,
                LaThuocKeDon = t.LaThuocKeDon ?? false,
                GiaBan = t.DonViTinhs.Where(d => d.LaDonViCoBan == true).Select(d => d.GiaBan ?? 0).FirstOrDefault(),
                TenDonVi = t.DonViTinhs.Where(d => d.LaDonViCoBan == true).Select(d => d.TenDonVi).FirstOrDefault(),
                MaDVT = t.DonViTinhs.Where(d => d.LaDonViCoBan == true).Select(d => d.MaDvt).FirstOrDefault()
            });

            if (!string.IsNullOrEmpty(gia))
            {
                var range = gia.Split('-');
                decimal min = decimal.Parse(range[0]);
                decimal max = decimal.Parse(range[1]);
                filteredQuery = filteredQuery.Where(x => x.GiaBan >= min && x.GiaBan <= max);
            }

            // 4. Sắp xếp
            filteredQuery = sapXep switch
            {
                "gia-tang" => filteredQuery.OrderBy(x => x.GiaBan),
                "gia-giam" => filteredQuery.OrderByDescending(x => x.GiaBan),
                "moi-nhat" => filteredQuery.OrderByDescending(x => x.MaThuoc),
                _ => filteredQuery.OrderByDescending(x => x.MaThuoc) // Mặc định
            };

            var total = await filteredQuery.CountAsync();
            var items = await filteredQuery.Skip((trang - 1) * soLuong).Take(soLuong).ToListAsync();

            return Ok(new { total, items });
        }

        [HttpGet("nha-san-xuat")]
        public async Task<IActionResult> GetNSX() =>
            Ok(await _context.Thuocs.Select(t => t.NhaSanXuat).Distinct().ToListAsync());
    }
}
