using System;
using System.Threading.Tasks;
using QuanLyQuayThuoc.Models; 
using QuanLyQuayThuoc.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;

namespace QuanLyQuayThuoc.Repositories
{
    public class DonHangRepository : IDonHangRepository
    {
        private readonly ApplicationDbContext _context;
        public DonHangRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(DonHang donHang)
        {
            await _context.DonHangs.AddAsync(donHang);
        }
        public async Task<DonHang> CreateOrderAsync(DonHang donHang)
        {
            await _context.DonHangs.AddAsync(donHang);
            await _context.SaveChangesAsync();
            return donHang;
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}