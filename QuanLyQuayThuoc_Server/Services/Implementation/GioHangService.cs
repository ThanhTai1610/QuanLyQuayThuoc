using QuanLyQuayThuoc.DTOs.DonHang;
using QuanLyQuayThuoc.Models;
using QuanLyQuayThuoc.Repositories.Interfaces;
using QuanLyQuayThuoc.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Repositories;

namespace QuanLyQuayThuoc.Services.Implementation
{
    public class GioHangService : IGioHangService
    {
        private readonly IGioHangRepository _gioHangRepo;
        private readonly IThuocRepository _thuocRepo;

        public GioHangService(IGioHangRepository gioHangRepo, IThuocRepository thuocRepo)
        {
            _gioHangRepo = gioHangRepo;
            _thuocRepo = thuocRepo;
        }

        public async Task<IEnumerable<GioHangItemDto>> LayDanhSachGioHangAsync(int maKhachHang)
        {
            var danhSachEntity = await _gioHangRepo.GetByKhachHangAsync(maKhachHang);
            var ketQua = new List<GioHangItemDto>();

            foreach (var item in danhSachEntity)
            {
                // Lấy thông tin thuốc để lấy danh sách đơn vị tính quy đổi (Vỉ, Hộp...)
                var thuoc = await _thuocRepo.GetByIdAsync(item.MaThuoc ?? 0);

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
            // Kiểm tra trùng lặp thuốc và đơn vị tính trong giỏ
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
    }
}