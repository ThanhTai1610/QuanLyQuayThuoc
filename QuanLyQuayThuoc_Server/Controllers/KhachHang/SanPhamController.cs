using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.DTOs.SanPham;
using QuanLyQuayThuoc.Repository.Interfaces;

namespace QuanLyQuayThuoc.Controllers.KhachHang
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ISanPhamRepository _repo;

        public SanPhamController(ApplicationDbContext context, ISanPhamRepository repo)
        {
            _context = context;
            _repo = repo;
        }

        // GET api/SanPham/trang-chu
        [HttpGet("trang-chu")]
        public async Task<ActionResult<IEnumerable<SanPhamCardDto>>> GetSanPhamTrangChu()
        {
            var sanPhams = await (from t in _context.Thuocs
                                  join d in _context.DanhMucs on t.MaDanhMuc equals d.MaDanhMuc
                                  join dvt in _context.DonViTinhs on t.MaThuoc equals dvt.MaThuoc
                                  where dvt.LaDonViCoBan == true
                                  select new SanPhamCardDto
                                  {
                                      Id = t.MaThuoc,
                                      TenThuoc = t.TenThuoc,
                                      HinhAnhChinh = t.HinhAnhChinh,
                                      TenDanhMuc = d.TenDanhMuc,
                                      GiaBan = dvt.GiaBan ?? 0,
                                      GiaCu = (dvt.GiaBan * 1.2m) ?? 0,
                                      PhanTramGiamGia = 20,
                                      DiemDanhGia = 4.8,
                                      LuotDanhGia = 120
                                  })
                                 .Take(8)
                                 .ToListAsync();

            return Ok(sanPhams);
        }


        [HttpGet("search-quick")]
        public async Task<ActionResult<IEnumerable<SanPhamSearchDto>>> SearchQuick([FromQuery] string q)
        {
            // Trả về rỗng nếu không có từ khóa
            if (string.IsNullOrWhiteSpace(q))
                return Ok(new List<SanPhamSearchDto>());

            try
            {
                var results = await _repo.SearchQuickAsync(q);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi tìm kiếm", error = ex.Message });
            }
        }
    }
}