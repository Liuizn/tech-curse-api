using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Domain.Entities;
using tech_curse_api.src.Domain.Enums;

namespace tech_curse_api.src.Application.Interfaces;

public interface IPaymentStrategy
{
    PaymentMethodType PaymentMethodType { get; }
    Task<GatewayResponse> ProcessAsync(Payment payment, IPaymentGatewayAdapter gateway, string idempotencyKey, CancellationToken cancellationToken);
}
