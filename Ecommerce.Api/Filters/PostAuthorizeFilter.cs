using Ecommerce.Domain.DTOs.General;
using Ecommerce.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Ecommerce.Api.Filters;
public class PostAuthorizeFilter : IAsyncActionFilter
{
    private readonly ISessionRepository _sessionRepository;


    public PostAuthorizeFilter(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;

        string sessionId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString()!;

        var findSession = await _sessionRepository.GetSessionAsync(int.Parse(sessionId));

        if (findSession is null)
        {
            context.Result = new JsonResult(new GeneralResponse
            {
                Data = false,
                Message = "Sesión inválida o expirada.",
            })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };

            return;
        }

        await next();
    }
}
