using Ecommerce.Domain.DTOs.Pagination;
using Ecommerce.Domain.DTOs.Orders;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces.Repositories;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddOrderWithTransactionAsync(Order order)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {

            var productIds = order.OrderItems.Select(i => i.ProductId).ToList();
            var productsInOrder = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            foreach (var item in order.OrderItems)
            {
                var product = productsInOrder[item.ProductId];
                product = Product.UpdateStock(product, item.Quantity);
            }

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task CancelAsync(Order order, List<OrderItem> orderItems)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        var productIds = orderItems.Select(i => i.ProductId).ToList();
        var productsInOrder = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        try
        {

            foreach (var item in orderItems)
            {
                var product = productsInOrder[item.ProductId];
                product = Product.ReturnStock(product, item.Quantity);
            }

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Order?> GetByIdAsync(int orderId)
    {
        return await _context.Orders
            .Include(o => o.OrderStatus)
            .Include(o => o.OrderAddress)
            .Include(o => o.OrderPayment)
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<PaginationResponse<Order>> GetOrderAsync(GetOrdersWithFiltersRequest paginationRequest)
    {
        var query = _context.Orders
            .Include(o => o.OrderStatus)
            .Include(o => o.OrderItems)
            .Include(o => o.User)
            .AsQueryable();

        // Filtrar por userId solo si no es nulo
        if (paginationRequest.UserId.HasValue)
        {
            query = query.Where(o => o.UserId == paginationRequest.UserId.Value);
        }

        // Aplicar búsqueda si searchTerm no es nulo o vacío
        if (!string.IsNullOrWhiteSpace(paginationRequest.SearchTerm))
        {
            var searchTerm = paginationRequest.SearchTerm.Trim().ToLower();
            query = query.Where(o =>
                o.OrderNumber.ToLower().Contains(searchTerm)
            );
        }

        query = query.OrderByDescending(o => o.Id);

        return await query.ToPagedListAsync(paginationRequest.PageNumber, paginationRequest.PageSize);
    }

    public async Task UpdateAsync(OrderStatus orderStatus)
    {
        _context.OrderStatuses.Update(orderStatus);
        await _context.SaveChangesAsync();
    }
}
