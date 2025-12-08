namespace Ecommerce.Domain.Entities;

public class Order
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public decimal Subtotal { get; private set; }
    public decimal Tax { get; private set; } = 0;
    public decimal ShippingCost { get; private set; } = 0;
    public decimal Discount { get; private set; } = 0;
    public decimal Total { get; private set; } = 0;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public virtual ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
    public virtual OrderPayment? OrderPayment { get; private set; }
    public virtual OrderStatus? OrderStatus { get; private set; }
    public virtual OrderAddress? OrderAddress { get; private set; }
    public virtual User? User { get; private set; }

    private Order() { }


}
