using Ecommerce.Api.Filters;
using Ecommerce.Domain.DTOs.General;
using Ecommerce.Application.UseCases.Images;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/images")]
public class ImageController : ControllerBase
{
    private readonly ImageUseCases _imageUseCases;
    private readonly IWebHostEnvironment _environment;
    private const string UploadsFolder = "uploads";

    public ImageController(ImageUseCases imageUseCases, IWebHostEnvironment environment)
    {
        _imageUseCases = imageUseCases;
        _environment = environment;
    }


    [HttpGet("{fileName}")]
    public IActionResult GetImage(string fileName)
    {
        var uploadsPath = Path.Combine(_environment.WebRootPath, UploadsFolder);
        var filePath = Path.Combine(uploadsPath, fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("Imagen no encontrada");
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var contentType = _imageUseCases.GetContentType(extension);

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, contentType);



    }

    [HttpPost("{productId}")]
    [Consumes("multipart/form-data")]
    [Authorize]
    [ServiceFilter(typeof(PostAuthorizeFilter))]
    [ServiceFilter(typeof(PostAuthorizeRoleFilter))]
    public async Task<IActionResult> AddImages([FromForm] IFormFileCollection request, int productId)
    {

        var result = await _imageUseCases.AddImagesAsync(request, productId);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });


    }


    [HttpDelete("{imageId}")]
    [Authorize]
    [ServiceFilter(typeof(PostAuthorizeFilter))]
    [ServiceFilter(typeof(PostAuthorizeRoleFilter))]
    public async Task<IActionResult> DeleteImage(int imageId)
    {
        var result = await _imageUseCases.DeleteImageAsync(imageId);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });

    }

}
