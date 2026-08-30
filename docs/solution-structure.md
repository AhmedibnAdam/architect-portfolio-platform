# ArchitectPortfolio Solution Architecture & Structure

This document outlines the solution architecture, project structure, dependency graph, layer responsibilities, and architectural constraints for the **ArchitectPortfolio** project.

---

## Architectural Principles & Strategy

The **ArchitectPortfolio** system is designed as a **Modular Monolith** applying **Clean Architecture** / **Onion Architecture** principles alongside **Dependency Inversion**.

Key principles:
1. **Enforced Architecture over Documentation:** The solution structure and compile-time project references enforce dependency rules rather than relying on developer convention.
2. **Lean Initial Footprint:** The initial solution uses a deliberate four-project production setup rather than over-engineering into multi-project micro-libraries or premature microservices.
3. **Domain-Centric Boundaries:** High-level horizontal architectural layers host vertical business module slices (*Profile*, *Experience*, *Skills*, *Projects*, *Articles*, *Administration*).

---

## Solution Folder Structure

```
ArchitectPortfolio/
│
├── src/
│   │
│   ├── ArchitectPortfolio.Api/
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   ├── Extensions/
│   │   ├── Configuration/
│   │   └── Program.cs
│   │
│   ├── ArchitectPortfolio.Application/
│   │   ├── Abstractions/
│   │   ├── Common/
│   │   ├── Profile/
│   │   ├── Experience/
│   │   ├── Skills/
│   │   ├── Projects/
│   │   ├── Articles/
│   │   └── Administration/
│   │
│   ├── ArchitectPortfolio.Domain/
│   │   ├── Common/
│   │   ├── Profile/
│   │   ├── Experience/
│   │   ├── Skills/
│   │   ├── Projects/
│   │   ├── Articles/
│   │   └── Administration/
│   │
│   └── ArchitectPortfolio.Infrastructure/
│       ├── Persistence/
│       ├── Authentication/
│       ├── Storage/
│       ├── Integrations/
│       ├── Observability/
│       └── DependencyInjection/
│
├── tests/
│   │
│   ├── ArchitectPortfolio.Domain.Tests/
│   ├── ArchitectPortfolio.Application.Tests/
│   ├── ArchitectPortfolio.Infrastructure.Tests/
│   └── ArchitectPortfolio.Api.Tests/
│
├── docs/
│   └── architecture/
│       ├── adr/
│       ├── c4/
│       ├── domain-model.md
│       └── solution-structure.md
│
├── .gitignore
├── README.md
└── ArchitectPortfolio.sln
```

---

## Dependency Graph & Direction

The primary architectural invariant is that **all dependencies point inward toward the Domain**.

```
                         ┌─────────────┐
                         │     API     │
                         └──────┬──────┘
                                │
                   ┌────────────┴────────────┐
                   ▼                         ▼
            ┌──────────────┐        ┌────────────────┐
            │ Application  │        │ Infrastructure │
            └──────┬───────┘        └───────┬────────┘
                   │                         │
                   ▼                         ▼
            ┌────────────────────────────────────┐
            │               Domain               │
            └────────────────────────────────────┘
```

### Detailed Class/Reference Topology

```
                         CLIENTS
                            │
                       HTTPS / REST
                            │
                            ▼
              ┌─────────────────────────┐
              │     ArchitectPortfolio  │
              │          .Api           │
              │                         │
              │ Controllers             │
              │ Middleware              │
              │ API Contracts           │
              └────────────┬────────────┘
                           │
                           ▼
              ┌─────────────────────────┐
              │     Application         │
              │                         │
              │ Profile                 │
              │ Experience              │
              │ Skills                  │
              │ Projects                │
              │ Articles                │
              │ Administration          │
              │                         │
              │ Use Cases               │
              │ Validation              │
              │ Interfaces              │
              └────────────┬────────────┘
                           │
                           ▼
              ┌─────────────────────────┐
              │        Domain            │
              │                         │
              │ Entities                │
              │ Aggregates              │
              │ Value Objects           │
              │ Business Rules          │
              └─────────────────────────┘
                           ▲
                           │
              ┌────────────┴────────────┐
              │     Infrastructure      │
              │                         │
              │ EF Core                 │
              │ Database                │
              │ File Storage             │
              │ External Integrations   │
              │ Authentication          │
              │ Observability           │
              └─────────────────────────┘
```

---

## Project Responsibilities

### 1. `ArchitectPortfolio.Domain`
This project represents the **pure business core** and domain logic. It has zero external package or layer dependencies.

* **Components:** Entities, Value Objects, Aggregates, Domain Rules, Domain Events, Domain Exceptions, Enums.
* **Prohibited Dependencies:** ASP.NET Core, EF Core, SQL/Database SDKs, Infrastructure, Application, HTTP libraries, Cloud SDKs.

```
Domain/
│
├── Projects/
│   ├── Project.cs
│   ├── ProjectId.cs
│   └── ProjectStatus.cs
│
├── Profile/
│   ├── Profile.cs
│   ├── SocialLink.cs
│   └── CV.cs
│
├── Experience/
│   └── Experience.cs
│
└── Skills/
    ├── Skill.cs
    └── SkillCategory.cs
```

