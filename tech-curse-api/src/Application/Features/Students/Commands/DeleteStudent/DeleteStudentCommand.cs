using MediatR;

namespace tech_curse_api.src.Application.Features.Students.Commands.DeleteStudent;

public record DeleteStudentCommand(int Id) : IRequest;
