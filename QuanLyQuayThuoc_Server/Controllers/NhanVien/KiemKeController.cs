using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.Models;
using System.Security.Claims;
using QuanLyQuayThuoc.DTOs.BaoCao;
namespace QuanLyQuayThuoc.Controllers.NhanVien
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KiemKeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KiemKeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("lich-su")]
        public async Task<IActionResult> GetLichSu()
        {
            try
            {
                var phieus = await _context.PhieuKiemKes
                    .Include(p => p.ChiTietKiemKes)
                        .ThenInclude(ct => ct.MaLoNavigation)
                        .ThenInclude(l => l.MaThuocNavigation)
                    .OrderByDescending(p => p.NgayKiem)
                    .ToListAsync(); // Lấy về bộ nhớ

                var lichSu = phieus.Select(p => new
                {
                    Ma = "KK-" + p.MaPhieu.ToString("D4"),
                    ThoiGian = p.NgayKiem?.ToString("dd-MM-yyyy HH:mm") ?? "",
                    Nguoi = _context.NguoiDungs.FirstOrDefault(u => u.MaNguoiDung == p.MaNhanVien)?.HoTen ?? "N/A",
                    ChiTietThuoc = p.ChiTietKiemKes.Select(ct => new {
                        TenThuoc = ct.MaLoNavigation?.MaThuocNavigation?.TenThuoc ?? "N/A",
                        ChenhLech = (ct.SoLuongThucTe ?? 0) - (ct.SoLuongHeThong ?? 0),
                        LyDo = ct.LyDoLech
                    }).ToList(),
                    TongSo = p.ChiTietKiemKes.Sum(ct => (ct.SoLuongThucTe ?? 0) - (ct.SoLuongHeThong ?? 0))
                    // TUYỆT ĐỐI KHÔNG CÓ DÒNG TONGGIA Ở ĐÂY
                }).ToList();

                return Ok(lichSu);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("danh-sach-lo")]
        public async Task<IActionResult> GetLoHang()
        {
            var data = await (from lo in _context.LoHangs
                              join thuoc in _context.Thuocs on lo.MaThuoc equals thuoc.MaThuoc
                              join dm in _context.DanhMucs on thuoc.MaDanhMuc equals dm.MaDanhMuc
                              select new
                              {
                                  Id = lo.MaLo,
                                  TenThuoc = thuoc.TenThuoc,
                                  SoLo = lo.SoLo,
                                  // Convert DateOnly to DateTime before formatting
                                  HanSuDung = lo.HanSuDung.ToDateTime(TimeOnly.MinValue).ToString("dd/MM/yyyy"),
                                  DanhMuc = dm.TenDanhMuc,
                                  ViTri = "Khu vực A",
                                  SoLuongTon = lo.SoLuongTon,
                                  DonGia = lo.GiaNhap,
                                  SoLuongThucTe = lo.SoLuongTon,
                                  ChenhLech = 0,
                                  LyDo = ""
                              }).ToListAsync();
            return Ok(data);
        }

        [HttpPost("luu-phieu")]
        public async Task<IActionResult> LuuPhieu([FromBody] KiemKeRequestDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
                var maNV = int.Parse(userIdStr);

                var phieu = new PhieuKiemKe
                {
                    NgayKiem = DateTime.Now,
                    MaNhanVien = maNV,
                    GhiChu = request.GhiChu ?? "Kiểm kê định kỳ",
                    TrangThai = "Hoàn tất"
                };
                _context.PhieuKiemKes.Add(phieu);
                await _context.SaveChangesAsync();

                foreach (var item in request.ChiTiet)
                {
                    var ct = new ChiTietKiemKe
                    {
                        MaPhieu = phieu.MaPhieu,
                        MaLo = item.MaLo,
                        SoLuongHeThong = item.SoLuongHeThong,
                        SoLuongThucTe = item.SoLuongThucTe,
                        LyDoLech = item.LyDo
                    };
                    _context.ChiTietKiemKes.Add(ct);

                    var loHang = await _context.LoHangs.FindAsync(item.MaLo);
                    if (loHang != null)
                    {
                        loHang.SoLuongTon = item.SoLuongThucTe;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Lưu phiếu thành công", maPhieu = phieu.MaPhieu });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { message = "Lỗi xử lý: " + ex.Message });
            }
        }
    }

    
}