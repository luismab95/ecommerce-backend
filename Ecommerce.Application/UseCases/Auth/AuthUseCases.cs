using Ecommerce.Domain.Interfaces.Repositories;
using Ecommerce.Domain.Interfaces.Services;
using Ecommerce.Domain.DTOs.Auth;
using Ecommerce.Domain.DTOs.Email;
using Ecommerce.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Ecommerce.Application.UseCases.Auth;


public class AuthUseCases
{
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;
    private readonly ISessionRepository _sessionRepository;
    private readonly IShoppingCartRepository _shoppingCartRepository;


    public AuthUseCases(IUserRepository userRepository, IProductRepository productRepository, IShoppingCartRepository shoppingCartRepository, IAuthService authService, IEmailService emailService, IConfiguration config, ISessionRepository sessionRepository)
    {
        _userRepository = userRepository;
        _productRepository = productRepository;
        _shoppingCartRepository = shoppingCartRepository;
        _authService = authService;
        _emailService = emailService;
        _config = config;
        _sessionRepository = sessionRepository;
    }


    public async Task<String> RegisterUserAsync(SignUpRequest request)
    {
        // Validar que el email no exista
        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new InvalidOperationException("El email ya está registrado");
        }

        // Hashear password
        var passwordHash = _authService.HashPassword(request.Password);

        // Crear usuario
        var user = User.Create(request.Email, passwordHash, request.FirstName, request.LastName, request.Phone);

        // Guardar usuario
        await _userRepository.AddAsync(user);

