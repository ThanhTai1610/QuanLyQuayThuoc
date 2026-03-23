using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyQuayThuoc.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChuDeSucKhoe",
                columns: table => new
                {
                    MaChuDe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenChuDe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TieuDePhu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NoiDungGiaiPhap = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChuDeSuc__3585451132964912", x => x.MaChuDe);
                });

            migrationBuilder.CreateTable(
                name: "DanhMuc",
                columns: table => new
                {
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DanhMuc__B3750887319BDF1A", x => x.MaDanhMuc);
                });

            migrationBuilder.CreateTable(
                name: "VaiTro",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__VaiTro__C24C41CF09F311D4", x => x.MaVaiTro);
                });

            migrationBuilder.CreateTable(
                name: "Thuoc",
                columns: table => new
                {
                    MaThuoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenThuoc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaDanhMuc = table.Column<int>(type: "int", nullable: true),
                    SoDangKy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    QuyCach = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DangBaoChe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NhaSanXuat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NuocSanXuat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HanSuDungThang = table.Column<int>(type: "int", nullable: true),
                    LaThuocKeDon = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    HinhAnhChinh = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    MoTaNgan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThanhPhan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CongDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CachDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoiTuongSuDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TacDungPhu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LuuY = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaoQuan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChongChiDinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LuotXem = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    NgayTao = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Thuoc__4BB1F6204ED9346A", x => x.MaThuoc);
                    table.ForeignKey(
                        name: "FK__Thuoc__MaDanhMuc__48CFD27E",
                        column: x => x.MaDanhMuc,
                        principalTable: "DanhMuc",
                        principalColumn: "MaDanhMuc");
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    SoDienThoai = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    MatKhau = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    AnhDaiDien = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    MaVaiTro = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Hoạt động"),
                    NgayTao = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NguoiDun__C539D762AE933BA2", x => x.MaNguoiDung);
                    table.ForeignKey(
                        name: "FK__NguoiDung__MaVai__3B75D760",
                        column: x => x.MaVaiTro,
                        principalTable: "VaiTro",
                        principalColumn: "MaVaiTro");
                });

            migrationBuilder.CreateTable(
                name: "DonViTinh",
                columns: table => new
                {
                    MaDVT = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThuoc = table.Column<int>(type: "int", nullable: true),
                    TenDonVi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    GiaTriQuyDoi = table.Column<int>(type: "int", nullable: true),
                    GiaBan = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LaDonViCoBan = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DonViTin__3D895AFE1BB90994", x => x.MaDVT);
                    table.ForeignKey(
                        name: "FK__DonViTinh__MaThu__5535A963",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc");
                });

            migrationBuilder.CreateTable(
                name: "HinhAnhThuoc",
                columns: table => new
                {
                    MaHinh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThuoc = table.Column<int>(type: "int", nullable: true),
                    DuongDan = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HinhAnhT__13EE10841389A792", x => x.MaHinh);
                    table.ForeignKey(
                        name: "FK__HinhAnhTh__MaThu__52593CB8",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc");
                });

            migrationBuilder.CreateTable(
                name: "LoHang",
                columns: table => new
                {
                    MaLo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThuoc = table.Column<int>(type: "int", nullable: true),
                    SoLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NgaySanXuat = table.Column<DateOnly>(type: "date", nullable: true),
                    HanSuDung = table.Column<DateOnly>(type: "date", nullable: false),
                    GiaNhap = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SoLuongTon = table.Column<int>(type: "int", nullable: false),
                    MaNhaCungCap = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LoHang__2725C75608560B53", x => x.MaLo);
                    table.ForeignKey(
                        name: "FK__LoHang__MaThuoc__5812160E",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc");
                });

            migrationBuilder.CreateTable(
                name: "Thuoc_ChuDe",
                columns: table => new
                {
                    MaThuoc = table.Column<int>(type: "int", nullable: false),
                    MaChuDe = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Thuoc_Ch__48E9A271E8067EEA", x => new { x.MaThuoc, x.MaChuDe });
                    table.ForeignKey(
                        name: "FK__Thuoc_Chu__MaChu__4F7CD00D",
                        column: x => x.MaChuDe,
                        principalTable: "ChuDeSucKhoe",
                        principalColumn: "MaChuDe");
                    table.ForeignKey(
                        name: "FK__Thuoc_Chu__MaThu__4E88ABD4",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc");
                });

            migrationBuilder.CreateTable(
                name: "DonHang",
                columns: table => new
                {
                    MaDonHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: true),
                    MaNhanVien = table.Column<int>(type: "int", nullable: true),
                    NgayDat = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PhuongThucThanhToan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AnhDonThuoc = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    DiaChiGiaoHang = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SoDienThoaiNhan = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DonHang__129584AD23635ABC", x => x.MaDonHang);
                    table.ForeignKey(
                        name: "FK__DonHang__MaKhach__6383C8BA",
                        column: x => x.MaKhachHang,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK__DonHang__MaNhanV__6477ECF3",
                        column: x => x.MaNhanVien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "LichSuChatbot",
                columns: table => new
                {
                    MaChat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: true),
                    CauHoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CauTraLoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGian = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LichSuCh__1B56CB6B505800A4", x => x.MaChat);
                    table.ForeignKey(
                        name: "FK__LichSuCha__MaKha__71D1E811",
                        column: x => x.MaKhachHang,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "PhieuKiemKe",
                columns: table => new
                {
                    MaPhieu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayKiem = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Hoàn tất")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PhieuKie__2660BFE0E83E705F", x => x.MaPhieu);
                    table.ForeignKey(
                        name: "FK__PhieuKiem__MaNha__5BE2A6F2",
                        column: x => x.MaNhanVien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "SoDiaChi",
                columns: table => new
                {
                    MaDiaChi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: true),
                    HoTenNguoiNhan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoaiNhan = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    TinhThanh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QuanHuyen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhuongXa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaChiChiTiet = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LaMacDinh = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    LoaiDiaChi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SoDiaChi__EB61213E0D918E59", x => x.MaDiaChi);
                    table.ForeignKey(
                        name: "FK__SoDiaChi__MaNguo__403A8C7D",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "GioHang",
                columns: table => new
                {
                    MaGioHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: true),
                    MaThuoc = table.Column<int>(type: "int", nullable: true),
                    MaDVT = table.Column<int>(type: "int", nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GioHang__F5001DA37BAF0A6A", x => x.MaGioHang);
                    table.ForeignKey(
                        name: "FK__GioHang__MaDVT__6EF57B66",
                        column: x => x.MaDVT,
                        principalTable: "DonViTinh",
                        principalColumn: "MaDVT");
                    table.ForeignKey(
                        name: "FK__GioHang__MaKhach__6D0D32F4",
                        column: x => x.MaKhachHang,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK__GioHang__MaThuoc__6E01572D",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonHang",
                columns: table => new
                {
                    MaChiTiet = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonHang = table.Column<int>(type: "int", nullable: true),
                    MaLo = table.Column<int>(type: "int", nullable: true),
                    MaDVT = table.Column<int>(type: "int", nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: true),
                    GiaBanTaiThoiDiem = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietD__CDF0A114511AB443", x => x.MaChiTiet);
                    table.ForeignKey(
                        name: "FK__ChiTietDo__MaDVT__6A30C649",
                        column: x => x.MaDVT,
                        principalTable: "DonViTinh",
                        principalColumn: "MaDVT");
                    table.ForeignKey(
                        name: "FK__ChiTietDo__MaDon__68487DD7",
                        column: x => x.MaDonHang,
                        principalTable: "DonHang",
                        principalColumn: "MaDonHang");
                    table.ForeignKey(
                        name: "FK__ChiTietDon__MaLo__693CA210",
                        column: x => x.MaLo,
                        principalTable: "LoHang",
                        principalColumn: "MaLo");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietKiemKe",
                columns: table => new
                {
                    MaPhieu = table.Column<int>(type: "int", nullable: false),
                    MaLo = table.Column<int>(type: "int", nullable: false),
                    SoLuongHeThong = table.Column<int>(type: "int", nullable: true),
                    SoLuongThucTe = table.Column<int>(type: "int", nullable: true),
                    LyDoLech = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietK__5412E395EA49E629", x => new { x.MaPhieu, x.MaLo });
                    table.ForeignKey(
                        name: "FK__ChiTietKi__MaPhi__5FB337D6",
                        column: x => x.MaPhieu,
                        principalTable: "PhieuKiemKe",
                        principalColumn: "MaPhieu");
                    table.ForeignKey(
                        name: "FK__ChiTietKie__MaLo__60A75C0F",
                        column: x => x.MaLo,
                        principalTable: "LoHang",
                        principalColumn: "MaLo");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_MaDonHang",
                table: "ChiTietDonHang",
                column: "MaDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_MaDVT",
                table: "ChiTietDonHang",
                column: "MaDVT");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_MaLo",
                table: "ChiTietDonHang",
                column: "MaLo");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietKiemKe_MaLo",
                table: "ChiTietKiemKe",
                column: "MaLo");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaKhachHang",
                table: "DonHang",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaNhanVien",
                table: "DonHang",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_DonViTinh_MaThuoc",
                table: "DonViTinh",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_GioHang_MaDVT",
                table: "GioHang",
                column: "MaDVT");

            migrationBuilder.CreateIndex(
                name: "IX_GioHang_MaKhachHang",
                table: "GioHang",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_GioHang_MaThuoc",
                table: "GioHang",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_HinhAnhThuoc_MaThuoc",
                table: "HinhAnhThuoc",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuChatbot_MaKhachHang",
                table: "LichSuChatbot",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_LoHang_MaThuoc",
                table: "LoHang",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_MaVaiTro",
                table: "NguoiDung",
                column: "MaVaiTro");

            migrationBuilder.CreateIndex(
                name: "UQ__NguoiDun__0389B7BD927BD88F",
                table: "NguoiDung",
                column: "SoDienThoai",
                unique: true,
                filter: "[SoDienThoai] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__NguoiDun__A9D1053484A3F442",
                table: "NguoiDung",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuKiemKe_MaNhanVien",
                table: "PhieuKiemKe",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_SoDiaChi_MaNguoiDung",
                table: "SoDiaChi",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_Thuoc_MaDanhMuc",
                table: "Thuoc",
                column: "MaDanhMuc");

            migrationBuilder.CreateIndex(
                name: "IX_Thuoc_ChuDe_MaChuDe",
                table: "Thuoc_ChuDe",
                column: "MaChuDe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietDonHang");

            migrationBuilder.DropTable(
                name: "ChiTietKiemKe");

            migrationBuilder.DropTable(
                name: "GioHang");

            migrationBuilder.DropTable(
                name: "HinhAnhThuoc");

            migrationBuilder.DropTable(
                name: "LichSuChatbot");

            migrationBuilder.DropTable(
                name: "SoDiaChi");

            migrationBuilder.DropTable(
                name: "Thuoc_ChuDe");

            migrationBuilder.DropTable(
                name: "DonHang");

            migrationBuilder.DropTable(
                name: "PhieuKiemKe");

            migrationBuilder.DropTable(
                name: "LoHang");

            migrationBuilder.DropTable(
                name: "DonViTinh");

            migrationBuilder.DropTable(
                name: "ChuDeSucKhoe");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "Thuoc");

            migrationBuilder.DropTable(
                name: "VaiTro");

            migrationBuilder.DropTable(
                name: "DanhMuc");
        }
    }
}
