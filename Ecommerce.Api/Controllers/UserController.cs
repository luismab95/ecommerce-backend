using Ecommerce.Api.Filters;
using Ecommerce.Domain.DTOs.General;
using Ecommerce.Domain.DTOs.Products;
using Ecommerce.Domain.DTOs.Pagination;
using Ecommerce.Domain.DTOs.Users;
using Ecommerce.Application.UseCases.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;


[ApiController]
[Authorize]
[ServiceFilter(typeof(PostAuthorizeFilter))]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly UserUseCases _userUseCase;

    public UserController(UserUseCases userUseCase)
    {
        _userUseCase = userUseCase;
    }


    [HttpGet("")]
    [ServiceFilter(typeof(PostAuthorizeRoleFilter))]
    public async Task<IActionResult> GetUsers([FromQuery] PaginationRequest request)
    {

        var result = await _userUseCase.GetUsersAsync(request);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });

    }


    [HttpGet("wishlist/{userId}")]
    public async Task<IActionResult> GetUserWishlist(int userId)
    {

        var result = await _userUseCase.GetUserWishlistAsync(userId);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });


    }


    [HttpPost("wishlist/{userId}")]
    public async Task<IActionResult> AddProductWishList([FromBody] AddProductWishListRequest request, int userId)
    {

        var result = await _userUseCase.AddProductWishListAsync(request, userId);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });


    }


    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(int userId)
    {

        var result = await _userUseCase.GetUserByIdAsync(userId);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });

    }


    [HttpPut("profile/{userId}")]
    public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserRequest request)
    {

        var result = await _userUseCase.UpdateUserAsync(userId, request);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });

    }


    [HttpPut("address/{userId}")]
    public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserAddressRequest request)
    {

        var result = await _userUseCase.UpdateUserAddressAsync(userId, request);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });

    }


    [HttpPut("role/{userId}")]
    [ServiceFilter(typeof(PostAuthorizeRoleFilter))]
    public async Task<IActionResult> UpdateUserRole(int userId)
    {

        var result = await _userUseCase.UpdateRoleAsync(userId);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });

    }


    [HttpDelete("{userId}")]
    [ServiceFilter(typeof(PostAuthorizeRoleFilter))]
    public async Task<IActionResult> DeleteUser(int userId)
    {

        var result = await _userUseCase.DeleteUserAsync(userId);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });

    }

}
