using Ecommerce.Domain.DTOs.General;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Ecommerce.Infrastructure.Extensions;

public static class JwtConfigExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // VALIDACIÓN DEL TOKEN
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["JwtSettings:Issuer"],
                    ValidAudience = config["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config["JwtSettings:Secret"]!)
                    )
                };

                // RESPUESTAS PERSONALIZADAS
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";

                        bool isExpired = context.AuthenticateFailure is SecurityTokenExpiredException;

                        if (isExpired)
                        {
                            context.Response.StatusCode = 401;

                            var response = new GeneralResponse
                            {
                                Data = "TOKEN_EXPIRED",
                                Message = "No autorizado. El token ha expirado.",
                            };

                            await context.Response.WriteAsJsonAsync(response);
                            return;
                        }

                        var defaultResponse = new GeneralResponse
                        {
                            Data = false,
                            Message = "No autorizado. El token es inválido o no fue enviado.",
                        };

                        await context.Response.WriteAsJsonAsync(defaultResponse);
                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";

                        var response = new GeneralResponse
                        {
                            Data = false,
                            Message = "Acceso denegado. No tiene permisos suficientes.",
                        };

                        await context.Response.WriteAsJsonAsync(response);
                    }
                };
            });

        return services;
    }
}
