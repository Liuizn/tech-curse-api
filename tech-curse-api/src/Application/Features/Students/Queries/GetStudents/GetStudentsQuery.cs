using MediatR;
using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Features.Students.Queries.GetStudents;

public record GetStudentsQuery(PaginationParamsDto SearchParams) : IRequest<PagedResultDto<StudentOutputDto>>;
