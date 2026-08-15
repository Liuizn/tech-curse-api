using MediatR;
using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Features.Courses.Commands.CreateCourse;

public record CreateCourseCommand(string Titulo, string Descricao, string Categoria, int CargaHoraria) : IRequest<CourseOutputDto>;
