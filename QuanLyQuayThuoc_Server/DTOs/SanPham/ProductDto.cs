namespace QuanLyQuayThuoc.DTOs.SanPham
{


        public class ThuocHienThiDto
        {
            public int MaThuoc { get; set; }
            public string TenThuoc { get; set; }
            public string HinhAnhChinh { get; set; }
            public string QuyCach { get; set; }
            public string NuocSanXuat { get; set; }
            public bool LaThuocKeDon { get; set; }
            public decimal GiaBan { get; set; } // Lấy giá từ đơn vị cơ bản
            public string TenDonVi { get; set; }
            public int MaDVT { get; set; }
        }
    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; }
        public int Total { get; set; }
    }
}
