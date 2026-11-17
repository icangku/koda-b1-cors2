using EfCore.Api.Middlewares;
using EfCore.Application.Common.Interfaces;
using EfCore.Application.Services;
using EfCore.Infrastructure.Persistence;
using EfCore.Infrastructure.Repositories;
using EfCore.Infrastructure.Seeding;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IProductRepository, EfProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EfCategoryRepository>();

// Business/Application Service
builder.Services.AddScoped<
    IProductService,
    ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("ConnectionString : Belum di configurasi");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString)
        .UseSeeding((context, _) => ProductSeedData.Seed(context))
        .UseAsyncSeeding((context, _, ct) => ProductSeedData.SeedAsync(context, ct));
});

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(option =>
{
   option.AddPolicy("ReactClient", policy =>
   {
      policy.WithOrigins(allowedOrigins)
      .AllowAnyHeader()
      .AllowAnyMethod();
   });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<RequestTimingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
