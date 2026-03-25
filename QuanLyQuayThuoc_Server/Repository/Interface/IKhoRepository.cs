using System.Threading.Tasks;
using QuanLyQuayThuoc.Models;

namespace QuanLyQuayThuoc.Repositories.Interfaces
{
    public interface IKhoRepository
    {
        Task UpdateSoLuongAsync(int maLo, int soLuongBan);
        Task<int> SaveChangesAsync();
        Task<IEnumerable<LoHang>> GetLoHangByThuocAsync(int maThuoc);
        Task<IEnumerable<Object>> TimKiemThuocAsync(string query);
    }
}