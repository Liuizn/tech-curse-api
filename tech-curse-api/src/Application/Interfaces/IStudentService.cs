using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Interfaces;

public interface IStudentService
{
    Task<StudentOutputDto> CreateAsync(StudentPostDto dto);
    Task<PagedResultDto<StudentOutputDto>> GetPagedAsync(PaginationParamsDto searchParams);
    Task<StudentOutputDto?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(StudentPutDto dto);
    Task<bool> DeleteAsync(int id);
}
