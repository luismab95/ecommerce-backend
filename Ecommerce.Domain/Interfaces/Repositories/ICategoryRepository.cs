using Ecommerce.Domain.DTOs.Pagination;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int categoryId);
    Task<PaginationResponse<Category>> GetCategoriesAsync(PaginationRequest paginationRequest);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
}
