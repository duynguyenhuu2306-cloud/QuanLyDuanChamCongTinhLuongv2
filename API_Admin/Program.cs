using BLL;
using BLL.Interfaces;
using BLL.Services;
using DAL;
using DAL.Helper;
using DAL.Helper.Interfaces;
using DAL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero,
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Nh?p Access Token theo d?ng: Bearer {token}"
        }
    );
    options.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference =new Microsoft.OpenApi.Models.OpenApiReference{Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,Id = "Bearer"}
                },
                Array.Empty<string>()
            }
        }
    );
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IDatabaseHelper, DatabaseHelper>();
builder.Services.AddTransient<INguoiDungRepository, NguoiDungRepository>();
builder.Services.AddTransient<IDuAnRepository, DuAnRepository>();
builder.Services.AddTransient<ICongViecRepository, CongViecRepository>();
builder.Services.AddTransient<IPhanCongRepository, PhanCongRepository>();
builder.Services.AddTransient<IChamCongRepository, ChamCongRepository>();
builder.Services.AddTransient<INghiPhepRepository, NghiPhepRepository>();
builder.Services.AddTransient<IChinhSachLuongRepository, ChinhSachLuongRepository>();
builder.Services.AddTransient<IChayBangLuongRepository, ChayBangLuongRepository>();
builder.Services.AddTransient<IPhieuLuongRepository, PhieuLuongRepository>();
builder.Services.AddTransient<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddTransient<INguoiDungBusiness, NguoiDungBusiness>();
builder.Services.AddTransient<IDuAnBusiness, DuAnBusiness>();
builder.Services.AddTransient<ICongViecBusiness, CongViecBusiness>();
builder.Services.AddTransient<IPhanCongBusiness, PhanCongBusiness>();
builder.Services.AddTransient<IChamCongBusiness, ChamCongBusiness>();
builder.Services.AddTransient<INghiPhepBusiness, NghiPhepBusiness>();
builder.Services.AddTransient<IChinhSachLuongBusiness, ChinhSachLuongBusiness>();
builder.Services.AddTransient<IChayBangLuongBusiness, ChayBangLuongBusiness>();
builder.Services.AddTransient<IPhieuLuongBusiness, PhieuLuongBusiness>();
builder.Services.AddTransient<IAuditLogBusiness, AuditLogBusiness>();
builder.Services.AddScoped<IAuthBusiness, AuthBusiness>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRefreshTokenRepository,RefreshTokenRepository>();
builder.Services.AddAuthorization();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();