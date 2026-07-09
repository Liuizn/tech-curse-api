namespace tech_curse_api.src.Domain.Entities;

public class Course
{
    public int CourseId { get; set; }
    public string Titulo{ get; set; }
    public string Descricao { get; set; }
    public string Categoria { get; set; }
    public int CargaHoraria { get; set; }
    public DateTime DataCriacao { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
