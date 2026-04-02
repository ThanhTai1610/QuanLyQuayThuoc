using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data; // Thay bằng DbContext của bạn
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
                // Bước 1: Tính toán ID và Số lượng bán ra (Tải về bộ nhớ bằng ToList)
                var topSellingData = await _context.ChiTietDonHangs
                    .Join(_context.LoHangs, ct => ct.MaLo, l => l.MaLo, (ct, l) => new { l.MaThuoc, ct.SoLuong })
                    .GroupBy(x => x.MaThuoc)
                    .Select(g => new {
                        MaThuoc = g.Key,
                        TongDaBan = g.Sum(x => (int?)x.SoLuong) ?? 0 // Dùng int? để tránh lỗi null
                    })
                    .OrderByDescending(x => x.TongDaBan)
                    .Take(6)
                    .ToListAsync();

                if (!topSellingData.Any()) return Ok(new List<object>());

                var ids = topSellingData.Select(x => x.MaThuoc).ToList();

                // Bước 2: Lấy thông tin chi tiết các thuốc này từ DB
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

                // Bước 3: Kết hợp dữ liệu (Thực hiện hoàn toàn trên RAM)
                var result = productDetails.Select(p => new {
                    p.Id,
                    p.Name,
                    p.Image,
                    p.Origin,
                    p.Price,
                    p.Unit,
                    // Lấy số lượng bán từ danh sách topSellingData đã tải ở Bước 1
                    TotalSold = topSellingData.FirstOrDefault(x => x.MaThuoc == p.Id)?.TongDaBan ?? 0
                })
                .OrderByDescending(p => p.TotalSold)
                .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log lỗi ra console để Tài dễ debug
                Console.WriteLine("Lỗi BestSellers: " + ex.Message);
                return StatusCode(500, "Lỗi server: " + ex.Message);
            }
        }
    }
}