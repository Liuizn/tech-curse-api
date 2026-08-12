 using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using System.Security.Claims;
using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;
using tech_curse_api.src.Domain.Entities;
using tech_curse_api.src.Domain.Enums;
using tech_curse_api.src.Domain.Exceptions;

namespace tech_curse_api.src.Application.Services;
public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly ICacheService _cacheService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    private const string STUDENT_ITEM_PREFIX = "students:item:";
    private const string STUDENT_LIST_PREFIX = "students:list:";

    public StudentService(
        IStudentRepository studentRepository,
        ICacheService cacheService,
        UserManager<IdentityUser> userManager,
        ICurrentUserService currentUserService
    ) {
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

        var dtos = courses.Select(c => new StudentOutputDto(c.StudentId, c.Nome, c.Email, c.DataCadastro));
        
        var result = new PagedResultDto<StudentOutputDto>(
            dtos,
            totalCount,
            searchParams.PageNumber,
            searchParams.PageSize
        );

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));

        return result;
    }

    protected async Task<bool> ValidateRoleAcess(string targetIdentityUserId)
    {
        var currentUserId = _currentUserService.GetUserId();
        var isAdmin = _currentUserService.IsInRole(UserRole.Admin);

        if (currentUserId != targetIdentityUserId && !isAdmin)
        {
            throw new NotAllowedException("Você não possuí permissão suficiente para acessar registro!");
        }

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

        await ValidateRoleAcess(student.IdentityUserId);

        var result = new StudentOutputDto(student.StudentId, student.Nome, student.Email, student.DataCadastro);

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));

        return result;
    }

    public async Task<StudentOutputDto?> GetSelfAsync()
    {
        var currentUserEmail = _currentUserService.GetUserEmail();

        var student = await _studentRepository.GetByEmailAsync(currentUserEmail);

        if (student == null) return null;

        var result = new StudentOutputDto(student.StudentId, student.Nome, student.Email, student.DataCadastro);

        return result;
    }

    public async Task<IEnumerable<CourseStudentOutputDto>> GetCoursesAsync(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        if (student == null) return null;

        await ValidateRoleAcess(student.IdentityUserId);

        return await _studentRepository.GetCoursesAsync(student);
    }

    public async Task<StudentOutputDto> CreateAsync(StudentPostDto dto)
    {
        bool emailExists = await _studentRepository.EmailExistsAsync(dto.Email);
        if (emailExists)
        {
            throw new ConflictException("O e-mail informado já está em uso por outro estudante.");
        }

        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
        {
            throw new ConflictException("Usuário não encontrado.");
        }

        var student = new Student
        {
            Nome = dto.Nome,
            Email = dto.Email,
            DataCadastro = DateTime.UtcNow,
            IdentityUserId = user.Id,
            IdentityUser = user,
            IsDeleted = false,
            Enrollments = new List<Enrollment>()
        };

        await _studentRepository.AddAsync(student);

        var result = new StudentOutputDto(student.StudentId, student.Nome, student.Email, student.DataCadastro);

        return result;
    }
    
    public async Task<bool> UpdateAsync(int id, StudentPutDto dto)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null) return false;

        await ValidateRoleAcess(student.IdentityUserId);

        student.Nome = dto.Nome;

        await _studentRepository.UpdateAsync(student);

        var updatedDto = new StudentOutputDto(student.StudentId, student.Nome, student.Email, student.DataCadastro);
        await _cacheService.SetAsync($"{STUDENT_ITEM_PREFIX}{id}", updatedDto, TimeSpan.FromMinutes(15));

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

        await _userManager.SetLockoutEndDateAsync(student.IdentityUser, DateTimeOffset.MaxValue);

        await _cacheService.RemoveAsync($"{STUDENT_ITEM_PREFIX}{id}");
        await _cacheService.RemoveByPrefixAsync(STUDENT_LIST_PREFIX);

        return true;
    }

    public async Task<bool> StudentIsActiveAsync(Student student)
    {
        return await _studentRepository.StudentIsActiveAsync(student);
    }
}
