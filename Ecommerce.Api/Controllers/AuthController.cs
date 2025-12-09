using Ecommerce.Api.Filters;
using Ecommerce.Domain.DTOs.Auth;
using Ecommerce.Domain.DTOs.General;
using Ecommerce.Application.UseCases.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthUseCases _authUseCases;

    public AuthController(AuthUseCases authUseCases)
    {
        _authUseCases = authUseCases;
    }


    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
    {

        var result = await _authUseCases.RegisterUserAsync(request);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });


    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
    {
        request.Ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        request.DeviceInfo = HttpContext.Request.Headers.UserAgent.ToString();

        var result = await _authUseCases.LoginUserAsync(request);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {

        if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            return Unauthorized(new GeneralResponse { Message = "Token no presente en la solicitud." });
        }

        var parts = authHeader.ToString().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2 || !parts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized(new GeneralResponse { Message = "Formato de token inválido." });
        }

        var token = parts[1];
        var result = await _authUseCases.RefreshTokenAsync(token);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });


    }


    [HttpPost("sign-out")]
    [Authorize]
    [ServiceFilter(typeof(PostAuthorizeFilter))]
    public async Task<IActionResult> Logout()
    {

        var rawAuth = HttpContext.Request.Headers.Authorization.ToString();
        var token = rawAuth.Replace("Bearer ", "").Trim();

        var result = await _authUseCases.LogoutUserAsync(token);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });

    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var result = await _authUseCases.ForgotPasswordAsync(request);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });
    }

    [HttpPost("reset-password")]
    [Authorize]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {

        var rawAuth = HttpContext.Request.Headers.Authorization.ToString();
        var token = rawAuth.Replace("Bearer ", "").Trim();
        request.Token = token;


        var result = await _authUseCases.ResetPasswordAsync(request);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });


    }

}
