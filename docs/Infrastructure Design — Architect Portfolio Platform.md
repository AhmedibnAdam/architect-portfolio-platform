# Infrastructure Design — Architect Portfolio Platform

**System:** Architect Portfolio Platform  
**Document:** Infrastructure Design  
**Status:** Draft  
**Date:** 2026-08-31  
**Architecture Style:** Modular Monolith

---

## 1. Purpose

This document defines the infrastructure architecture of the Architect Portfolio Platform.

It translates the architectural and domain decisions into concrete technical infrastructure.

The infrastructure layer is responsible for implementing technical concerns such as:

- Persistence
- Database access
- Authentication
- Authorization
- File storage
- Caching
- External integrations
- Domain event dispatching
- Logging
- Observability
- Dependency injection
- Configuration
- Resilience
- Infrastructure testing

The Infrastructure layer must support the Domain and Application layers without introducing infrastructure concerns into the Domain model.

---

# 2. Architectural Position

The Infrastructure layer belongs to the backend Modular Monolith.

```text
┌──────────────────────────────────────────────────────┐
│                    Backend                           │
│                                                      │
│  ┌────────────┐                                     │
│  │    API     │                                     │
│  └─────┬──────┘                                     │
│        │                                             │
│        ▼                                             │
│  ┌────────────┐                                     │
│  │ Application│                                     │
│  └─────┬──────┘                                     │
│        │                                             │
│        ▼                                             │
│  ┌────────────┐                                     │
│  │   Domain   │                                     │
│  └─────▲──────┘                                     │
│        │                                             │
│  ┌─────┴──────────────┐                              │
│  │   Infrastructure   │                              │
│  └────────────────────┘                              │
│                                                      │
└──────────────────────────────────────────────────────┘
```

The dependency direction is:

```text
API
 ↓
Application
 ↓
Domain

Infrastructure
 ↓
implements Application / Domain abstractions
```

The Domain must never depend directly on Infrastructure.

---

# 3. Infrastructure Goals

Infrastructure should satisfy the following architectural goals.

### Maintainability

Infrastructure concerns must be isolated from business logic.

### Testability

External dependencies should be replaceable through abstractions.

### Reliability

Database, storage, and external-service failures must be handled predictably.

### Performance

Frequently accessed portfolio data should be efficiently retrieved and optionally cached.

### Security

Authentication, authorization, secrets, and sensitive data must be handled securely.

### Observability

The system must provide logs, metrics, traces, and health information.

### Deployability

The infrastructure must support reproducible local and production environments.

### Scalability

The application should remain stateless where possible so that additional API instances can be introduced later.

---

# 4. Initial Technology Stack

The initial infrastructure technology decisions are:

| Concern | Technology |
|---|---|
| Backend | ASP.NET Core |
| Runtime | .NET |
| API | ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Database | PostgreSQL |
| Database Hosting | Docker locally |
| Authentication | JWT-based authentication |
| Authorization | RBAC + Permissions |
| Cache | Redis |
| File Storage | S3-compatible Object Storage |
| API Documentation | OpenAPI |
| Logging | Structured logging |
| Observability | OpenTelemetry |
| Health Checks | ASP.NET Core Health Checks |
| Containers | Docker |
| Testing | xUnit + integration testing |
| CI/CD | GitHub Actions |
| Configuration | ASP.NET Core Configuration |
| Secrets | Environment / Secret Management |

These are **initial decisions** and may change during implementation.

---

# 5. Infrastructure Components

```text
Infrastructure
│
├── Persistence
│   ├── EF Core
│   ├── DbContext
│   ├── Entity Configurations
│   ├── Repositories
│   ├── Transactions
│   └── Migrations
│
├── Authentication
│   ├── JWT
│   └── Identity Integration
│
├── Authorization
│   ├── Roles
│   └── Permissions
│
├── Storage
│   └── Object Storage
│
├── Caching
│   └── Redis
│
├── External Integrations
│   ├── GitHub
│   ├── LinkedIn
│   └── Medium
│
├── Domain Events
│   └── Event Dispatcher
│
├── Observability
│   ├── Logging
│   ├── Metrics
│   └── Tracing
│
├── Health Checks
│
├── Configuration
│
└── Dependency Injection
```

---

# 6. Persistence

## 6.1 Database

The initial database technology is:

```text
PostgreSQL
```

The database will run locally through Docker.

```text
┌───────────────────┐
│ ASP.NET Core API  │
└─────────┬─────────┘
          │
          │ EF Core
          ▼
┌───────────────────┐
│    PostgreSQL     │
└───────────────────┘
```

