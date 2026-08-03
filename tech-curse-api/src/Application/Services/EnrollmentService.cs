
using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;
using tech_curse_api.src.Domain.Entities;
using tech_curse_api.src.Domain.Enums;
using tech_curse_api.src.Domain.Exceptions;


namespace tech_curse_api.src.Application.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ICourseRepository _courseRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public EnrollmentService(ICurrentUserService currentUserService, ICourseRepository courseRepository, IStudentRepository studentRepository, IEnrollmentRepository enrollmentRepository)
    {
        _currentUserService = currentUserService;
        _courseRepository = courseRepository;
        _studentRepository = studentRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<bool> CreateAsync(EnrollmentInputDto input)
    {
        var isAdmin = _currentUserService.IsInRole(UserRole.Admin);
        var isStudent = _currentUserService.IsInRole(UserRole.Student);
        if (!isStudent && !isAdmin)
        {
            throw new NotAllowedException("Apenas estudantes e administradores podem criar matrículas!");
        }

        var userEmail = _currentUserService.GetUserEmail();
        if (userEmail == null)
        {
            throw new NotAllowedException("Email do usuário não encontrado!");
        }

        var student = isAdmin ? await _studentRepository.GetByIdAsync(input.StudentId) : await _studentRepository.GetByEmailAsync(userEmail);
        if (student == null)
        {
            throw new NotAllowedException("Estudante não encontrado!");
        }

        if (!await _studentRepository.StudentIsActiveAsync(student))
        {
            throw new NotAllowedException("Estudante não está ativo!");
        }

        var course = await _courseRepository.GetByIdAsync(input.CourseId);
        if (course == null)
        {
            throw new NotAllowedException("Curso não encontrado!");
        }

        var existingEnrollment = await _enrollmentRepository.GetByStudentCourseAsync(student.StudentId, course.CourseId);
        if (existingEnrollment != null)
        {
            throw new NotAllowedException("Estudante já está matriculado neste curso!");
        }

        Enrollment enrollment = new Enrollment
        {
            StudentId = student.StudentId,
            CourseId = course.CourseId,
            DataMatricula = DateTime.UtcNow
        };
        
        await _enrollmentRepository.AddAsync(enrollment);

        return true;
    }
}
