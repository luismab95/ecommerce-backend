namespace Ecommerce.Domain.Entities;

public class User
{

    public int Id { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public virtual ICollection<Session>? Sessions { get; private set; }
    public virtual ICollection<WishList>? WishLists { get; private set; }
    public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();
    public virtual UserAddress? UserAddress { get; private set; }

    private User() { }

    public static User Create(string email, string passwordHash, string firstName, string lastName, string phone)
    {
        return new User
        {
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            Role = "Cliente",
            Phone = phone,
            IsActive = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };
    }

    public static User Update(User user, string firstName, string lastName, string email, string phone)
    {
        user.FirstName = firstName;
        user.LastName = lastName;
        user.Email = email;
        user.Phone = phone;
        user.UpdatedAt = DateTime.Now;
        return user;
    }

    public static User UpdateRole(User user)
    {
        user.Role = user.Role == "Cliente" ? "Administrador" : "Cliente";
        user.UpdatedAt = DateTime.Now;
        return user;
    }

    public static User Delete(User user)
    {
        user.IsActive = false;
        user.UpdatedAt = DateTime.Now;
        return user;
    }

    public static object ToSafeResponse(User user)
    {
        return new
        {
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Phone,
            user.Role,
            user.IsActive,
            user.UserAddress?.UseSameAddressForBilling,
            shippingAddress = new
            {
                City = user.UserAddress?.ShippingCity,
                Country = user.UserAddress?.ShippingCountry,
                State = user.UserAddress?.ShippingState,
                Code = user.UserAddress?.ShippingZipCode,
                Street = user.UserAddress?.ShippingStreet
            },
            billingAddress = new
            {
                City = user.UserAddress?.BillingCity,
                Country = user.UserAddress?.BillingCountry,
                State = user.UserAddress?.BillingState,
                Code = user.UserAddress?.BillingZipCode,
                Street = user.UserAddress?.BillingStreet
            },
            CreatedAt = user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdatedAt = user.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
        };
    }

    public static User ResetPassword(User user, string passwordHash)
    {

        user.PasswordHash = passwordHash;
        user.UpdatedAt = DateTime.Now;
        return user;
    }
}
