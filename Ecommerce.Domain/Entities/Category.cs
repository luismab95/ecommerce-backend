namespace Ecommerce.Domain.Entities;

public class Category
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public DateTime UpdatedAt { get; private set; }

    public virtual ICollection<Product>? Products { get; private set; }

    private Category() { }

    public static Category Create(string name, string description)
    {
        return new Category
        {
            Name = name,
            Description = description,
            IsActive = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };
    }

    public static object ToSafeResponse(Category category)
    {
        return new
        {
            category.Id,
            category.Name,
            category.Description,
            category.IsActive,
            CreatedAt = category.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdatedAt = category.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
        };
    }

    public static Category Update(Category category, string name, string description)
    {
        category.Name = name;
        category.Description = description;
        category.UpdatedAt = DateTime.Now;
        return category;
    }


    public static Category Delete(Category category)
    {
        category.IsActive = false;
        category.UpdatedAt = DateTime.Now;
        return category;
    }

}
