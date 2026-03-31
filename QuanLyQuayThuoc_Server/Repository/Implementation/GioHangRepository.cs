using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data; 
using QuanLyQuayThuoc.Models;
using QuanLyQuayThuoc.Repositories.Interfaces;

namespace QuanLyQuayThuoc.Repositories
{
    public class GioHangRepository : IGioHangRepository
    {
        private readonly ApplicationDbContext _context;

        public GioHangRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GioHang>> GetByKhachHangAsync(int maKhachHang)
        {
            return await _context.GioHangs
                .Include(g => g.MaThuocNavigation)       
                .Include(g => g.MaDvtNavigation)     
                .Where(g => g.MaKhachHang == maKhachHang)
                .ToListAsync();
        }

        public async Task<GioHang?> GetCartItemAsync(int maKhachHang, int maThuoc, int maDVT)
        {
            return await _context.GioHangs
                .FirstOrDefaultAsync(g => g.MaKhachHang == maKhachHang &&
                                         g.MaThuoc == maThuoc &&
                                         g.MaDvt == maDVT);
        }

        public async Task<GioHang?> GetByIdAsync(int maGioHang)
        {
            return await _context.GioHangs.FindAsync(maGioHang);
        }

        public async Task AddAsync(GioHang gioHang)
        {
            await _context.GioHangs.AddAsync(gioHang);
        }

        public void Update(GioHang gioHang)
        {
            _context.GioHangs.Update(gioHang);
        }

        public void Delete(GioHang gioHang)
        {
            _context.GioHangs.Remove(gioHang);
        }

        public async Task DeleteAllAsync(int maKhachHang)
        {
            var items = await _context.GioHangs.Where(g => g.MaKhachHang == maKhachHang).ToListAsync();
            _context.GioHangs.RemoveRange(items);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}