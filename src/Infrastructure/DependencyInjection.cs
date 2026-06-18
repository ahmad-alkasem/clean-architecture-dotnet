using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<AppDbContext>(options =>
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                options.UseInMemoryDatabase("CleanArchitectureSample");
            else
                options.UseNpgsql(connectionString);
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        return services;
    }
}
