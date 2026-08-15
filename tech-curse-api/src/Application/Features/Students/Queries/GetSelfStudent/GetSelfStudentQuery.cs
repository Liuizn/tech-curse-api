using MediatR;
using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Features.Students.Queries.GetSelfStudent;

public record GetSelfStudentQuery() : IRequest<StudentOutputDto>;
