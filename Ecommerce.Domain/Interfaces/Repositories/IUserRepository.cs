using Ecommerce.Domain.DTOs.Pagination;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int userId);
    Task<PaginationResponse<User>> GetUsersAsync(PaginationRequest paginationRequest);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task CreateUserAddressAsync(UserAddress userAddress);
    Task UpdateUserAddressAsync(UserAddress userAddress);
    Task<bool> ExistsByEmailAsync(string email);
    Task<List<WishList>> GetWishListByUserIdAsync(int userId);
    Task<WishList?> GetProductInWishListAsync(int userId, int productId);
    Task AddProductWishListAsync(WishList newWishlist);
    Task UpdateProductWishListAsync(WishList updateWishlist);

}
