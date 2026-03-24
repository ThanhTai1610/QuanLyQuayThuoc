using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.Repositories;
using QuanLyQuayThuoc.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Giải quyết lỗi JwtBearerDefaults
using Microsoft.IdentityModel.Tokens;               // Giải quyết lỗi TokenValidationParameters, SymmetricSecurityKey
using System.Text;                                  // Giải quyết lỗi Encoding

var builder = WebApplication.CreateBuilder(args);

// --- 1. KẾT NỐI DATABASE ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. CẤU HÌNH CORS ---
builder.Services.AddCors(options => {
    options.AddPolicy("AllowVueApp", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// --- 3. ĐĂNG KÝ REPOSITORY & SERVICES ---
builder.Services.AddScoped<INguoiDungRepository, NguoiDungRepository>();
// Tài thêm các Repo khác ở đây...

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CẤU HÌNH JWT (Tài đảm bảo đã có phần này để [Authorize] hoạt động) ---
// builder.Services.AddAuthentication(...)...
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, // Chỉnh thành true nếu bạn có quy định Issuer
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Chuoi_Secret_Key_Cua_Tai_Phai_Du_Dai_32_Ky_Tu"))
    };
});
var app = builder.Build();

// --- 4. CẤU HÌNH PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowVueApp");

app.UseAuthentication(); // Phải có cái này trước Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();