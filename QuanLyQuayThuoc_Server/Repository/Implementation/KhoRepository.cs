using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.Models;
using QuanLyQuayThuoc.Repositories.Interfaces;
using QuanLyQuayThuoc.DTOs.Kho;
using QuanLyQuayThuoc.DTOs.SanPham;

namespace QuanLyQuayThuoc.Repositories
{
    public class KhoRepository : IKhoRepository
    {
        private readonly ApplicationDbContext _context;

        public KhoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. CÁC HÀM PHỤC VỤ BÁN HÀNG (GIỮ NGUYÊN)
        // ==========================================
        public async Task<IEnumerable<object>> TimKiemThuocAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<object>();

            return await _context.Thuocs
                .Where(t => t.TenThuoc.Contains(query) || t.ThanhPhan.Contains(query))
                .Select(t => new {
                    t.MaThuoc,
                    t.TenThuoc,
                    HamLuong = t.ThanhPhan,
                    SoLuongTon = t.LoHangs.Sum(l => (int?)l.SoLuongTon) ?? 0,
                    DanhSachDonVi = t.DonViTinhs.Select(d => new {
                        d.MaDvt,
                        d.TenDonVi,
                        d.GiaBan,
                        d.GiaTriQuyDoi,
                        d.LaDonViCoBan
                    }).ToList(),
                    GiaBanHienTai = t.DonViTinhs.Where(d => d.LaDonViCoBan == true).Select(d => (decimal?)d.GiaBan).FirstOrDefault() ?? 0,
                    TenDonVi = t.DonViTinhs.Where(d => d.LaDonViCoBan == true).Select(d => d.TenDonVi).FirstOrDefault() ?? "Đơn vị"
                })
                .OrderByDescending(t => t.TenThuoc.StartsWith(query))
                .ThenBy(t => t.TenThuoc)
                .Take(10).ToListAsync();
        }

        public async Task<LoHang> GetByIdAsync(int maLo) => await _context.LoHangs.FindAsync(maLo);

