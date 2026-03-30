using QuanLyQuayThuoc.DTOs.DonHang;
using QuanLyQuayThuoc.Models;

namespace QuanLyQuayThuoc.Services.Interfaces
{
    public interface IBanHangService
    {
        Task<int> ThanhToanTaiQuayAsync(TaoDonHangDto dto, int maNhanVien);
        Task<IEnumerable<Object>> TimKiemThuocNhanhAsync(string query);
        Task<IEnumerable<LoHang>> LayDanhSachLoCuaThuocAsync(int maThuoc);
    }
}