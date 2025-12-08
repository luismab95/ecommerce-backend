using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Extensions;
using Ecommerce.Domain.DTOs.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ecommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostBuilder host,
        IHostEnvironment environment)
    {
        // Configuración de SQL Server
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));


        // Configurar JWT
        services.Configure<JwtResponse>(configuration.GetSection("JwtSettings"));
        services.AddJwtAuthentication(configuration);
        services.AddAuthorization();

        // cors
        services.AddCors(options =>
        {
            options.AddPolicy("NewPolicy", app =>
            {
                app.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });

        return services;
    }
}