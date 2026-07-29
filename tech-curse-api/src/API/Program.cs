using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using tech_curse_api.src.API.Configuration;
using tech_curse_api.src.Application.Interfaces;
using tech_curse_api.src.Application.Services;
using tech_curse_api.src.Infrastructure.Data;
using tech_curse_api.src.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();

builder.Services.AddControllers();

var apiConnectionString =
    builder.Configuration.GetConnectionString("APITechCurse")
    ?? throw new InvalidOperationException("Connection string 'APITechCurse' not found.");

var cacheConnectionString =
    builder.Configuration.GetConnectionString("RedisCache")
    ?? throw new InvalidOperationException("Connection string 'RedisCache' not found.");

// EF Core
builder.Services.AddDbContext<TechCurseContext>(opt =>
    opt.UseSqlServer(apiConnectionString));

builder.Services.AddIdentityAuthenticationSetup(builder.Configuration);

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    // A string de conexão idealmente vem do appsettings.json
    options.Configuration = cacheConnectionString;
    options.InstanceName = "TechCurseAPI_"; 
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(cacheConnectionString));

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tech Curse API", Version = "v1" });

    // 1. Define o esquema de segurança (Cria o botão "Authorize" no Swagger)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Cabeçalho de autorização JWT usando o esquema Bearer. 
                      Escreva 'Bearer' [espaço] e em seguida o seu token na caixa de texto abaixo.
                      Exemplo: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // 2. Aplica o requisito de segurança globalmente nos endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seção de Seed de Dados
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Executa o seed de forma assíncrona
        await DbInitializer.SeedDataAsync(services);
    }
    catch (Exception ex)
    {
        // Registre o erro no log caso algo dê errado na inicialização
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao rodar o Seed do banco de dados.");
    }
}

app.Run();
