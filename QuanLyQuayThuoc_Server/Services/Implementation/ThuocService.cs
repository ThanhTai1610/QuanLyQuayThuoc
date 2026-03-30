using QuanLyQuayThuoc.DTOs.SanPham;
using QuanLyQuayThuoc.Repository;

namespace QuanLyQuayThuoc.Services
{
    public class ThuocService : IThuocService
    {
        private readonly IThuocRepository _thuocRepository;

        public ThuocService(IThuocRepository thuocRepository)
        {
            _thuocRepository = thuocRepository;
        }

        public async Task<SanPhamDetailDto> GetDetailAsync(int id)
        {
            var thuoc = await _thuocRepository.GetByIdAsync(id);
            if (thuoc == null) return null;

            // Logic Business: Tăng lượt xem
            thuoc.LuotXem = (thuoc.LuotXem ?? 0) + 1;
            await _thuocRepository.SaveChangesAsync();

            // Mapping Entity sang DTO
            return new SanPhamDetailDto
            {
                MaThuoc = thuoc.MaThuoc,
                TenThuoc = thuoc.TenThuoc,
                MaDanhMuc = thuoc.MaDanhMuc,
                SoDangKy = thuoc.SoDangKy,
                QuyCach = thuoc.QuyCach,
                DangBaoChe = thuoc.DangBaoChe,
                NhaSanXuat = thuoc.NhaSanXuat,
                NuocSanXuat = thuoc.NuocSanXuat,
                HanSuDungThang = thuoc.HanSuDungThang,
                LaThuocKeDon = thuoc.LaThuocKeDon ?? false,
                HinhAnhChinh = thuoc.HinhAnhChinh,
                MoTaNgan = thuoc.MoTaNgan,
                ThanhPhan = thuoc.ThanhPhan,
                CongDung = thuoc.CongDung,
                CachDung = thuoc.CachDung,
                DoiTuongSuDung = thuoc.DoiTuongSuDung,
                ChongChiDinh = thuoc.ChongChiDinh,
                TacDungPhu = thuoc.TacDungPhu,
                LuuY = thuoc.LuuY,
                BaoQuan = thuoc.BaoQuan,

                DonViTinhs = thuoc.DonViTinhs.Select(d => new DonViTinhDto
                {
                    MaDvt = d.MaDvt,
                    TenDonVi = d.TenDonVi,
                    GiaBan = d.GiaBan ?? 0,
                    GiaTriQuyDoi = d.GiaTriQuyDoi,
                    LaDonViCoBan = d.LaDonViCoBan ?? false
                }).ToList(),

                HinhAnhThuocs = thuoc.HinhAnhThuocs.Select(h => h.DuongDan).ToList(),

                LoHangs = thuoc.LoHangs.Select(l => new LoHangDto
                {
                    MaLo = l.MaLo,
                    SoLo = l.SoLo,
                    SoLuongTon = l.SoLuongTon,
                    HanSuDung = l.HanSuDung.ToDateTime(TimeOnly.MinValue) 
                }).ToList()
            };
        }

        public async Task<IEnumerable<SanPhamCardDto>> GetRelatedProductsAsync(int maDanhMuc, int currentProductId)
        {
            var relatedEntities = await _thuocRepository.GetRelatedAsync(maDanhMuc, currentProductId);

            return relatedEntities.Select(t => new SanPhamCardDto
            {
                // ⬇️ Đổi từ MaThuoc thành Id cho khớp với DTO của bạn
                Id = t.MaThuoc,
                TenThuoc = t.TenThuoc,

                // ⬇️ Đổi từ HinhAnhChinh thành HinhAnh cho khớp với DTO của bạn
                HinhAnh = t.HinhAnhChinh,

                // Có thể bổ sung lấy giá bán cơ bản (nếu Repository của bạn đã include DonViTinhs)
                GiaBan = t.DonViTinhs?.FirstOrDefault(d => d.LaDonViCoBan == true)?.GiaBan ?? 0
            }).ToList();
        }
    }
}