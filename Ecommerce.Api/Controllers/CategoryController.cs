using Ecommerce.Api.Filters;
using Ecommerce.Domain.DTOs.Categories;
using Ecommerce.Domain.DTOs.Pagination;
using Ecommerce.Domain.DTOs.General;
using Ecommerce.Application.UseCases.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;


[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly CategoryUseCases _categoryUseCases;


    public CategoryController(CategoryUseCases categoryUseCases)
    {
        _categoryUseCases = categoryUseCases;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetCategories([FromQuery] PaginationRequest request)
    {

        var result = await _categoryUseCases.GetCategoriesAsync(request);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });

    }

    [HttpGet("{categoryId}")]
    public async Task<IActionResult> GetCategoryById(int categoryId)
    {

        var result = await _categoryUseCases.GetCategoryByIdAsync(categoryId);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });

    }

    [HttpPost("")]
    [Authorize]
    [ServiceFilter(typeof(PostAuthorizeFilter))]
    [ServiceFilter(typeof(PostAuthorizeRoleFilter))]
    public async Task<IActionResult> AddCategory([FromBody] CategoryRequest request)
    {

        var result = await _categoryUseCases.AddCategoryAsync(request);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });

    }



    [HttpPut("{categoryId}")]
    [Authorize]
    [ServiceFilter(typeof(PostAuthorizeFilter))]
    [ServiceFilter(typeof(PostAuthorizeRoleFilter))]
    public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] CategoryRequest request)
    {

        var result = await _categoryUseCases.UpdateCategoryAsync(categoryId, request);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });

    }


    [HttpDelete("{categoryId}")]
    [Authorize]
    [ServiceFilter(typeof(PostAuthorizeFilter))]
    [ServiceFilter(typeof(PostAuthorizeRoleFilter))]
    public async Task<IActionResult> DeleteCategory(int categoryId)
    {

        var result = await _categoryUseCases.DeleteCategoryAsync(categoryId);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });


    }

}

