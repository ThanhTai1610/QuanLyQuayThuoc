using System.Threading.Tasks;
using QuanLyQuayThuoc.Models;

namespace QuanLyQuayThuoc.Repositories.Interfaces
{
    // PHẢI dùng từ khóa 'interface', KHÔNG dùng 'class'
    public interface IKhoRepository
    {
        // Interface không có thân hàm, kết thúc bằng dấu chấm phẩy
        Task UpdateSoLuongAsync(int maLo, int soLuongBan);

        // Thêm luôn hàm SaveChanges vào đây để Service có thể gọi
        Task<int> SaveChangesAsync();
    }
}