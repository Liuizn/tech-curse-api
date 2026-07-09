using Microsoft.AspNetCore.Identity;

namespace tech_curse_api.src.Domain.Entities;

public class Student
{
    public int StudentId { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public DateTime DataCadastro { get; set; }

    public string IdentityUserId { get; set; }
    public IdentityUser IdentityUser { get; set; } // O e-mail virá daqui

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
