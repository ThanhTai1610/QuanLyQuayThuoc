namespace QuanLyQuayThuoc.DTOs.NguoiDung
{
    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
        public string? TenThuoc { get; set; }
    }

    public class ChatResponse
    {
        public string Reply { get; set; } = string.Empty;
    }
}