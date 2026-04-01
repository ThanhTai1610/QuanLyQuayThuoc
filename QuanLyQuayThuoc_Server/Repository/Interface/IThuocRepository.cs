using QuanLyQuayThuoc.Models;

namespace QuanLyQuayThuoc.Repositories
{
    public interface IThuocRepository
    {
        Task<Thuoc> GetByIdAsync(int id);
        Task<IEnumerable<Thuoc>> GetRelatedAsync(int maDanhMuc, int currentProductId);
        Task SaveChangesAsync();
    }
}