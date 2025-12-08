using Ecommerce.Domain.DTOs.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static async Task<PaginationResponse<T>> ToPagedListAsync<T>(
        this IQueryable<T> source,
        int pageNumber,
        int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginationResponse<T>(items, count, pageNumber, pageSize);
    }
}