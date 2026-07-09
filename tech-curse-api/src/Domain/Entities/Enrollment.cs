namespace tech_curse_api.src.Domain.Entities;

public class Enrollment
{
    public int EnrollmentId { get; set; } // Chave primária surrogada (opcional, mas recomendada)
    public int StudentId { get; set; }
    public Student Student { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public bool Status { get; set; }
    public DateTime DataMatricula { get; set; }
}
