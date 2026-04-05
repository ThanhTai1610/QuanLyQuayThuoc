using QuanLyQuayThuoc.DTOs.Kho;
using QuanLyQuayThuoc.DTOs.SanPham;

namespace QuanLyQuayThuoc.Services.Interfaces
{
    public interface IKhoService
    {
        // Lấy dữ liệu tab Tổng quan
        Task<KhoTongQuanResponseDto> GetTongQuanAsync(int? maDanhMuc, string search);

        // Lấy dữ liệu tab Lô hàng & Cảnh báo
        Task<KhoLoHangResponseDto> GetLoHangAsync(string search, string thang, string loai);

        // Xử lý nhập kho
        Task<bool> NhapKhoAsync(PhieuNhapKhoDto phieuNhap);
        Task<IEnumerable<DanhMucDto>> GetDanhMucAsync();
        Task<bool> SuaLoHangAsync(int maLo, SuaLoHangDto dto);
    }
}