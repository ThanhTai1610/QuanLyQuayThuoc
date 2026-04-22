using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.DTOs.SanPham;
using QuanLyQuayThuoc.Repository.Interfaces; // Đảm bảo Tài đã có Interface tương ứng

namespace QuanLyQuayThuoc.Repository.Implementation
{
    public class SanPhamRepository : ISanPhamRepository
    {
        private readonly ApplicationDbContext _context;

        public SanPhamRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SanPhamSearchDto>> SearchQuickAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<SanPhamSearchDto>();

            var searchTerm = query.Trim().ToLower();

            var results = await (from t in _context.Thuocs
                                 join d in _context.DanhMucs on t.MaDanhMuc equals d.MaDanhMuc
                                 join dvt in _context.DonViTinhs on t.MaThuoc equals dvt.MaThuoc
                                 where dvt.LaDonViCoBan == true &&
                                       (EF.Functions.Collate(t.TenThuoc, "SQL_Latin1_General_CP1_CI_AI").Contains(searchTerm) ||
                                        EF.Functions.Collate(d.TenDanhMuc, "SQL_Latin1_General_CP1_CI_AI").Contains(searchTerm))
                                 select new SanPhamSearchDto
                                 {
                                     Id = t.MaThuoc, // Đảm bảo t.MaThuoc của 2 thuốc phải khác nhau
                                     TenThuoc = t.TenThuoc,
                                     HinhAnhChinh = t.HinhAnhChinh,
                                     GiaBan = dvt.GiaBan ?? 0,
                                     TenDanhMuc = d.TenDanhMuc
                                 })
                                 .Take(10) // Lấy 10 kết quả cho dư dả
                                 .ToListAsync();

            // Chỉ dùng Distinct nếu thực sự có dữ liệu trùng lặp hoàn toàn
            return results.GroupBy(p => p.Id).Select(g => g.First()).ToList();
        }
    }
}
