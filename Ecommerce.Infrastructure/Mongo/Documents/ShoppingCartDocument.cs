using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Ecommerce.Infrastructure.Mongo.Documents;

public class ShoppingCartDocument
{
    [BsonId]
    public ObjectId? Id { get; set; } = ObjectId.GenerateNewId();
    public int UserId { get; set; } = 0;
    public List<CartItemDocument> Items { get; set; } = new List<CartItemDocument>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


    public static ShoppingCartDocument Create(int userId, List<CartItemDocument> items)
    {
        return new ShoppingCartDocument
        {
            UserId = userId,
            Items = items,
        };
    }

}

public class CartItemDocument
{
    public int ProductId { get; set; } = default!;
    public int Quantity { get; set; }

    public static CartItemDocument Create(int productId, int quantity)
    {
        return new CartItemDocument
        {
            ProductId = productId,
            Quantity = quantity,
        };
    }
}
