using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NetArchTest.Rules;
using tech_curse_api.Test.Architecture.Common;

namespace tech_curse_api.Test.Architecture.Controllers;

public class ControllerArchitectureTests
{
    [Fact]
    public void Controllers_Should_InheritFrom_ControllerBase()
    {
        // Act
        var result = Types.InAssembly(ArchitectureConstants.ApiAssembly)
            .That()
            .ResideInNamespace("tech_curse_api.src.API.Controllers")
            .And()
            .AreClasses()
            .Should()
            .Inherit(typeof(ControllerBase))
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"All classes in API.Controllers must inherit from ControllerBase. Failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Controllers_Should_Not_DirectlyDependOn_DbContext()
    {
        // Act
        var result = Types.InAssembly(ArchitectureConstants.ApiAssembly)
            .That()
            .ResideInNamespace("tech_curse_api.src.API.Controllers")
            .ShouldNot()
            .HaveDependencyOnAny(
                "Microsoft.EntityFrameworkCore",
                "tech_curse_api.src.Infrastructure.Data")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"Controllers must not directly depend on DbContext or EntityFrameworkCore. Failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Controllers_Should_Not_DirectlyDependOn_RepositoryImplementations()
    {
        // Act
        var result = Types.InAssembly(ArchitectureConstants.ApiAssembly)
            .That()
            .ResideInNamespace("tech_curse_api.src.API.Controllers")
            .ShouldNot()
            .HaveDependencyOn("tech_curse_api.src.Infrastructure.Repositories")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"Controllers must not directly depend on repository implementations. Failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Controllers_Should_Not_DirectlyDependOn_RepositoryInterfaces()
    {
        // Act
        var result = Types.InAssembly(ArchitectureConstants.ApiAssembly)
            .That()
            .ResideInNamespace("tech_curse_api.src.API.Controllers")
            .ShouldNot()
            .HaveDependencyOn("tech_curse_api.src.Domain.Interfaces.Repositories")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"Controllers must not directly depend on repository interfaces (use MediatR/Application Services instead). Failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }
}
