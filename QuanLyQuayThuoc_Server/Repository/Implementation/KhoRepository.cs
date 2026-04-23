using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.Models;
using QuanLyQuayThuoc.Repositories.Interfaces;
using QuanLyQuayThuoc.DTOs.Kho;
using QuanLyQuayThuoc.DTOs.SanPham;
using BarcodeStandard;
using SkiaSharp;
using Type = BarcodeStandard.Type;

namespace QuanLyQuayThuoc.Repositories
{
    public class KhoRepository : IKhoRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;

        public KhoRepository(ApplicationDbContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IEnumerable<object>> TimKiemThuocAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<object>();

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
                        d.MaDvt,
                        d.TenDonVi,
                        d.GiaBan,
                        d.GiaTriQuyDoi,
                        d.LaDonViCoBan
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

        public async Task<KhoTongQuanResponseDto> GetTongQuanAsync(int? maDanhMuc, string search)
        {
            var query = _context.Thuocs.AsQueryable();

            if (maDanhMuc.HasValue) query = query.Where(t => t.MaDanhMuc == maDanhMuc);
            if (!string.IsNullOrEmpty(search)) query = query.Where(t => t.TenThuoc.Contains(search));

            var items = await query.Select(t => new KhoTongQuanItemDto
            {
                MaThuoc = t.MaThuoc,
                TenThuoc = t.TenThuoc,
                TenDanhMuc = t.MaDanhMucNavigation != null ? t.MaDanhMucNavigation.TenDanhMuc : "Không có",
                TongTon = t.LoHangs.Sum(l => l.SoLuongTon)
            }).ToListAsync();

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
            var today = DateOnly.FromDateTime(DateTime.Now);
            var sixMonthsLater = today.AddMonths(6);

            var query = _context.LoHangs.Include(l => l.MaThuocNavigation).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(l => l.MaThuocNavigation.TenThuoc.Contains(search) || l.SoLo.Contains(search));
            }

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
            using var giaoDich = await _context.Database.BeginTransactionAsync();
            try
            {
                int viTri = 0;
                var now = DateTime.Now;
                var today = DateOnly.FromDateTime(now);

                foreach (var item in phieuNhap.ChiTiet)
                {
                    var donViTinh = await _context.DonViTinhs
                         .Include(d => d.MaThuocNavigation)
                         .FirstOrDefaultAsync(d => d.MaThuoc == item.MaThuoc
                          && d.TenDonVi.ToLower().Trim() == item.TenDonVi.ToLower().Trim());

                    if (donViTinh == null) continue;

                    string maVach;
                    if (!string.IsNullOrEmpty(donViTinh.MaVach))
                    {
                        maVach = donViTinh.MaVach;
                    }
                    else
                    {
                        maVach = now.ToString("yyMMdd") + item.MaThuoc.ToString("D4") + viTri.ToString("D2");
                        donViTinh.MaVach = maVach;
                        _context.DonViTinhs.Update(donViTinh);
                    }

                    // 1. Phân tích hệ số quy đổi của đơn vị nhập
                    int heSoQuyDoi = donViTinh.GiaTriQuyDoi ?? 1;

                    // 2. Chuẩn hóa về đơn vị cơ bản (hệ số 1)
                    int tongSoLuongLe = item.SoLuong * heSoQuyDoi;
                    decimal giaNhapLe = heSoQuyDoi > 0 ? (item.GiaNhap / (decimal)heSoQuyDoi) : item.GiaNhap;

                    var loHangMoi = new LoHang
                    {
                        MaThuoc = item.MaThuoc,
                        SoLo = item.SoLo,
                        HanSuDung = DateOnly.FromDateTime(item.HanSuDung),
                        GiaNhap = giaNhapLe,
                        SoLuongTon = tongSoLuongLe,
                        NgaySanXuat = today
                    };

                    _context.LoHangs.Add(loHangMoi);

                    item.MaVach = maVach;
                    item.TenThuoc = donViTinh.MaThuocNavigation?.TenThuoc;
                    item.HinhAnhMaVach = TaoHinhAnhMaVachBase64(maVach);
                    viTri++;
                }

                await _context.SaveChangesAsync();
                await giaoDich.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await giaoDich.RollbackAsync();
                return false;
            }
        }

