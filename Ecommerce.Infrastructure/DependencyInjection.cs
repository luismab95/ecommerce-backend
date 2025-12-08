using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Extensions;
using Ecommerce.Domain.DTOs.Auth;
using Ecommerce.Domain.Interfaces.Repositories;
using Ecommerce.Domain.Interfaces.Services;
using Ecommerce.Infrastructure.Repositories;
using Ecommerce.Infrastructure.Services;
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


        // Repositorios
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        // Servicios
        services.AddScoped<IAuthService, JwtAuthService>();
        services.AddScoped<IEmailService, GmailSmtpEmailSender>();
        services.AddScoped<IUploadImageService, LocalStorageImageService>();


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