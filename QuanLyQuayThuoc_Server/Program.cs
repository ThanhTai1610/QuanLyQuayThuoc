using Microsoft.EntityFrameworkCore;
using QuanLyQuayThuoc.Data;
// Đừng quên using các namespace của Repository và Service của bạn ở đây
// ví dụ: using QuanLyQuayThuoc.Repository.Interfaces; 

var builder = WebApplication.CreateBuilder(args);

// --- 1. KẾT NỐI DATABASE ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. CẤU HÌNH CORS (Chỉ cần 1 cái này là đủ cho máy xanh gọi máy tím) ---
builder.Services.AddCors(options => {
    options.AddPolicy("AllowVueApp", policy =>
        policy.WithOrigins("http://localhost:5173") // Cổng mặc định của Vite/Vue 3
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// --- 3. ĐĂNG KÝ REPOSITORY & SERVICES (BẮT BUỘC PHẢI CÓ) ---
// Tài thêm các dòng này để hệ thống hiểu các file trong folder Repository/Service nhé
// builder.Services.AddScoped<IAuthService, AuthService>();
// builder.Services.AddScoped<IThuocRepository, ThuocRepository>();
// ... Thêm các cái khác tương tự ở đây ...

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 4. CẤU HÌNH PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// KÍCH HOẠT CORS (Phải nằm TRƯỚC Authorization)
app.UseCors("AllowVueApp");

app.UseAuthorization();

app.MapControllers();

app.Run();