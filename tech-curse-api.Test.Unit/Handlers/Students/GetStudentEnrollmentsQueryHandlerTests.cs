using FluentAssertions;
using Moq;
using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Features.Students.Queries.GetStudentEnrollments;
using tech_curse_api.src.Application.Interfaces;
using tech_curse_api.src.Domain.Entities;
using tech_curse_api.src.Domain.Enums;
using tech_curse_api.src.Domain.Exceptions;
using Xunit;

namespace tech_curse_api.Test.Unit.Handlers.Students;

public class GetStudentEnrollmentsQueryHandlerTests
{
    private readonly Mock<IStudentRepository> _studentRepositoryMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly GetStudentEnrollmentsQueryHandler _handler;

    public GetStudentEnrollmentsQueryHandlerTests()
    {
        _handler = new GetStudentEnrollmentsQueryHandler(_studentRepositoryMock.Object, _currentUserServiceMock.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_WhenStudentNotFoundOrDeleted_ShouldThrowNotFoundException()
    {
        // Arrange
        _studentRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Student?)null);

        var query = new GetStudentEnrollmentsQuery(1);

        // Act
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Estudante não encontrado.");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_WhenNonAdminAndMismatchUser_ShouldThrowNotAllowedException()
    {
        // Arrange
        var student = new Student { StudentId = 1, IdentityUserId = "user-real", IsDeleted = false };
        _studentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(student);

        _currentUserServiceMock.Setup(u => u.GetUserId()).Returns("user-fake");
        _currentUserServiceMock.Setup(u => u.IsInRole(UserRole.Admin)).Returns(false);

        var query = new GetStudentEnrollmentsQuery(1);

        // Act
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotAllowedException>()
            .WithMessage("Você não possui permissão suficiente para acessar este registro.");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_WhenValid_ShouldReturnEnrollments()
    {
        // Arrange
        var student = new Student { StudentId = 1, IdentityUserId = "user-1", IsDeleted = false };
        _studentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(student);

        _currentUserServiceMock.Setup(u => u.GetUserId()).Returns("user-1");
        _currentUserServiceMock.Setup(u => u.IsInRole(UserRole.Admin)).Returns(false);

        var coursesList = new List<CourseStudentOutputDto>
        {
            new(1, "Curso C#", "Desc", "Tech", true)
        };

        _studentRepositoryMock.Setup(r => r.GetCoursesAsync(student))
            .ReturnsAsync(coursesList);

        var query = new GetStudentEnrollmentsQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(coursesList);
    }
}
