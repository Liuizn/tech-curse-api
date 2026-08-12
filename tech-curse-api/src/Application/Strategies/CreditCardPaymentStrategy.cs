using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;
using tech_curse_api.src.Domain.Entities;
using tech_curse_api.src.Domain.Enums;

namespace tech_curse_api.src.Application.Strategies;

public class CreditCardPaymentStrategy : IPaymentStrategy
{
    public PaymentMethodType PaymentMethodType => PaymentMethodType.CreditCard;

    public async Task<GatewayResponse> ProcessAsync(Payment payment, IPaymentGatewayAdapter gateway, string idempotencyKey, CancellationToken cancellationToken)
    {
        // Para cartão, geralmente chamamos o Create que já debita o valor
        return await gateway.CreateTransactionAsync(payment.Amount, idempotencyKey, cancellationToken);
    }
}
