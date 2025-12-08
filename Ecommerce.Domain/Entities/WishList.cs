namespace Ecommerce.Domain.Entities;

public class WishList
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public int UserId { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Propiedad de navegación para la relación con Product y User
    public virtual Product? Product { get; private set; }
    public virtual User? User { get; private set; }

    private WishList() { }

}
