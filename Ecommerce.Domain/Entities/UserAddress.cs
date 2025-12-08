namespace Ecommerce.Domain.Entities;

public class UserAddress
{

    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string ShippingStreet { get; private set; } = string.Empty;
    public string ShippingCity { get; private set; } = string.Empty;
    public string ShippingState { get; private set; } = string.Empty;
    public string ShippingZipCode { get; private set; } = string.Empty;
    public string ShippingCountry { get; private set; } = string.Empty;
    public string BillingStreet { get; private set; } = string.Empty;
    public string BillingCity { get; private set; } = string.Empty;
    public string BillingState { get; private set; } = string.Empty;
    public string BillingZipCode { get; private set; } = string.Empty;
    public string BillingCountry { get; private set; } = string.Empty;
    public bool UseSameAddressForBilling { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public virtual User? User { get; private set; }

    public UserAddress() { }

}
