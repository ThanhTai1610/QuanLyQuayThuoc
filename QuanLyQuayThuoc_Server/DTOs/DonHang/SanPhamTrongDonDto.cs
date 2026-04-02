namespace QuanLyQuayThuoc.DTOs.DonHang
{
    // DTO hiển thị từng dòng sản phẩm trong đơn hàng
    public class SanPhamTrongDonDto
    {
        public int MaChiTiet { get; set; }
        public string TenThuoc { get; set; }
        public string TenDonVi { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaBan { get; set; }
        public decimal ThanhTien => SoLuong * GiaBan;
    }
}