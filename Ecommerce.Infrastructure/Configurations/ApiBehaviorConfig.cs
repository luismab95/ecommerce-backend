using Ecommerce.Domain.DTOs.General;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace Ecommerce.Infrastructure.Configurations;

public static class ApiBehaviorConfig
{
    public static void AddCustomInvalidModelStateResponse(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {

                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("ApiBehavior");

                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .Select(e => new
                    {
                        Field = e.Key,
                        Errors = e.Value?.Errors.Select(x => x.ErrorMessage)
                    });

                logger.LogError("Invalid ModelState: {@Errors}", errors);

                var customResponse = new GeneralResponse
                {
                    Data = errors,
                    Message = "Parámetros inválidos."
                };

                return new ObjectResult(customResponse)
                {
                    StatusCode = 422
                };
            };
        });
    }
}
