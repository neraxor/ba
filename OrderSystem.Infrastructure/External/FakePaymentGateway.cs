using OrderSystem.Application.Ports;
using OrderSystem.Domain.ValueObjects;

namespace OrderSystem.Infrastructure.External;

public class FakePaymentGateway : IPaymentGateway
{
    public Task<PaymentResult> ProcessPaymentAsync(Guid orderId, Money amount)
    {
        // Simuliert erfolgreiche Zahlung
        var transactionId = Guid.NewGuid().ToString();
        return Task.FromResult(new PaymentResult(true, transactionId, null));
    }
}