The application must not depend on PostgreSQL-specific features unless there is a clear architectural reason.

---

# 7. Entity Framework Core

Entity Framework Core will provide the persistence abstraction and object-relational mapping.

Responsibilities:

- Entity mapping
- Database queries
- Change tracking
- Transactions
- Migrations
- Concurrency handling
- Relationship mapping

Example structure:

```text
Infrastructure
└── Persistence
    ├── PortfolioDbContext.cs
    │
    ├── Configurations
    │   ├── ArchitectProfileConfiguration.cs
    │   ├── ExperienceConfiguration.cs
    │   ├── SkillConfiguration.cs
    │   ├── ProjectConfiguration.cs
    │   ├── ProjectImageConfiguration.cs
    │   ├── ArticleConfiguration.cs
    │   └── ...
    │
    ├── Repositories
    │   ├── ProjectRepository.cs
    │   ├── ArticleRepository.cs
    │   └── ...
    │
    └── Migrations
```

---

# 8. DbContext

A dedicated DbContext will represent the persistence boundary.

```text
PortfolioDbContext
│
├── ArchitectProfiles
├── Experiences
├── Skills
├── SocialProfiles
├── Projects
├── ProjectImages
├── Articles
├── Categories
├── Tags
├── Documents
│
├── Users
├── Roles
├── Permissions
├── UserRoles
├── RolePermissions
└── AuditLogs
```

The DbContext remains inside Infrastructure.

The Domain must not reference it.

---

# 9. Entity Configuration

Entity configuration will use separate configuration classes rather than placing database-specific configuration directly inside domain entities.

Example:

```text
Project
   │
   ▼
ProjectConfiguration
   │
   ▼
EF Core Mapping
   │
   ▼
PROJECTS
```

This keeps persistence concerns separated from business behavior.

---

# 10. Repository Strategy

Repositories will be used around aggregate roots.

Examples:

```text
IProjectRepository
IArticleRepository
IArchitectProfileRepository
IUserRepository
IRoleRepository
```

The Domain/Application layers depend on abstractions.

Infrastructure provides implementations.

```text
Application
    │
    ▼
IProjectRepository
    ▲
    │
    │ implements
    │
ProjectRepository
    │
    ▼
EF Core
```

Repositories should not be created for every database table automatically.

The aggregate boundary determines the repository boundary.

---

# 11. Transactions

Application commands that modify persistent state should execute within appropriate transaction boundaries.

Example:

```text
PublishProjectCommand
        │
        ▼
Load Project
        │
        ▼
Validate Domain Rules
        │
        ▼
Publish Project
        │
        ▼
Raise ProjectPublished
        │
        ▼
Persist Changes
        │
        ▼
Commit Transaction
```

Transactions should protect consistency without creating unnecessarily large transaction scopes.

---

# 12. Optimistic Concurrency

Important mutable aggregates will use optimistic concurrency.

Initial entities include:

```text
ArchitectProfile
Project
Article
User
```

A concurrency token such as:

```text
row_version
```

will be used where appropriate.

Example:

```text
Client A reads version 5
Client B reads version 5

Client A updates
        ↓
Version becomes 6

Client B attempts update using version 5
        ↓
Concurrency conflict
        ↓
HTTP 409 Conflict
```

---

# 13. Database Migrations

EF Core migrations will manage schema evolution.

Example workflow:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Migrations will be committed to source control.

Production deployments should apply migrations through a controlled deployment process rather than relying on developers manually modifying the production database.

---

# 14. Authentication

The Administration Dashboard requires authentication.

The initial API design uses token-based authentication.

```text
Admin Dashboard
      │
      │ Login
      ▼
Authentication
      │
      ▼
JWT Access Token
      │
      ▼
REST API
```

The API validates the token before allowing protected operations.

Authentication concerns remain outside the Portfolio domain.

---

# 15. Authorization

Authorization uses:

```text
RBAC
+
Permissions
```

Conceptually:

```text
User
 │
 ├── Role
 │     │
 │     └── Permissions
 │
 └── Role
       │
       └── Permissions
```

Example:

```text
Editor
 ├── projects:read
 ├── projects:write
 ├── articles:read
 └── articles:write

SuperAdmin
 ├── users:read
 ├── users:write
 ├── projects:publish
 ├── articles:publish
 └── ...
```

Authorization must be enforced at the application/API boundary.

The Domain should not know about JWT, HTTP claims, or ASP.NET authorization attributes.

---

# 16. File Storage

Binary files should not normally be stored directly inside PostgreSQL.

Examples:

