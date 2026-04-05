using System.Threading.Tasks;
using QuanLyQuayThuoc.DTOs.Kho;
using QuanLyQuayThuoc.DTOs.SanPham;
using QuanLyQuayThuoc.Models;

namespace QuanLyQuayThuoc.Repositories.Interfaces
{
    public interface IKhoRepository
    {
        // --- CÁC HÀM CŨ (PHỤC VỤ BÁN HÀNG) ---
        Task UpdateSoLuongAsync(int maLo, int soLuongBan);
        Task<int> SaveChangesAsync();
        Task<IEnumerable<LoHang>> GetLoHangByThuocAsync(int maThuoc);
        Task<IEnumerable<object>> TimKiemThuocAsync(string query);

        // --- CÁC HÀM MỚI (PHỤC VỤ QUẢN LÝ KHO) ---
        // Tab 1: Tổng quan tồn kho
        Task<KhoTongQuanResponseDto> GetTongQuanAsync(int? maDanhMuc, string search);

        // Tab 2 & 4: Danh sách lô hàng & Cảnh báo hạn dùng
        Task<KhoLoHangResponseDto> GetLoHangAsync(string search, string thang, string loai);

        // Tab 3: Nhập hàng mới & Sinh mã vạch tự động
        Task<bool> NhapKhoAsync(PhieuNhapKhoDto phieuNhap);

        Task<IEnumerable<DanhMucDto>> GetDanhMucAsync();
        Task<bool> SuaLoHangAsync(int maLo, SuaLoHangDto dto);
    }
}