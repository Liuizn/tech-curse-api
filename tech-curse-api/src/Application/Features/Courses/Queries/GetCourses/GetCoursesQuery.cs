using MediatR;
using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Features.Courses.Queries.GetCourses;

public record GetCoursesQuery(CoursePaginationParamsDto SearchParams) : IRequest<PagedResultDto<CourseOutputDto>>;
