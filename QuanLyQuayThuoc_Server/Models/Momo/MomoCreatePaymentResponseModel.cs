namespace QuanLyQuayThuoc.Models.Momo
{
    public class MomoCreatePaymentResponseModel
    {
        public string RequestId { get; set; }
        public int ResultCode { get; set; }
        public string Message { get; set; }
        public string PayUrl { get; set; }
    }
}