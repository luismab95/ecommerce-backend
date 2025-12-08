using Ecommerce.Domain.DTOs.General;
using Ecommerce.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Ecommerce.Api.Filters;
public class PostAuthorizeRoleFilter : IAsyncActionFilter
{
    private readonly IUserRepository _userRepository;

    public PostAuthorizeRoleFilter(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;

        string userEmail = user.FindFirst(ClaimTypes.Email)?.Value.ToString()!;

        var findUser = await _userRepository.GetByEmailAsync(userEmail);

        if (findUser is null || findUser.Role == "Cliente")
        {
            context.Result = new JsonResult(new GeneralResponse
            {
                Data = false,
                Message = "No tiene permisos suficientes para realizar esta operación.",
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };

            return;
        }

        await next();
    }
}
