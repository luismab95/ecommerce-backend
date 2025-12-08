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

}
