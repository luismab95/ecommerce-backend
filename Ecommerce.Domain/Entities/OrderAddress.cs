using Ecommerce.Domain.DTOs.Orders;

namespace Ecommerce.Domain.Entities;

public class OrderAddress
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public string ShippingStreet { get; private set; } = string.Empty;
    public string ShippingCity { get; private set; } = string.Empty;
    public string ShippingState { get; private set; } = string.Empty;
    public string ShippingZipCode { get; private set; } = string.Empty;
    public string ShippingCountry { get; private set; } = string.Empty;
    public string ShippingPhone { get; private set; } = string.Empty;
    public string ShippingEmail { get; private set; } = string.Empty;
    public string BillingStreet { get; private set; } = string.Empty;
    public string BillingCity { get; private set; } = string.Empty;
    public string BillingState { get; private set; } = string.Empty;
    public string BillingZipCode { get; private set; } = string.Empty;
    public string BillingCountry { get; private set; } = string.Empty;
    public bool UseSameAddressForBilling { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public virtual Order? Order { get; private set; }

    private OrderAddress() { }

    public static OrderAddress Create(AddressDto shippingAddress, AddressDto billingAddress)
    {
        var orderAddress = new OrderAddress()
        {
            ShippingStreet = shippingAddress.Street,
            ShippingCity = shippingAddress.City,
            ShippingState = shippingAddress.State,
            ShippingZipCode = shippingAddress.Code,
            ShippingCountry = shippingAddress.Country,
            ShippingEmail = shippingAddress.Email!,
            ShippingPhone = shippingAddress.Phone!,
            BillingStreet = billingAddress.Street,
            BillingCity = billingAddress.City,
            BillingState = billingAddress.State,
            BillingCountry = billingAddress.Country,
            BillingZipCode = billingAddress.Code,
            UseSameAddressForBilling = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        if (orderAddress.AreAddressesEqual())
        {
            orderAddress.UseSameAddressForBilling = true;
        }

        return orderAddress;
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


    public static dynamic ToSafeResponse(OrderAddress orderAddress)
    {
        return new
        {
            orderAddress.ShippingStreet,
            orderAddress.ShippingCountry,
            orderAddress.ShippingState,
            orderAddress.ShippingCity,
            orderAddress.ShippingZipCode,
            orderAddress.ShippingEmail,
            orderAddress.ShippingPhone,
            orderAddress.BillingStreet,
            orderAddress.BillingState,
            orderAddress.BillingCountry,
            orderAddress.BillingCity,
            orderAddress.BillingZipCode,
        };
    }

}
