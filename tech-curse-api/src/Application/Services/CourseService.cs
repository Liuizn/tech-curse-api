using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;
using tech_curse_api.src.Domain.Entities;

namespace tech_curse_api.src.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICacheService _cacheService;

    private const string COURSE_ITEM_PREFIX = "courses:item:";
    private const string COURSE_LIST_PREFIX = "courses:list:";

    public CourseService(ICourseRepository courseRepository, ICacheService cacheService)
    {
        _courseRepository = courseRepository;
        _cacheService = cacheService;
    }

    public async Task<PagedResultDto<CourseOutputDto>> GetPagedAsync(CoursePaginationParamsDto searchParams)
    {
        var cacheKey = $"{COURSE_LIST_PREFIX}page:{searchParams.PageNumber}:size:{searchParams.PageSize}:sort:{searchParams.SortBy}_{searchParams.SortDirection}";

        if (!string.IsNullOrWhiteSpace(searchParams.Categoria))
        {
            cacheKey = $"{COURSE_LIST_PREFIX}cat:{searchParams.Categoria}:page:{searchParams.PageNumber}:size:{searchParams.PageSize}:sort:{searchParams.SortBy}_{searchParams.SortDirection}";
        }

        var cachedResult = await _cacheService.GetAsync<PagedResultDto<CourseOutputDto>>(cacheKey);
        if (cachedResult != null)
        {
            return cachedResult;
        }

        var (courses, totalCount) = await _courseRepository.GetPagedAsync(searchParams);

        var dtos = courses.Select(c => new CourseOutputDto(c.CourseId, c.Titulo, c.Descricao, c.Categoria, c.CargaHoraria, c.DataCriacao, c.Enrollments));
        
        var result = new PagedResultDto<CourseOutputDto>(
            dtos,
            totalCount,
            searchParams.PageNumber,
            searchParams.PageSize
        );

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));

        return result;
    }

    public async Task<CourseOutputDto?> GetByIdAsync(int id)
    {
        var cacheKey = $"{COURSE_ITEM_PREFIX}{id}";
        
        var cachedCourse = await _cacheService.GetAsync<CourseOutputDto>(cacheKey);
        if (cachedCourse != null)
        {
            return cachedCourse;
        }

        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null) return null;

        var result = new CourseOutputDto(course.CourseId, course.Titulo, course.Descricao, course.Categoria, course.CargaHoraria, course.DataCriacao, course.Enrollments);

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));

        return result;
    }

    public async Task<CourseOutputDto> CreateAsync(CoursePostDto dto)
    {
        var course = new Course
        {
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            Categoria = dto.Categoria,
            CargaHoraria = dto.CargaHoraria,
            DataCriacao = dto.DataCriacao
        };

        await _courseRepository.AddAsync(course);

        var result =  new CourseOutputDto(course.CourseId, course.Titulo, course.Descricao, course.Categoria, course.CargaHoraria, course.DataCriacao, course.Enrollments);

        var cacheKey = $"{COURSE_ITEM_PREFIX}{course.CourseId}";

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));

        return result;
    }

    public async Task<bool> UpdateAsync(CoursePutDto dto)
    {
        var course = await _courseRepository.GetByIdAsync(dto.Id);

        if (course == null) return false;

        course.CourseId = dto.Id;
        course.Titulo = dto.Titulo;
        course.Descricao = dto.Descricao;
        course.Categoria = dto.Categoria;
        course.CargaHoraria = dto.CargaHoraria;

        await _courseRepository.UpdateAsync(course);

        var updatedDto = new CourseOutputDto(course.CourseId, course.Titulo, course.Descricao, course.Categoria, course.CargaHoraria, course.DataCriacao, course.Enrollments);
        await _cacheService.SetAsync($"{COURSE_ITEM_PREFIX}{dto.Id}", updatedDto, TimeSpan.FromMinutes(15));

        await _cacheService.RemoveByPrefixAsync(COURSE_LIST_PREFIX);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null) return false;

        await _courseRepository.DeleteAsync(course);

        await _cacheService.RemoveAsync($"{COURSE_ITEM_PREFIX}{id}");

        await _cacheService.RemoveByPrefixAsync(COURSE_LIST_PREFIX);

        return true;
    }
}
