using QuanLyQuayThuoc.DTOs.NguoiDung;

namespace QuanLyQuayThuoc.Repositories.Interfaces
{
    public interface INguoiDungRepository
    {
        Task<NguoiDungInfoDto?> LayHoSoCaNhan(int maNguoiDung);
        Task<bool> LuuCapNhatHoSo(int maNguoiDung, CapNhatHoSoDto duLieu);
    }
}