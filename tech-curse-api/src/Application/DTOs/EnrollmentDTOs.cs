using System.ComponentModel.DataAnnotations;

namespace tech_curse_api.src.Application.DTOs;
public record EnrollmentInputDto([Required] int CourseId, int StudentId);