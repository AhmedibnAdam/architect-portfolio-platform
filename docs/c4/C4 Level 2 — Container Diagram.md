# C4 Level 2 — Container Diagram

**System:** Architect Portfolio Platform  
**Diagram Level:** C4 Level 2 — Container  
**Status:** Draft  
**Date:** 2026-08-30

---

## 1. Purpose

The Container Diagram decomposes the Architect Portfolio Platform into its major runtime building blocks.

It shows:

- Client applications
- Backend API
- Database
- Authentication boundary
- External integration boundary
- File storage
- Communication between containers

It intentionally does not describe classes, interfaces, controllers, repositories, or individual code components.

---

# 2. Container Diagram

```text
                           PEOPLE
                              │
             ┌────────────────┴────────────────┐
             │                                 │
             ▼                                 ▼
       ┌──────────────┐                 ┌──────────────┐
       │   Visitor    │                 │    Admin     │
       └──────┬───────┘                 └──────┬───────┘
              │                                │
              ▼                                ▼
       ┌──────────────┐                 ┌──────────────┐
       │    Public    │                 │    Admin     │
       │    Website   │                 │   Dashboard  │
       └──────┬───────┘                 └──────┬───────┘
              │                                │
              │          HTTPS / REST          │
              └───────────────┬────────────────┘
                              │
                              ▼
                   ┌─────────────────────┐
                   │                     │
                   │   Portfolio API     │
                   │   ASP.NET Core      │
                   │                     │
                   │  Application Logic  │
                   │  Business Rules     │
                   │  Authentication     │
                   │  Authorization      │
                   │  REST Endpoints     │
                   │                     │
                   └──────────┬──────────┘
                              │
             ┌────────────────┼────────────────┐
             │                │                │
             │                │                │
             ▼                ▼                ▼
      ┌──────────────┐ ┌──────────────┐ ┌───────────────┐
      │  Relational  │ │    File      │ │   External    │
      │   Database   │ │   Storage    │ │ Integrations  │
      │              │ │              │ │               │
      │ Portfolio    │ │ CV / Assets  │ │ GitHub        │
      │ Data         │ │              │ │ LinkedIn      │
      │              │ │              │ │ Medium        │
      └──────────────┘ └──────────────┘ └───────────────┘


                         iOS Client
                              │
                              │ HTTPS / REST
                              ▼
                   ┌─────────────────────┐
                   │    Portfolio API    │
                   └─────────────────────┘
```

---

# 3. Containers

## 3.1 Public Website

**Type:** Client Application

**Technology:** To be decided

**Responsibility:**

Provides the public-facing portfolio experience.

It allows visitors to:

- View profile
- View experience
- View skills
- View projects
- View articles
- Access CV
- Access social links
- Contact the portfolio owner

**Communication:**

```text
Public Website
      │
    HTTPS
      │
      ▼
Portfolio API
```

The website does not directly access the database.

---

## 3.2 Administration Dashboard

**Type:** Client Application

**Technology:** To be decided

**Responsibility:**

Provides the portfolio owner with an interface for managing portfolio content.

Capabilities include:

- Authentication
- Authorization
- Profile management
- Experience management
- Skills management
- Project management
- Article management
- Social link management
- CV management

The administration dashboard communicates with the backend exclusively through the API.

```text
Admin Dashboard
       │
     HTTPS
       │
       ▼
Portfolio API
```

---

## 3.3 iOS Application

**Type:** Client Application

**Technology:** Swift / iOS

**Status:** Future/Should Have

**Responsibility:**

Provides a mobile client for consuming portfolio information.

The iOS application uses the same backend API as the public website.

```text
iOS Application
       │
     HTTPS
       │
       ▼
Portfolio API
```

The project requirements explicitly define the iOS application as a client consuming the same backend API.

---

# 4. Portfolio API

**Type:** Backend Application

**Technology:** .NET / ASP.NET Core

