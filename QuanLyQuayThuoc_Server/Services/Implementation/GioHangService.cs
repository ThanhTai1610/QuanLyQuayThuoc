// GioHangService.cs
using QuanLyQuayThuoc.DTOs.DonHang;
using QuanLyQuayThuoc.Models;
using QuanLyQuayThuoc.Repositories.Interfaces;
using QuanLyQuayThuoc.Services.Interfaces;
using QuanLyQuayThuoc.Repositories;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;

namespace QuanLyQuayThuoc.Services.Implementation
{
    public class GioHangService : IGioHangService
    {
        private readonly IGioHangRepository _gioHangRepo;
        private readonly IThuocRepository _thuocRepo;
        private readonly ApplicationDbContext _context;

        public GioHangService(
            IGioHangRepository gioHangRepo,
            IThuocRepository thuocRepo,
            ApplicationDbContext context)
        {
            _gioHangRepo = gioHangRepo;
            _thuocRepo = thuocRepo;
            _context = context;
        }

        public async Task<IEnumerable<GioHangItemDto>> LayDanhSachGioHangAsync(int maKhachHang)
        {
            var danhSachEntity = await _gioHangRepo.GetByKhachHangAsync(maKhachHang);
            var ketQua = new List<GioHangItemDto>();

            foreach (var item in danhSachEntity)
            {
                var thuoc = await _thuocRepo.GetByIdAsync(item.MaThuoc ?? 0);

                // Chọn lô FEFO: hết hạn sớm nhất xuất trước
                var loHangGoiY = await _context.LoHangs
                    .Where(l => l.MaThuoc == item.MaThuoc && l.SoLuongTon > 0)
                    .OrderBy(l => l.HanSuDung)
                    .FirstOrDefaultAsync();

                var dsDonViTinh = thuoc?.DonViTinhs.Select(d => new DonViTinhTrongGioHangDto
                {
                    MaDVT = d.MaDvt,
                    TenDonVi = d.TenDonVi,
                    GiaBan = d.GiaBan ?? 0
                }).ToList() ?? new List<DonViTinhTrongGioHangDto>();

                ketQua.Add(new GioHangItemDto
                {
                    MaGioHang = item.MaGioHang,
                    MaThuoc = item.MaThuoc ?? 0,
                    MaLo = loHangGoiY?.MaLo ?? 0,
                    TenThuoc = item.MaThuocNavigation?.TenThuoc ?? "Không xác định",
                    HinhAnhChinh = item.MaThuocNavigation?.HinhAnhChinh ?? "",
                    MoTaNgan = item.MaThuocNavigation?.MoTaNgan ?? "",
                    MaDVT = item.MaDvt ?? 0,
                    TenDonVi = item.MaDvtNavigation?.TenDonVi ?? "",
                    GiaBan = item.MaDvtNavigation?.GiaBan ?? 0,
                    SoLuong = item.SoLuong ?? 0,
                    DanhSachDVT = dsDonViTinh
                });
            }
            return ketQua;
        }

        public async Task<bool> ThemVaoGioHangAsync(int maKhachHang, int maThuoc, int maDvt, int soLuong)
        {
            var itemHienTai = await _gioHangRepo.GetCartItemAsync(maKhachHang, maThuoc, maDvt);

            if (itemHienTai != null)
            {
                itemHienTai.SoLuong += soLuong;
                _gioHangRepo.Update(itemHienTai);
            }
            else
            {
                var itemMoi = new GioHang
                {
                    MaKhachHang = maKhachHang,
                    MaThuoc = maThuoc,
                    MaDvt = maDvt,
                    SoLuong = soLuong
                };
                await _gioHangRepo.AddAsync(itemMoi);
            }

            return await _gioHangRepo.SaveChangesAsync();
        }

        public async Task<bool> CapNhatGioHangAsync(List<CapNhatGioHangDto> danhSachCapNhat)
        {
            foreach (var dto in danhSachCapNhat)
            {
                var item = await _gioHangRepo.GetByIdAsync(dto.MaGioHang);
                if (item != null)
                {
                    item.SoLuong = dto.SoLuong;
                    item.MaDvt = dto.MaDVT;
                    _gioHangRepo.Update(item);
                }
            }
            return await _gioHangRepo.SaveChangesAsync();
        }

        public async Task<bool> XoaKhoiGioHangAsync(int maGioHang)
        {
            var item = await _gioHangRepo.GetByIdAsync(maGioHang);
            if (item == null) return false;

            _gioHangRepo.Delete(item);
            return await _gioHangRepo.SaveChangesAsync();
        }

        public async Task<bool> XoaToanBoGioHangAsync(int maKhachHang)
        {
            await _gioHangRepo.DeleteAllAsync(maKhachHang);
            return await _gioHangRepo.SaveChangesAsync();
        }

        /// <summary>
        /// Đặt hàng online. maKhachHang luôn lấy từ JWT token qua Controller.
        /// Dùng transaction để đảm bảo toàn vẹn dữ liệu:
        /// nếu bất kỳ bước nào thất bại (kể cả sau khi tạo đơn), toàn bộ rollback.
        /// </summary>
        public async Task<int> DatHangAsync(DatHangKhachHangDto dto, int maKhachHang)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Tính tổng tiền
                decimal tongTien = dto.ChiTiet.Sum(c => c.GiaBan * c.SoLuong) - dto.GiamGia;

                // 2. Tạo đơn hàng
                var donHang = new DonHang
                {
                    MaKhachHang = maKhachHang, // lấy từ JWT, không từ body request
                    MaNhanVien = null,
                    NgayDat = DateTime.Now,
                    TongTien = tongTien,
                    PhuongThucThanhToan = dto.PhuongThucThanhToan,
                    TrangThai = "Chờ xử lý",
                    DiaChiGiaoHang = dto.DiaChiGiaoHang,
                    SoDienThoaiNhan = dto.SoDienThoaiNhan,
                    GhiChu = dto.GhiChu
                };

                _context.DonHangs.Add(donHang);
                await _context.SaveChangesAsync();

                // 3. Tạo chi tiết + trừ tồn kho
                foreach (var ct in dto.ChiTiet)
                {
                    var lo = await _context.LoHangs.FindAsync(ct.MaLo);
                    if (lo == null || lo.SoLuongTon < ct.SoLuong)
                        throw new Exception($"Lô hàng {ct.MaLo} không đủ số lượng tồn kho.");

                    _context.ChiTietDonHangs.Add(new ChiTietDonHang
                    {
                        MaDonHang = donHang.MaDonHang,
                        MaLo = ct.MaLo,
                        MaDvt = ct.MaDVT,
                        SoLuong = ct.SoLuong,
                        GiaBanTaiThoiDiem = ct.GiaBan
                    });

                    lo.SoLuongTon -= ct.SoLuong;
                }

                await _context.SaveChangesAsync();

                // 4. Xóa giỏ hàng sau khi đặt thành công
                await _gioHangRepo.DeleteAllAsync(maKhachHang);
                await _gioHangRepo.SaveChangesAsync();

                await transaction.CommitAsync();

                return donHang.MaDonHang;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception("Đặt hàng thất bại: " + errorMessage);
            }
        }
    }
}