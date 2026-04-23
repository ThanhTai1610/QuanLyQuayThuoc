using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.DTOs.SanPham;

namespace QuanLyQuayThuoc.Controllers.KhachHang
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThuocKhachHangController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ThuocKhachHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ThuocKhachHang/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SanPhamDetailDto>> GetDetail(int id)
        {
            var thuoc = await _context.Thuocs
                .Include(t => t.DonViTinhs)
                .Include(t => t.HinhAnhThuocs)
                .Include(t => t.LoHangs)
                .FirstOrDefaultAsync(t => t.MaThuoc == id);

            if (thuoc == null) return NotFound("Không tìm thấy thuốc!");

            // Tăng lượt xem (LuotXem++)
            thuoc.LuotXem = (thuoc.LuotXem ?? 0) + 1;
            await _context.SaveChangesAsync();

            // Mapping Entity sang DTO
            var dto = new SanPhamDetailDto
            {
                MaThuoc = thuoc.MaThuoc,
                TenThuoc = thuoc.TenThuoc,
                MaDanhMuc = thuoc.MaDanhMuc,
                SoDangKy = thuoc.SoDangKy,
                QuyCach = thuoc.QuyCach,
                DangBaoChe = thuoc.DangBaoChe,
                NhaSanXuat = thuoc.NhaSanXuat,
                NuocSanXuat = thuoc.NuocSanXuat,
                HanSuDungThang = thuoc.HanSuDungThang,
                LaThuocKeDon = thuoc.LaThuocKeDon ?? false,
                HinhAnhChinh = thuoc.HinhAnhChinh,
                MoTaNgan = thuoc.MoTaNgan,
                ThanhPhan = thuoc.ThanhPhan,
                CongDung = thuoc.CongDung,
                CachDung = thuoc.CachDung,
                DoiTuongSuDung = thuoc.DoiTuongSuDung,
                ChongChiDinh = thuoc.ChongChiDinh,
                TacDungPhu = thuoc.TacDungPhu,
                LuuY = thuoc.LuuY,
                BaoQuan = thuoc.BaoQuan,

                DonViTinhs = thuoc.DonViTinhs.Select(d => new DonViTinhDto
                {
                    MaDvt = d.MaDvt,
                    TenDonVi = d.TenDonVi,
                    GiaBan = d.GiaBan ?? 0,
                    GiaTriQuyDoi = d.GiaTriQuyDoi,
                    LaDonViCoBan = d.LaDonViCoBan ?? false
                }).ToList(),

                HinhAnhThuocs = thuoc.HinhAnhThuocs.Select(h => h.DuongDan).ToList(),

                LoHangs = thuoc.LoHangs.Select(l => new LoHangDto
                {
                    MaLo = l.MaLo,
                    SoLo = l.SoLo,
                    SoLuongTon = l.SoLuongTon,
                    HanSuDung = l.HanSuDung.ToDateTime(TimeOnly.MinValue)
                }).ToList()
            };

            return Ok(dto);
        }

        [HttpGet("BestSellers")]
        public async Task<IActionResult> GetBestSellers()
        {
            try
            {
                var topSellingData = await _context.ChiTietDonHangs
                    .Join(_context.LoHangs, ct => ct.MaLo, l => l.MaLo, (ct, l) => new { l.MaThuoc, ct.SoLuong })
                    .GroupBy(x => x.MaThuoc)
                    .Select(g => new {
                        MaThuoc = g.Key,
                        TongDaBan = g.Sum(x => (int?)x.SoLuong) ?? 0
                    })
                    .OrderByDescending(x => x.TongDaBan)
                    .Take(6)
                    .ToListAsync();

                if (!topSellingData.Any()) return Ok(new List<object>());

                var ids = topSellingData.Select(x => x.MaThuoc).ToList();

                var productDetails = await _context.Thuocs
                    .Where(t => ids.Contains(t.MaThuoc))
                    .Select(t => new {
                        Id = t.MaThuoc,
                        Name = t.TenThuoc,
                        Image = t.HinhAnhChinh,
                        Origin = t.NuocSanXuat,
                        Price = t.DonViTinhs.Where(d => d.LaDonViCoBan == true).Select(d => d.GiaBan).FirstOrDefault() ?? 0,
                        Unit = t.DonViTinhs.Where(d => d.LaDonViCoBan == true).Select(d => d.TenDonVi).FirstOrDefault() ?? ""
                    })
                    .ToListAsync();

                var result = productDetails.Select(p => new {
                    p.Id,
                    p.Name,
                    p.Image,
                    p.Origin,
                    p.Price,
                    p.Unit,
                    TotalSold = topSellingData.FirstOrDefault(x => x.MaThuoc == p.Id)?.TongDaBan ?? 0
                })
                .OrderByDescending(p => p.TotalSold)
                .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi server: " + ex.Message);
            }
        }

        [HttpGet("Related")]
        public async Task<IActionResult> GetRelatedProducts(int maDanhMuc, int currentProductId)
        {
            try
            {
                var relatedProducts = await _context.Thuocs
                    .Where(t => t.MaDanhMuc == maDanhMuc && t.MaThuoc != currentProductId)
                    .Take(10)
                    .Select(t => new
                    {
                        t.MaThuoc,
                        t.TenThuoc,
                        t.HinhAnhChinh,
                        GiaBan = t.DonViTinhs.Where(d => d.LaDonViCoBan == true).Select(d => d.GiaBan).FirstOrDefault() ?? 0,
                        MaDvt = t.DonViTinhs.Where(d => d.LaDonViCoBan == true).Select(d => d.MaDvt).FirstOrDefault()
                    })
                    .ToListAsync();

                return Ok(relatedProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi lấy sản phẩm tương tự: " + ex.Message);
            }
        }

        [HttpGet("FrequentlyBoughtWith/{maDanhMuc}/{currentProductId}")]
        public async Task<IActionResult> GetFrequentlyBoughtWith(int maDanhMuc, int currentProductId)
        {
            try
            {
                var mapping = new Dictionary<int, List<int>>
                {
                    { 1, new List<int> { 5, 8 } },
                    { 2, new List<int> { 8, 10 } },
                    { 7, new List<int> { 6 } },
                    { 6, new List<int> { 7 } }
                };

                List<int> targetCategoryIds = mapping.ContainsKey(maDanhMuc) 
                    ? mapping[maDanhMuc] 
                    : await _context.DanhMucs.Where(d => d.MaDanhMuc != maDanhMuc).Select(d => d.MaDanhMuc).Take(1).ToListAsync();

                var suggestedProducts = await _context.Thuocs
                    .Where(t => targetCategoryIds.Contains(t.MaDanhMuc ?? 0) && t.MaThuoc != currentProductId)
                    .OrderBy(t => Guid.NewGuid())
                    .Take(6)
                    .Select(t => new {
                        Id = t.MaThuoc,
                        Name = t.TenThuoc,
                        Image = t.HinhAnhChinh,
                        Price = t.DonViTinhs.Where(d => d.LaDonViCoBan == true).Select(d => d.GiaBan).FirstOrDefault() ?? 0,
                        Unit = t.DonViTinhs.Where(d => d.LaDonViCoBan == true).Select(d => d.TenDonVi).FirstOrDefault() ?? "Viên",
                        MaDvt = t.DonViTinhs.Where(d => d.LaDonViCoBan == true).Select(d => d.MaDvt).FirstOrDefault(),
                        CategoryName = t.MaDanhMucNavigation != null ? t.MaDanhMucNavigation.TenDanhMuc : "Chưa phân loại"
                    })
                    .ToListAsync();

                return Ok(suggestedProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi: " + ex.Message);
            }
        }
    }
}