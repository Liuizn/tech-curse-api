using MediatR;

namespace tech_curse_api.src.Application.Features.Students.Commands.UpdateStudent;

public record UpdateStudentCommand(int Id, string Nome) : IRequest;
