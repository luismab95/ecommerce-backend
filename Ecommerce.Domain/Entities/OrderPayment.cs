using Ecommerce.Domain.DTOs.Orders;
using System.Security.Cryptography;

namespace Ecommerce.Domain.Entities;

public class OrderPayment
{
    public enum PaymentStatus
    {
        Pending,
        Processing,
        Completed,
        Failed,
        Refunded,
        PartiallyRefunded
    }

    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public string PaymentGatewayId { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string CardHolderName { get; private set; } = string.Empty;
    public string CardLastFour { get; private set; } = string.Empty;
    public string Method { get; private set; } = string.Empty;
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public DateTime? PaidAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public virtual Order? Order { get; private set; }

    private OrderPayment() { }


    public static OrderPayment Create(PaymentInfoDto orderPayment, decimal amount)
    {
        return new OrderPayment()
        {
            PaymentGatewayId = GenerarRandomString(12),
            Status = PaymentStatus.Completed,
            PaidAt = DateTime.UtcNow,
            CardHolderName = orderPayment.CardHolderName,
            CardLastFour = orderPayment.CardLastFour,
            Method = orderPayment.Method,
            Amount = amount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
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

    public static OrderPayment Cancel(OrderPayment orderPayment)
    {
        orderPayment.Status = PaymentStatus.Refunded;
        orderPayment.UpdatedAt = DateTime.UtcNow;
        return orderPayment;
    }

    public static object ToSafeResponse(OrderPayment orderPayment)
    {
        return new
        {
            orderPayment.CardLastFour,
            orderPayment.CardHolderName,
            orderPayment.Method
        };
    }

}
