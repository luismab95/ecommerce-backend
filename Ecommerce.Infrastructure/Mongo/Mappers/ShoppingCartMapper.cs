using Ecommerce.Infrastructure.Mongo.Documents;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Infrastructure.Mongo.Mappers;

public static class ShoppingCartMapper
{
    public static ShoppingCart ToDomain(ShoppingCartDocument doc)
    {
        var items = doc.Items.Select(i => CartItem.Create(i.ProductId, i.Quantity)).ToList();
        return ShoppingCart.Create(doc.UserId, items);
    }

    public static ShoppingCartDocument ToDocument(ShoppingCart entity)
    {
        var items = entity.Items.Select(i => CartItemDocument.Create(i.ProductId, i.Quantity)).ToList();
        return ShoppingCartDocument.Create(entity.UserId, items);
    }
}
