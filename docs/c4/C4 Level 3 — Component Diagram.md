# C4 Level 3 — Component Diagram

**System:** Architect Portfolio Platform  
**Container:** Portfolio API  
**Diagram Level:** C4 Level 3 — Component  
**Status:** Draft  
**Date:** 2026-08-30

---

## 1. Purpose

The Component Diagram zooms into the **Portfolio API** container and describes its major internal components.

The objective is to establish:

- Responsibility boundaries
- Dependency direction
- Business module boundaries
- Application orchestration
- Infrastructure responsibilities
- External integration boundaries

This diagram intentionally does not describe individual classes or implementation details.

---

# 2. Component Diagram

```text
                         Portfolio API
                    ASP.NET Core Application
                              │
                              │
                    ┌─────────▼─────────┐
                    │   API / Web Layer │
                    │                   │
                    │ Controllers       │
                    │ Request Models     │
                    │ Response Models    │
                    │ Exception Mapping  │
                    └─────────┬─────────┘
                              │
                              ▼
                    ┌─────────────────────┐
                    │  Application Layer  │
                    │                     │
                    │ Use Cases           │
                    │ Commands / Queries  │
                    │ Validation          │
                    │ Authorization       │
                    │ DTO Mapping         │
                    └──────────┬──────────┘
                               │
                               ▼
              ┌────────────────────────────────┐
              │          Domain Layer           │
              │                                │
              │ ┌──────────┐ ┌──────────────┐ │
              │ │ Profile  │ │ Experience   │ │
              │ └──────────┘ └──────────────┘ │
              │                                │
              │ ┌──────────┐ ┌──────────────┐ │
              │ │  Skills  │ │   Projects   │ │
              │ └──────────┘ └──────────────┘ │
              │                                │
              │ ┌──────────┐ ┌──────────────┐ │
              │ │ Articles │ │ Administration│ │
              │ └──────────┘ └──────────────┘ │
              │                                │
              │        Business Rules          │
              └────────────────┬───────────────┘
                               │
                               ▼
                 ┌──────────────────────────┐
                 │   Infrastructure Layer   │
                 │                          │
                 │ EF Core                  │
                 │ Repositories             │
                 │ Database Access           │
                 │ File Storage              │
                 │ External Integrations    │
                 │ Authentication Provider  │
                 └────────────┬─────────────┘
                              │
                 ┌────────────┼─────────────┐
                 ▼            ▼             ▼
             Database     File Storage   External APIs
                                          │
                                    ┌─────┼─────┐
                                    ▼     ▼     ▼
                                  GitHub LinkedIn Medium
```

---

# 3. Architectural Layers

The Portfolio API is organized into four primary architectural layers:

```text
API
 ↓
Application
 ↓
Domain
 ↓
Infrastructure
```

The most important dependency rule is:

> **Business logic must not depend on infrastructure implementation details.**

Infrastructure implements the technical mechanisms required by the application and domain.

---

# 4. API / Web Layer

## Responsibility

The API layer represents the HTTP boundary of the system.

It is responsible for:

- HTTP endpoints
- Routing
- Request binding
- Authentication entry points
- Authorization policies
- HTTP response mapping
- API contracts
- Exception-to-response mapping

Example:

```text
HTTP Request
     │
     ▼
ProjectsController
     │
     ▼
GetProjectQuery
     │
     ▼
Application Layer
```

The API layer should contain minimal business logic.

### It should NOT:

- Implement business rules
- Directly manipulate EF Core entities
- Contain database queries
- Call external APIs directly
- Contain complex workflows

---

# 5. Application Layer

## Responsibility

The Application Layer orchestrates application use cases.

It translates an external request into a business operation.

Examples:

```text
Get Profile
Update Profile
Get Projects
Create Project
Update Project
Delete Project
Publish Article
Manage Skills
```

The Application Layer is responsible for:

- Use cases
- Commands
- Queries
- Validation orchestration
- Authorization orchestration
- DTOs
- Mapping
- Transaction coordination
- Calling domain behavior
- Coordinating repositories

Example:

```text
Get Project
     │
     ▼
GetProjectQueryHandler
     │
     ▼
Project Repository
     │
     ▼
Domain / Persistence
```

The Application Layer should not contain infrastructure implementation details.

---

# 6. Domain Layer

