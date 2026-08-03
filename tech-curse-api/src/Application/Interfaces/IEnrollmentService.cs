using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Interfaces;

public interface IEnrollmentService
{
    Task<bool> CreateAsync(EnrollmentInputDto input);
}
