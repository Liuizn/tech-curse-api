using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using tech_curse_api.src.Application.Interfaces;
using tech_curse_api.src.Domain.Exceptions;
using tech_curse_api.src.Infrastructure.Data;
using tech_curse_api.src.Infrastructure.Identity;

namespace tech_curse_api.src.API.Configuration;

public static class IdentityAuthenticationSetup
{
    public static IServiceCollection AddIdentityAuthenticationSetup(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // 1. Configuração do Identity
        services.AddIdentityCore<IdentityUser>(options =>
        {
            // Senhas
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;

            // Bloqueio de conta (Lockout)
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // Usuário
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<TechCurseContext>()
        .AddSignInManager();

        // 2. Extração das configurações do JWT
        var jwtIssuer = configuration["Jwt:Issuer"];
        var jwtAudience = configuration["Jwt:Audience"];
        var jwtSigningKey = configuration["Jwt:SigningKey"];

        if (string.IsNullOrEmpty(jwtSigningKey) || jwtSigningKey.Length < 32)
        {
            throw new InvalidOperationException("A chave secreta do JWT (SigningKey) não está configurada ou possui menos de 32 caracteres.");
        }

        // 3. Configuração da Autenticação
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey)),

                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();

                    throw new UnauthorizedException("Acesso negado. Token ausente ou inválido.");
                },
                OnForbidden = context =>
                {
                    throw new ForbiddenAccessException("Você não tem permissão para acessar este recurso.");
                }
            };
        });

        return services;
    }
}