- CV
- Project images
- Future portfolio media

The architecture uses object storage.

```text
API
 │
 ▼
File Storage Abstraction
 │
 ▼
Object Storage
 │
 └── CV
 └── Project Images
 └── Other Media
```

The database stores metadata:

```text
Document
├── FileName
├── ContentType
├── FileSize
├── StorageKey
└── UploadedAt
```

This keeps large binary data outside the relational database.

---

# 17. Storage Abstraction

The Application layer should depend on an abstraction such as:

```text
IFileStorage
```

Infrastructure provides the implementation.

```text
Application
     │
     ▼
IFileStorage
     ▲
     │
     ▼
S3FileStorage
```

This allows the storage provider to change without modifying business logic.

For example:

```text
S3
Azure Blob Storage
MinIO
Other Object Storage
```

can be substituted behind the same abstraction.

---

# 18. Caching

Redis is the initial caching technology.

Caching is intended primarily for read-heavy public portfolio data.

Potential cache candidates:

```text
Portfolio Profile
Published Projects
Published Articles
Skills
Social Profiles
```

Example:

```text
Portfolio Web
      │
      ▼
REST API
      │
      ▼
Cache
 ┌────┴────┐
 │         │
Hit       Miss
 │         │
 ▼         ▼
Return   PostgreSQL
           │
           ▼
         Cache
```

The cache must never become the system of record.

PostgreSQL remains the source of truth.

---

# 19. Cache Invalidation

Domain events can be used to trigger cache invalidation.

Example:

```text
ProjectPublished
       │
       ▼
Invalidate:
projects:published
       │
       ▼
Next request
       │
       ▼
PostgreSQL
       │
       ▼
Repopulate Cache
```

Cache invalidation must be designed carefully because stale public data is possible.

The system should prefer predictable invalidation over complex caching strategies during the initial implementation.

---

# 20. External Integrations

The platform may integrate with external professional platforms.

Potential integrations:

```text
GitHub
LinkedIn
Medium
```

These integrations should be isolated behind interfaces.

Example:

```text
Application
    │
    ▼
IGitHubService
    ▲
    │
    ▼
GitHubClient
```

The same principle applies to:

```text
ILinkedInService
IMediumService
```

External APIs must never become direct dependencies of the Domain layer.

---

# 21. External Integration Resilience

External services can fail independently from our platform.

Therefore integrations should support:

```text
Timeout
Retry where appropriate
Circuit breaking where justified
Error handling
Logging
Observability
```

Retries must only be applied to operations where retrying is safe.

The initial implementation should avoid unnecessary distributed-system complexity.

---

# 22. Domain Events

Domain events represent business events such as:

```text
ProjectPublished
ArticlePublished
ProfileUpdated
UserRoleAssigned
```

Initial event flow:

```text
Domain Aggregate
      │
      ▼
Domain Event
      │
      ▼
Application Event Dispatcher
      │
      ▼
Infrastructure Handler
```

Example:

```text
ProjectPublished
       │
       ├── Invalidate Cache
       │
       └── Update Search Index
```

The initial implementation can use in-process event dispatching.

A message broker should only be introduced if future requirements justify asynchronous distributed processing.

---

# 23. Logging

The application will use structured logging.

Logs should contain contextual information such as:

```text
Timestamp
Log Level
Request ID
Correlation ID
User ID where appropriate
Operation
Duration
Result
Error information
```

Example conceptual log:

```text
ProjectPublished
projectId=...
userId=...
duration=...
```

Sensitive information must not be logged.

Passwords, access tokens, secrets, and other credentials must never appear in logs.

---

# 24. Observability

Observability will initially use three pillars:

```text
Logs
Metrics
Traces
```

```text
                Observability
                     │
          ┌──────────┼──────────┐
          ▼          ▼          ▼
        Logs      Metrics     Traces
```

OpenTelemetry will provide the common instrumentation approach.

Important measurements include:

- API request duration
- Request rate
- Error rate
- Database operation duration
- Cache hit/miss rate
- External API failures
- Authentication failures

---

# 25. Health Checks

The API should expose health endpoints.

Example:

```text
/health
/health/ready
```

Readiness should verify required infrastructure dependencies.

Potential checks:

```text
API
 │
 ├── PostgreSQL
 │
 ├── Redis
 │
 └── Object Storage
```

Example:

```text
GET /health
      │
      ├── Database       ✓
      ├── Redis          ✓
      └── Storage        ✓
```

The exact health-check strategy may differ between local and production environments.

---

# 26. Configuration

Configuration must be externalized.

Examples:

