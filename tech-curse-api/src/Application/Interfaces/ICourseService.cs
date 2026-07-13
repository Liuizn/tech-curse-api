using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Interfaces;

public interface ICourseService
{
    Task<CourseOutputDto> CreateAsync(CoursePostDto dto);
    Task<IEnumerable<CourseOutputDto>> GetAllAsync();
    Task<CourseOutputDto?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(CoursePutDto dto);
    Task<bool> DeleteAsync(int id);
}
