using System.Threading.Tasks;
using QuanLyQuayThuoc.Models;

namespace QuanLyQuayThuoc.Repositories.Interfaces
{
    public interface IDonHangRepository
    {
        Task AddAsync(DonHang donHang);
        Task<int> SaveChangesAsync();
        Task<DonHang> CreateOrderAsync(DonHang donHang);
    }
}