using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.DTOs.DonHang;
using QuanLyQuayThuoc.Models;
using QuanLyQuayThuoc.Repositories.Interfaces;
using QuanLyQuayThuoc.Services.Interfaces;
using QuanLyQuayThuoc.Data;

namespace QuanLyQuayThuoc.Services.Implementation
{
    public class BanHangService : IBanHangService
    {
        private readonly IDonHangRepository _donHangRepo;
        private readonly IKhoRepository _khoRepo;
        private readonly ApplicationDbContext _context;
        public BanHangService(IDonHangRepository donHangRepo, IKhoRepository khoRepo, ApplicationDbContext context)
        {
            _donHangRepo = donHangRepo;
            _khoRepo = khoRepo;
            _context = context;
        }
        public async Task<IEnumerable<Object>> TimKiemThuocNhanhAsync(string query)
        {
            return await _khoRepo.TimKiemThuocAsync(query);
        }

        public async Task<IEnumerable<LoHang>> LayDanhSachLoCuaThuocAsync(int maThuoc)
        {
            return await _khoRepo.GetLoHangByThuocAsync(maThuoc);
        }

        public async Task<int> ThanhToanTaiQuayAsync(TaoDonHangDto dto, int maNhanVien)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var donHang = new DonHang
                {
                    MaNhanVien = maNhanVien,
                    NgayDat = DateTime.Now,
                    PhuongThucThanhToan = dto.PhuongThucThanhToan,
                    TrangThai = "Hoàn tất",
                    TongTien = 0
                };

                _context.DonHangs.Add(donHang);
                await _context.SaveChangesAsync();

                decimal tongTienDonHang = 0;

                foreach (var item in dto.ChiTiet)
                {
                    var dvt = await _context.DonViTinhs
                        .FirstOrDefaultAsync(d => d.MaDvt == item.MaDVT);

                    if (dvt == null) throw new Exception("Không tìm thấy đơn vị tính mã {item.MaDVT}");
                    int heSoQuyDoi = dvt.GiaTriQuyDoi ?? 1;
                    int soLuongQuyDoi = item.SoLuong * heSoQuyDoi;
                    await _khoRepo.UpdateSoLuongAsync(item.MaLo, soLuongQuyDoi);

                    var chiTiet = new ChiTietDonHang
                    {
                        MaDonHang = donHang.MaDonHang,
                        MaLo = item.MaLo,
                        MaDvt = item.MaDVT,
                        SoLuong = item.SoLuong,
                        GiaBanTaiThoiDiem = item.GiaBan
                    };
                    _context.ChiTietDonHangs.Add(chiTiet);
                    tongTienDonHang += (decimal)item.SoLuong * item.GiaBan;
                }
                decimal giamGia = dto.GiamGia;
                donHang.TongTien = tongTienDonHang - giamGia;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return donHang.MaDonHang;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception("Thanh toán thất bại: " + errorMessage);
            }
        }
    }
}