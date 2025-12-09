using Ecommerce.Domain.DTOs.Pagination;
using Ecommerce.Domain.DTOs.Products;
using Ecommerce.Domain.DTOs.Users;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Application.UseCases.Users;

public class UserUseCases
{
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly IConfiguration _config;


    public UserUseCases(IUserRepository userRepository, IProductRepository productRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _productRepository = productRepository;
        _config = config;
    }

    public async Task<object> GetUserByIdAsync(int userId)
    {
        var findUser = await _userRepository.GetByIdAsync(userId) ??
            throw new InvalidOperationException("Usuario no encontrado.");

        return User.ToSafeResponse(findUser);
    }

    public async Task<object> GetUserWishlistAsync(int userId)
    {
        var result = await _userRepository.GetWishListByUserIdAsync(userId);

        var wishlists = new List<object>();
        result.ForEach(wishlist =>
        {
            if (wishlist != null && wishlist.Product!.Images != null)
            {
                string baseUrl = $"{_config["App:StaticUrl"]}";
                var imagesList = new List<Image>();

                wishlist.Product.Images.ToList().ForEach(image =>
                {
                    if (image.IsActive)
                        imagesList.Add(Image.UpdatePath(image, baseUrl));
                });

                var product = Product.SetImages(wishlist.Product, imagesList);
                wishlist = WishList.SetProduct(wishlist, product);
            }
            wishlists.Add(Product.ToSafeResponse(wishlist!.Product!));
        });



        return wishlists;
    }

    public async Task<string> AddProductWishListAsync(AddProductWishListRequest request, int userId)
    {

        var findProduct = await _productRepository.GetByIdAsync(request.ProductId) ??
            throw new InvalidOperationException("Producto no encontrado.");

        var findProductInWishlist = await _userRepository.GetProductInWishListAsync(userId, request.ProductId);

        if (findProductInWishlist == null)
        {
            var newProductWishlist = WishList.Create(request.ProductId, userId);
            await _userRepository.AddProductWishListAsync(newProductWishlist);
        }
        else
        {
            var updateProductWishlist = WishList.Update(findProductInWishlist);
            await _userRepository.UpdateProductWishListAsync(updateProductWishlist);
        }

        return $"Producto '{findProduct.Name}' {(findProductInWishlist == null ? "agregado" : findProductInWishlist.IsActive ? "agregado" : "quitado")} de la lista de deseados exitosamente.";
    }


    public async Task<object> GetUsersAsync(PaginationRequest request)
    {
        var result = await _userRepository.GetUsersAsync(request);

        var safeUserResponse = new List<object>();
        result.Items.ForEach(user =>
        {
            safeUserResponse.Add(User.ToSafeResponse(user));
        });

        return new
        {
            Items = safeUserResponse,
            result.TotalCount,
            result.PageNumber,
            result.PageSize,
            result.TotalPages,
            result.HasPreviousPage,
            result.HasNextPage,
        };
    }

    public async Task<string> UpdateUserAsync(int userId, UpdateUserRequest user)
    {
        var findUser = await _userRepository.GetByIdAsync(userId) ??
            throw new InvalidOperationException("Usuario no encontrado.");

        var updateUser = User.Update(findUser, user.FirstName, user.LastName, user.Email, user.Phone);
        await _userRepository.UpdateAsync(updateUser);

        return "Usuario actualizado exitosamente.";

    }


    public async Task<string> UpdateUserAddressAsync(int userId, UpdateUserAddressRequest request)
    {
        var findUser = await _userRepository.GetByIdAsync(userId) ??
            throw new InvalidOperationException("Usuario no encontrado.");

        if (findUser.UserAddress is null)
        {
            var createUser = UserAddress.Create(findUser.Id, request.ShippingAddress.Street, request.ShippingAddress.City, request.ShippingAddress.State, request.ShippingAddress.Code, request.ShippingAddress.Country, request.BillingAddress.Street, request.BillingAddress.City, request.BillingAddress.State, request.BillingAddress.Code, request.BillingAddress.Country);
            await _userRepository.CreateUserAddressAsync(createUser);

        }
        else
        {
            var updateUser = UserAddress.Update(findUser.UserAddress, request.ShippingAddress.Street, request.ShippingAddress.City, request.ShippingAddress.State, request.ShippingAddress.Code, request.ShippingAddress.Country, request.BillingAddress.Street, request.BillingAddress.City, request.BillingAddress.State, request.BillingAddress.Code, request.BillingAddress.Country);
            await _userRepository.UpdateUserAddressAsync(updateUser);
        }


        return "Dirección actualizada exitosamente.";

    }


    public async Task<string> UpdateRoleAsync(int userId)
    {
        var findUser = await _userRepository.GetByIdAsync(userId) ??
            throw new InvalidOperationException("Usuario no encontrado.");

        var updateUser = User.UpdateRole(findUser);
        await _userRepository.UpdateAsync(updateUser);

        return "Rol de usuario actualizado exitosamente.";
    }

    public async Task<string> DeleteUserAsync(int userId)
    {
        var findUser = await _userRepository.GetByIdAsync(userId) ??
            throw new InvalidOperationException("Usuario no encontrado.");

        if (findUser.Role != "Cliente")
        {
            throw new InvalidOperationException("No se puede eliminar a un Usuario administrador.");
        }

        var updateUser = User.Delete(findUser);
        await _userRepository.UpdateAsync(updateUser);

        return "Usuario eliminado exitosamente.";
    }

}
