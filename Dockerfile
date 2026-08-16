# 1. Estágio Base (Runtime leve para rodar a aplicação)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# 2. Estágio de Build (SDK pesado com as ferramentas de compilação)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia a solução e os projetos para restaurar os pacotes corretamente
COPY ["tech-curse-api.slnx", "./"]
COPY ["tech-curse-api/src/Domain/tech-curse-api.Domain.csproj", "tech-curse-api/src/Domain/"]
COPY ["tech-curse-api/src/Application/tech-curse-api.Application.csproj", "tech-curse-api/src/Application/"]
COPY ["tech-curse-api/src/Infrastructure/tech-curse-api.Infrastructure.csproj", "tech-curse-api/src/Infrastructure/"]
COPY ["tech-curse-api/src/API/tech-curse-api.API.csproj", "tech-curse-api/src/API/"]
COPY ["tech-curse-api.Test.Integration/tech-curse-api.Test.Integration.csproj", "tech-curse-api.Test.Integration/"]
COPY ["tech-curse-api.Test.Unit/tech-curse-api.Test.Unit.csproj", "tech-curse-api.Test.Unit/"]

# Restaura as dependências pela Solution
RUN dotnet restore "tech-curse-api.slnx"

# Copia todo o resto do código para dentro do container
COPY . .

# Vai para a pasta do projeto principal (API) e compila
WORKDIR "/src/tech-curse-api/src/API"
RUN dotnet build "tech-curse-api.API.csproj" -c Release -o /app/build

# 3. Estágio de Publish (Gera os artefatos otimizados)
FROM build AS publish
RUN dotnet publish "tech-curse-api.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Estágio Final (Copia os artefatos publicados para a imagem base)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "tech-curse-api.API.dll"]