using Ecommerce.Api.Filters;
using Ecommerce.Domain.DTOs.General;
using Ecommerce.Domain.DTOs.Products;
using Ecommerce.Application.UseCases.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly ProductUseCases _productUseCases;

    public ProductController(ProductUseCases productUseCases)
    {
        _productUseCases = productUseCases;
    }


    [HttpGet("")]
    public async Task<IActionResult> GetProducts([FromQuery] GetProductsWithFiltersRequest request)
    {

        var result = await _productUseCases.GetProductsAsync(request);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });

    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductById(int productId)
    {

        var result = await _productUseCases.GetProductByIdAsync(productId);

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
    public async Task<IActionResult> AddProduct([FromBody] ProductRequest request)
    {

        var result = await _productUseCases.AddProductAsync(request);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });
    }

    [HttpPut("{productId}")]
    [Authorize]
    [ServiceFilter(typeof(PostAuthorizeFilter))]
    [ServiceFilter(typeof(PostAuthorizeRoleFilter))]
    public async Task<IActionResult> UpdateProduct(int productId, [FromBody] ProductRequest request)
    {

        var result = await _productUseCases.UpdateProductAsync(productId, request);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });

    }


    [HttpDelete("{productId}")]
    [Authorize]
    [ServiceFilter(typeof(PostAuthorizeFilter))]
    [ServiceFilter(typeof(PostAuthorizeRoleFilter))]
    public async Task<IActionResult> DeleteProduct(int productId)
    {
        var result = await _productUseCases.DeleteProductAsync(productId);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });

    }
}



