using QuanLyQuayThuoc.DTOs.Kho;
using QuanLyQuayThuoc.DTOs.SanPham;
using QuanLyQuayThuoc.Repositories.Interfaces;
using QuanLyQuayThuoc.Services.Interfaces;

namespace QuanLyQuayThuoc.Services.Implementation
{
    public class KhoService : IKhoService
    {
        private readonly IKhoRepository _khoRepository;

        public KhoService(IKhoRepository khoRepository)
        {
            _khoRepository = khoRepository;
        }

        public async Task<IEnumerable<DanhMucDto>> GetDanhMucAsync()
        {
            return await _khoRepository.GetDanhMucAsync();
        }
        public async Task<KhoTongQuanResponseDto> GetTongQuanAsync(int? maDanhMuc, string search)
        {
            // Có thể thêm logic kiểm tra quyền ở đây nếu cần
            return await _khoRepository.GetTongQuanAsync(maDanhMuc, search);
        }

        public async Task<KhoLoHangResponseDto> GetLoHangAsync(string search, string thang, string loai)
        {
            return await _khoRepository.GetLoHangAsync(search, thang, loai);
        }

        public async Task<bool> NhapKhoAsync(PhieuNhapKhoDto phieuNhap)
        {
            if (phieuNhap == null || phieuNhap.ChiTiet == null || phieuNhap.ChiTiet.Count == 0)
            {
                return false;
            }

            return await _khoRepository.NhapKhoAsync(phieuNhap);
        }
        public async Task<bool> SuaLoHangAsync(int maLo, SuaLoHangDto dto)
        {
            return await _khoRepository.SuaLoHangAsync(maLo, dto);
        }
        public async Task<IEnumerable<MaVachDto>> GetMaVachTheoThuocAsync(int maThuoc)
        {
            return await _khoRepository.GetMaVachTheoThuocAsync(maThuoc);
        }
        public async Task<bool> ThemThuocMoiVaNhapKhoAsync(ThemThuocMoiVaNhapKhoDto dto)
        {
            return await _khoRepository.ThemThuocMoiVaNhapKhoAsync(dto);
        }
    }
}