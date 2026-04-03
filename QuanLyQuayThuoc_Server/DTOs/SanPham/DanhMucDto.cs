namespace QuanLyQuayThuoc.DTOs.SanPham
{
    public class DanhMucDTO
    {
        public int MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }
        public int? MaDanhMucCha { get; set; }
        public string Icon { get; set; }
        public string MoTa { get; set; }
        public string Slug { get; set; }
        public string TrangThai { get; set; }
        public int SoSanPham { get; set; } // Tài tính toán count từ bảng Thuoc

        // Quan trọng nhất để hiện Tree View
        public List<DanhMucDTO> Children { get; set; } = new List<DanhMucDTO>();
    }
}