### 2. `ArchitectPortfolio.Application`
This layer encapsulates **business use cases** and orchestrates application workflow execution.

* **Components:** Commands, Queries, Handlers, DTOs, Feature-level Validators, Abstractions (Interfaces for Repositories, Storage, Notifications), Authorization policies.
* **Pattern:** Vertical slice layout inside domain modules (Feature folders per use-case).

```
Application/
└── Projects/
    │
    ├── CreateProject/
    │   ├── CreateProjectCommand.cs
    │   ├── CreateProjectHandler.cs
    │   └── CreateProjectValidator.cs
    │
    ├── GetProject/
    │   ├── GetProjectQuery.cs
    │   ├── GetProjectHandler.cs
    │   └── ProjectDto.cs
    │
    ├── UpdateProject/
    │   ├── UpdateProjectCommand.cs
    │   ├── UpdateProjectHandler.cs
    │   └── UpdateProjectValidator.cs
    │
    └── DeleteProject/
        ├── DeleteProjectCommand.cs
        └── DeleteProjectHandler.cs
```

*Example Repository Interface (Application Layer):*
```csharp
public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(
        ProjectId id,
        CancellationToken cancellationToken);
}
```

### 3. `ArchitectPortfolio.Infrastructure`
Contains technical implementations and external system integrations.

* **Components:** EF Core `DbContext`, Entity Configurations, Repository Implementations, Migrations, File Storage providers, Authentication/Identity providers, Third-party APIs (GitHub, LinkedIn, Medium), Logging & Health Checks, Service Collection Extensions.

```
Infrastructure/
│
├── Persistence/
│   ├── PortfolioDbContext.cs
│   ├── Configurations/
│   ├── Repositories/
│   └── Migrations/
│
├── Authentication/
├── Storage/
├── Integrations/
│   ├── GitHub/
│   ├── LinkedIn/
│   └── Medium/
│
├── Observability/
│   ├── Logging/
│   └── HealthChecks/
│
└── DependencyInjection/
    └── ServiceCollectionExtensions.cs
```

### 4. `ArchitectPortfolio.Api`
Serves as the application's HTTP/REST boundary and the **Composition Root**.

* **Components:** Controllers, Custom Middleware, Request/Response DTOs, Swagger/OpenAPI configs, App settings, `Program.cs`.
* **Constraint:** Controllers remain ultrathin; no direct database queries, business rule processing, or direct SDK invocations inside endpoints.

---

## Composition Root Mechanism

The **Composition Root** connects abstractions defined in `Application` / `Domain` with concrete implementations defined in `Infrastructure`.

```
┌─────────────────────────────────┐
│             API                 │
│                                 │
│          Composition Root       │
│                                 │
│ IProjectRepository ──────────┐  │
│                              │  │
└──────────────────────────────┼──┘
                               ▼
                       ProjectRepository (Infrastructure)
```

In `Program.cs` / `ServiceCollectionExtensions.cs`:
```csharp
// Program.cs wires up Infrastructure implementations to Application abstractions
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
```

---

## Formal Architectural Rules & Dependency Matrix

### Dependency Matrix

| Project | Domain | Application | Infrastructure | API |
| :--- | :---: | :---: | :---: | :---: |
| **Domain** | — | ❌ | ❌ | ❌ |
| **Application** | ✅ | — | ❌ | ❌ |
| **Infrastructure** | ✅ | ✅ | — | ❌ |
| **API** | ❌* | ✅ | ✅ | — |

*\*Note: API project should generally interact through Application contracts rather than consuming Domain entities directly.*

### Enforced Rules

1. **Rule 1:** `Domain` depends on **nothing**.
2. **Rule 2:** `Application` depends **only** on `Domain`.
3. **Rule 3:** `Infrastructure` depends on `Application` and `Domain`.
4. **Rule 4:** `API` depends on `Application` and `Infrastructure` (for DI wiring).
5. **Rule 5:** `Domain` must **never** reference EF Core, ASP.NET Core, SQL, or Infrastructure packages.
6. **Rule 6:** `Application` depends on **abstractions**, never concrete infrastructure classes.

---

## Project References (`.csproj` Configuration)

The concrete project dependencies defined in `.csproj` files:

* **`ArchitectPortfolio.Application.csproj`**
  ```xml
  <ItemGroup>
    <ProjectReference Include="..\ArchitectPortfolio.Domain\ArchitectPortfolio.Domain.csproj" />
  </ItemGroup>
  ```

* **`ArchitectPortfolio.Infrastructure.csproj`**
  ```xml
  <ItemGroup>
    <ProjectReference Include="..\ArchitectPortfolio.Domain\ArchitectPortfolio.Domain.csproj" />
    <ProjectReference Include="..\ArchitectPortfolio.Application\ArchitectPortfolio.Application.csproj" />
  </ItemGroup>
  ```

* **`ArchitectPortfolio.Api.csproj`**
  ```xml
  <ItemGroup>
    <ProjectReference Include="..\ArchitectPortfolio.Application\ArchitectPortfolio.Application.csproj" />
    <ProjectReference Include="..\ArchitectPortfolio.Infrastructure\ArchitectPortfolio.Infrastructure.csproj" />
  </ItemGroup>
  ```

