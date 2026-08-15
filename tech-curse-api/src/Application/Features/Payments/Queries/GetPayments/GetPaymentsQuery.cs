using MediatR;
using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Features.Payments.Queries.GetPayments;

public record GetPaymentsQuery(PaginationParamsDto SearchParams) : IRequest<PagedResultDto<PaymentOutputDto>>;
