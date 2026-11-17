using EfCore.Api.Middlewares;
using EfCore.Application.Common.Interfaces;
using EfCore.Application.Services;
using EfCore.Infrastructure.Persistence;
using EfCore.Infrastructure.Repositories;
using EfCore.Infrastructure.Seeding;
using KodaStore.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
// builder.Services.AddScoped<IProductRepository, EfProductRepository>();
// builder.Services.AddScoped<ICategoryRepository, EfCategoryRepository>();
// // Business/Application Service
// builder.Services.AddScoped<
//     IProductService,
//     ProductService>();
// builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<
    IProductService,
    ProductService>();

// builder.Services.AddScoped<
//     IOrderService,
//     OrderService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
// var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("ConnectionString : Belum di configurasi");

// builder.Services.AddDbContext<AppDbContext>(options =>
// {
//     options.UseSqlServer(connectionString)
//         .UseSeeding((context, _) => ProductSeedData.Seed(context))
//         .UseAsyncSeeding((context, _, ct) => ProductSeedData.SeedAsync(context, ct));
// });

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

builder.Services.AddRateLimiter(option =>
{
    option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    option.AddConcurrencyLimiter("category-writes", limiter =>
    {
        limiter.PermitLimit = 8;
        limiter.QueueLimit = 0;
    });

    // Untuk Login
    option.AddFixedWindowLimiter("LoginLimiter", limiter =>
    {
        limiter.PermitLimit = 3;
        limiter.Window = TimeSpan.FromMinutes(3);
        limiter.QueueLimit = 0;
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
app.UseStaticFiles();
app.UseCors("ReactClient");
app.UseAuthorization();
app.UseRateLimiter();
app.UseRouting();
app.MapControllers();

app.Run();
