using MediatR;

namespace tech_curse_api.src.Application.Features.Enrollments.Commands.CreateEnrollment;

public record CreateEnrollmentCommand(int StudentId, int CourseId) : IRequest;