        return "¡Bienvenido! Tu cuenta ha sido creada correctamente.";
    }


    public async Task<string> LogoutUserAsync(string token)
    {
        // Obtener payload del token
        var jwtPayload = await _authService.GetPayloadJwtTokenAsync(token);

        // Buscar session 
        int sessionId = int.Parse(jwtPayload[ClaimTypes.NameIdentifier].ToString()!);
        var session = await _sessionRepository.GetSessionAsync(sessionId) ??
            throw new InvalidOperationException("Sesión no encontrada.");


        var updatedSession = Session.Update(session);
        await _sessionRepository.UpdateAsync(updatedSession);

        return "La sesión se ha cerrado correctamente.";
    }

    public async Task<AuthResponse> LoginUserAsync(SignInRequest request)
    {
        // Validar que el email exista
        var findUser = await _userRepository.GetByEmailAsync(request.Email) ??
            throw new InvalidOperationException("Creedenciales Incorrectas.");


        // Valid password
        var isValidPassword = _authService.VerifyPassword(request.Password, findUser.PasswordHash);
        if (!isValidPassword)
        {
            throw new InvalidOperationException("Creedenciales Incorrectas.");
        }

        // Generar refreshToken
        var refreshToken = await _authService.GenerateRefreshTokenAsync(findUser);

        // Generar session 
        int.TryParse(_config["JwtSettings:RefreshExpireDays"], out int refreshExpireDays);
        var session = Session.Create(findUser.Id, request.DeviceInfo, request.Ip, refreshToken, DateTime.Now.AddDays(refreshExpireDays));
        var newSession = await _sessionRepository.AddAsync(session);

        // Generar accessToken
        var accessToken = await _authService.GenerateTokenAsync(findUser, newSession.Id);

        // Obtener items del carrito
        var cartItems = await GetShoppingCartItemsAsync(findUser.Id);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = User.ToSafeResponse(findUser),
            ShoppingCart = cartItems
        };
    }

    public async Task<string> ResetPasswordAsync(ResetPasswordRequest request)
    {
        // Validar token
        var isValidToken = _authService.ValidateToken(request.Token, true);
        if (!isValidToken)
        {
            throw new InvalidOperationException("Token no válido.");
        }

        // Validar email y role
        var jwtPayload = await _authService.GetPayloadJwtTokenAsync(request.Token);
        if (jwtPayload[ClaimTypes.Email].ToString() != request.Email)
        {
            throw new InvalidOperationException("El email proporcionado no coincide con el email de la solicitud de restablecimiento de contraseña.");
        }

        if (jwtPayload[ClaimTypes.Role].ToString() != "RESET_PASSWORD")
        {
            throw new InvalidOperationException("Token no válido.");
        }

        // Buscar usuario
        var findUser = await _userRepository.GetByEmailAsync(request.Email) ??
            throw new InvalidOperationException("El usuario no esta registrado.");

        // actualizar usuario
        var hashPassword = _authService.HashPassword(request.Password);
        var user = User.ResetPassword(findUser, hashPassword);

        await _userRepository.UpdateAsync(user);

        return "Tu contraseña ha sido restablecida con éxito.";
    }


    public async Task<AuthResponse> RefreshTokenAsync(string token)
    {
        // Obtener payload del token
        var isValidToken = _authService.ValidateToken(token, false);
        if (!isValidToken)
        {
            throw new InvalidOperationException("Token no válido.");
        }

        // Buscar sesión 
        var session = await _sessionRepository.GetSessionByTokenAsync(token) ??
            throw new InvalidOperationException("Sesión no encontrada.");


        // Validar token
        var isValidRefreshToken = _authService.ValidateToken(session.RefreshToken, true);
        if (session == null || session.ExpiresAt < DateTime.UtcNow || session.Revoked || !isValidRefreshToken)
            throw new InvalidOperationException("Sesión expirada.");

        // REVOKE SESSION
        var revokeSession = Session.Update(session);
        await _sessionRepository.UpdateAsync(revokeSession);

        // NEW SESSION
        // Buscar usuario
        var jwtPayload = await _authService.GetPayloadJwtTokenAsync(token);
        var findUser = await _userRepository.GetByEmailAsync(jwtPayload[ClaimTypes.Email].ToString()!) ?? throw new InvalidOperationException("Sesión expirada.");

        int.TryParse(_config["JwtSettings:RefreshExpireDays"], out int refreshExpireDays);
        var newRefreshToken = await _authService.GenerateRefreshTokenAsync(findUser);
        var newSession = Session.Create(findUser.Id, session.DeviceInfo, session.IpAddress, newRefreshToken, DateTime.Now.AddDays(refreshExpireDays));
        await _sessionRepository.AddAsync(newSession);

        // Generar nuevo token
        var accessToken = await _authService.GenerateTokenAsync(findUser, newSession.Id);

        // Obtener items del carrito
        var cartItems = await GetShoppingCartItemsAsync(findUser.Id);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            User = User.ToSafeResponse(findUser),
            ShoppingCart = cartItems
        };
    }

    public async Task<string> ForgotPasswordAsync(ForgotPasswordRequest request)
    {

        var findUser = await _userRepository.GetByEmailAsync(request.Email) ??
            throw new InvalidOperationException("El usuario no esta registrado.");

        var resetPwdAccessToken = await _authService.GenerateResetPasswordTokenAsync(findUser);

        var msg = new EmailMessage
        {
            Body = GetPasswordResetTemplate(),
            From = _config["Gmail:Username"]!,
            IsHtml = true,
            Subject = "Restablecer tu contraseña",
            To = new List<string> { request.Email }
        };

        await _emailService.SendAsync(msg);

        return resetPwdAccessToken;

    }

    private static string GetPasswordResetTemplate()
    {
        return $@" <div style='font-family: Arial, sans-serif; max-width: 480px; margin: auto; padding: 20px;'> <h2 style='color:#333;'>Restablecer tu contraseña</h2> <p style='color:#555; font-size: 15px;'>
                    Has solicitado restablecer tu contraseña. </p>

                    <div style='text-align: center; margin: 30px 0;'>
                        <a 
                           style='background: #4a90e2; color: white; padding: 12px 20px; 
                                  border-radius: 6px; text-decoration: none; font-size: 16px;'>
                            Tienes 10 minutos para realizar el cambio.
                        </a>
                    </div>

                    <p style='color:#777; font-size: 14px;'>
                        Si no solicitaste este cambio, simplemente ignora este mensaje.
                    </p>

                    <p style='color:#aaa; font-size: 12px; margin-top: 20px;'>
                        © {DateTime.UtcNow.Year} Mi Aplicación — Seguridad de cuentas
                    </p>
                </div>";
    }

    private async Task<List<ShoppingCartItem>> GetShoppingCartItemsAsync(int userId)
    {
        var findShoppingCart = await _shoppingCartRepository.GetByIdAsync(userId);
        var shoppingCartItems = new List<ShoppingCartItem>();
        if (findShoppingCart != null)
        {
            foreach (var item in findShoppingCart.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);

                string baseUrl = $"{_config["App:StaticUrl"]}";
                var imagesList = new List<Image>();
                product!.Images.ToList().ForEach(image =>
                {
                    if (image.IsActive)
                        imagesList.Add(Image.UpdatePath(image, baseUrl));
                });
                product = Product.SetImages(product, imagesList);
                shoppingCartItems.Add(ShoppingCartItem.Create(Product.ToSafeResponse(product!), item.Quantity));
            }
        }
        return shoppingCartItems;
    }

}