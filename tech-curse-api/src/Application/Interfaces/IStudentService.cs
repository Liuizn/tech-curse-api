using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Domain.Entities;

namespace tech_curse_api.src.Application.Interfaces;

public interface IStudentService
{
    Task<StudentOutputDto> CreateAsync(StudentPostDto dto);
    Task<PagedResultDto<StudentOutputDto>> GetPagedAsync(PaginationParamsDto searchParams);
    Task<StudentOutputDto?> GetByIdAsync(int id);
    Task<StudentOutputDto?> GetSelfAsync();
    Task<IEnumerable<CourseStudentOutputDto>> GetCoursesAsync(int id);
    Task<bool> UpdateAsync(int id, StudentPutDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> StudentIsActiveAsync(Student student);
}
