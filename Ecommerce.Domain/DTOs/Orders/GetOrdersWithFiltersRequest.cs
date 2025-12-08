using Ecommerce.Domain.DTOs.Pagination;

namespace Ecommerce.Domain.DTOs.Orders;

public class GetOrdersWithFiltersRequest : PaginationRequest
{
    public int? UserId { get; set; }
}
