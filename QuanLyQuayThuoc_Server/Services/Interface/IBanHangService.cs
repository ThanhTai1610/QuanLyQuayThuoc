using QuanLyQuayThuoc.DTOs.DonHang;

namespace QuanLyQuayThuoc.Services.Interfaces
{
    public interface IBanHangService
    {
        Task<int> ThanhToanTaiQuayAsync(TaoDonHangDto dto, int maNhanVien);
    }
}