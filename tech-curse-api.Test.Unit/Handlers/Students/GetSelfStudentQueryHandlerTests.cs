using FluentAssertions;
using Moq;
using tech_curse_api.src.Application.Features.Students.Queries.GetSelfStudent;
using tech_curse_api.src.Application.Interfaces;
using tech_curse_api.src.Domain.Entities;
using tech_curse_api.src.Domain.Exceptions;
using Xunit;

namespace tech_curse_api.Test.Unit.Handlers.Students;

public class GetSelfStudentQueryHandlerTests
{
    private readonly Mock<IStudentRepository> _studentRepositoryMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly GetSelfStudentQueryHandler _handler;

    public GetSelfStudentQueryHandlerTests()
    {
        _handler = new GetSelfStudentQueryHandler(_studentRepositoryMock.Object, _currentUserServiceMock.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_WhenStudentNotFoundOrDeleted_ShouldThrowNotFoundException()
    {
        // Arrange
        _currentUserServiceMock.Setup(u => u.GetUserEmail()).Returns("student@example.com");
        _studentRepositoryMock.Setup(r => r.GetByEmailAsync("student@example.com"))
            .ReturnsAsync((Student?)null);

        var query = new GetSelfStudentQuery();

        // Act
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Perfil de estudante não encontrado ou inativo.");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_WhenStudentExists_ShouldReturnStudentDto()
    {
        // Arrange
        var student = new Student
        {
            StudentId = 1,
            Nome = "Carlos",
            Email = "carlos@example.com",
            DataCadastro = DateTime.UtcNow,
            IsDeleted = false
        };

        _currentUserServiceMock.Setup(u => u.GetUserEmail()).Returns("carlos@example.com");
        _studentRepositoryMock.Setup(r => r.GetByEmailAsync("carlos@example.com"))
            .ReturnsAsync(student);

        var query = new GetSelfStudentQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Nome.Should().Be("Carlos");
        result.Email.Should().Be("carlos@example.com");
    }
}
