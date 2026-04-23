using QuanLyQuayThuoc.DTOs.NguoiDung;

namespace QuanLyQuayThuoc.Services.Interfaces
{
    public interface INguoiDungService
    {
        Task<PhanQuyenDto?> DangNhap(DangNhapDto duLieu);
        Task<PhanQuyenDto?> DangNhapBangGoogle(DangNhapGoogleDto duLieu);
        Task<NguoiDungInfoDto?> LayHoSoCaNhan(int maNguoiDung);
        Task<bool> CapNhatHoSo(int maNguoiDung, CapNhatHoSoDto duLieu);
    }
}
