using FluentAssertions;
using FluentValidation;
using MediatR;
using NetArchTest.Rules;
using tech_curse_api.Test.Architecture.Common;

namespace tech_curse_api.Test.Architecture.DesignPatterns;

public class DesignPatternTests
{
    [Fact]
    public void Handlers_Should_ResideIn_Application_Features()
    {
        // Act
        var result = Types.InAssembly(ArchitectureConstants.ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .Or()
            .ImplementInterface(typeof(IRequestHandler<>))
            .Should()
            .ResideInNamespaceStartingWith("tech_curse_api.src.Application.Features")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"All MediatR handlers must reside in Application.Features namespaces. Failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Validators_Should_ResideIn_Application_Features()
    {
        // Act
        var result = Types.InAssembly(ArchitectureConstants.ApplicationAssembly)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .Should()
            .ResideInNamespaceStartingWith("tech_curse_api.src.Application.Features")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"All FluentValidation validators must reside in Application.Features namespaces. Failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void RepositoryImplementations_Should_ResideIn_Infrastructure_Repositories()
    {
        // Act
        var result = Types.InAssembly(ArchitectureConstants.InfrastructureAssembly)
            .That()
            .HaveNameEndingWith("Repository")
            .And()
            .AreClasses()
            .Should()
            .ResideInNamespace("tech_curse_api.src.Infrastructure.Repositories")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"All repository implementations must reside in Infrastructure.Repositories namespace. Failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Controllers_Should_ResideIn_API_Controllers()
    {
        // Act
        var result = Types.InAssembly(ArchitectureConstants.ApiAssembly)
            .That()
            .HaveNameEndingWith("Controller")
            .And()
            .AreClasses()
            .Should()
            .ResideInNamespace("tech_curse_api.src.API.Controllers")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"All controllers must reside in API.Controllers namespace. Failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }
}
