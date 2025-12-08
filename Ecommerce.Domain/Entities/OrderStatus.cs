namespace Ecommerce.Domain.Entities;

public class OrderStatus
{

    public enum OrderStatusType
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public OrderStatusType Status { get; private set; } = OrderStatusType.Pending;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public virtual Order? Order { get; private set; }


    private OrderStatus() { }

}