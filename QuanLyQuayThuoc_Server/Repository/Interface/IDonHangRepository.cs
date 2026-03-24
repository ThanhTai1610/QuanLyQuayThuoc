using System.Threading.Tasks;
using QuanLyQuayThuoc.Models;

namespace QuanLyQuayThuoc.Repositories.Interfaces
{
    public interface IDonHangRepository
    {
        // Phải khai báo thì Service mới gọi được _donHangRepo.AddAsync(...)
        Task AddAsync(DonHang donHang);

        // Phải khai báo thì Service mới gọi được _donHangRepo.SaveChangesAsync()
        Task<int> SaveChangesAsync();

        // Giữ lại hàm cũ của bạn nếu có
        Task<DonHang> CreateOrderAsync(DonHang donHang);
    }
}