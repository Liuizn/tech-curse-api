using MediatR;
using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Features.Students.Queries.GetStudentEnrollments;

public record GetStudentEnrollmentsQuery(int Id) : IRequest<IEnumerable<CourseStudentOutputDto>>;
