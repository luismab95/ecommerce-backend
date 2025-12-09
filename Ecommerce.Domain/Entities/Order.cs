using System.Security.Cryptography;

namespace Ecommerce.Domain.Entities;

public class Order
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public decimal Subtotal { get; private set; }
    public decimal Tax { get; private set; } = 0;
    public decimal ShippingCost { get; private set; } = 0;
    public decimal Discount { get; private set; } = 0;
    public decimal Total { get; private set; } = 0;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public virtual ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
    public virtual OrderPayment? OrderPayment { get; private set; }
    public virtual OrderStatus? OrderStatus { get; private set; }
    public virtual OrderAddress? OrderAddress { get; private set; }
    public virtual User? User { get; private set; }

    private Order() { }

    public static Order Create(int userId, List<OrderItem> orderItems, OrderAddress orderAddress, OrderPayment paymentInfo, OrderStatus orderStatus, decimal total, decimal subtotal, decimal tax, decimal discount, decimal shippingCost)
    {
        return new Order
        {

            UserId = userId,
            OrderNumber = GenerateOrdenNumber(),
            OrderAddress = orderAddress,
            OrderPayment = paymentInfo,
            OrderStatus = orderStatus,
            OrderItems = orderItems,
            Subtotal = subtotal,
            Tax = tax,
            Discount = discount,
            ShippingCost = shippingCost,
            Total = total,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
    }

    public static Order Cancel(Order order, OrderStatus orderStatus, OrderPayment orderPayment)
    {
        order.UpdatedAt = DateTime.UtcNow;
        order.OrderStatus = orderStatus;
        order.OrderPayment = orderPayment;
        return order;
    }



    private static string GenerateOrdenNumber()
    {
        var date = DateTime.Now;
        var randomPart = GenerarRandomString(5);
        string numeroOrden = $"ORD{date.Year}{randomPart}";
        return numeroOrden;
    }

    private static string GenerarRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);

        var result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = chars[bytes[i] % chars.Length];
        }

        return new string(result);
    }


    public static object ToSafeResponse(Order order)
    {
        return new
        {
            order.Id,
            order.OrderNumber,
            order.UserId,
            Status = order?.OrderStatus?.Status.ToString(),
            order?.Total,
            order?.User?.Email,
            order?.User?.FirstName,
            order?.User?.LastName,
            Items = ToSafeResponseOrderItem(order?.OrderItems.ToList<OrderItem>()!),
            CreatedAt = order?.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdatedAt = order?.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
        };
    }

    private static List<object> ToSafeResponseOrderItem(List<OrderItem> orderItems)
    {
        var items = new List<object>();
        orderItems.ForEach(o =>
        {
            items.Add(OrderItem.ToSafeResponse(o));
        });
        return items;
    }

    public static object ToSafeResponseDetail(Order order)
    {

        var baseResponse = ToSafeResponse(order);
        dynamic baseDynamic = baseResponse;


        var orderAddress = OrderAddress.ToSafeResponse(order.OrderAddress!);

        return new
        {
            baseDynamic.Id,
            baseDynamic.OrderNumber,
            baseDynamic.UserId,
            baseDynamic.Status,
            baseDynamic.Total,
            baseDynamic.Items,
            baseDynamic.CreatedAt,
            baseDynamic.UpdatedAt,
            order.Tax,
            order.Subtotal,
            order.ShippingCost,
            BillingAddress = new
            {
                Street = orderAddress.BillingStreet,
                Country = orderAddress.BillingCountry,
                State = orderAddress.BillingState,
                City = orderAddress.BillingCity,
                Code = orderAddress.BillingZipCode,
                Email = orderAddress.ShippingEmail
            },
            ShippingAddress = new
            {
                Street = orderAddress.ShippingStreet,
                Country = orderAddress.ShippingCountry,
                State = orderAddress.ShippingState,
                City = orderAddress.ShippingCity,
                Code = orderAddress.ShippingZipCode,
                Email = orderAddress.ShippingEmail,
                Phone = orderAddress.ShippingPhone,
            },
            PaymentInfo = OrderPayment.ToSafeResponse(order.OrderPayment!)
        };
    }
}
