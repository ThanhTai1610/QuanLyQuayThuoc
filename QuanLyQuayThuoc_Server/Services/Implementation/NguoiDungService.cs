using Google.Apis.Auth;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.DTOs.NguoiDung;
using QuanLyQuayThuoc.Helpers;
using QuanLyQuayThuoc.Models;
using QuanLyQuayThuoc.Repositories.Interfaces;
using QuanLyQuayThuoc.Services.Interfaces;

namespace QuanLyQuayThuoc.Services.Implementation
{
    public class NguoiDungService : INguoiDungService
    {
        private readonly INguoiDungRepository _repository;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public NguoiDungService(
            INguoiDungRepository repository,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _repository = repository;
            _context = context;
            _configuration = configuration;
        }

        public async Task<PhanQuyenDto?> DangNhap(DangNhapDto duLieu)
        {
            var user = await _repository.GetByEmailAsync(duLieu.Email);

            if (user == null || !PasswordHasher.VerifyPassword(duLieu.MatKhau, user.MatKhau))
                return null;

            return new PhanQuyenDto
            {
                MaNguoiDung = user.MaNguoiDung,
                HoTen = user.HoTen,
                MaVaiTro = user.MaVaiTro
            };
        }

        public async Task<PhanQuyenDto?> DangNhapBangGoogle(DangNhapGoogleDto duLieu)
        {
            if (string.IsNullOrWhiteSpace(duLieu.IdToken))
                return null;

            var clientId = _configuration["Authentication:Google:ClientId"];
            if (string.IsNullOrWhiteSpace(clientId))
                throw new InvalidOperationException("Server chưa cấu hình Authentication:Google:ClientId.");

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(
                    duLieu.IdToken,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { clientId }
                    });
            }
            catch (InvalidJwtException)
            {
                return null;
            }

            var email = payload.Email?.Trim();
            if (string.IsNullOrWhiteSpace(email) || payload.EmailVerified != true)
                return null;

            var user = await _repository.GetByEmailAsync(email);

            if (user == null)
            {
                user = new NguoiDung
                {
                    HoTen = !string.IsNullOrWhiteSpace(payload.Name) ? payload.Name : email.Split('@')[0],
                    Email = email,
                    AnhDaiDien = payload.Picture,
                    MatKhau = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString("N")),
                    MaVaiTro = 3,
                    TrangThai = "Hoạt động",
                    NgayTao = DateTime.Now
                };

                _context.NguoiDungs.Add(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                var hasChanges = false;

                if (string.IsNullOrWhiteSpace(user.HoTen) && !string.IsNullOrWhiteSpace(payload.Name))
                {
                    user.HoTen = payload.Name;
                    hasChanges = true;
                }

                if (!string.IsNullOrWhiteSpace(payload.Picture) && user.AnhDaiDien != payload.Picture)
                {
                    user.AnhDaiDien = payload.Picture;
                    hasChanges = true;
                }

                if (hasChanges)
                    await _context.SaveChangesAsync();
            }

            return new PhanQuyenDto
            {
                MaNguoiDung = user.MaNguoiDung,
                HoTen = user.HoTen,
                MaVaiTro = user.MaVaiTro
            };
        }

        public async Task<NguoiDungInfoDto?> LayHoSoCaNhan(int maNguoiDung)
        {
            return await _repository.LayHoSoCaNhan(maNguoiDung);
        }

        public async Task<bool> CapNhatHoSo(int maNguoiDung, CapNhatHoSoDto duLieu)
        {
            return await _repository.LuuCapNhatHoSo(maNguoiDung, duLieu);
        }
    }
}
