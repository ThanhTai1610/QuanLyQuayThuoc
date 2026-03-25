using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyQuayThuoc.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__NguoiDung__MaVai__3B75D760",
                table: "NguoiDung");

            migrationBuilder.AlterColumn<int>(
                name: "MaVaiTro",
                table: "NguoiDung",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HanOtp",
                table: "NguoiDung",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaOtp",
                table: "NguoiDung",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK__NguoiDung__MaVai__3B75D760",
                table: "NguoiDung",
                column: "MaVaiTro",
                principalTable: "VaiTro",
                principalColumn: "MaVaiTro",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__NguoiDung__MaVai__3B75D760",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "HanOtp",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "MaOtp",
                table: "NguoiDung");

            migrationBuilder.AlterColumn<int>(
                name: "MaVaiTro",
                table: "NguoiDung",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK__NguoiDung__MaVai__3B75D760",
                table: "NguoiDung",
                column: "MaVaiTro",
                principalTable: "VaiTro",
                principalColumn: "MaVaiTro");
        }
    }
}
