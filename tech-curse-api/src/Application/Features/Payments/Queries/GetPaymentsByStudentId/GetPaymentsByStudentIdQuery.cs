using MediatR;
using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Features.Payments.Queries.GetPaymentsByStudentId;

public record GetPaymentsByStudentIdQuery(int StudentId, PaginationParamsDto SearchParams) : IRequest<PagedResultDto<PaymentOutputDto>>;
