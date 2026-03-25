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
    }
}