using Microsoft.EntityFrameworkCore;
using tech_curse_api.src.Infrastructure.Data;

namespace tech_curse_api.src.API.Configuration;

public static class EFCoreSetup
{
    public static IServiceCollection AddEFCoreSetup(this IServiceCollection services, IConfiguration configuration)
    {
        var apiConnectionString =
            configuration.GetConnectionString("APITechCurse")
            ?? throw new InvalidOperationException("Connection string 'APITechCurse' not found.");

        services.AddDbContext<TechCurseContext>(opt =>
            opt.UseSqlServer(apiConnectionString));

        return services;
    }
}
