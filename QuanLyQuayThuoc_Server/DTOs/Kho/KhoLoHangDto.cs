using System.Collections.Generic;
using QuanLyQuayThuoc.DTOs.Kho;

namespace QuanLyQuayThuoc.DTOs.Kho
{
    public class LoHangDto
    {
        public int MaLo { get; set; }
        public string SoLo { get; set; }
        public string HanSuDung { get; set; }
        public string NgaySanXuat { get; set; }
        public int SoLuongTon { get; set; }
        public decimal GiaNhap { get; set; }
        public string TenThuoc { get; set; }
        public string MaVach { get; set; }

        // Trạng thái cảnh báo: 
        // 0: Bình thường, 1: Sắp hết hạn (vàng), 2: Đã hết hạn (đỏ)
        public int MucDoCanhBao { get; set; }
    }

    public class KhoLoHangResponseDto
    {
        public List<LoHangDto> Items { get; set; }
        public ThongKeKhoDto ThongKe { get; set; }
    }
    public class SuaLoHangDto
    {
        public string SoLo { get; set; }
        public string HanSuDung { get; set; }
        public int SoLuongTon { get; set; }
        public decimal GiaNhap { get; set; }
    }
}