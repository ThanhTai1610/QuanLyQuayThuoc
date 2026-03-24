using QuanLyQuayThuoc.DTOs.NguoiDung;
using QuanLyQuayThuoc.Helpers;
using QuanLyQuayThuoc.Repositories.Interfaces;
using QuanLyQuayThuoc.Services.Interfaces;

namespace QuanLyQuayThuoc.Services.Implementation
{
    // Cực kỳ quan trọng: Phải có dấu ":" và tên Interface ở đây
    public class NguoiDungService : INguoiDungService
    {
        private readonly INguoiDungRepository _repository;

        public NguoiDungService(INguoiDungRepository repository)
        {
            _repository = repository;
        }

        // 1. Thực thi hàm Đăng nhập
        public async Task<PhanQuyenDto?> DangNhap(DangNhapDto duLieu)
        {
            var user = await _repository.GetByEmailAsync(duLieu.Email);

            // Kiểm tra mật khẩu dùng BCrypt qua Helper
            if (user == null || !PasswordHasher.VerifyPassword(duLieu.MatKhau, user.MatKhau))
                return null;

            return new PhanQuyenDto
            {
                MaNguoiDung = user.MaNguoiDung,
                HoTen = user.HoTen,
                // Xử lý lỗi ép kiểu int? sang int bằng toán tử ??
                MaVaiTro = user.MaVaiTro // Mặc định là khách hàng nếu null
            };
        }

        // 2. Thực thi hàm Lấy hồ sơ (Kết nối với Repository Tài đã viết)
        public async Task<NguoiDungInfoDto?> LayHoSoCaNhan(int maNguoiDung)
        {
            return await _repository.LayHoSoCaNhan(maNguoiDung);
        }

        // 3. Thực thi hàm Cập nhật hồ sơ
        public async Task<bool> CapNhatHoSo(int maNguoiDung, CapNhatHoSoDto duLieu)
        {
            return await _repository.LuuCapNhatHoSo(maNguoiDung, duLieu);
        }
    }
}