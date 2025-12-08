using Ecommerce.Domain.DTOs.Orders;
using Ecommerce.Domain.DTOs.Pagination;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int orderId);
    Task<PaginationResponse<Order>> GetOrderAsync(GetOrdersWithFiltersRequest paginationRequest);
    Task AddOrderWithTransactionAsync(Order order);
    Task UpdateAsync(OrderStatus orderStatus);
    Task CancelAsync(Order order, List<OrderItem> orderItems);
}
