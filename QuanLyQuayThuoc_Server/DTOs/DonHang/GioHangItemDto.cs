using System.Collections.Generic;

namespace QuanLyQuayThuoc.DTOs.DonHang
{
    public class GioHangItemDto
    {
        public int MaGioHang { get; set; }
        public int MaThuoc { get; set; }
        public int MaLo { get; set; }
        public string TenThuoc { get; set; } = "";
        public string HinhAnhChinh { get; set; } = "";
        public string MoTaNgan { get; set; } = "";
        public int MaDVT { get; set; }
        public string TenDonVi { get; set; } = "";
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }
        public List<DonViTinhTrongGioHangDto> DanhSachDVT { get; set; } = new List<DonViTinhTrongGioHangDto>();
    }

    public class DonViTinhTrongGioHangDto
    {
        public int MaDVT { get; set; }
        public string TenDonVi { get; set; } = "";
        public decimal GiaBan { get; set; }
    }

    // Class này dùng để nhận dữ liệu cập nhật từ Frontend
    public class CapNhatGioHangDto
    {
        public int MaGioHang { get; set; }
        public int SoLuong { get; set; }
        public int MaDVT { get; set; }
    }
}