namespace Ecommerce.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Ecommerce.Domain.DTOs.Pagination;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces.Repositories;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Extensions;

public class UserRepository : IUserRepository
{

    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
    }

    public async Task<PaginationResponse<User>> GetUsersAsync(PaginationRequest paginationRequest)
    {
        var query = _context.Users.Where(u => u.IsActive);

        // Aplicar búsqueda si searchTerm no es nulo o vacío
        if (!string.IsNullOrWhiteSpace(paginationRequest.SearchTerm))
        {
            var searchTerm = paginationRequest.SearchTerm.Trim().ToLower();
            query = query.Where(u =>
                u.FirstName.ToLower().Contains(searchTerm) ||
                u.LastName.ToLower().Contains(searchTerm) ||
                u.Email.ToLower().Contains(searchTerm)
            );
        }

        query = query.OrderBy(u => u.Id);

        return await query.ToPagedListAsync(paginationRequest.PageNumber, paginationRequest.PageSize);
    }


    public async Task<User?> GetByIdAsync(int userId)
    {
        return await _context.Users
            .Include(ua => ua.UserAddress)
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email && u.IsActive);
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task AddProductWishListAsync(WishList newWishlist)
    {
        await _context.WishLists.AddAsync(newWishlist);
        await _context.SaveChangesAsync();
    }


    public async Task UpdateProductWishListAsync(WishList updateWishlist)
    {
        _context.WishLists.Update(updateWishlist);
        await _context.SaveChangesAsync();
    }

    public async Task<List<WishList>> GetWishListByUserIdAsync(int userId)
    {
        return await _context.WishLists
            .Include(w => w.Product!)
                .ThenInclude(p => p.Images)
            .Where(w => w.IsActive && w.UserId == userId)
            .OrderBy(w => w.UpdatedAt)
            .ToListAsync();
    }

    public async Task CreateUserAddressAsync(UserAddress userAddress)
    {
        await _context.UserAddress.AddAsync(userAddress);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAddressAsync(UserAddress userAddress)
    {
        _context.UserAddress.Update(userAddress);
        await _context.SaveChangesAsync();
    }

    public async Task<WishList?> GetProductInWishListAsync(int userId, int productId)
    {
        return await _context.WishLists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
    }

}
