# Tech Curse API
[![CI/CD Pipeline](https://github.com/Liuizn/tech-curse-api/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/Liuizn/tech-curse-api/actions/workflows/ci-cd.yml)

## Objetivo do Projeto
API REST funcional desenvolvida para gerenciar cursos, estudantes, matrículas e transações financeiras em uma plataforma de cursos online. 

O projeto nasceu de um desafio prático focado no dia a dia do desenvolvimento backend em .NET e evoluiu para uma infraestrutura pronta para produção. O objetivo central é fornecer uma estrutura robusta orientada a **Clean Architecture** e **Domain-Driven Design (DDD)**, aplicando princípios **SOLID**, resiliência em integrações de pagamentos e alta observabilidade.

## 🛠️ Tecnologias Utilizadas

**Backend & Arquitetura**
* .NET 8.0 (C# 12)
* ASP.NET Core Web API
* Entity Framework Core 8 (ORM)
* ASP.NET Core Identity + JWT Bearer
* Arquitetura em Camadas (API, Application, Domain, Infrastructure)

**Infraestrutura & DevOps**
* Docker & Docker Compose (Containerização multi-stage)
* GitHub Actions (Pipeline de CI/CD Automatizado)
* SQL Server (Banco de dados relacional via container)
* Redis (Cache distribuído em memória via container)

**Observabilidade & Qualidade**
* Serilog + Seq (Centralização e monitoramento de logs estruturados)
* ASP.NET Core Health Checks (Monitoramento de vitalidade da API, Banco e Cache)
* Swagger/OpenAPI (Documentação interativa)
* xUnit + Moq (Testes de Unidade e Integração)

---

## ⚙️ Pré-requisitos

Com a nova infraestrutura containerizada, você **não precisa** ter o SQL Server ou o Redis instalados nativamente na sua máquina. Você precisará apenas de:

* [Docker Desktop](https://www.docker.com/products/docker-desktop/) (ou Docker Engine + Docker Compose)
* Git instalado

*(Opcional para desenvolvimento ativo)*
* SDK .NET 8.0
* IDE/Editor: Visual Studio 2022, JetBrains Rider ou VS Code

---

## 🚀 Como Rodar o Projeto Localmente (Via Docker)

Toda a infraestrutura (API, Banco de Dados, Cache e Logs) está orquestrada. Siga os passos abaixo:

### 1. Clonar o Repositório
```bash
git clone https://github.com/Liuizn/tech-curse-api.git
cd tech-curse-api
```

### 2. Configurar as Variáveis de Ambiente
Por segurança, senhas e chaves não ficam no repositório. Crie um arquivo chamado `.env` na raiz do projeto, usando o `.env.example` como base:

```env
# Exemplo de conteúdo do .env
DB_PASSWORD=SuaSenhaForteAqui123!
REDIS_PASSWORD=SuaSenhaDeCache123!
SEQ_PASSWORD=SuaSenhaAdminSeq123!

JWT_SIGNING_KEY=SuaChaveSecretaMuitoLongaEComplexaParaJWT
JWT_ISSUER=SeuIssuer
JWT_AUDIENCE=SeuAudience
```

### 3. Iniciar a Infraestrutura
Na raiz do projeto (onde está o `docker-compose.yml`), execute o comando:
```bash
docker-compose up -d --build
```
> **Nota:** A aplicação está configurada com resiliência (`RetryOnFailure`). A API aguardará o banco de dados inicializar e **aplicará as Migrations e o Seed de dados automaticamente** no primeiro uso.

### 4. Acessos aos Serviços
Com os containers rodando, acesse os serviços nos seguintes endereços:

* **Documentação Swagger (API):** [http://localhost:8080/swagger](http://localhost:8080/swagger)
* **Monitoramento de Logs (Seq):** [http://localhost:9000](http://localhost:9000) (Use o usuário `admin` e a senha definida no seu `.env`).
* **Health Check da API:** [http://localhost:8080/health](http://localhost:8080/health)

---

## 🧪 Como Executar os Testes

O projeto conta com uma suíte de testes de unidade e integração. Para rodá-los localmente via .NET CLI, utilize:

```bash
dotnet test tech-curse-api.slnx --configuration Release
```
*(No pipeline de CI/CD, estes testes são executados automaticamente a cada Pull Request).*

---

## 🔐 Autenticação no Swagger
A aplicação é protegida por JWT (JSON Web Token).
1. Crie um usuário ou faça login no endpoint `/tech-curse/Auth/register` ou `/login`.
2. Copie o token retornado.
3. No topo da página do Swagger, clique no botão **Authorize**.
4. Digite `Bearer ` seguido de um espaço e cole o seu token (Ex: `Bearer eyJhbG...`).
5. Clique em *Authorize* para liberar o acesso aos endpoints protegidos.

---

## 🆘 Troubleshooting (Solução de Problemas)

* **O banco de dados não criou as tabelas:** Verifique se a senha do `DB_PASSWORD` no `.env` é forte o suficiente (mínimo de 8 caracteres, com letras maiúsculas, minúsculas e números). O SQL Server rejeita inicializações com senhas fracas.
* **Portas em uso:** Se ocorrer conflito de portas, certifique-se de que não há nenhuma instância local do SQL Server (porta 1433), Redis (porta 6380) ou outro serviço rodando nas portas 8080 ou 9000.
* **Logs da API:** Para investigar erros de inicialização, rode `docker-compose logs api` no terminal.
