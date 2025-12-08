namespace Ecommerce.Domain.Entities;

public class Product
{
    public int Id { get; private set; }
    public int CategoryId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; } = 0;
    public int Stock { get; private set; } = 0;
    public bool Featured { get; private set; } = false;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public DateTime UpdatedAt { get; private set; }


    public ICollection<Image> Images { get; private set; } = new List<Image>();
    public virtual ICollection<WishList>? WishLists { get; private set; }
    public virtual ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
    public virtual Category? Category { get; private set; }


    private Product() { }

}