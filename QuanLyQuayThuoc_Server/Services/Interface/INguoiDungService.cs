using QuanLyQuayThuoc.DTOs.NguoiDung;

namespace QuanLyQuayThuoc.Services.Interfaces
{
    // Đổi 'class' thành 'interface' và thêm chữ 'I' ở đầu tên
    public interface INguoiDungService
    {
        // 1. Chức năng Đăng nhập & Phân quyền
        Task<PhanQuyenDto?> DangNhap(DangNhapDto duLieu);

        // 2. Chức năng lấy Hồ sơ cá nhân (Dùng cho trang /ho-so ở Vue)
        Task<NguoiDungInfoDto?> LayHoSoCaNhan(int maNguoiDung);

        // 3. Chức năng cập nhật Hồ sơ
        Task<bool> CapNhatHoSo(int maNguoiDung, CapNhatHoSoDto duLieu);
    }
}