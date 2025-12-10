using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Interfaces.Repositories;

public interface IShoppingCartRepository
{
    Task<ShoppingCart?> GetByIdAsync(int userId);
    Task<bool> CreateOrUpdateAsync(ShoppingCart shoppingCart, int userId);
    Task<bool> DeleteAsync(string shoppingCartId);
}
