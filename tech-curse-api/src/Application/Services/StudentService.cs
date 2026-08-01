using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using System.Security.Claims;
using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;
using tech_curse_api.src.Domain.Entities;
using tech_curse_api.src.Domain.Enums;

namespace tech_curse_api.src.Application.Services;
// UNDONE: Implement the UpdateAsync and DeleteAsync methods for the StudentService class, similar to the CourseService class, ensuring that they handle caching appropriately and return the correct types.
public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly ICacheService _cacheService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    private const string STUDENT_ITEM_PREFIX = "students:item:";
    private const string STUDENT_LIST_PREFIX = "students:list:";

    public StudentService(IStudentRepository studentRepository, ICacheService cacheService, UserManager<IdentityUser> userManager, ICurrentUserService currentUserService)
    {
        _studentRepository = studentRepository;
        _cacheService = cacheService;
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResultDto<StudentOutputDto>> GetPagedAsync(PaginationParamsDto searchParams)
    {
        var cacheKey = $"{STUDENT_ITEM_PREFIX}page:{searchParams.PageNumber}:size:{searchParams.PageSize}:sort:{searchParams.SortBy}_{searchParams.SortDirection}";

        var cachedResult = await _cacheService.GetAsync<PagedResultDto<StudentOutputDto>>(cacheKey);
        if (cachedResult != null)
        {
            return cachedResult;
        }

        var (courses, totalCount) = await _studentRepository.GetPagedAsync(searchParams);

        var dtos = courses.Select(c => new StudentOutputDto(c.StudentId, c.Nome, c.Email, c.DataCadastro, c.Enrollments));
        
        var result = new PagedResultDto<StudentOutputDto>(
            dtos,
            totalCount,
            searchParams.PageNumber,
            searchParams.PageSize
        );

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));

        return result;
    }

    protected async Task<bool> validateRoleAcess(string targetIdentityUserId)
    {
        var currentUserId = _currentUserService.GetUserId();
        var isAdmin = _currentUserService.IsInRole(UserRole.Admin);

        if (currentUserId != targetIdentityUserId && !isAdmin) return false;

        return true;
    }

    public async Task<StudentOutputDto?> GetByIdAsync(int id)
    {
        var cacheKey = $"{STUDENT_ITEM_PREFIX}{id}";
        
        var cachedStudent = await _cacheService.GetAsync<StudentOutputDto>(cacheKey);
        if (cachedStudent != null)
        {
            return cachedStudent;
        }

        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null) return null;

        if (await validateRoleAcess(student.IdentityUserId) == false) return null;

        var result = new StudentOutputDto(student.StudentId, student.Nome, student.Email, student.DataCadastro, student.Enrollments);

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));

        return result;
    }

    public async Task<StudentOutputDto> CreateAsync(StudentPostDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        var student = new Student
        {
            Nome = dto.Nome,
            Email = dto.Email,
            DataCadastro = dto.DataCadastro,
            IdentityUserId = user.Id,
            IdentityUser = user,
            Enrollments = new List<Enrollment>()
        };

        await _studentRepository.AddAsync(student);

        var result = new StudentOutputDto(student.StudentId, student.Nome, student.Email, student.DataCadastro, student.Enrollments);

        return result;
    }
    
    public async Task<bool> UpdateAsync(StudentPutDto dto)
    {
        var student = await _studentRepository.GetByIdAsync(dto.Id);

        if (student == null) return false;

        if (await validateRoleAcess(student.IdentityUserId) == false) return false;

        student.Nome = dto.Nome;

        await _studentRepository.UpdateAsync(student);

        var updatedDto = new StudentOutputDto(student.StudentId, student.Nome, student.Email, student.DataCadastro, student.Enrollments);
        await _cacheService.SetAsync($"{STUDENT_ITEM_PREFIX}{dto.Id}", updatedDto, TimeSpan.FromMinutes(15));

        await _cacheService.RemoveByPrefixAsync(STUDENT_LIST_PREFIX);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null) return false;

        student.IsDeleted = true;
        student.DeletedAt = DateTime.UtcNow;

        await _studentRepository.UpdateAsync(student);

        await _cacheService.RemoveAsync($"{STUDENT_ITEM_PREFIX}{id}");
        await _cacheService.RemoveByPrefixAsync(STUDENT_LIST_PREFIX);

        return true;
    }
}
