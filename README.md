# Tech Curse API

## Objetivo do Projeto
API REST funcional desenvolvida para gerenciar cursos, estudantes e matrículas em uma plataforma de cursos online. O projeto nasceu de um desafio prático focado no dia a dia do desenvolvimento backend em .NET. O objetivo central é fornecer uma estrutura robusta com operações de CRUD, autenticação, autorização e documentação completa.
  
## Tecnologias Utilizadas

- .NET 8.0 (C# 12)
- ASP.NET Core Web API
- Entity Framework Core 8 (ORM)
- ASP.NET Core Identity + JWT Bearer
- SQL Server (Banco de dados)
- Redis (Banco de dados em memória para Cache)
- Swagger/OpenAPI (Documentação da API)

## Pré-requisitos

Para rodar e compilar este projeto localmente, você precisará ter instalado em sua máquina:

* SDK .NET 8.0: [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
* SQL Server (LocalDB, Express ou via Docker)
* IDE/Editor: Visual Studio 2022, JetBrains Rider ou VS Code (com a extensão *C# Dev Kit*)
* Redis: (usando o WSL ou Memurai)
  
## Como Rodar o Projeto Localmente

Siga o passo a passo abaixo para configurar e executar a API na sua máquina.

### 1 Clonar o Repositório
```bash
git clone https://github.com/Liuizn/tech-curse-api.git
cd tech-curse-api
```
### 2 Configurar a String de Conexão
Abra o arquivo appsettings.Development.json na raiz do projeto principal e atualize a string de conexão com as credenciais do seu SQL Server local:

```json
{
  "Jwt": {
    "SigningKey": "userChaveDeDesenvolvimentoMuitoLongaDeSegurancaParaOJWT2026",
    "Issuer": "userIssuer",
    "Audience": "userAudience"
  },
  "ConnectionStrings": {
    "APITechCurse": "Server=(localdb)\\MSSQLLocalDB;Database=APITechCurse;Trusted_Connection=True;Trustservercertificate=true",
    "RedisCache": "localhost:6380,password=sua_senha,abortConnect=false,connectTimeout=5000"
  }
}
```

### 3 Restaure as dependêncisa:
```bash
dotnet restore
```

### 4 Executar as Migrations
```bash
dotnet ef database update
```

### 5 Executar a API
Navegue até a pasta do projeto da API (onde está o arquivo .csproj principal) e execute:
```bash
dotnet run
```
A API iniciará e os endpoints ficarão disponíveis em:
* HTTP: http://localhost:5130
* HTTPS: https://localhost:7106

### 6 Acessar a Documentação (Swagger)
Com a aplicação rodando em ambiente de Desenvolvimento, você pode testar todos os endpoints diretamente pelo navegador acessando:
* http://localhost:5130/swagger/index.html
* https://localhost:7106/swagger/index.html
* **Autenticação:** A aplicação é protegida por JWT (JSON Web Token).
   - Realize uma requisição POST no endpoint de Login (ex: `/api/auth/login`) com suas credenciais para receber o token.
   - Copie o token retornado.
   - No topo da página do Swagger, clique no botão **Authorize**.
   - No campo de valor, digite `Bearer` seguido de um espaço e cole o seu token (exemplo: `Bearer eyJhbG...`).
   - Clique em *Authorize*. Agora você poderá testar os endpoints protegidos diretamente pela interface.
 
Neste projeto contém um collection para importação no Postman.

<img width="849" height="806" alt="image" src="https://github.com/user-attachments/assets/3e133210-d9e1-411d-9dee-889cb20adf54" />
