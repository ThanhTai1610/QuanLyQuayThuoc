using QuanLyQuayThuoc.Models;

namespace QuanLyQuayThuoc.Repositories.Interfaces
{
    public interface IGioHangRepository
    {
        // Lấy toàn bộ item trong giỏ của khách, kèm theo thông tin Thuốc và Đơn vị tính
        Task<IEnumerable<GioHang>> GetByKhachHangAsync(int maKhachHang);

        // Tìm một item cụ thể trong giỏ (để kiểm tra xem thuốc đó đã có trong giỏ chưa)
        Task<GioHang?> GetCartItemAsync(int maKhachHang, int maThuoc, int maDVT);

        // Tìm theo ID chính (để xóa hoặc cập nhật nhanh)
        Task<GioHang?> GetByIdAsync(int maGioHang);

        Task AddAsync(GioHang gioHang);
        void Update(GioHang gioHang);
        void Delete(GioHang gioHang);
        Task DeleteAllAsync(int maKhachHang);

        Task<bool> SaveChangesAsync();
    }
}