        private string TaoHinhAnhMaVachBase64(string noiDungMa)
        {
            try
            {
                var congCuVe = new Barcode();

                var img = congCuVe.Encode(Type.Code128, noiDungMa, SKColors.Black, SKColors.White, 250, 80); using (var data = img.Encode(SKEncodedImageFormat.Png, 100))
                using (var boNhoTam = new MemoryStream())
                {
                    data.SaveTo(boNhoTam);
                    return "data:image/png;base64," + Convert.ToBase64String(boNhoTam.ToArray());
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private async Task<ThongKeKhoDto> GetThongKeChung()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var soon = today.AddMonths(6);

            return new ThongKeKhoDto
            {
                TongGiaTri = await _context.LoHangs
                    .SumAsync(l => l.SoLuongTon * (l.GiaNhap ?? 0)),

                SoLoHetHan = await _context.LoHangs
                    .CountAsync(l => l.HanSuDung < today),

                SoLoSapHetHan = await _context.LoHangs
                    .CountAsync(l => l.HanSuDung >= today && l.HanSuDung <= soon),

                SoMatHangSapHetTon = (await _context.Thuocs.ToListAsync())
                    .Count(t => _context.LoHangs.Where(l => l.MaThuoc == t.MaThuoc).Sum(l => l.SoLuongTon) < 50)
            };
        }
        public async Task<IEnumerable<MaVachDto>> GetMaVachTheoThuocAsync(int maThuoc)
        {
            var danhSach = await _context.DonViTinhs
                .Where(d => d.MaThuoc == maThuoc && !string.IsNullOrEmpty(d.MaVach))
                .Include(d => d.MaThuocNavigation)
                .ToListAsync();

            return danhSach.Select(d => new MaVachDto
            {
                MaVach = d.MaVach,
                TenThuoc = d.MaThuocNavigation?.TenThuoc ?? "",
                TenDonVi = d.TenDonVi,
                HinhAnhMaVach = TaoHinhAnhMaVachBase64(d.MaVach)
            });
        }
        public async Task<bool> ThemThuocMoiVaNhapKhoAsync(ThemThuocMoiVaNhapKhoDto dto)
        {
            using var giaoDich = await _context.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.Now;
                var today = DateOnly.FromDateTime(now);

                var thuocMoi = new Thuoc
                {
                    TenThuoc = dto.TenThuoc,
                    MaDanhMuc = dto.MaDanhMuc,
                    SoDangKy = dto.SoDangKy,
                    QuyCach = dto.QuyCach,
                    DangBaoChe = dto.DangBaoChe,
                    NhaSanXuat = dto.NhaSanXuat,
                    NuocSanXuat = dto.NuocSanXuat,
                    ThanhPhan = dto.ThanhPhan,
                    MoTaNgan = dto.MoTaNgan,
                    LaThuocKeDon = dto.LaThuocKeDon ?? false,
                    NgayTao = now
                };

                if (!string.IsNullOrEmpty(dto.HinhAnh) && dto.HinhAnh.Contains("base64,"))
                {
                    try
                    {
                        string rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                        var drugsPath = Path.Combine(rootPath, "uploads", "drugs");
                        if (!Directory.Exists(drugsPath)) Directory.CreateDirectory(drugsPath);

                        var base64Data = dto.HinhAnh.Split("base64,")[1];
                        var imageBytes = Convert.FromBase64String(base64Data);
                        var fileName = $"drug_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString().Substring(0, 8)}.jpg";
                        var filePath = Path.Combine(drugsPath, fileName);

                        await File.WriteAllBytesAsync(filePath, imageBytes);
                        thuocMoi.HinhAnhChinh = $"/uploads/drugs/{fileName}";
                    }
                    catch (Exception ex)
                    {
                        // Log error or ignore if image fails
                        Console.WriteLine("Error saving drug image: " + ex.Message);
                    }
                }

                // 1. Phân tích đơn vị nhập để quy đổi
                var donViNhapInfo = dto.ChiTiet.FirstOrDefault(x => x.TenDonVi.ToLower() == dto.TenDonViNhap.ToLower());
                int heSoQuyDoi = donViNhapInfo?.GiaTriQuyDoi ?? 1;

                // 2. Chuẩn hóa về đơn vị cơ bản
                int tongSoLuongLe = dto.SoLuong * heSoQuyDoi;
                decimal giaNhapLe = heSoQuyDoi > 0 ? (dto.GiaNhap / (decimal)heSoQuyDoi) : dto.GiaNhap;

                // 3. Tạo thuốc và lưu để lấy MaThuoc
                _context.Thuocs.Add(thuocMoi);
                await _context.SaveChangesAsync();

                // 4. Lưu danh sách đơn vị tính
                for (int i = 0; i < dto.ChiTiet.Count; i++)
                {
                    var item = dto.ChiTiet[i];
                    var maVach = now.ToString("yyMMdd") + thuocMoi.MaThuoc.ToString("D4") + i.ToString("D2");

                    var donViTinh = new DonViTinh
                    {
                        MaThuoc = thuocMoi.MaThuoc,
                        TenDonVi = item.TenDonVi,
                        GiaBan = item.GiaBan,
                        GiaTriQuyDoi = item.GiaTriQuyDoi,
                        LaDonViCoBan = item.LaDonViCoBan,
                        MaVach = maVach
                    };
                    _context.DonViTinhs.Add(donViTinh);

                    // Trả về thông tin barcode & tính toán quy đổi để UI hiển thị
                    item.MaVach = maVach;
                    item.HinhAnhMaVach = TaoHinhAnhMaVachBase64(maVach);

                    // Tính toán hiển thị (Logic backend)
                    double ratio = (double)heSoQuyDoi / (item.GiaTriQuyDoi > 0 ? item.GiaTriQuyDoi : 1);
                    item.GiaNhap = (decimal)((double)dto.GiaNhap / ratio);
                    item.SoLuong = (int)((double)dto.SoLuong * ratio);
                }

                // 5. Lưu DUY NHẤT 1 lô hàng (Đã được quy đổi về đơn vị gốc)
                var loHangMoi = new LoHang
                {
                    MaThuoc = thuocMoi.MaThuoc,
                    SoLo = dto.SoLo,
                    HanSuDung = DateOnly.FromDateTime(dto.HanSuDung),
                    GiaNhap = giaNhapLe,
                    SoLuongTon = tongSoLuongLe,
                    NgaySanXuat = today
                };
                _context.LoHangs.Add(loHangMoi);

                await _context.SaveChangesAsync();
                await giaoDich.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await giaoDich.RollbackAsync();
                return false;
            }
        }
        public async Task<object?> TimThuocTheoBarcodeAsync(string maVach)
        {
            var donVi = await _context.DonViTinhs
                .Include(d => d.MaThuocNavigation)
                    .ThenInclude(t => t.LoHangs)
                .Include(d => d.MaThuocNavigation)
                    .ThenInclude(t => t.DonViTinhs)
                .FirstOrDefaultAsync(d => d.MaVach == maVach);

            if (donVi == null || donVi.MaThuocNavigation == null)
                return null;

            var thuoc = donVi.MaThuocNavigation;

            return new
            {
                thuoc.MaThuoc,
                thuoc.TenThuoc,
                HamLuong = thuoc.ThanhPhan,
                GiaBan = donVi.GiaBan,
                MaDvtMacDinh = donVi.MaDvt,
                MaDvtSelected = donVi.MaDvt,

                DanhSachDonVi = thuoc.DonViTinhs.Select(d => new
                {
                    d.MaDvt,
                    d.TenDonVi,
                    d.GiaBan,
                    d.GiaTriQuyDoi,
                    d.LaDonViCoBan
                }).ToList(),

                DanhSachLo = thuoc.LoHangs
                    .Where(lo => lo.SoLuongTon > 0
                              && lo.HanSuDung >= DateOnly.FromDateTime(DateTime.Today))
                    .OrderBy(lo => lo.HanSuDung)
                    .Select(lo => new
                    {
                        lo.MaLo,
                        lo.SoLo,
                        HanSuDung = lo.HanSuDung.ToString("yyyy-MM-dd"),
                        lo.SoLuongTon
                    }).ToList()
            };
        }
    }
}