## Responsibility

The Domain Layer contains the business model and business rules.

Initial domain areas are:

```text
Profile
Experience
Skills
Projects
Articles
Administration
```

The domain should contain:

- Entities
- Value Objects where justified
- Domain rules
- Domain services where required
- Domain events where justified
- Business invariants

Example:

```text
Project
 ├── Title
 ├── Description
 ├── Technologies
 ├── RepositoryUrl
 ├── DemoUrl
 └── Visibility
```

The domain model should represent business concepts rather than database structures.

---

# 7. Domain Modules

The Modular Monolith will initially organize the domain into logical business modules.

## Profile Module

Responsible for:

- Professional profile
- Summary
- Contact information
- Professional identity

---

## Experience Module

Responsible for:

- Employment history
- Positions
- Companies
- Responsibilities
- Achievements
- Dates

---

## Skills Module

Responsible for:

- Technical skills
- Skill categories
- Proficiency information
- Technologies

---

## Projects Module

Responsible for:

- Portfolio projects
- Project descriptions
- Technologies
- Repository references
- Demo references
- Project metadata

---

## Articles Module

Responsible for:

- Article metadata
- External article references
- Publication state
- Categories/tags when introduced

The portfolio platform initially stores references rather than attempting to duplicate all external article content.

---

## Administration Module

Responsible for:

- Administrative users
- Administrative operations
- Authorization-related business rules
- Content management permissions

Authentication implementation itself remains an infrastructure/security concern.

---

# 8. Infrastructure Layer

## Responsibility

The Infrastructure Layer provides technical implementations required by the application.

It contains:

- EF Core
- Database access
- Repository implementations
- File storage implementations
- External API clients
- Authentication infrastructure
- Logging infrastructure
- Infrastructure configuration

Conceptually:

```text
Application / Domain
        │
        │ abstractions
        ▼
Infrastructure
        │
   ┌────┼───────────────┐
   ▼    ▼               ▼
 EF Core File Storage External APIs
```

Infrastructure details should remain replaceable.

---

# 9. Persistence

The initial persistence strategy is a relational database.

The Infrastructure Layer will provide:

```text
Application
     │
     ▼
Repository Abstraction
     │
     ▼
EF Core
     │
     ▼
Relational Database
```

The concrete database technology is intentionally not fixed by this C4 diagram.

That decision belongs to technical design.

---

# 10. External Integration Boundary

External integrations must be isolated.

```text
                    Application
                         │
                         ▼
              External Integration
                    Abstractions
                         │
                         ▼
                 Infrastructure
                         │
              ┌──────────┼──────────┐
              ▼          ▼          ▼
           GitHub      LinkedIn    Medium
```

This prevents external platform-specific implementation details from leaking into the domain.

It also allows future integrations to be introduced without changing core business rules.

---

# 11. Authentication and Authorization

Authentication and authorization are cross-cutting security responsibilities.

Conceptually:

```text
                    HTTP Request
                         │
                         ▼
                Authentication
                         │
                         ▼
                 Authorization
                         │
                         ▼
                  Application
                         │
                         ▼
                    Use Case
```

Authorization should be enforced at the application/API boundary and must not rely solely on client-side restrictions.

---

# 12. Dependency Direction

The most important architectural rule is:

```text
API
 │
 ▼
Application
 │
 ▼
Domain

Infrastructure
 │
 └──── implements abstractions required by
       Application / Domain
```

The preferred dependency relationship is therefore:

```text
API ───────────────▶ Application
                       │
                       ▼
                     Domain

Infrastructure ───────▶ Application / Domain
```

The Domain must not depend on:

- ASP.NET Core
- EF Core
- HTTP
- External APIs
- Database-specific implementation
- File-storage providers

This keeps business rules independent from infrastructure technology.

---

# 13. Component Interaction Example

Consider the use case:

> Administrator updates a project.

The flow is:

```text
Admin Dashboard
      │
      │ PUT /api/projects/{id}
      ▼
Projects Controller
      │
      ▼
Update Project Command
      │
      ▼
Update Project Handler
      │
      ▼
Project Domain Model
      │
      │ Business Rules
      ▼
Project Repository
      │
      ▼
EF Core
      │
      ▼
Database
```

The controller does not implement the business rule.

The database does not contain the business workflow.

