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

        public async Task<IEnumerable<object>> TimKiemThuocAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<object>();

            return await _context.Thuocs
                .Where(t => t.TenThuoc.Contains(query) || t.ThanhPhan.Contains(query))
                .Select(t => new
                {
                    t.MaThuoc,
                    t.TenThuoc,
                    HamLuong = t.ThanhPhan,
                    SoLuongTon = t.LoHangs.Sum(l => (int?)l.SoLuongTon) ?? 0,
                    DanhSachDonVi = t.DonViTinhs.Select(d => new
                    {
                        MaDvt = d.MaDvt,
                        TenDonVi = d.TenDonVi,
                        GiaBan = d.GiaBan,
                        GiaTriQuyDoi = d.GiaTriQuyDoi,
                        LaDonViCoBan = d.LaDonViCoBan
                    }).ToList(),

                    GiaBanHienTai = t.DonViTinhs
                        .Where(d => d.LaDonViCoBan == true)
                        .Select(d => (decimal?)d.GiaBan)
                        .FirstOrDefault() ?? 0,

                    TenDonVi = t.DonViTinhs
                        .Where(d => d.LaDonViCoBan == true)
                        .Select(d => d.TenDonVi)
                        .FirstOrDefault() ?? "Đơn vị"
                })
                .OrderBy(t => t.TenThuoc)
                .Take(10)
                .ToListAsync();
        }
        public async Task<LoHang> GetByIdAsync(int maLo)
        {
            return await _context.LoHangs.FindAsync(maLo);
        }
        public async Task UpdateSoLuongAsync(int maLo, int soLuongTru)
        {
            var lo = await _context.LoHangs.FindAsync(maLo);
            if (lo == null) throw new Exception("Không tìm thấy lô hàng để trừ kho.");

            if (lo.SoLuongTon < soLuongTru)
                throw new Exception($"Lô {lo.SoLo} không đủ hàng. Tồn: {lo.SoLuongTon}, Cần: {soLuongTru}");

            lo.SoLuongTon -= soLuongTru;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<LoHang>> GetLoHangByThuocAsync(int maThuoc)
        {
            return await _context.LoHangs
                .Where(l => l.MaThuoc == maThuoc && l.SoLuongTon > 0)
                .OrderBy(l => l.HanSuDung)
                .ToListAsync();
        }
    }
}