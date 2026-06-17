# Tech Curse API

Uma API robusta e escalável desenvolvida em **.NET 8** para o gerenciamento de cursos, turmas e matrículas.
O projeto utiliza o **SQL Server** como banco de dados relacional e segue as melhores práticas de arquitetura.

---

## Objetivo do Projeto

O principal objetivo desta API é centralizar e automatizar a gestão acadêmica de cursos. A plataforma permite o cadastro de cursos, controle de módulos, gerenciamento de alunos e relatórios de matrículas. 

Foi desenvolvida com o foco em performance, facilidade de integração com aplicações Frontend (Web/Mobile) e facilidade de manutenção de código.

---

## Requisitos e Pré-requisitos

Para rodar e compilar este projeto localmente, você precisará ter instalado em sua máquina:

* SDK .NET 8.0: [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
* SQL Server (LocalDB, Express ou via Docker)
* IDE/Editor: Visual Studio 2022, JetBrains Rider ou VS Code (com a extensão *C# Dev Kit*)

---

## Tecnologias Utilizadas

- .NET 8.0 (C# 12)
- ASP.NET Core Web API
- Entity Framework Core 8 (ORM)
- ASP.NET Core Identity + JWT Bearer
- SQL Server (Banco de dados)
- Swagger/OpenAPI (Documentação da API)

---

## Como Rodar o Projeto Localmente

Siga o passo a passo abaixo para configurar e executar a API na sua máquina.

### 1. Clonar o Repositório
```bash
git clone https://github.com/Liuizn/tech-curse-api.git
cd tech-curse-api
```
### 2. Configurar a String de Conexão
Abra o arquivo appsettings.Development.json na raiz do projeto principal e atualize a string de conexão com as credenciais do seu SQL Server local:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=GestaoCursosDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```
### 3 Executar as Migrations (Criar o Banco de Dados)
Certifique-se de que a ferramenta dotnet-ef está instalada globalmente. Se não estiver, execute:

```bash
dotnet tool install --global dotnet-ef
```
Em seguida, execute o comando para aplicar as migrations e criar a estrutura das tabelas no seu SQL Server:
```bash
dotnet ef database update
```

### 4 Executar a API
Navegue até a pasta do projeto da API (onde está o arquivo .csproj principal) e execute:

```bash
dotnet run
```
A API iniciará e os endpoints ficarão disponíveis em:
* HTTP: http://localhost:5130
* HTTPS: https://localhost:7106

### 5. Acessar a Documentação (Swagger)
Com a aplicação rodando em ambiente de Desenvolvimento, você pode testar todos os endpoints diretamente pelo navegador acessando:
* http://localhost:5130/swagger/index.html
* https://localhost:7106/swagger/index.html
