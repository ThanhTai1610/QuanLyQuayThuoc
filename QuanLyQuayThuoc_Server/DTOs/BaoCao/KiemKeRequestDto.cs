namespace QuanLyQuayThuoc.DTOs.BaoCao
{
    public class KiemKeRequestDto
    {
        public string? GhiChu { get; set; }
        public List<KiemKeItemDto> ChiTiet { get; set; } = new();
    }

    public class KiemKeItemDto
    {
        public int MaLo { get; set; }
        public int SoLuongHeThong { get; set; }
        public int SoLuongThucTe { get; set; }
        public string? LyDo { get; set; }
    }
}