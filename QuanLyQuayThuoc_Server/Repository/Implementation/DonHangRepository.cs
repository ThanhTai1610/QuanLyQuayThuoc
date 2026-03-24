using System;
using System.Threading.Tasks;
using QuanLyQuayThuoc.Models; // Namespace chứa ApplicationDbContext và DonHang
using QuanLyQuayThuoc.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;

namespace QuanLyQuayThuoc.Repositories
{
    public class DonHangRepository : IDonHangRepository
    {
        private readonly ApplicationDbContext _context;

        // Inject DbContext vào để làm việc với Database
        public DonHangRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Thực thi hàm thêm đơn hàng
        public async Task AddAsync(DonHang donHang)
        {
            await _context.DonHangs.AddAsync(donHang);
        }

        // Thực thi hàm tạo đơn hàng (trả về đối tượng sau khi lưu)
        public async Task<DonHang> CreateOrderAsync(DonHang donHang)
        {
            await _context.DonHangs.AddAsync(donHang);
            await _context.SaveChangesAsync();
            return donHang;
        }

        // Hàm lưu mọi thay đổi xuống Database
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}