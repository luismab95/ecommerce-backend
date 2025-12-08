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

    public static Image Create(string path, int productId)
    {
        return new Image
        {
            Path = path,
            ProductId = productId,
            IsActive = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
    }

    public static Image UpdatePath(Image image, string baseUrl)
    {
        image.Path = baseUrl + image.Path;
        image.UpdatedAt = DateTime.Now;
        return image;

    }

    public static Image Delete(Image image)
    {
        image.IsActive = false;
        image.UpdatedAt = DateTime.Now;
        return image;
    }


    public static object ToSafeResponse(Image image)
    {
        return new
        {
            image.Id,
            image.Path,
            CreatedAt = image.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdatedAt = image.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
        };
    }
}
