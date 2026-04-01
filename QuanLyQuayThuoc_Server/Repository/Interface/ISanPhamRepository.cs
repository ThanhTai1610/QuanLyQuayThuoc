using QuanLyQuayThuoc.DTOs.SanPham;

namespace QuanLyQuayThuoc.Repository.Interfaces
{
    public interface ISanPhamRepository
    {
        // 1. Khai báo hàm tìm kiếm nhanh (Tài đã có)
        Task<IEnumerable<SanPhamSearchDto>> SearchQuickAsync(string query);

    }
}