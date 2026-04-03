using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
using QuanLyQuayThuoc.Repositories;
using QuanLyQuayThuoc.Repositories.Interfaces;
using QuanLyQuayThuoc.Services.Interfaces;
using QuanLyQuayThuoc.Services.Implementation;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Giải quyết lỗi JwtBearerDefaults
using Microsoft.IdentityModel.Tokens;
using QuanLyQuayThuoc.Helpers;
using System.Text;
using QuanLyQuayThuoc.Repository.Interfaces;
using QuanLyQuayThuoc.Repository.Implementation;
using QuanLyQuayThuoc.Models.Momo;
using QuanLyQuayThuoc.Services.Momo;

// Giải quyết lỗi Encoding

// ... các phần Using giữ nguyên ...
System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;
var builder = WebApplication.CreateBuilder(args);

// --- 1. KẾT NỐI DATABASE & CORS ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options => {
    options.AddPolicy("AllowVueApp", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// --- 2. ĐĂNG KÝ REPOSITORY & SERVICES ---
builder.Services.AddScoped<INguoiDungRepository, NguoiDungRepository>();
builder.Services.AddScoped<IDonHangRepository, DonHangRepository>();
builder.Services.AddScoped<IKhoRepository, KhoRepository>();
builder.Services.AddScoped<INguoiDungService, NguoiDungService>();
builder.Services.AddScoped<IBanHangService, BanHangService>();
builder.Services.AddScoped<IGioHangRepository, GioHangRepository>();
builder.Services.AddScoped<IGioHangService, GioHangService>();
builder.Services.AddScoped<IThuocRepository, ThuocRepository>();
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<ISanPhamRepository, SanPhamRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoOption"));
builder.Services.AddScoped<IMomoService, MomoService>();

// --- 3. CẤU HÌNH SWAGGER (DI CHUYỂN LÊN ĐÂY) ---
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Chỉ cần dán mã Token vào đây (Không cần ghi chữ Bearer)"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// --- 4. CẤU HÌNH JWT ---
var secretKey = builder.Configuration["Jwt:Key"] ?? "Chuoi_Secret_Key_Cua_Tai_Phai_Du_Dai_32_Ky_Tu";
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// CHỐT CẤU HÌNH TẠI ĐÂY
var app = builder.Build();
app.UseStaticFiles();
// --- 5. CẤU HÌNH PIPELINE (MIDDLEWARE) ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowVueApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();