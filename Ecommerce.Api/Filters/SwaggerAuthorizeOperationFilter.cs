using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ecommerce.Api.Filters
{
    public class SwaggerAuthorizeOperationFilter : IOperationFilter
    {
        private const string SchemeId = "bearer";

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Verificar si el método o el controlador tiene [Authorize]
            var hasAuthorize =
                context.MethodInfo.DeclaringType!
                    .GetCustomAttributes(true)
                    .OfType<AuthorizeAttribute>()
                    .Any()
                ||
                context.MethodInfo
                    .GetCustomAttributes(true)
                    .OfType<AuthorizeAttribute>()
                    .Any();

            // Verificar si tiene [AllowAnonymous]
            var hasAllowAnonymous =
                context.MethodInfo
                    .GetCustomAttributes(true)
                    .OfType<AllowAnonymousAttribute>()
                    .Any();

            // Si NO requiere auth → no poner el candado
            if (!hasAuthorize || hasAllowAnonymous)
                return;

            // Asegurar que operation.Security exista
            operation.Security ??= new List<OpenApiSecurityRequirement>();

            // Agregar el requisito de seguridad Bearer
            // Nuevo método requerido para .NET 10
            var requirement = new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(SchemeId, context.Document)] = new()
            };

            operation.Security.Add(requirement);
        }
    }
}
