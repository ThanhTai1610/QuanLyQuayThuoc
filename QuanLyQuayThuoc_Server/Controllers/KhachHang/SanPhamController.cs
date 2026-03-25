using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.DTOs.SanPham;

namespace QuanLyQuayThuoc.Controllers.KhachHang
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SanPhamController(ApplicationDbContext context)
        {
            _context = context;
        }

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
                                      // Nối thêm đường dẫn thư mục để Frontend nhận được link đầy đủ
                                      // Bỏ chữ wwwroot đi, chỉ bắt đầu từ dấu gạch chéo /
                                      HinhAnh = "/images/products/" + t.HinhAnhChinh,
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
    }
}