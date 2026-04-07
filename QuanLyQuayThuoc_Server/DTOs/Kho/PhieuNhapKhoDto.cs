using System;
using System.Collections.Generic;

namespace QuanLyQuayThuoc.DTOs.Kho
{
    public class PhieuNhapKhoDto
    {
        public string NhaCungCap { get; set; }
        public string NguoiNhap { get; set; }
        public DateTime NgayNhap { get; set; }
        public string? GhiChu { get; set; }
        public List<ChiTietNhapDto> ChiTiet { get; set; }
    }

    public class ChiTietNhapDto
    {
        public int MaThuoc { get; set; }
        public string? TenThuoc { get; set; }
        public string SoLo { get; set; }
        public DateTime HanSuDung { get; set; }
        public decimal GiaNhap { get; set; }
        public int SoLuong { get; set; }
        public string TenDonVi { get; set; } // "Hộp", "Vỉ", "Viên"
        public string? MaVach { get; set; }
        public string? HinhAnhMaVach { get; set; }
    }
}