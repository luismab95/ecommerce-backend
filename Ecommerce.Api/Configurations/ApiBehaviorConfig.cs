using Ecommerce.Domain.DTOs.General;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Configurations;

public static class ApiBehaviorConfig
{
    public static void AddCustomInvalidModelStateResponse(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .Select(e => new
                    {
                        Field = e.Key,
                        Errors = e.Value?.Errors.Select(x => x.ErrorMessage)
                    });

                var customResponse = new GeneralResponse
                {

                    Data = errors,
                    Message = "Parámetros inválidos."
                };

                return new ObjectResult(customResponse)
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };

            };
        });
    }
}