```text
Database connection string
JWT configuration
Redis connection
Object storage configuration
External API credentials
Logging configuration
```

Configuration hierarchy:

```text
Application
    │
    ▼
ASP.NET Core Configuration
    │
    ├── appsettings.json
    ├── appsettings.{Environment}.json
    ├── Environment Variables
    └── Secret Management
```

Secrets must not be committed to source control.

---

# 27. Dependency Injection

ASP.NET Core's built-in dependency injection container will be used.

Infrastructure registrations should be centralized.

Example:

```text
Infrastructure
└── DependencyInjection.cs
```

Conceptually:

```csharp
services.AddInfrastructure(configuration);
```

This method registers:

```text
DbContext
Repositories
File Storage
Cache
External Clients
Event Handlers
Observability
```

The API startup should not contain detailed infrastructure construction logic.

---

# 28. Docker

Docker will provide reproducible local infrastructure.

Initial local infrastructure:

```text
Docker
│
├── PostgreSQL
│
└── Redis
```

Potential future services:

```text
Object Storage
Message Broker
Observability Stack
```

Services should only be added when required.

Example:

```text
docker compose up -d
```

starts the required infrastructure.

---

# 29. Local Development Architecture

```text
┌────────────────────────────────────────────┐
│                 Developer                  │
│                                            │
│  ┌─────────────┐                           │
│  │ ASP.NET API │                           │
│  └──────┬──────┘                           │
│         │                                  │
│         ├───────────────┐                  │
│         ▼               ▼                  │
│  ┌────────────┐   ┌────────────┐           │
│  │ PostgreSQL │   │   Redis    │           │
│  │  Docker    │   │  Docker    │           │
│  └────────────┘   └────────────┘           │
│                                            │
└────────────────────────────────────────────┘
```

The API may run directly from the development machine while infrastructure dependencies run through Docker.

The entire application may later be containerized as well.

---

# 30. Production Architecture

The initial production topology should remain simple.

```text
                 Internet
                    │
                    ▼
             ┌─────────────┐
             │ Load Balancer│
             └──────┬──────┘
                    │
             ┌──────▼──────┐
             │ ASP.NET API │
             └──────┬──────┘
                    │
          ┌─────────┼─────────┐
          ▼         ▼         ▼
     PostgreSQL   Redis   Object Storage
```

The API should remain stateless so multiple instances can be introduced later.

---

# 31. Security Boundaries

```text
                    Internet
                       │
                       ▼
                ┌─────────────┐
                │    HTTPS    │
                └──────┬──────┘
                       │
                       ▼
                ┌─────────────┐
                │     API     │
                └──────┬──────┘
                       │
          ┌────────────┼─────────────┐
          ▼            ▼             ▼
     Application   Authentication  Authorization
          │
          ▼
      Infrastructure
```

Security requirements include:

- HTTPS
- Secure authentication
- Strong password hashing if local credentials are used
- Token validation
- Role/permission enforcement
- Input validation
- Rate limiting where required
- Secure secrets management
- Least-privilege database access
- Secure file handling
- Audit logging

---

# 32. Error Handling

Infrastructure exceptions should not leak directly to API consumers.

Example:

```text
PostgreSQL Exception
        │
        ▼
Infrastructure
        │
        ▼
Application / API Error Mapping
        │
        ▼
Problem Details Response
```

The API should expose standardized error responses.

Example:

```text
400 Bad Request
401 Unauthorized
403 Forbidden
404 Not Found
409 Conflict
422 Unprocessable Entity
500 Internal Server Error
```

Internal implementation details should not be exposed to clients.

---

# 33. Performance Strategy

Performance optimization follows the quality attributes defined earlier.

Initial strategy:

```text
1. Efficient database queries
2. Proper indexes
3. Pagination
4. Async I/O
5. Redis caching
6. Efficient file storage
7. Avoid unnecessary external API calls
```

Optimization should be evidence-driven.

Premature optimization should be avoided.

---

# 34. Scalability Strategy

The initial system is a Modular Monolith.

Scaling strategy:

```text
                    Load Balancer
                         │
              ┌──────────┼──────────┐
              ▼          ▼          ▼
           API #1      API #2      API #3
              │          │          │
              └──────────┼──────────┘
                         │
                ┌────────┴────────┐
                ▼                 ▼
            PostgreSQL          Redis
```

The application should avoid local server state where possible.

User sessions, cache state, and uploaded files should not depend on a specific API instance.

---

# 35. Infrastructure Testing

Infrastructure will be tested using integration tests.

Examples:

