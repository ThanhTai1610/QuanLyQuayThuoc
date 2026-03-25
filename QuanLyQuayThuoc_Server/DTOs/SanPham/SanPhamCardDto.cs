namespace QuanLyQuayThuoc.DTOs.SanPham
{
    public class SanPhamCardDto
    {
        public int Id { get; set; }
        public string TenThuoc { get; set; } = string.Empty;
        public string? HinhAnh { get; set; }
        public string? TenDanhMuc { get; set; }

        // Giá bán hiện tại (lấy từ đơn vị tính cơ bản)
        public decimal GiaBan { get; set; }

        // Giá cũ để hiển thị gạch ngang (nếu có)
        public decimal? GiaCu { get; set; }

        // Phần trăm giảm giá (ví dụ: 10)
        public int? PhanTramGiamGia { get; set; }

        // Đánh giá trung bình (Tạm thời để cứng hoặc tính từ bảng đánh giá sau này)
        public double DiemDanhGia { get; set; } = 5.0;
        public int LuotDanhGia { get; set; } = 0;
    }
}