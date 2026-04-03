namespace QuanLyQuayThuoc.Models.Momo
{
    public class MomoExecuteResponseModel
    {
        public string OrderId { get; set; }
        public string Amount { get; set; }
        public string OrderInfo { get; set; }
        public string ResultCode { get; set; } // Mã trạng thái (0 là thành công)
        public string Message { get; set; }    // <--- Thêm dòng này
        public string LocalMessage { get; set; } // (Tùy chọn) Thông báo tiếng Việt từ MoMo
    }
}