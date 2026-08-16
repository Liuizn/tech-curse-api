using System.Reflection;
using tech_curse_api.src.API.Controllers;
using tech_curse_api.src.Application.Features.Courses.Commands.CreateCourse;
using tech_curse_api.src.Domain.Entities;
using tech_curse_api.src.Infrastructure.Data;

namespace tech_curse_api.Test.Architecture.Common;

public static class ArchitectureConstants
{
    public const string DomainNamespace = "tech_curse_api.src.Domain";
    public const string ApplicationNamespace = "tech_curse_api.src.Application";
    public const string InfrastructureNamespace = "tech_curse_api.src.Infrastructure";
    public const string ApiNamespace = "tech_curse_api.src.API";

    public static readonly Assembly DomainAssembly = typeof(Course).Assembly;
    public static readonly Assembly ApplicationAssembly = typeof(CreateCourseCommand).Assembly;
    public static readonly Assembly InfrastructureAssembly = typeof(TechCurseContext).Assembly;
    public static readonly Assembly ApiAssembly = typeof(CourseController).Assembly;
}
