using Microsoft.EntityFrameworkCore;
using tech_curse_api.src.Infrastructure.Data;

namespace tech_curse_api.src.API.Configuration;

public static class EFCoreSetup
{
    public static IServiceCollection AddEFCoreSetup(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            // Skip SQL Server registration when running tests with InMemory database
            return services;
        }

        var apiConnectionString =
            configuration.GetConnectionString("APITechCurse")
            ?? throw new InvalidOperationException("Connection string 'APITechCurse' not found.");

        services.AddDbContext<TechCurseContext>(options =>
        {
            options.UseSqlServer(
                apiConnectionString,
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5, // Tenta até 5 vezes
                    maxRetryDelay: TimeSpan.FromSeconds(10), // Espera até 10 segundos entre as tentativas
                    errorNumbersToAdd: null);
                });
        });

        services.AddHealthChecks().AddSqlServer(apiConnectionString, name: "Database_SQLServer");

        return services;
    }
}
