using QuanLyQuayThuoc.DTOs.SanPham;

namespace QuanLyQuayThuoc.Services
{
    public interface IThuocService
    {
        Task<SanPhamDetailDto> GetDetailAsync(int id);
        Task<IEnumerable<SanPhamCardDto>> GetRelatedProductsAsync(int maDanhMuc, int currentProductId);
    }
}