using QuanLyQuayThuoc.DTOs.DonHang;

namespace QuanLyQuayThuoc.Services.Interfaces
{
    public interface IGioHangService
    {
        Task<IEnumerable<GioHangItemDto>> LayDanhSachGioHangAsync(int maKhachHang);
        Task<bool> ThemVaoGioHangAsync(int maKhachHang, int maThuoc, int maDvt, int soLuong);
        Task<bool> CapNhatGioHangAsync(List<CapNhatGioHangDto> danhSachCapNhat);
        Task<bool> XoaKhoiGioHangAsync(int maGioHang);
        Task<bool> XoaToanBoGioHangAsync(int maKhachHang);
    }
}