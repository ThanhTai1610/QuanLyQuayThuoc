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
                        Price = t.DonViTinhs.OrderByDescending(d => d.GiaTriQuyDoi).Select(d => d.GiaBan).FirstOrDefault() ?? 0,
                        Unit = t.DonViTinhs.OrderByDescending(d => d.GiaTriQuyDoi).Select(d => d.TenDonVi).FirstOrDefault() ?? ""
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
        // 1. API: Lấy sản phẩm tương tự (Cùng danh mục, loại trừ sản phẩm hiện tại)
        [HttpGet("Related")]
        public async Task<IActionResult> GetRelatedProducts(int maDanhMuc, int currentProductId)
        {
            try
            {
                var relatedProducts = await _context.Thuocs
                    .Where(t => t.MaDanhMuc == maDanhMuc && t.MaThuoc != currentProductId)
                    .Take(10) // Lấy tối đa 10 sản phẩm
                    .Select(t => new
                    {
                        t.MaThuoc,
                        t.TenThuoc,
                        t.HinhAnhChinh,
                        // Lấy giá của đơn vị lớn nhất
                        GiaBan = t.DonViTinhs.OrderByDescending(d => d.GiaTriQuyDoi).Select(d => d.GiaBan).FirstOrDefault() ?? 0
                    })
                    .ToListAsync();

                return Ok(relatedProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi lấy sản phẩm tương tự: " + ex.Message);
            }
        }

        // 2. API: Lấy sản phẩm thường mua cùng (Sản phẩm xuất hiện cùng trong các đơn hàng khác)
        [HttpGet("FrequentlyBoughtWith")]
        public async Task<IActionResult> GetFrequentlyBoughtWith(int currentProductId)
        {
            try
            {
                // Bước 1: Tìm danh sách các MaDonHang có chứa sản phẩm này
                var orderIds = await _context.ChiTietDonHangs
                    .Join(_context.LoHangs, ct => ct.MaLo, l => l.MaLo, (ct, l) => new { ct.MaDonHang, l.MaThuoc })
                    .Where(x => x.MaThuoc == currentProductId)
                    .Select(x => x.MaDonHang)
                    .Distinct()
                    .ToListAsync();

                if (!orderIds.Any()) return Ok(new List<object>());

                // Bước 2: Tìm các sản phẩm khác (MaThuoc) nằm trong các đơn hàng đó
                var suggestedProductIds = await _context.ChiTietDonHangs
                    .Join(_context.LoHangs, ct => ct.MaLo, l => l.MaLo, (ct, l) => new { ct.MaDonHang, l.MaThuoc })
                    .Where(x => orderIds.Contains(x.MaDonHang) && x.MaThuoc != currentProductId)
                    .GroupBy(x => x.MaThuoc)
                    .OrderByDescending(g => g.Count()) // Ưu tiên sản phẩm xuất hiện nhiều nhất
                    .Select(g => g.Key)
                    .Take(10)
                    .ToListAsync();

                // Bước 3: Lấy thông tin chi tiết các sản phẩm gợi ý
                var results = await _context.Thuocs
                    .Where(t => suggestedProductIds.Contains(t.MaThuoc))
                    .Select(t => new
                    {
                        t.MaThuoc,
                        t.TenThuoc,
                        t.HinhAnhChinh,
                        GiaBan = t.DonViTinhs.OrderByDescending(d => d.GiaTriQuyDoi).Select(d => d.GiaBan).FirstOrDefault() ?? 0
                    })
                    .ToListAsync();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi lấy sản phẩm mua cùng: " + ex.Message);
            }
        }

        [HttpGet("FrequentlyBoughtWith/{maDanhMuc}/{currentProductId}")]
        public async Task<IActionResult> GetFrequentlyBoughtWith(int maDanhMuc, int currentProductId)
        {
            try
            {
                // Bước 1: Định nghĩa các cặp danh mục đi kèm (Hard-code logic)
                // Key: Mã danh mục đang xem | Value: Danh sách các mã danh mục gợi ý mua cùng
                var mapping = new Dictionary<int, List<int>>
        {
            { 1, new List<int> { 5, 8 } }, // Ví dụ: Kháng sinh (1) -> Gợi ý Men tiêu hóa (5), Vitamin (8)
            { 2, new List<int> { 8, 10 } }, // Ví dụ: Thuốc ho (2) -> Gợi ý Vitamin C (8), Khẩu trang (10)
            { 7, new List<int> { 6 } },     // Ví dụ: Giải pháp làn da (7) -> Gợi ý Chăm sóc da mặt (6)
            { 6, new List<int> { 7 } }      // Ngược lại
        };

                List<int> targetCategoryIds = new List<int>();

                // Kiểm tra xem danh mục hiện tại có nằm trong sơ đồ gợi ý không
                if (mapping.ContainsKey(maDanhMuc))
                {
                    targetCategoryIds = mapping[maDanhMuc];
                }
                else
                {
                    // Nếu không có trong mapping, lấy đại 1 danh mục bất kỳ khác để không bị trống UI
                    targetCategoryIds = _context.DanhMucs
                        .Where(d => d.MaDanhMuc != maDanhMuc)
                        .Select(d => d.MaDanhMuc)
                        .Take(1).ToList();
                }

                // Bước 2: Lấy sản phẩm từ các danh mục mục tiêu
                var suggestedProducts = await _context.Thuocs
                    .Where(t => targetCategoryIds.Contains(t.MaDanhMuc ?? 0) && t.MaThuoc != currentProductId)
                    .OrderBy(t => Guid.NewGuid()) // Lấy ngẫu nhiên để mỗi lần load lại ra cái mới
                    .Take(6) // Hiển thị 4 cái cho đẹp layout của bạn
                    .Select(t => new {
                        Id = t.MaThuoc,
                        Name = t.TenThuoc,
                        Image = t.HinhAnhChinh,
                        Price = t.DonViTinhs.OrderByDescending(d => d.GiaTriQuyDoi).Select(d => d.GiaBan).FirstOrDefault() ?? 0,
                        Unit = t.DonViTinhs.OrderByDescending(d => d.GiaTriQuyDoi).Select(d => d.TenDonVi).FirstOrDefault() ?? "Viên",
                        CategoryName = t.MaDanhMucNavigation != null ? t.MaDanhMucNavigation.TenDanhMuc : "Chưa phân loại" // Để hiện tên danh mục gợi ý
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