//using Microsoft.AspNetCore.Mvc;
//using QuanLyQuayThuoc.Data;
//using QuanLyQuayThuoc.Services;

//[Route("api/[controller]")]
//[ApiController]
//public class DonViTinhController : ControllerBase
//{
//    private readonly IBarcodeService _barcodeService;
//    private readonly ApplicationDbContext _context; // DB Context của bạn

//    public DonViTinhController(IBarcodeService barcodeService, ApplicationDbContext context)
//    {
//        _barcodeService = barcodeService;
//        _context = context;
//    }

//    // API lấy thông tin đơn vị tính kèm hình ảnh mã vạch
//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetDonViTinh(int id)
//    {
//        var dvt = await _context.DonViTinhs.FindAsync(id);
//        if (dvt == null) return NotFound();

//        // Tạo hình ảnh từ mã vạch lưu trong DB
//        var base64Image = _barcodeService.GenerateBarcode(dvt.MaVach);

//        return Ok(new
//        {
//            tenDonVi = dvt.TenDonVi,
//            maVach = dvt.MaVach,
//            hinhAnh = base64Image // Gửi chuỗi ảnh về cho Vue.js
//        });
//    }
//}