The domain remains responsible for business invariants.

---

# 14. Public Read Example

Consider:

> Visitor opens a project.

The flow is:

```text
Visitor
   │
   ▼
Public Website
   │
   │ GET /api/projects/{id}
   ▼
Projects Controller
   │
   ▼
Get Project Query
   │
   ▼
Query Handler
   │
   ▼
Project Repository
   │
   ▼
Database
   │
   ▼
Project DTO
   │
   ▼
REST Response
   │
   ▼
Public Website
```

The visitor does not interact directly with the domain database.

---

# 15. Module Boundary Principle

Although the application is a monolith, modules should behave as if they were independent boundaries.

For example:

```text
Projects
    │
    ├── Project Entity
    ├── Project Use Cases
    ├── Project Repository
    └── Project API

Articles
    │
    ├── Article Entity
    ├── Article Use Cases
    ├── Article Repository
    └── Article API
```

A module should avoid directly accessing another module's internal implementation.

Instead:

```text
Projects
   │
   ▼
Public Contract
   │
   ▼
Articles
```

This is important because it preserves the possibility of future module extraction if a genuine requirement appears.

---

# 16. Why We Are Not Introducing CQRS Yet

The architecture does not initially introduce full CQRS.

The system currently has relatively simple CRUD-oriented portfolio operations.

Therefore:

```text
Command
Query
```

may exist as application concepts where useful, but we are not introducing separate read/write databases or independent architectures without a demonstrated need.

CQRS can be introduced later if requirements create drivers such as:

- Highly complex read models
- Significant read/write asymmetry
- Independent scaling requirements
- Complex reporting
- Performance constraints

The project roadmap explicitly treats CQRS as a future architectural capability rather than an MVP requirement.

---

# 17. Why We Are Not Introducing Domain Events Yet

The domain model should support future evolution, but domain events are not required simply because DDD is being considered.

They should be introduced when a real requirement exists, such as:

```text
Project Published
       │
       ├── Update Analytics
       ├── Send Notification
       └── Synchronize External Platform
```

At that point an event-driven approach can be evaluated.

For the MVP, synchronous application workflows are sufficient.

---

# 18. Architectural Constraints

The following constraints apply to the initial component architecture:

### Constraint 1

Domain logic must remain independent of infrastructure.

### Constraint 2

Controllers must remain thin.

### Constraint 3

Database access belongs in Infrastructure.

### Constraint 4

External APIs belong behind integration abstractions.

### Constraint 5

Modules should communicate through explicit contracts.

### Constraint 6

Cross-module access to internal implementation is discouraged.

### Constraint 7

New architectural complexity requires an identified requirement or technical problem.

---

# 19. Initial Component Structure

The resulting conceptual structure is:

```text
Portfolio API
│
├── API
│   ├── Controllers
│   ├── Contracts
│   ├── Authentication
│   └── HTTP concerns
│
├── Application
│   ├── Profile
│   ├── Experience
│   ├── Skills
│   ├── Projects
│   ├── Articles
│   └── Administration
│
├── Domain
│   ├── Profile
│   ├── Experience
│   ├── Skills
│   ├── Projects
│   ├── Articles
│   └── Administration
│
└── Infrastructure
    ├── Persistence
    ├── File Storage
    ├── External Integrations
    ├── Authentication
    └── Observability
```

---

# 20. Architectural Outcome

The C4 Level 3 design establishes a:

> **Modular Monolith using layered dependency boundaries.**

The architecture combines:

```text
Modular Monolith
       +
API / Application / Domain / Infrastructure boundaries
       +
Explicit business modules
       +
REST API
       +
Infrastructure isolation
```

This gives the project strong internal structure without prematurely introducing distributed-system complexity.

---

# 21. Next Step

The next architectural activity is **Domain Modeling**.

We now know the major modules:

```text
Profile
Experience
Skills
Projects
Articles
Administration
```

The next question is:

> **What are the actual domain concepts, entities, relationships, invariants, and boundaries inside these modules?**

Therefore the next deliverable should be:

```text
docs/architecture/domain-model.md
```

We will identify:

- Entities
- Value Objects
- Aggregates
- Relationships
- Business rules
- Module boundaries
- Cross-module dependencies
- Potential bounded contexts

Only after this should we finalize the actual project/folder structure.