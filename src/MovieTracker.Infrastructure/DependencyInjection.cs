using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieTracker.Application.Common.Interfaces;
using MovieTracker.Infrastructure.Data;
using MovieTracker.Infrastructure.Services;

namespace MovieTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // Services
        services.AddScoped<ITokenService, TokenService>();
        services.AddHttpClient<ITmdbService, TmdbService>();

        // Memory Cache
        services.AddMemoryCache();

        return services;
    }
}