```text
Database Integration Tests
Repository Tests
API Integration Tests
Authentication Tests
Authorization Tests
Cache Integration Tests
External Integration Tests
```

A test database should be isolated from development and production data.

Docker can be used to provide reproducible infrastructure for integration testing.

---

# 36. Infrastructure Project Structure

The backend Infrastructure project should evolve toward:

```text
Infrastructure/
│
├── Persistence/
│   ├── PortfolioDbContext.cs
│   │
│   ├── Configurations/
│   │
│   ├── Repositories/
│   │
│   └── Migrations/
│
├── Authentication/
│
├── Authorization/
│
├── Storage/
│
├── Caching/
│
├── Integrations/
│   ├── GitHub/
│   ├── LinkedIn/
│   └── Medium/
│
├── Events/
│
├── Observability/
│
├── HealthChecks/
│
├── Configuration/
│
└── DependencyInjection.cs
```

The exact folder structure may evolve as implementation progresses.

---

# 37. Dependency Direction

The final dependency rule is:

```text
┌─────────────┐
│     API     │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│ Application │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│   Domain    │
└─────────────┘
       ▲
       │
┌──────┴──────────┐
│ Infrastructure  │
└─────────────────┘
```

More precisely:

```text
API
 └──> Application
       └──> Domain

Infrastructure
 ├──> Application
 └──> Domain
```

The Domain has no dependency on:

```text
EF Core
PostgreSQL
Redis
ASP.NET Core
JWT
S3
Docker
```

---

# 38. Technology Decision Principles

Technology selection must follow the same principle established throughout the architecture process:

> **Architecture should be driven by requirements, not preferences.**

Therefore, technologies may be replaced when implementation evidence or architectural drivers justify the change.

For example:

```text
PostgreSQL
    ↓
Could change to SQL Server

Redis
    ↓
Could be removed if caching is unnecessary

S3
    ↓
Could become Azure Blob Storage

JWT
    ↓
Could become an external Identity Provider
```

Such changes should be documented through Architecture Decision Records.

---

# 39. Architecture Decision Records

Infrastructure decisions should be documented separately when they have significant architectural impact.

Potential ADRs:

```text
docs/architecture/adr/

ADR-001-architectural-style.md
ADR-002-database-selection.md
ADR-003-authentication-strategy.md
ADR-004-file-storage.md
ADR-005-caching-strategy.md
ADR-006-observability.md
```

Not every implementation detail requires an ADR.

ADRs should focus on decisions with meaningful architectural consequences.

---

# 40. Open Decisions

The following decisions remain intentionally open:

- Final database provider
- Production cloud provider
- Production object-storage provider
- Production Redis hosting
- Identity provider strategy
- Search strategy
- Message broker requirement
- CDN requirement
- Deployment topology
- Disaster recovery strategy

These will be finalized when their requirements and operational constraints are better understood.

---

# 41. Initial Infrastructure Decision

The initial infrastructure architecture is:

```text
                  ┌──────────────────────┐
                  │   Portfolio Website  │
                  └──────────┬───────────┘
                             │
                  ┌──────────▼───────────┐
                  │   Admin Dashboard    │
                  └──────────┬───────────┘
                             │
                  ┌──────────▼───────────┐
                  │     iOS Application  │
                  └──────────┬───────────┘
                             │
                             ▼
                  ┌──────────────────────┐
                  │      REST API       │
                  └──────────┬───────────┘
                             │
                  ┌──────────▼───────────┐
                  │    Application       │
                  └──────────┬───────────┘
                             │
                  ┌──────────▼───────────┐
                  │       Domain         │
                  └──────────▲───────────┘
                             │
                  ┌──────────┴───────────┐
                  │    Infrastructure    │
                  └──────┬────┬────┬─────┘
                         │    │    │
                         ▼    ▼    ▼
                    PostgreSQL Redis Storage
```

This architecture provides a simple starting point while preserving the ability to scale and replace infrastructure components as the platform evolves.

---

# 42. Implementation Readiness

The architecture is now ready to transition from architectural design into technical implementation.

The implementation sequence will be:

```text
Infrastructure Design
        ↓
Technology Decisions / ADRs
        ↓
.NET Solution
        ↓
Project References
        ↓
Domain Implementation
        ↓
Application Implementation
        ↓
Infrastructure Implementation
        ↓
API Implementation
        ↓
Database Migrations
        ↓
Portfolio Web
        ↓
Admin Dashboard
        ↓
iOS Application
        ↓
Integration Testing
        ↓
CI/CD
        ↓
Deployment
```

The next immediate implementation artifact is the **actual .NET solution and project structure**.