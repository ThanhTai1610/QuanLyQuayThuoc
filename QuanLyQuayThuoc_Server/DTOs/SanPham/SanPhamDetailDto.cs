using System.Collections.Generic;

namespace QuanLyQuayThuoc.DTOs.SanPham
{
    public class SanPhamDetailDto
    {
        // --- 1. THÔNG TIN CHUNG ---
        public int MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public int? MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; } // Join thêm để hiển thị ở Breadcrumb
        public string HinhAnhChinh { get; set; }


        // --- 2. THÔNG TIN KỸ THUẬT (Dưới tên thuốc) ---
        public string SoDangKy { get; set; }
        public string QuyCach { get; set; }
        public string DangBaoChe { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public int? HanSuDungThang { get; set; }
        public bool LaThuocKeDon { get; set; }


        // --- 3. NỘI DUNG CHI TIẾT (Chia theo 3 Tabs ở giao diện) ---

        // Tab 1: Đặc điểm nổi bật
        public string MoTaNgan { get; set; }

        // Tab 2: Thông tin chi tiết
        public string ThanhPhan { get; set; }
        public string CongDung { get; set; }
        public string CachDung { get; set; }
        public string DoiTuongSuDung { get; set; }

        // Tab 3: Lưu ý & Bảo quản
        public string ChongChiDinh { get; set; }
        public string TacDungPhu { get; set; }
        public string LuuY { get; set; }
        public string BaoQuan { get; set; }


        // --- 4. DỮ LIỆU LIÊN QUAN (1-N) ---

        // Đơn vị tính: Để Render dropdown bảng giá (Hộp/Vỉ/Viên)
        public List<DonViTinhDto> DonViTinhs { get; set; } = new List<DonViTinhDto>();

        // Album ảnh phụ: Đưa vào slider thumbnail của Vue.js
        public List<string> HinhAnhThuocs { get; set; } = new List<string>();

        // Lô hàng: Để Backend / Frontend tính tổng số lượng tồn thực tế
        public List<LoHangDto> LoHangs { get; set; } = new List<LoHangDto>();
    }


    // --- CÁC DTO PHỤ ĐI KÈM TRONG TRANG CHI TIẾT ---

    public class DonViTinhDto
    {
        public int MaDvt { get; set; }
        public string TenDonVi { get; set; }
        public decimal GiaBan { get; set; }
        public int? GiaTriQuyDoi { get; set; }
        public bool LaDonViCoBan { get; set; }
    }

    public class LoHangDto
    {
        public int MaLo { get; set; }
        public string SoLo { get; set; }
        public int SoLuongTon { get; set; }
        public System.DateTime HanSuDung { get; set; }
    }
}