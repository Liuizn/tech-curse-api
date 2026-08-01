using tech_curse_api.src.Domain.Entities;

namespace tech_curse_api.src.Application.DTOs;

public record StudentPostDto(string Nome, string Email, string Categoria, int CargaHoraria, DateTime DataCadastro);
public record StudentPutDto(int Id, string Nome);
public record StudentOutputDto(int Id, string Nome, string Email, DateTime DataCadastro, ICollection<Enrollment> Enrollments);