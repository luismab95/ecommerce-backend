using Ecommerce.Domain.DTOs.Products;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Application.UseCases.Products;

public class ProductUseCases
{
    private readonly IProductRepository _productRepository;
    private readonly IConfiguration _config;

    public ProductUseCases(IProductRepository productRepository, IConfiguration config)
    {
        _productRepository = productRepository;
        _config = config;
    }

    public async Task<object> GetProductsAsync(GetProductsWithFiltersRequest request)
    {
        var result = await _productRepository.GetProductsAsync(request);

        var safeProductResponse = new List<object>();

        result.Items.ForEach(product =>
        {
            if (product != null && product.Images != null)
            {
                string baseUrl = $"{_config["App:StaticUrl"]}";
                var imagesList = new List<Image>();

                product.Images.ToList().ForEach(image =>
                {
                    if (image.IsActive)
                        imagesList.Add(Image.UpdatePath(image, baseUrl));
                });

                product = Product.SetImages(product, imagesList);
            }

            safeProductResponse.Add(Product.ToSafeResponse(product!));
        });


        return new
        {
            Items = safeProductResponse,
            result.TotalCount,
            result.PageNumber,
            result.PageSize,
            result.TotalPages,
            result.HasPreviousPage,
            result.HasNextPage,
        };
    }

    public async Task<object> GetProductByIdAsync(int productId)
    {
        var findProduct = await _productRepository.GetByIdAsync(productId) ??
            throw new InvalidOperationException("Producto no encontrado.");

        if (findProduct != null && findProduct.Images != null)
        {
            string baseUrl = $"{_config["App:StaticUrl"]}";
            var imagesList = new List<Image>();
            findProduct.Images.ToList().ForEach(image =>
            {
                if (image.IsActive)
                    imagesList.Add(Image.UpdatePath(image, baseUrl));
            });

            findProduct = Product.SetImages(findProduct, imagesList);
        }

        return Product.ToSafeResponse(findProduct!);
    }

    public async Task<string> AddProductAsync(ProductRequest request)
    {
        var addProduct = Product.Create(request.CategoryId, request.Name, request.Description, request.Price, request.Stock, request.Featured);
        await _productRepository.AddAsync(addProduct);

        return "Producto agregado exitosamente.";

    }

    public async Task<string> UpdateProductAsync(int productId, ProductRequest product)
    {
        var findProduct = await _productRepository.GetByIdAsync(productId) ??
            throw new InvalidOperationException("Producto no encontrado.");

        var updateProduct = Product.Update(findProduct, product.Name, product.Description, product.Price, product.Stock, product.Featured, product.CategoryId);
        await _productRepository.UpdateAsync(updateProduct);

        return "Producto actualizado exitosamente.";

    }

    public async Task<string> DeleteProductAsync(int categoryId)
    {
        var findProduct = await _productRepository.GetByIdAsync(categoryId) ??
            throw new InvalidOperationException("Producto no encontrado.");

        var updateProduct = Product.Delete(findProduct);
        await _productRepository.UpdateAsync(updateProduct);

        return "Producto eliminado exitosamente.";
    }


}
