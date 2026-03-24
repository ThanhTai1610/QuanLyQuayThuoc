using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.Models;
using QuanLyQuayThuoc.Repositories.Interfaces;

namespace QuanLyQuayThuoc.Repositories
{
    public class KhoRepository : IKhoRepository
    {
        private readonly ApplicationDbContext _context;

        public KhoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy thông tin lô hàng để kiểm tra trước khi trừ
        public async Task<LoHang> GetByIdAsync(int maLo)
        {
            return await _context.LoHangs.FindAsync(maLo);
        }

        // Hàm quan trọng nhất: Trừ kho
        public async Task UpdateSoLuongAsync(int maLo, int soLuongBan)
        {
            var loHang = await _context.LoHangs.FindAsync(maLo);
            if (loHang != null)
            {
                // Kiểm tra nếu tồn kho đủ để trừ
                if (loHang.SoLuongTon < soLuongBan)
                {
                    throw new Exception($"Lô hàng {loHang.SoLo} không đủ số lượng tồn.");
                }

                loHang.SoLuongTon -= soLuongBan;
                _context.LoHangs.Update(loHang);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Thực thi hàm lấy danh sách lô theo thuốc (để hiện lên Vue)
        public async Task<IEnumerable<LoHang>> GetLoHangByThuocAsync(int maThuoc)
        {
            return await _context.LoHangs
                .Where(l => l.MaThuoc == maThuoc && l.SoLuongTon > 0)
                .OrderBy(l => l.HanSuDung) // Ưu tiên lô hết hạn trước (FEFO)
                .ToListAsync();
        }
    }
}