        public async Task UpdateSoLuongAsync(int maLo, int soLuongTru)
        {
            var lo = await _context.LoHangs.FindAsync(maLo);
            if (lo == null) throw new Exception("Không tìm thấy lô hàng.");
            if (lo.SoLuongTon < soLuongTru) throw new Exception($"Lô {lo.SoLo} không đủ hàng.");
            lo.SoLuongTon -= soLuongTru;
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<IEnumerable<LoHang>> GetLoHangByThuocAsync(int maThuoc)
        {
            return await _context.LoHangs
                .Where(l => l.MaThuoc == maThuoc && l.SoLuongTon > 0)
                .OrderBy(l => l.HanSuDung).ToListAsync();
        }

        // ==========================================
        // 2. CÁC HÀM QUẢN LÝ KHO (MỚI)
        // ==========================================

        public async Task<KhoTongQuanResponseDto> GetTongQuanAsync(int? maDanhMuc, string search)
        {
            // 1. Tạo query từ bảng Thuocs nhưng CHƯA thực thi (lazy loading)
            var query = _context.Thuocs.AsQueryable();

            // 2. Lọc dữ liệu như bình thường
            if (maDanhMuc.HasValue) query = query.Where(t => t.MaDanhMuc == maDanhMuc);
            if (!string.IsNullOrEmpty(search)) query = query.Where(t => t.TenThuoc.Contains(search));

            // 3. ĐÂY LÀ CHỖ QUAN TRỌNG: 
            // Chúng ta dùng Select để "ép" EF chỉ lấy những cột tồn tại. 
            // EF sẽ không bao giờ nhìn tới các cột Slug, TrangThai... nữa.
            var items = await query.Select(t => new KhoTongQuanItemDto
            {
                MaThuoc = t.MaThuoc,
                TenThuoc = t.TenThuoc,
                // Chỉ lấy TenDanhMuc từ bảng liên kết, bỏ qua toàn bộ object DanhMuc
                TenDanhMuc = t.MaDanhMucNavigation != null ? t.MaDanhMucNavigation.TenDanhMuc : "Không có",
                TongTon = t.LoHangs.Sum(l => l.SoLuongTon)
            }).ToListAsync();

            // 4. Gán trạng thái sau khi đã có dữ liệu trong RAM
            foreach (var item in items)
            {
                item.TrangThai = item.TongTon < 50 ? "Sắp hết hàng" : "Còn hàng";
            }

            return new KhoTongQuanResponseDto
            {
                Items = items,
                ThongKe = await GetThongKeChung()
            };
        }
        public async Task<IEnumerable<DanhMucDto>> GetDanhMucAsync()
        {
            return await _context.DanhMucs
                .Select(d => new DanhMucDto
                {
                    MaDanhMuc = d.MaDanhMuc,
                    TenDanhMuc = d.TenDanhMuc
                })
                .ToListAsync();
        }
        public async Task<KhoLoHangResponseDto> GetLoHangAsync(string search, string thang, string loai)
        {
            // Chuyển mốc thời gian về DateOnly để so sánh với Database
            var today = DateOnly.FromDateTime(DateTime.Now);
            var sixMonthsLater = today.AddMonths(6);

            var query = _context.LoHangs.Include(l => l.MaThuocNavigation).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(l => l.MaThuocNavigation.TenThuoc.Contains(search) || l.SoLo.Contains(search));
            if (!string.IsNullOrEmpty(thang) && DateTime.TryParse(thang + "-01", out var parsedDate))
            {
                var thangDateOnly = DateOnly.FromDateTime(parsedDate);
                var thangSau = thangDateOnly.AddMonths(1);
                query = query.Where(l => l.HanSuDung >= thangDateOnly && l.HanSuDung < thangSau);
            }
            var result = await query.ToListAsync();
            var items = result.Select(l => new QuanLyQuayThuoc.DTOs.Kho.LoHangDto
            {
                MaLo = l.MaLo,
                SoLo = l.SoLo,
                HanSuDung = l.HanSuDung.ToString("yyyy-MM-dd"),
                SoLuongTon = l.SoLuongTon,
                GiaNhap = l.GiaNhap ?? 0,
                TenThuoc = l.MaThuocNavigation?.TenThuoc,
                // So sánh DateOnly với DateOnly (Fix lỗi Operator <)
                MucDoCanhBao = l.HanSuDung < today ? 2 : (l.HanSuDung <= sixMonthsLater ? 1 : 0)
            }).ToList();

            if (loai == "expired") items = items.Where(i => i.MucDoCanhBao == 2).ToList();
            else if (loai == "soon") items = items.Where(i => i.MucDoCanhBao == 1).ToList();

            return new KhoLoHangResponseDto { Items = items, ThongKe = await GetThongKeChung() };
        }
        public async Task<bool> SuaLoHangAsync(int maLo, SuaLoHangDto dto)
        {
            var lo = await _context.LoHangs.FindAsync(maLo);
            if (lo == null) return false;

            lo.SoLo = dto.SoLo;
            lo.HanSuDung = DateOnly.Parse(dto.HanSuDung);
            lo.SoLuongTon = dto.SoLuongTon;
            lo.GiaNhap = dto.GiaNhap;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> NhapKhoAsync(PhieuNhapKhoDto phieuNhap)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int index = 0;
                var now = DateTime.Now;
                var todayDateOnly = DateOnly.FromDateTime(now);

                foreach (var item in phieuNhap.ChiTiet)
                {
                    // TỰ ĐỘNG SINH MÃ VẠCH: yyMMddHHmmss + index
                    string barcode = now.ToString("yyMMddHHmmss") + index.ToString("D2");
                    index++;

                    // Thêm Lô
                    _context.LoHangs.Add(new LoHang
                    {
                        MaThuoc = item.MaThuoc,
                        SoLo = item.SoLo,
                        // Chuyển DateTime từ DTO sang DateOnly của Model (Fix lỗi Convert)
                        HanSuDung = DateOnly.FromDateTime(item.HanSuDung),
                        GiaNhap = item.GiaNhap,
                        SoLuongTon = item.SoLuong,
                        NgaySanXuat = todayDateOnly
                    });

                    // Cập nhật Mã vạch vào DonViTinh
                    var dvt = await _context.DonViTinhs.FirstOrDefaultAsync(d => d.MaThuoc == item.MaThuoc && d.TenDonVi == item.TenDonVi);
                    if (dvt != null) dvt.MaVach = barcode;

                    item.MaVach = barcode;
                }   
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch { await transaction.RollbackAsync(); return false; }
        }

        private async Task<ThongKeKhoDto> GetThongKeChung()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var soon = today.AddMonths(6);

            // Tính toán thống kê với DateOnly
            return new ThongKeKhoDto
            {
                TongGiaTri = await _context.LoHangs.SumAsync(l => l.SoLuongTon * (l.GiaNhap ?? 0)),
                SoLoHetHan = await _context.LoHangs.CountAsync(l => l.HanSuDung < today),
                SoLoSapHetHan = await _context.LoHangs.CountAsync(l => l.HanSuDung >= today && l.HanSuDung <= soon),
                SoMatHangSapHetTon = (await _context.Thuocs.ToListAsync())
                    .Count(t => _context.LoHangs.Where(l => l.MaThuoc == t.MaThuoc).Sum(l => l.SoLuongTon) < 50)
            };
        }
    }
}