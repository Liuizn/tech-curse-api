# syntax=docker/dockerfile:1.4

# ===================================================
# 1. RUNTIME BASE (Imagem Chiseled Ultraleve e Segura)
# ===================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS base
USER app
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_EnableDiagnostics=0

# ===================================================
# 2. BUILD STAGE (Compilação com Cache de NuGet)
# ===================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

# Copia APENAS os projetos que compõem a aplicação (sem projetos de teste)
COPY ["tech-curse-api/src/Domain/tech-curse-api.Domain.csproj", "tech-curse-api/src/Domain/"]
COPY ["tech-curse-api/src/Application/tech-curse-api.Application.csproj", "tech-curse-api/src/Application/"]
COPY ["tech-curse-api/src/Infrastructure/tech-curse-api.Infrastructure.csproj", "tech-curse-api/src/Infrastructure/"]
COPY ["tech-curse-api/src/API/tech-curse-api.API.csproj", "tech-curse-api/src/API/"]

# Restaura dependências com Cache Mount do BuildKit
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore "tech-curse-api/src/API/tech-curse-api.API.csproj"

# Copia todo o código-fonte
COPY tech-curse-api/src/ tech-curse-api/src/

# Publica a aplicação otimizada com ReadyToRun (R2R)
WORKDIR "/src/tech-curse-api/src/API"
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "tech-curse-api.API.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:PublishReadyToRun=true

# ===================================================
# 3. FINAL IMAGE (Apenas os binários compilados)
# ===================================================
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "tech-curse-api.API.dll"]