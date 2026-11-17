using EfCore.Application.Common.Interfaces;
using EfCore.Application.Repositories;
using EfCore.Infrastructure.Persistence;
using EfCore.Infrastructure.Repositories;
using EfCore.Infrastructure.Seeding;
using KodaStore.Application.Repositories;
using KodaStore.Infrastructure.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KodaStore.Infrastructure;

public static class DepedencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ecommercedb") ?? throw new InvalidOperationException("Connection  string ecommercedb belum dikonfigurasi");

        services.AddDbContext<AppDbContext>(option =>
        {
            option.UseSqlServer(connectionString)
             .UseSeeding((context, _) =>
             {
                 ProductSeedData.Seed(context);
                 CategorySeedData.Seed(context);
                 OrderSeedData.Seed(context);
             })
             .UseAsyncSeeding(async (context, _, ct) =>
             {
                 await ProductSeedData.SeedAsync(context, ct);
                 await CategorySeedData.SeedAsync(context, ct);
                 await OrderSeedData.SeedAsync(context, ct);
             });
        });

        services.AddScoped<IProductRepository, EfProductRepository>();
        services.AddScoped<IOrderRepository, EfOrderRepository>();
        services.AddScoped<ICategoryRepository, EfCategoryRepository>();

        return services;
    }
}