**Deployment:** Single deployable application

**Responsibility:**

The Portfolio API is the primary application container.

It is responsible for:

- REST API endpoints
- Request handling
- Authentication
- Authorization
- Validation
- Business rules
- Application workflows
- Portfolio management
- Persistence coordination
- Error handling
- Logging
- Health checks

The API acts as the only application-level gateway to portfolio data.

```text
Clients
   │
 HTTPS
   ▼
Portfolio API
   │
   ├── Business Logic
   ├── Validation
   ├── Authentication
   ├── Authorization
   └── Persistence
```

The initial backend MVP explicitly includes ASP.NET Core, domain/application/infrastructure layers, EF Core, authentication, authorization, REST endpoints, tests, health checks, and logging.

---

# 5. Relational Database

**Type:** Database

**Technology:** To be decided

**Responsibility:**

Stores the platform's structured application data.

Potential data includes:

- Profile
- Experience
- Skills
- Projects
- Articles
- Social links
- CV metadata
- Administrative data

The database is accessed by the Portfolio API.

Clients never access the database directly.

```text
Public Website ──┐
Admin Dashboard ─┼──▶ Portfolio API ──▶ Database
iOS Application ─┘
```

The exact database technology will be decided separately.

---

# 6. File Storage

**Type:** External/Infrastructure Container

**Responsibility:**

Stores files and binary assets such as:

- CV
- Portfolio images
- Project images
- Other uploaded assets

The API manages access to stored files.

Clients should not require direct knowledge of the underlying storage implementation.

```text
Client
   │
   ▼
Portfolio API
   │
   ▼
File Storage
```

The concrete storage technology remains an open technical-design decision.

---

# 7. External Integrations

**Type:** Integration Boundary

**External Systems:**

- GitHub
- LinkedIn
- Medium

**Responsibility:**

Provides controlled interaction with external professional platforms when required.

For the MVP, these platforms primarily provide external references/links.

Future releases may introduce synchronization or integration capabilities.

The SRS explicitly states that external platforms remain authoritative for their respective external content.

Therefore:

```text
Portfolio API
      │
      ▼
External Integration Boundary
      │
 ┌────┼─────┐
 ▼    ▼     ▼
GitHub LinkedIn Medium
```

External APIs should not be called directly from domain logic.

---

# 8. Authentication and Authorization

Authentication and authorization are treated as responsibilities of the Portfolio API rather than separate microservices.

The initial model is:

```text
Admin
  │
  │ credentials
  ▼
Portfolio API
  │
  ├── Authentication
  │
  └── Authorization
          │
          ▼
   Administrative Operations
```

This avoids introducing a dedicated identity service when the current system does not require one.

The exact authentication mechanism will be decided during technical design.

---

# 9. Communication Model

The primary communication protocol is:

> **HTTPS + REST**

Client applications communicate synchronously with the Portfolio API.

```text
Website ──────┐
              │
Dashboard ────┼── HTTPS/REST ──▶ Portfolio API
              │
iOS ──────────┘
```

The initial architecture does not introduce:

- gRPC
- Message brokers
- Event buses
- Service mesh
- Internal asynchronous messaging

These may be introduced later if justified by architectural drivers.

---

# 10. Container Responsibilities

| Container | Primary Responsibility | State |
|---|---|---|
| Public Website | Public portfolio experience | Client |
| Admin Dashboard | Portfolio management | Client |
| iOS Application | Mobile portfolio experience | Future |
| Portfolio API | Application/business logic | Core |
| Relational Database | Structured persistence | Core |
| File Storage | Binary/file storage | Supporting |
| External Integrations | External platform interaction | Future/Optional |

---

# 11. Dependency Rules

The following dependency rules are established at the container level.

### Rule 1 — Clients do not access persistence directly

```text
Client
  ✗
  │
  └──────▶ Database
```

is prohibited.

Instead:

