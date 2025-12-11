using Ecommerce.Api.Filters;
using Ecommerce.Domain.DTOs.Auth;
using Ecommerce.Domain.DTOs.General;
using Ecommerce.Application.UseCases.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;

namespace Ecommerce.Api.Controllers;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthUseCases _authUseCases;
    private readonly IAntiforgery _xsrfService;
    private readonly IConfiguration _config;
    private readonly IHostEnvironment _environment;

    public AuthController(AuthUseCases authUseCases, IAntiforgery xsrfService, IConfiguration config, IHostEnvironment environment)
    {
        _authUseCases = authUseCases;
        _xsrfService = xsrfService;
        _config = config;
        _environment = environment;
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

        // Set HttpOnly cookie for refresh token
        var isDev = _environment.IsDevelopment();
        int.TryParse(_config["JwtSettings:RefreshExpireDays"], out int refreshExpireDays);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Path = "/api/auth",
            Expires = DateTimeOffset.UtcNow.AddDays(refreshExpireDays),
            SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.None,
            Secure = !isDev // HTTPS en producción, HTTP en dev
        };
        Response.Cookies.Append("refreshToken", result.RefreshToken!, cookieOptions);

        // Set XSRF token cookie (not HttpOnly) so Angular can read/send it
        var tokens = _xsrfService.GetAndStoreTokens(HttpContext);
        var xsrfOptions = new CookieOptions
        {
            HttpOnly = false,
            Path = "/",
            SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.None,
            Secure = !isDev // HTTPS en producción, HTTP en dev
        };
        Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!, xsrfOptions);

        return Ok(new GeneralResponse
        {
            Data = new AuthResponse
            {
                AccessToken = result.AccessToken,
                User = result.User,
                ShoppingCart = result.ShoppingCart
            },
            Message = "Proceso realizado con éxito."
        });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {

        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            return Unauthorized(new GeneralResponse { Message = "RefreshToken no presente en la solicitud." });

        var result = await _authUseCases.RefreshTokenAsync(refreshToken);


        // Set HttpOnly cookie for refresh token
        var isDev = _environment.IsDevelopment();
        int.TryParse(_config["JwtSettings:RefreshExpireDays"], out int refreshExpireDays);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Path = "/api/auth",
            Expires = DateTimeOffset.UtcNow.AddDays(refreshExpireDays),
            SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.None,
            Secure = !isDev // HTTPS en producción, HTTP en dev
        };
        Response.Cookies.Append("refreshToken", result.RefreshToken!, cookieOptions);

        // Set XSRF token cookie (not HttpOnly) so Angular can read/send it
        var tokens = _xsrfService.GetAndStoreTokens(HttpContext);
        var xsrfOptions = new CookieOptions
        {
            HttpOnly = false,
            Path = "/",
            SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.None,
            Secure = !isDev // HTTPS en producción, HTTP en dev
        };
        Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!, xsrfOptions);

        return Ok(new GeneralResponse
        {
            Data = new AuthResponse
            {
                AccessToken = result.AccessToken,
                User = result.User,
                ShoppingCart = result.ShoppingCart
            },
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

        // Remove cookie
        Response.Cookies.Delete("refreshToken", new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.None, Path = "/api/auth" });
        Response.Cookies.Delete("XSRF-TOKEN");

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
