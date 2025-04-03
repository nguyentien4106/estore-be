using EStore.Application.Data;
using EStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EStore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<EStoreDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString, assembly =>
            {
                assembly.MigrationsAssembly(typeof(EStoreDbContext).Assembly.FullName);
            });
        });

        services.AddTransient<IEStoreDbContext>(provider => provider.GetRequiredService<EStoreDbContext>());


        return services;
    }
}