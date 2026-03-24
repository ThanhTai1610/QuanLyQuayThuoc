using System;
using System.Linq;
using System.Threading.Tasks;
using QuanLyQuayThuoc.DTOs.DonHang;
using QuanLyQuayThuoc.Models;
using QuanLyQuayThuoc.Repositories.Interfaces;
using QuanLyQuayThuoc.Services.Interfaces;

namespace QuanLyQuayThuoc.Services.Implementation
{
    public class BanHangService : IBanHangService
    {
        private readonly IDonHangRepository _donHangRepo;
        private readonly IKhoRepository _khoRepo;

        // Tiêm (Inject) các Repository độc lập vào đây
        public BanHangService(IDonHangRepository donHangRepo, IKhoRepository khoRepo)
        {
            _donHangRepo = donHangRepo;
            _khoRepo = khoRepo;
        }

        public async Task<int> ThanhToanTaiQuayAsync(TaoDonHangDto dto, int maNhanVien)
        {
            try
            {
                // 1. Tính tổng tiền từ danh sách chi tiết (kiểm tra lại từ Backend cho chắc chắn)
                decimal tongTienHang = dto.ChiTiet.Sum(x => x.GiaBan * x.SoLuong);
                decimal tongThanhToan = tongTienHang - dto.GiamGia;

                // 2. Tạo đối tượng đơn hàng
                var donHang = new DonHang
                {
                    MaKhachHang = dto.MaKhachHang,
                    MaNhanVien = maNhanVien,
                    NgayDat = DateTime.Now,
                    TongTien = tongThanhToan,
                    PhuongThucThanhToan = dto.PhuongThucThanhToan,
                    TrangThai = "Hoàn tất",
                    // Chuyển đổi từ DTO sang Model ChiTietDonHang
                    ChiTietDonHangs = dto.ChiTiet.Select(c => new ChiTietDonHang
                    {
                        MaLo = c.MaLo,
                        MaDvt = c.MaDVT,
                        SoLuong = c.SoLuong,
                        GiaBanTaiThoiDiem = c.GiaBan
                    }).ToList()
                };

                // 3. Xử lý trừ kho cho từng mặt hàng
                foreach (var item in dto.ChiTiet)
                {
                    // Gọi sang LoHangRepository đã viết ở bước trước
                    await _khoRepo.UpdateSoLuongAsync(item.MaLo, item.SoLuong);
                }

                // 4. Lưu đơn hàng vào Database qua DonHangRepository
                await _donHangRepo.AddAsync(donHang);

                // 5. Lưu tất cả thay đổi (DB commit)
                await _donHangRepo.SaveChangesAsync();

                return donHang.MaDonHang;
            }
            catch (Exception ex)
            {
                // Nếu có bất kỳ lỗi nào (hết hàng, lỗi DB...), Exception sẽ bắn ra ngoài
                throw new Exception("Lỗi xử lý thanh toán: " + ex.Message);
            }
        }
    }
}