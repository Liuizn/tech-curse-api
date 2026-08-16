using MediatR;

namespace tech_curse_api.src.Application.Features.Courses.Commands.UpdateCourse;

public record UpdateCourseCommand(
    int Id,
    string Titulo,
    string Descricao,
    string Categoria,
    int CargaHoraria
) : IRequest<Unit>;
