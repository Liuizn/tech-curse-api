using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Domain.Entities; 

namespace tech_curse_api.src.Application.Interfaces;

public interface IStudentRepository
{
    Task<(IEnumerable<Student> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto searchParams);
    Task<IEnumerable<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(int id);
    Task<Student?> GetByEmailAsync(string email);
    Task<IEnumerable<CourseStudentOutputDto>> GetCoursesAsync(Student student);
    Task AddAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(Student student);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> StudentIsActiveAsync(Student student);
}