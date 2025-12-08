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

    public static UserAddress Create(
      int userId,
      string shippingStreet, string shippingCity, string shippingState,
      string shippingZipCode, string shippingCountry,
      string billingStreet, string billingCity,
      string billingState, string billingZipCode,
      string billingCountry)
    {
        var userAddress = new UserAddress
        {
            UserId = userId,
            ShippingStreet = shippingStreet,
            ShippingCity = shippingCity,
            ShippingState = shippingState,
            ShippingZipCode = shippingZipCode,
            ShippingCountry = shippingCountry,
            BillingStreet = billingStreet,
            BillingCity = billingCity,
            BillingState = billingState,
            BillingCountry = billingCountry,
            BillingZipCode = billingZipCode,
            UseSameAddressForBilling = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (userAddress.AreAddressesEqual())
        {
            userAddress.UseSameAddressForBilling = true;
        }

        return userAddress;
    }

    // Update address
    public static UserAddress Update(
        UserAddress userAddress,
        string shippingStreet, string shippingCity, string shippingState,
        string shippingZipCode, string shippingCountry,
        string billingStreet, string billingCity,
        string billingState, string billingZipCode,
        string billingCountry)
    {

        userAddress.ShippingStreet = shippingStreet;
        userAddress.ShippingCity = shippingCity;
        userAddress.ShippingState = shippingState;
        userAddress.ShippingZipCode = shippingZipCode;
        userAddress.ShippingCountry = shippingCountry;
        userAddress.BillingStreet = billingStreet;
        userAddress.BillingCity = billingCity;
        userAddress.BillingState = billingState;
        userAddress.BillingCountry = billingCountry;
        userAddress.BillingZipCode = billingZipCode;
        userAddress.UseSameAddressForBilling = false;
        userAddress.UpdatedAt = DateTime.UtcNow;


        if (userAddress.AreAddressesEqual())
        {
            userAddress.UseSameAddressForBilling = true;
        }

        return userAddress;
    }


    // Safe response
    public static object ToSafeResponse(UserAddress userAddress)
    {
        return new
        {
            userAddress.Id,
            userAddress.ShippingZipCode,
            userAddress.ShippingCountry,
            userAddress.ShippingCity,
            userAddress.ShippingState,
            userAddress.ShippingStreet,
            userAddress.BillingCity,
            userAddress.BillingCountry,
            userAddress.BillingZipCode,
            userAddress.BillingState,
            userAddress.BillingStreet,
            userAddress.UseSameAddressForBilling,
            CreatedAt = userAddress.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdatedAt = userAddress.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
        };
    }

    // Método para verificar si las direcciones son iguales
    public bool AreAddressesEqual()
    {
        return ShippingStreet == BillingStreet &&
               ShippingCity == BillingCity &&
               ShippingState == BillingState &&
               ShippingZipCode == BillingZipCode &&
               ShippingCountry == BillingCountry;
    }

}
