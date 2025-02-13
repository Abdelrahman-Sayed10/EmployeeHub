using EmployeeHub.Application;
using EmployeeHub.Application.Contracts.IRepository;
using EmployeeHub.Application.Contracts.IService;
using EmployeeHub.Application.Services;
using EmployeeHub.Infrastructure;
using EmployeeHub.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

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

//Department
builder.Services.AddScoped<IDepartmentService, DepartmentService>();

//Employee
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
