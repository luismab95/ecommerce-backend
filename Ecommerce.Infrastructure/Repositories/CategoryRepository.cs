using Ecommerce.Domain.DTOs.Pagination;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces.Repositories;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{

    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(int categoryId)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId && c.IsActive);
    }

    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task<PaginationResponse<Category>> GetCategoriesAsync(PaginationRequest paginationRequest)
    {
        var query = _context.Categories.Where(u => u.IsActive);

        // Aplicar búsqueda si searchTerm no es nulo o vacío
        if (!string.IsNullOrWhiteSpace(paginationRequest.SearchTerm))
        {
            var searchTerm = paginationRequest.SearchTerm.Trim().ToLower();
            query = query.Where(c =>
                 c.Name.ToLower().Contains(searchTerm) ||
                 c.Description.ToLower().Contains(searchTerm)
            );
        }

        query = query.OrderBy(c => c.Id);

        return await query.ToPagedListAsync(paginationRequest.PageNumber, paginationRequest.PageSize);
    }

}
