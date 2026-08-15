using MediatR;
using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Features.Courses.Queries.GetCourseById;

public record GetCourseByIdQuery(int Id) : IRequest<CourseOutputDto>;
