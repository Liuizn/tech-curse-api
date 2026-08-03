using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using tech_curse_api.src.Domain.Entities;

namespace tech_curse_api.src.Infrastructure.Data;

public class TechCurseContext : IdentityDbContext<IdentityUser>
{
    public TechCurseContext(DbContextOptions<TechCurseContext> options) : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<Enrollment> Enrollments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(TechCurseContext).Assembly);

        builder.Entity<Student>().HasQueryFilter(s => !s.IsDeleted);
    }
}
