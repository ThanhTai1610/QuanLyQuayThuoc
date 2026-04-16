namespace QuanLyQuayThuoc.DTOs.BaoCao
{
    public class ThongKeDoanhThuDto
    {
        // Nhãn trục X (Ví dụ: "T1", "T2" hoặc "15/04", "16/04")
        public List<string> Nhan { get; set; } = new List<string>();

        // Dữ liệu cột Doanh thu
        public List<decimal> DoanhThu { get; set; } = new List<decimal>();

        // Dữ liệu cột Lợi nhuận
        public List<decimal> LoiNhuan { get; set; } = new List<decimal>();

        // Có thể thêm tổng số đơn hàng nếu Tài muốn hiện thêm thông tin phụ
        public List<int> SoDonHang { get; set; } = new List<int>();
    }
    public class DoanhThuLoiNhuanDto // Đảm bảo tên này khớp với Controller
    {
        public List<string> Nhan { get; set; } = new();
        public List<decimal> DoanhThu { get; set; } = new();
        public List<decimal> LoiNhuan { get; set; } = new();
    }
}