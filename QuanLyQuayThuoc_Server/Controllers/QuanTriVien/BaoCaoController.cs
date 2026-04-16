using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.DTOs.BaoCao;
using QuanLyQuayThuoc.Models;

namespace QuanLyQuayThuoc.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaoCaoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BaoCaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("doanh-thu-loi-nhuan")]
        public async Task<IActionResult> GetDoanhThuLoiNhuan([FromQuery] string ky = "thang")
        {
            try
            {
                int namHienTai = DateTime.Now.Year;
                var result = new DoanhThuLoiNhuanDto();

                // 1. Truy vấn dùng TÊN GỐC để EF dịch sang SQL được
                // Thay vì ct.DonHang, ta dùng ct.MaDonHangNavigation
                var dataThang = await _context.Set<ChiTietDonHang>()
                    .Where(ct => ct.MaDonHangNavigation != null &&
                                 ct.MaDonHangNavigation.NgayDat != null &&
                                 ct.MaDonHangNavigation.NgayDat.Value.Year == namHienTai)
                    .GroupBy(ct => ct.MaDonHangNavigation.NgayDat.Value.Month)
                    .Select(g => new {
                        Thang = g.Key,
                        DoanhThu = g.Sum(x => (decimal)((x.SoLuong ?? 0) * (x.GiaBanTaiThoiDiem ?? 0))),
                        // Lợi nhuận = (Giá bán - Giá nhập từ MaLoNavigation) * Số lượng
                        LoiNhuan = g.Sum(x => (decimal)(((x.GiaBanTaiThoiDiem ?? 0) - (x.MaLoNavigation.GiaNhap ?? 0)) * (x.SoLuong ?? 0)))
                    })
                    .ToListAsync();

                // 2. Đổ dữ liệu vào DTO như cũ
                if (ky == "thang")
                {
                    for (int i = 1; i <= 12; i++)
                    {
                        result.Nhan.Add($"T{i}");
                        var item = dataThang.FirstOrDefault(x => x.Thang == i);
                        result.DoanhThu.Add(item?.DoanhThu ?? 0);
                        result.LoiNhuan.Add(item?.LoiNhuan ?? 0);
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }
        [HttpGet("top-ban-chay")]
        public async Task<IActionResult> GetTopBanChay()
        {
            try
            {
                // 1. Lấy dữ liệu và nhóm theo thuốc
                var topProducts = await _context.Set<ChiTietDonHang>()
                    .Include(ct => ct.MaLoNavigation)
                        .ThenInclude(l => l.MaThuocNavigation)
                    .GroupBy(ct => ct.MaLoNavigation.MaThuocNavigation.TenThuoc)
                    .Select(g => new
                    {
                        TenThuoc = g.Key ?? "Không xác định",
                        SoLuongDaBan = g.Sum(x => x.SoLuong ?? 0)
                    })
                    .OrderByDescending(x => x.SoLuongDaBan)
                    .Take(5) // Lấy 5 thằng đầu bảng
                    .ToListAsync();

                // 2. Tính tổng để tính %
                var tongTatCa = topProducts.Sum(x => x.SoLuongDaBan);

                // 3. Trả về định dạng FE cần
                var result = topProducts.Select(x => new
                {
                    x.TenThuoc,
                    x.SoLuongDaBan,
                    PhanTram = tongTatCa > 0 ? Math.Round((double)x.SoLuongDaBan / tongTatCa * 100, 1) : 0
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("top-xem-nhieu")]
        public async Task<IActionResult> GetTopXemNhieu()
        {
            try
            {
                // Lấy danh sách thuốc, sắp xếp theo LuotXem giảm dần
                var topViews = await _context.Set<Thuoc>()
                    .OrderByDescending(t => t.LuotXem)
                    .Take(5) // Lấy top 5 sản phẩm
                    .Select(t => new
                    {
                        t.MaThuoc,
                        t.TenThuoc,
                        LuotXem = t.LuotXem ?? 0
                    })
                    .ToListAsync();

                return Ok(topViews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("ton-kho-thap")]
        public async Task<IActionResult> GetTonKhoThap()
        {
            try
            {
                var lowStock = await _context.Set<Thuoc>()
                    .Include(t => t.LoHangs) // QUAN TRỌNG: Phải có dòng này để lấy dữ liệu từ bảng LoHang
                    .Select(t => new
                    {
                        t.MaThuoc,
                        t.TenThuoc,
                        NguongToiThieu = 10,
                        // Tính tổng tồn kho từ danh sách lô hàng đã được Include
                        TonHienTai = t.LoHangs.Any() ? t.LoHangs.Sum(l => l.SoLuongTon) : 0
                    })
                    // Thử nới lỏng điều kiện lọc để kiểm tra xem có dữ liệu không
                    // Ví dụ: Lấy tất cả những thằng có tồn kho < 50
                    .Where(x => x.TonHienTai < 50)
                    .OrderBy(x => x.TonHienTai)
                    .Take(10)
                    .ToListAsync();

                return Ok(lowStock);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("canh-bao-han-dung")]
        public async Task<IActionResult> GetCanhBaoHanDung()
        {
            try
            {
                DateOnly ngayHienTai = DateOnly.FromDateTime(DateTime.Now);
                // Mốc 6 tháng tới tính từ hôm nay
                DateOnly sau6Thang = ngayHienTai.AddMonths(6);

                var data = await _context.Set<LoHang>()
                    .Include(l => l.MaThuocNavigation)
                    .Where(l => l.SoLuongTon > 0 &&
                                l.HanSuDung <= sau6Thang) // Lấy tất cả lô có hạn dùng nhỏ hơn mốc 6 tháng tới (bao gồm cả đã hết hạn)
                    .OrderBy(l => l.HanSuDung) // Thằng nào hết hạn trước xếp lên đầu
                    .Select(l => new
                    {
                        MaLo = l.MaLo,
                        TenThuoc = l.MaThuocNavigation.TenThuoc,
                        SoLo = l.SoLo,
                        HanSuDung = l.HanSuDung.ToString("dd/MM/yyyy"),
                        // Nếu HanSuDung < ngayHienTai thì coi như là 0 tháng (hoặc âm) để báo đỏ
                        ConLaiThang = ((l.HanSuDung.Year - ngayHienTai.Year) * 12) + l.HanSuDung.Month - ngayHienTai.Month
                    })
                    .Take(20) // Lấy 20 lô cấp bách nhất
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        //[HttpGet("chi-so-chinh")]
        //public async Task<IActionResult> GetChiSoChinh()
        //{
        //    try
        //    {
        //        var bayGio = DateTime.Now;
        //        var dauThangNay = new DateTime(bayGio.Year, bayGio.Month, 1);
        //        var dauThangTruoc = dauThangNay.AddMonths(-1);

        //        // 1. Tính Doanh thu tháng này & tháng trước
        //        var doanhThuThangNay = await _context.Set<HoaDon>()
        //            .Where(h => h.NgayBan >= dauThangNay)
        //            .SumAsync(h => h.TongTien ?? 0);

        //        var doanhThuThangTruoc = await _context.Set<HoaDon>()
        //            .Where(h => h.NgayBan >= dauThangTruoc && h.NgayBan < dauThangNay)
        //            .SumAsync(h => h.TongTien ?? 0);

        //        // 2. Số lượng đơn hàng
        //        var donHangThangNay = await _context.Set<HoaDon>()
        //            .CountAsync(h => h.NgayBan >= dauThangNay);

        //        // 3. Khách hàng mới (Ví dụ dựa trên số điện thoại mới trong hóa đơn)
        //        var khachMoi = await _context.Set<HoaDon>()
        //            .Where(h => h.NgayBan >= dauThangNay)
        //            .Select(h => h.SdtKhachHang) // Giả sử có cột này
        //            .Distinct()
        //            .CountAsync();

        //        // 4. Tính toán phần trăm tăng trưởng (Logic đơn giản)
        //        double phanTramDT = doanhThuThangTruoc > 0
        //            ? Math.Round(((double)(doanhThuThangNay - doanhThuThangTruoc) / (double)doanhThuThangTruoc) * 100, 1)
        //            : 100;

        //        return Ok(new
        //        {
        //            tongDoanhThu = doanhThuThangNay,
        //            phanTramDoanhThu = phanTramDT,
        //            soLuongDonHang = donHangThangNay,
        //            phanTramDonHang = 12.5, // Giả định hoặc tính tương tự
        //            khachHangMoi = khachMoi,
        //            phanTramKhachHang = 5.2,
        //            tyLeHuyDon = 2.1,
        //            chenhLechHuyDon = 0.5
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}
    }
}