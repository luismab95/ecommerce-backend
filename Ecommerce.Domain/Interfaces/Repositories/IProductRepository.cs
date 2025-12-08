using Ecommerce.Domain.DTOs.Pagination;
using Ecommerce.Domain.DTOs.Products;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Interfaces.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int productId);
    Task<PaginationResponse<Product>> GetProductsAsync(GetProductsWithFiltersRequest paginationRequest);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
}
