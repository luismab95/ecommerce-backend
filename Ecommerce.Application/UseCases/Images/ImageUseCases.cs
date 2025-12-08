namespace Ecommerce.Application.UseCases.Images;

using Ecommerce.Domain.Interfaces.Repositories;
using Ecommerce.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Image = Ecommerce.Domain.Entities.Image;

public class ImageUseCases
{
    private readonly IImageRepository _imageRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUploadImageService _uploadImageService;


    public ImageUseCases(IImageRepository imageRepository, IUploadImageService uploadImageService, IProductRepository productRepository)
    {
        _imageRepository = imageRepository;
        _productRepository = productRepository;
        _uploadImageService = uploadImageService;
    }

    public async Task<string> AddImagesAsync(IFormFileCollection request, int productId)
    {
        var findProduct = await _productRepository.GetByIdAsync(productId) ?? throw new InvalidOperationException("Producto no encontrado.");

        var images = await _uploadImageService.AddImageAsync(request);

        foreach (var item in images)
        {
            var newImage = Image.Create(item, findProduct.Id);
            await _imageRepository.AddAsync(newImage);
        }

        return "Imágen(s) agregadas exitosamente.";
    }

    public async Task<string> DeleteImageAsync(int imageId)
    {
        var findImage = await _imageRepository.GetByIdAsync(imageId) ??
            throw new InvalidOperationException("Imágen no encontrada.");

        var updateImage = Image.Delete(findImage);
        await _imageRepository.UpdateAsync(updateImage);

        return "Imagen eliminada exitosamente.";

    }


    public string GetContentType(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };
    }

}
