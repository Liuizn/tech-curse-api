using tech_curse_api.src.Domain.Entities;

namespace tech_curse_api.src.Application.DTOs;

public record CoursePostDto(string Titulo, string Descricao, string Categoria, int CargaHoraria, DateTime DataCriacao);
public record CoursePutDto(int Id, string Titulo, string Descricao, string Categoria, int CargaHoraria, DateTime DataCriacao);
public record CourseOutputDto(int Id, string Titulo, string Descricao, string Categoria, int CargaHoraria, DateTime DataCriacao, ICollection<Enrollment> Enrollments);