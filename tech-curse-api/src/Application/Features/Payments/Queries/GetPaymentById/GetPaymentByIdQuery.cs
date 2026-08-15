using MediatR;
using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Features.Payments.Queries.GetPaymentById;

public record GetPaymentByIdQuery(int Id) : IRequest<PaymentOutputDto>;
