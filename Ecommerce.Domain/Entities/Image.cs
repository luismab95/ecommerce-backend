namespace Ecommerce.Domain.Entities;

public class Image
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public string Path { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }


    // Propiedad de navegación para la relación con Product
    public virtual Product? Product { get; private set; }

    private Image() { }


}
