using System.Text.Json.Serialization;

namespace Ecommerce.Domain.Entities;

public class ShoppingCart
{

    public string? Id { get; private set; }
    public int UserId { get; private set; } = 0;
    public List<CartItem> Items { get; private set; } = new List<CartItem>();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;



    [JsonConstructor]
    public ShoppingCart(string? id, int userId, List<CartItem> items, DateTime createdAt, DateTime updatedAt)
    {
        Id = id;
        UserId = userId;
        Items = items;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static ShoppingCart Create(int userId, List<CartItem> items)
    {
        return new ShoppingCart(null, userId, items, DateTime.UtcNow, DateTime.UtcNow);
    }

}

public class CartItem
{
    public int ProductId { get; private set; } = default!;
    public int Quantity { get; private set; }

    [JsonConstructor]
    public CartItem(int productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }

    public static CartItem Create(int productId, int quantity) =>
       new CartItem(productId, quantity);

}
public class ShoppingCartItem
{
    public dynamic Product { get; private set; } = default!;
    public int Quantity { get; private set; }

    public static ShoppingCartItem Create(dynamic product, int quantity)
    {
        return new ShoppingCartItem
        {
            Product = product,
            Quantity = quantity,
        };
    }

}
