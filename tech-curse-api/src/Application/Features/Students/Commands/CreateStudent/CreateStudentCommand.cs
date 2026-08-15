using MediatR;
using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Features.Students.Commands.CreateStudent;

public record CreateStudentCommand(string Nome, string Email) : IRequest<StudentOutputDto>;
