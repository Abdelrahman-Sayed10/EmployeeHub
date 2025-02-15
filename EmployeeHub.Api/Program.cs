using EmployeeHub.Api.Middleware;
using EmployeeHub.Application;
using EmployeeHub.Application.Contracts.IRepository;
using EmployeeHub.Application.Contracts.IService;
using EmployeeHub.Application.Services;
using EmployeeHub.Dtos.AuthDto;
using EmployeeHub.Infrastructure;
using EmployeeHub.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region DbContext
builder.Services.AddDbContextPool<EmployeeHubDbContext>(options =>
{
    options
    .UseSqlServer(new ConfigurationManager()
    .AddJsonFile("appsettings.json")
    .Build()
    .GetConnectionString("DefaultConnection"));
});
#endregion

#region DI
// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Department
builder.Services.AddScoped<IDepartmentService, DepartmentService>();

// Employee
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// AuthService
builder.Services.AddScoped<IAuthService, AuthService>();

// HttpContextAccessor
builder.Services.AddHttpContextAccessor();
#endregion

#region Jwt
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero        
    };
});
#endregion

builder.Services.Configure<GoogleAuthSettings>(
    builder.Configuration.GetSection("Google")
);


var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
