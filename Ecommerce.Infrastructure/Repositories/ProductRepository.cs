using Ecommerce.Domain.DTOs.Pagination;
using Ecommerce.Domain.DTOs.Products;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces.Repositories;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task<Product?> GetByIdAsync(int productId)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);
    }

    public async Task<PaginationResponse<Product>> GetProductsAsync(GetProductsWithFiltersRequest paginationRequest)
    {

        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.IsActive);


        // Aplicar búsqueda si searchTerm no es nulo o vacío
        if (!string.IsNullOrWhiteSpace(paginationRequest.SearchTerm))
        {
            var searchTerm = paginationRequest.SearchTerm.Trim().ToLower();
            query = query.Where(p =>
                 p.Name.ToLower().Contains(searchTerm) ||
                 p.Description.ToLower().Contains(searchTerm)
            );
        }

        // Aplicar filtro por categoría
        if (paginationRequest.CategoryId.HasValue && paginationRequest.CategoryId.Value > 0)
        {
            query = query.Where(p => p.CategoryId == paginationRequest.CategoryId);
        }

        // Aplicar filtro por precio máximo
        if (paginationRequest.PriceMax.HasValue && paginationRequest.PriceMax.Value > 0)
        {
            query = query.Where(p => p.Price <= paginationRequest.PriceMax.Value);
        }

        // Aplicar filtro por precio mínimo
        if (paginationRequest.PriceMin.HasValue && paginationRequest.PriceMin.Value > 0)
        {
            query = query.Where(p => p.Price >= paginationRequest.PriceMin.Value);
        }

        // Aplicar filtro por productos destacados
        if (paginationRequest.Featured.HasValue)
        {
            query = query.Where(p => p.Featured == paginationRequest.Featured.Value);
        }

        query = query.OrderBy(p => p.Id);


        return await query.ToPagedListAsync(paginationRequest.PageNumber, paginationRequest.PageSize);

    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
}
