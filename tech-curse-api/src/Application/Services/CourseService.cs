using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;
using tech_curse_api.src.Domain.Entities;

namespace tech_curse_api.src.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
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

        return new CourseOutputDto(course.CourseId, course.Titulo, course.Descricao, course.Categoria, course.CargaHoraria, course.DataCriacao, course.Enrollments);
    }

    public async Task<IEnumerable<CourseOutputDto>> GetAllAsync()
    {
        var courses = await _courseRepository.GetAllAsync();
        return courses.Select(c => new CourseOutputDto(c.CourseId, c.Titulo, c.Descricao, c.Categoria, c.CargaHoraria, c.DataCriacao, c.Enrollments));

    }

    public async Task<CourseOutputDto?> GetByIdAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null) return null;

        return new CourseOutputDto(course.CourseId, course.Titulo, course.Descricao, course.Categoria, course.CargaHoraria, course.DataCriacao, course.Enrollments);
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
        course.DataCriacao = dto.DataCriacao;

        await _courseRepository.UpdateAsync(course);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null) return false;

        await _courseRepository.DeleteAsync(course);

        return true;
    }
}
