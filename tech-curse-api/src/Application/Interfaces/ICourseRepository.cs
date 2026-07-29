using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Domain.Entities; 

namespace tech_curse_api.src.Application.Interfaces;

public interface ICourseRepository
{
    Task<(IEnumerable<Course> Items, int TotalCount)> GetPagedAsync(CoursePaginationParamsDto searchParams);
    Task<IEnumerable<Course>> GetAllAsync();
    Task<Course?> GetByIdAsync(int id);
    Task AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(Course course);
}