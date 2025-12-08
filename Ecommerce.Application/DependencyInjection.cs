using Ecommerce.Application.UseCases.Auth;
using Ecommerce.Application.UseCases.Categories;
using Ecommerce.Application.UseCases.Images;
using Ecommerce.Application.UseCases.Orders;
using Ecommerce.Application.UseCases.Products;
using Ecommerce.Application.UseCases.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services
       )
    {

        services.AddScoped<AuthUseCases>();
        services.AddScoped<UserUseCases>();
        services.AddScoped<CategoryUseCases>();
        services.AddScoped<ImageUseCases>();
        services.AddScoped<ProductUseCases>();
        services.AddScoped<OrderUseCases>();

        return services;
    }
}