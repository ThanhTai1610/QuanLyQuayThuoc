// DTOs/Kho/ThemThuocMoiVaNhapKhoDto.cs
namespace QuanLyQuayThuoc.DTOs.Kho
{
    public class ThemThuocMoiVaNhapKhoDto
    {
        public string? NhaCungCap { get; set; }
        public string? NguoiNhap { get; set; }
        public DateTime NgayNhap { get; set; }
        public string? GhiChu { get; set; }

        // Thông tin thuốc
        public string TenThuoc { get; set; } = null!;
        public int? MaDanhMuc { get; set; }
        public string? SoDangKy { get; set; }
        public string? QuyCach { get; set; }
        public string? DangBaoChe { get; set; }
        public string? NhaSanXuat { get; set; }
        public string? NuocSanXuat { get; set; }
        public string? ThanhPhan { get; set; }
        public string? MoTaNgan { get; set; }
        public bool? LaThuocKeDon { get; set; }
        public string? HinhAnh { get; set; }

        // Thông tin Lô (Duy nhất cho thuốc mới khi tạo)
        public string SoLo { get; set; } = null!;
        public DateTime HanSuDung { get; set; }
        public decimal GiaNhap { get; set; }       // Giá của "Đơn vị nhập"
        public int SoLuong { get; set; }           // Số lượng của "Đơn vị nhập"
        public string TenDonViNhap { get; set; } = null!; // Tên đơn vị dùng để nhập (Hộp/Vỉ...)

        public List<DonViTinhVaLoDto> ChiTiet { get; set; } = new();
    }

    public class DonViTinhVaLoDto
    {
        public string TenDonVi { get; set; } = null!;    // "Hộp", "Vỉ", "Viên"...
        public decimal GiaBan { get; set; }
        public int GiaTriQuyDoi { get; set; }             // Quy đổi ra đơn vị nhỏ nhất, VD: 1 Hộp = 10 vỉ → 10
        public bool LaDonViCoBan { get; set; }            // true = đơn vị nhỏ nhất
        
        // Các trường barcode/ảnh được server sinh ra dựa trên đơn vị tính này
        public string? MaVach { get; set; }
        public string? HinhAnhMaVach { get; set; }
    }
}