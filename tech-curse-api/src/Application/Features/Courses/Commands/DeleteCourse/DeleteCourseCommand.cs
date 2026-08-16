using MediatR;

namespace tech_curse_api.src.Application.Features.Courses.Commands.DeleteCourse;

public record DeleteCourseCommand(int Id) : IRequest<Unit>;
