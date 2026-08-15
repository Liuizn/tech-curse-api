using MediatR;
using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Features.Payments.Queries.GetPaymentsByEnrollmentId;

public record GetPaymentsByEnrollmentIdQuery(int EnrollmentId) : IRequest<IEnumerable<PaymentOutputDto>>;
