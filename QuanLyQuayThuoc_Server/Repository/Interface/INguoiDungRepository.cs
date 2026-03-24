using QuanLyQuayThuoc.DTOs.NguoiDung;
using QuanLyQuayThuoc.Models;

namespace QuanLyQuayThuoc.Repositories.Interfaces
{
    public interface INguoiDungRepository
    {
        Task<NguoiDungInfoDto?> LayHoSoCaNhan(int maNguoiDung);
        Task<bool> LuuCapNhatHoSo(int maNguoiDung, CapNhatHoSoDto duLieu);
        Task<NguoiDung?> GetByEmailAsync(string email);
    }
}