using Microsoft.EntityFrameworkCore;
using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;
using tech_curse_api.src.Domain.Entities;
using tech_curse_api.src.Infrastructure.Data;

namespace tech_curse_api.src.Infrastructure.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly TechCurseContext _context;

    public EnrollmentRepository(TechCurseContext context)
    {
        _context = context;
    }

    public async Task<Enrollment?> GetByIdAsync(int id)
        => await _context.Enrollments.FindAsync(id);

    public async Task<Enrollment?> GetByStudentCourseAsync(int studentId, int courseId)
        => await _context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

    public async Task<bool> EnrollmentIsActiveAsync(int id)
        => await _context.Enrollments
            .Where(e => e.EnrollmentId == id && e.Status == false).AnyAsync();

    public async Task<bool> EnrollmentIsActiveAsync(int studentId, int courseId)
        => await _context.Enrollments
            .Where(e => e.StudentId == studentId && e.CourseId == courseId && e.Status == false).AnyAsync();

    public async Task AddAsync(Enrollment enrollment)
    {
        await _context.Enrollments.AddAsync(enrollment);
        await _context.SaveChangesAsync();
    }
}