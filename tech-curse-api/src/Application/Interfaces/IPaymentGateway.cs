using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Domain.Entities;

namespace tech_curse_api.src.Application.Interfaces;

public interface IPaymentGatewayAdapter
{
    Task<GatewayResponse> CreateTransactionAsync(decimal amount, string idempotencyKey, CancellationToken cancellationToken);
    Task<GatewayResponse> ConfirmTransactionAsync(string ExternalTransactionId, CancellationToken cancellationToken);
    Task<GatewayResponse> RefundTransactionAsync(string ExternalTransactionId, CancellationToken cancellationToken, string idempotencyKey);
}
