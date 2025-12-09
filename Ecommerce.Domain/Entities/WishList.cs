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


    public static WishList Create(int productId, int userId)
    {
        return new WishList
        {
            UserId = userId,
            ProductId = productId,
            IsActive = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };
    }

    public static WishList SetProduct(WishList wishList, Product product)
    {
        wishList.Product = product;
        return wishList;
    }

    public static WishList ToggleStatus(WishList wishList)
    {
        wishList.IsActive = !wishList.IsActive;
        wishList.UpdatedAt = DateTime.Now;
        return wishList;
    }


    public static WishList Update(WishList wishList)
    {
        wishList.UpdatedAt = DateTime.Now;
        wishList.IsActive = !wishList.IsActive;
        return wishList;
    }

}
