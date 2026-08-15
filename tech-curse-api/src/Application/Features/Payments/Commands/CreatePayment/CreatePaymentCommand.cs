using MediatR;
using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Features.Payments.Commands.CreatePayment;

public record CreatePaymentCommand(int EnrollmentId, decimal Amount) : IRequest<PaymentOutputDto>;
