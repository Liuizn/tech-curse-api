# 1. Estágio Base (Runtime leve para rodar a aplicação)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# 2. Estágio de Build (SDK pesado com as ferramentas de compilação)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia a solução e o projeto principal para restaurar os pacotes
COPY ["tech-curse-api.slnx", "./"]
COPY ["tech-curse-api/tech-curse-api.csproj", "tech-curse-api/"]

# Restaura as dependências
RUN dotnet restore "tech-curse-api/tech-curse-api.csproj"

# Copia todo o resto do código para dentro do container
COPY . .

# Vai para a pasta do projeto e compila
WORKDIR "/src/tech-curse-api"
RUN dotnet build "tech-curse-api.csproj" -c Release -o /app/build

# 3. Estágio de Publish (Gera os artefatos otimizados)
FROM build AS publish
RUN dotnet publish "tech-curse-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Estágio Final (Copia os artefatos publicados para a imagem base)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "tech-curse-api.dll"]