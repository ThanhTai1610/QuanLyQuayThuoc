using System.ComponentModel.DataAnnotations;

namespace QuanLyQuayThuoc.DTOs.SanPham
{
    public class ChuDeSucKhoe
    {
        [Key]
        public int MaChuDe { get; set; }
        public string TenChuDe { get; set; }      // Ví dụ: Bệnh hô hấp, Cảm cúm
        public string? TieuDePhu { get; set; }    // Ví dụ: Giải pháp bảo vệ phổi
        public string? NoiDungGiaiPhap { get; set; }
        public string? HinhAnh { get; set; }       // Ảnh nền cho Card giới thiệu
        public int TrangThai { get; set; }         // 1: Hiện, 0: Ẩn
    }
}
