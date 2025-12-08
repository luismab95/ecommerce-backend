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


}
