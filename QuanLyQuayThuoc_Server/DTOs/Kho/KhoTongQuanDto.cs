public class KhoTongQuanItemDto
{
    public int MaThuoc { get; set; }
    public string TenThuoc { get; set; }
    public string TenDanhMuc { get; set; }
    public int TongTon { get; set; }
    public string TrangThai { get; set; } // "Còn hàng", "Hết hàng", "Sắp hết hàng"
}

public class ThongKeKhoDto
{
    public decimal TongGiaTri { get; set; }
    public int SoLoHetHan { get; set; }      // Cảnh báo: Count các lô có HanSuDung < Now
    public int SoLoSapHetHan { get; set; }   // Cảnh báo: Count các lô có HanSuDung trong vòng 6 tháng tới
    public int SoMatHangSapHetTon { get; set; } // Cảnh báo: Thuốc có tổng tồn < 50
}

public class KhoTongQuanResponseDto
{
    public List<KhoTongQuanItemDto> Items { get; set; }
    public ThongKeKhoDto ThongKe { get; set; }
}