```text
Client
  │
  ▼
Portfolio API
  │
  ▼
Database
```

---

### Rule 2 — External systems are isolated

Business logic should not depend directly on GitHub, LinkedIn, or Medium APIs.

Instead:

```text
Domain/Application
       │
       ▼
Integration Boundary
       │
       ▼
External Platform
```

---

### Rule 3 — One backend API

The public website, administration dashboard, and iOS application use the same backend API.

```text
                  Portfolio API
                 /      |       \
                /       |        \
             Web      Admin      iOS
```

This avoids duplicating business logic across clients.

---

### Rule 4 — Single deployment initially

The Portfolio API is initially deployed as one application.

```text
┌──────────────────────────────┐
│       Portfolio API          │
│                              │
│ Profile                      │
│ Experience                   │
│ Skills                       │
│ Projects                     │
│ Articles                     │
│ Administration               │
│ Authentication               │
└──────────────────────────────┘
```

This is consistent with the selected Modular Monolith architectural style.

---

# 12. Why These Containers?

The containers are intentionally kept at a relatively coarse level.

The goal is to establish the system's major responsibilities without prematurely designing implementation details.

The architecture therefore avoids creating containers such as:

```text
Profile Service
Project Service
Article Service
Authentication Service
Notification Service
Analytics Service
Search Service
```

because these would imply independently deployable distributed services.

The current requirements do not justify that level of decomposition.

The project's MVP explicitly excludes microservices, Kubernetes, and event-driven distributed architecture without justification.

---

# 13. Future Evolution

The container architecture intentionally leaves room for future evolution.

For example:

```text
Current

Portfolio API
     │
     ├── Database
     └── File Storage
```

could evolve into:

```text
Future

                Portfolio API
                     │
        ┌────────────┼─────────────┐
        ▼            ▼             ▼
      Cache       Database     Message Broker
                                   │
                         ┌─────────┼─────────┐
                         ▼         ▼         ▼
                     Analytics Notifications Search
```

Only actual requirements should trigger these additions.

Potential future drivers include:

- High traffic
- Advanced search
- Analytics
- Notifications
- Background processing
- External synchronization
- Independent scaling
- Complex read/write workloads

The roadmap already identifies these as potential future capabilities rather than MVP requirements.

---

# 14. Container-Level Architecture Decision

The current container architecture is:

```text
                    ┌──────────────────┐
                    │  Public Website  │
                    └────────┬─────────┘
                             │
                    ┌────────▼─────────┐
                    │                  │
                    │   Portfolio API  │
                    │  ASP.NET Core    │
                    │                  │
                    └───┬────────┬─────┘
                        │        │
             ┌──────────┘        └──────────┐
             ▼                              ▼
      ┌──────────────┐              ┌──────────────┐
      │  Relational  │              │ File Storage │
      │   Database   │              │              │
      └──────────────┘              └──────────────┘

      ┌──────────────────┐
      │ Admin Dashboard  │──────▶ Portfolio API
      └──────────────────┘

      ┌──────────────────┐
      │  iOS Application │──────▶ Portfolio API
      └──────────────────┘

      Portfolio API ──────▶ External Integrations
                              │
                         ┌────┼────┐
                         ▼    ▼    ▼
                       GitHub LinkedIn Medium
```

---

## 15. Next Step

The next architectural level is:

> **C4 Level 3 — Component Diagram**

The Component Diagram will zoom into the **Portfolio API**.

It will answer:

> **What are the major components inside the Portfolio API, and how do they collaborate?**

That is where we can begin defining:

```text
Portfolio API
      │
      ├── API / Presentation
      │
      ├── Application
      │
      ├── Domain
      │
      ├── Infrastructure
      │
      └── Cross-Cutting Concerns
```

and then map the actual portfolio domains:

```text
Profile
Experience
Skills
Projects
Articles
Administration
```

to the **Modular Monolith boundaries** selected in ADR-001.