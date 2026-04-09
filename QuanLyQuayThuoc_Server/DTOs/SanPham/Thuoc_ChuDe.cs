using QuanLyQuayThuoc.Models;

namespace QuanLyQuayThuoc.DTOs.SanPham
{
    public class ThuocChuDe
    {
        public int MaThuoc { get; set; }
        public virtual Thuoc Thuoc { get; set; }

        public int MaChuDe { get; set; }
        public virtual ChuDeSucKhoe ChuDe { get; set; }
    }
}
