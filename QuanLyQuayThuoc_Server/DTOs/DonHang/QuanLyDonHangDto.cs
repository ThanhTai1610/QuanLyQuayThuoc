using System.Collections.Generic;

namespace QuanLyQuayThuoc.DTOs.DonHang
{
    // DTO chứa toàn bộ thông tin chi tiết đơn hàng để Admin xem
    public class DonHangChiTietAdminDto
    {
        public int MaDonHang { get; set; }
        public string NgayDat { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoaiNhan { get; set; }
        public string DiaChiGiaoHang { get; set; }
        public string GhiChu { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
        public bool LaThuocKeDon { get; set; }
        public string AnhDonThuoc { get; set; }

        // Danh sách các sản phẩm trong đơn (Chỉ xem, không chọn lô)
        public List<SanPhamTrongDonDto> DanhSachSanPham { get; set; }
    }
}