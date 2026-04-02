    namespace QuanLyQuayThuoc.Models.Momo
    {
        public class OrderInfoModel
        {
            public string OrderId { get; set; }
            public string OrderInfo { get; set; }
            public long Amount { get; set; }
            public string UserType { get; set; } // Ví dụ: "KhachHang"
        }
    }