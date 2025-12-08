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

}
