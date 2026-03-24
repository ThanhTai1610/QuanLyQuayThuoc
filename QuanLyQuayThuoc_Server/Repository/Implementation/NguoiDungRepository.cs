using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.DTOs.NguoiDung;
using QuanLyQuayThuoc.Repositories.Interfaces;

namespace QuanLyQuayThuoc.Repositories
{
    public class NguoiDungRepository : INguoiDungRepository
    {
        private readonly ApplicationDbContext _db;

        public NguoiDungRepository(ApplicationDbContext db) => _db = db;

        public async Task<NguoiDungInfoDto?> LayHoSoCaNhan(int maNguoiDung)
        {
            return await _db.NguoiDungs
                .Include(n => n.MaVaiTroNavigation)
                .Where(n => n.MaNguoiDung == maNguoiDung)
                .Select(n => new NguoiDungInfoDto
                {
                    MaNguoiDung = n.MaNguoiDung,
                    HoTen = n.HoTen,
                    Email = n.Email,
                    SoDienThoai = n.SoDienThoai,
                    AnhDaiDien = n.AnhDaiDien,
                    TenVaiTro = n.MaVaiTroNavigation != null ? n.MaVaiTroNavigation.TenVaiTro : null,
                    TrangThai = n.TrangThai
                }).FirstOrDefaultAsync();
        }

        public async Task<bool> LuuCapNhatHoSo(int maNguoiDung, CapNhatHoSoDto duLieu)
        {
            var user = await _db.NguoiDungs.FindAsync(maNguoiDung);
            if (user == null) return false;

            user.HoTen = duLieu.HoTen;
            user.Email = duLieu.Email;
            user.SoDienThoai = duLieu.SoDienThoai;

            return await _db.SaveChangesAsync() > 0;
        }
    }
}