---

## Testing Strategy

The test structure strictly mirrors the production project layout:

```
tests/
│
├── ArchitectPortfolio.Domain.Tests/
│   └── Tests pure domain entities, value objects, invariants, and business rules. (Fast, no external dependencies)
│
├── ArchitectPortfolio.Application.Tests/
│   └── Tests feature handlers, command validation, authorization policies using test doubles/mocks.
│
├── ArchitectPortfolio.Infrastructure.Tests/
│   └── Tests EF Core persistence, repository behavior, database integration, external service clients.
│
└── ArchitectPortfolio.Api.Tests/
    └── Tests end-to-end HTTP endpoints, serialization, request pipelines, API contracts.
```

---

## CLI Setup Commands

Commands to instantiate this exact solution setup via the .NET CLI:

```bash
# 1. Create Solution
dotnet new sln -n ArchitectPortfolio

# 2. Create Source Projects
dotnet new webapi -n ArchitectPortfolio.Api -o src/ArchitectPortfolio.Api
dotnet new classlib -n ArchitectPortfolio.Application -o src/ArchitectPortfolio.Application
dotnet new classlib -n ArchitectPortfolio.Domain -o src/ArchitectPortfolio.Domain
dotnet new classlib -n ArchitectPortfolio.Infrastructure -o src/ArchitectPortfolio.Infrastructure

# 3. Create Test Projects
dotnet new xunit -n ArchitectPortfolio.Domain.Tests -o tests/ArchitectPortfolio.Domain.Tests
dotnet new xunit -n ArchitectPortfolio.Application.Tests -o tests/ArchitectPortfolio.Application.Tests
dotnet new xunit -n ArchitectPortfolio.Infrastructure.Tests -o tests/ArchitectPortfolio.Infrastructure.Tests
dotnet new xunit -n ArchitectPortfolio.Api.Tests -o tests/ArchitectPortfolio.Api.Tests

# 4. Add Projects to Solution
dotnet sln add src/ArchitectPortfolio.Api/ArchitectPortfolio.Api.csproj
dotnet sln add src/ArchitectPortfolio.Application/ArchitectPortfolio.Application.csproj
dotnet sln add src/ArchitectPortfolio.Domain/ArchitectPortfolio.Domain.csproj
dotnet sln add src/ArchitectPortfolio.Infrastructure/ArchitectPortfolio.Infrastructure.csproj

dotnet sln add tests/ArchitectPortfolio.Domain.Tests/ArchitectPortfolio.Domain.Tests.csproj
dotnet sln add tests/ArchitectPortfolio.Application.Tests/ArchitectPortfolio.Application.Tests.csproj
dotnet sln add tests/ArchitectPortfolio.Infrastructure.Tests/ArchitectPortfolio.Infrastructure.Tests.csproj
dotnet sln add tests/ArchitectPortfolio.Api.Tests/ArchitectPortfolio.Api.Tests.csproj

# 5. Configure Project References
# Application -> Domain
dotnet add src/ArchitectPortfolio.Application/ArchitectPortfolio.Application.csproj reference src/ArchitectPortfolio.Domain/ArchitectPortfolio.Domain.csproj

# Infrastructure -> Domain & Application
dotnet add src/ArchitectPortfolio.Infrastructure/ArchitectPortfolio.Infrastructure.csproj reference src/ArchitectPortfolio.Domain/ArchitectPortfolio.Domain.csproj
dotnet add src/ArchitectPortfolio.Infrastructure/ArchitectPortfolio.Infrastructure.csproj reference src/ArchitectPortfolio.Application/ArchitectPortfolio.Application.csproj

# Api -> Application & Infrastructure
dotnet add src/ArchitectPortfolio.Api/ArchitectPortfolio.Api.csproj reference src/ArchitectPortfolio.Application/ArchitectPortfolio.Application.csproj
dotnet add src/ArchitectPortfolio.Api/ArchitectPortfolio.Api.csproj reference src/ArchitectPortfolio.Infrastructure/ArchitectPortfolio.Infrastructure.csproj

# Test References
dotnet add tests/ArchitectPortfolio.Domain.Tests/ArchitectPortfolio.Domain.Tests.csproj reference src/ArchitectPortfolio.Domain/ArchitectPortfolio.Domain.csproj
dotnet add tests/ArchitectPortfolio.Application.Tests/ArchitectPortfolio.Application.Tests.csproj reference src/ArchitectPortfolio.Application/ArchitectPortfolio.Application.csproj
dotnet add tests/ArchitectPortfolio.Infrastructure.Tests/ArchitectPortfolio.Infrastructure.Tests.csproj reference src/ArchitectPortfolio.Infrastructure/ArchitectPortfolio.Infrastructure.csproj
dotnet add tests/ArchitectPortfolio.Api.Tests/ArchitectPortfolio.Api.Tests.csproj reference src/ArchitectPortfolio.Api/ArchitectPortfolio.Api.csproj
```
