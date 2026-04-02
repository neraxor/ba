using OrderSystem.Domain.ValueObjects;

namespace OrderSystem.Application.Ports;

public interface IPaymentGateway
{
    Task<PaymentResult> ProcessPaymentAsync(Guid orderId, Money amount);
}

public record PaymentResult(bool Success, string? TransactionId, string? ErrorMessage);