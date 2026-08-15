using MediatR;
using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Features.Payments.Commands.RefundPayment;

public record RefundPaymentCommand(int PaymentId, string IdempotencyKey) : IRequest<RefundPaymentOutputDto>;
