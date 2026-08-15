using MediatR;
using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Domain.Enums;

namespace tech_curse_api.src.Application.Features.Payments.Commands.ProcessPayment;

public record ProcessPaymentCommand(int PaymentId, PaymentMethodType Type, string IdempotencyKey) : IRequest<ProcessPaymentOutputDto>;
