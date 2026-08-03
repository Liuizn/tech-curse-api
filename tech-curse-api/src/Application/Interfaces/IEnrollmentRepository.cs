using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Domain.Entities; 

namespace tech_curse_api.src.Application.Interfaces;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(int id);
    Task<Enrollment?> GetByStudentCourseAsync(int studentId, int courseId);
    Task<bool> EnrollmentIsActiveAsync(int id);
    Task<bool> EnrollmentIsActiveAsync(int studentId, int courseId);    
    Task AddAsync(Enrollment enrollment);
}