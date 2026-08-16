using MediatR;
using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Features.Students.Queries.GetStudentById;

public record GetStudentByIdQuery(int Id) : IRequest<StudentOutputDto>;
