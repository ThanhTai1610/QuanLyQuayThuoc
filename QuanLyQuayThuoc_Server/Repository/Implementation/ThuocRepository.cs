using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.Models;

namespace QuanLyQuayThuoc.Repository
{
    public class ThuocRepository : IThuocRepository
    {
        private readonly ApplicationDbContext _context;

        public ThuocRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Thuoc> GetByIdAsync(int id)
        {
            return await _context.Thuocs
                .Include(t => t.DonViTinhs)
                .Include(t => t.HinhAnhThuocs)
                .Include(t => t.LoHangs)
                .FirstOrDefaultAsync(t => t.MaThuoc == id);
        }

        public async Task<IEnumerable<Thuoc>> GetRelatedAsync(int maDanhMuc, int currentProductId)
        {
            return await _context.Thuocs
                .Where(t => t.MaDanhMuc == maDanhMuc && t.MaThuoc != currentProductId)
                .Take(10) // Lấy tối đa 10 sản phẩm
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}