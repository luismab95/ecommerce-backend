using Ecommerce.Domain.DTOs.Orders;

namespace Ecommerce.Domain.Entities;

public class OrderItem
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public int ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public decimal Subtotal { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public virtual Order? Order { get; private set; }
    public virtual Product? Product { get; private set; }


    private OrderItem() { }


    public static OrderItem Create(OrderItemDto orderItem)
    {
        return new OrderItem()
        {
            ProductId = orderItem.ProductId,
            ProductName = orderItem.ProductName,
            Price = orderItem.Price,
            Quantity = orderItem.Quantity,
            Subtotal = orderItem.Price * orderItem.Quantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
    }

    public static object ToSafeResponse(OrderItem orderItem)
    {
        return new
        {
            orderItem.ProductId,
            orderItem.ProductName,
            orderItem.Price,
            orderItem.Quantity,
            Subtotal = orderItem.Price * orderItem.Quantity,
        };
    }

}
