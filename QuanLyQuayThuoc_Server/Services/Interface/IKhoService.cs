using QuanLyQuayThuoc.DTOs.Kho;
using QuanLyQuayThuoc.DTOs.SanPham;

namespace QuanLyQuayThuoc.Services.Interfaces
{
    public interface IKhoService
    {
        Task<KhoTongQuanResponseDto> GetTongQuanAsync(int? maDanhMuc, string search);
        Task<KhoLoHangResponseDto> GetLoHangAsync(string search, string thang, string loai);
        Task<bool> NhapKhoAsync(PhieuNhapKhoDto phieuNhap);
        Task<IEnumerable<DanhMucDto>> GetDanhMucAsync();
        Task<bool> SuaLoHangAsync(int maLo, SuaLoHangDto dto);
        Task<IEnumerable<MaVachDto>> GetMaVachTheoThuocAsync(int maThuoc);
        Task<bool> ThemThuocMoiVaNhapKhoAsync(ThemThuocMoiVaNhapKhoDto dto);
    }
}