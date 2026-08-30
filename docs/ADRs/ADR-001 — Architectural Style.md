# ADR-001 — Architectural Style

**Status:** Accepted  
**Date:** 2026-08-30  
**Decision:** Modular Monolith + REST API  
**Scope:** Backend Architecture

---

## 1. Context

The Portfolio Platform requires a backend that serves multiple clients, including:

- Public Website
- Administration Dashboard
- iOS Application

The backend must provide centralized portfolio data through RESTful APIs while supporting secure administration, business rules, persistence, testing, observability, and future evolution.

The current system has:

- A single portfolio owner
- Relatively small initial data volume
- Initially low traffic
- A single developer
- Reasonable infrastructure-cost constraints
- No current requirement for independent service deployment
- No current requirement for distributed processing

The project also explicitly aims to avoid introducing architectural complexity simply to demonstrate technologies.

The architecture therefore needs to balance:

- Maintainability
- Testability
- Security
- Performance
- Reliability
- Availability
- Scalability
- Observability
- Deployability
- Extensibility
- Infrastructure complexity and cost

---

## 2. Decision Drivers

The primary drivers for this decision are:

1. **Maintainability** — The system should remain easy to understand and modify.
2. **Testability** — Business logic and application behavior should be independently testable.
3. **Security** — Administrative functionality must have clear security boundaries.
4. **Deployability** — Deployment should remain simple for the initial project stage.
5. **Cost Efficiency** — Infrastructure should be appropriate for the expected traffic and data volume.
6. **Extensibility** — The architecture should support future capabilities without requiring a complete rewrite.
7. **Scalability** — The architecture should allow future growth without prematurely introducing distributed-system complexity.
8. **Operational Simplicity** — The initial system should not require unnecessary infrastructure such as service meshes, message brokers, or Kubernetes.

These drivers are derived from the project's requirements, constraints, and quality attributes.

---

## 3. Options Considered

### Option A — Traditional Layered Monolith

A single deployable application organized primarily into technical layers such as:

```text
API
 ↓
Business Logic
 ↓
Data Access
 ↓
Database
```

**Advantages**

- Simple deployment
- Low infrastructure complexity
- Easy to develop initially
- Low operational overhead

**Disadvantages**

- Business boundaries can become unclear as the system grows
- Technical layers alone do not strongly enforce module boundaries
- Changes in one area can easily affect unrelated areas
- Long-term maintainability can deteriorate

---

### Option B — Modular Monolith

A single deployable application divided into explicit business/functional modules.

Conceptually:

```text
                    Portfolio API
                         │
        ┌────────────────┼────────────────┐
        │                │                │
     Profile          Projects         Articles
        │                │                │
        └────────────────┼────────────────┘
                         │
                    Shared Core
                         │
                     Database
```

Each module owns its application logic and exposes controlled interfaces to other modules.

**Advantages**

- Single deployment unit
- Low operational complexity
- Clearer business boundaries
- Easier testing
- Easier local development
- Can scale vertically and horizontally
- Provides a potential path toward future service extraction

**Disadvantages**

- Requires discipline to maintain module boundaries
- Still shares the same deployment unit
- Modules initially share infrastructure resources
- Does not provide independent service scaling

---

### Option C — Microservices

The system would be divided into independently deployable services.

For example:

```text
                API Gateway
                     │
       ┌─────────────┼─────────────┐
       │             │             │
   Profile       Projects       Articles
   Service       Service        Service
       │             │             │
      DB            DB            DB
```

**Advantages**

- Independent deployment
- Independent scaling
- Strong service boundaries
- Fault isolation
- Suitable for large and distributed systems

**Disadvantages**

- Higher infrastructure complexity
- Network communication between services
- Distributed failure modes
- More complicated testing
- More complicated deployment
- Increased observability requirements
- Increased operational cost
- Unnecessary complexity for the current system

The current requirements do not justify this level of distribution. The project explicitly excludes microservices without justification from the MVP scope.

---

## 4. Decision

We will use a:

> **Modular Monolith + REST API**

for the initial backend architecture.

The backend will remain a single deployable application while being internally organized around clear functional and domain boundaries.

The initial structure will follow the general direction:

```text
                    REST API
                       │
              ┌────────┴────────┐
              │                 │
        Application         Authentication
              │
       ┌──────┼────────┐
       │      │        │
    Profile Projects Articles
       │      │        │
       └──────┼────────┘
              │
           Domain
              │
       Infrastructure
              │
           Database
```

The exact internal boundaries will be refined during technical design and domain analysis.

---

## 5. Rationale

The Modular Monolith provides the best balance between the current architecture drivers and the project's constraints.

### Why not a traditional layered monolith?

A traditional layered monolith provides excellent initial simplicity, but it does not provide sufficiently strong business boundaries for a project intended to evolve and demonstrate architectural practices.

The Modular Monolith provides similar deployment simplicity while establishing clearer boundaries between functional areas.

### Why not microservices?

The current system does not have sufficient drivers for distributed architecture.

There is:

- No high traffic requirement
- No requirement for independent scaling
- No requirement for independent deployments
- No large development organization
- No requirement for service-level fault isolation
- No requirement for geographically distributed processing

Introducing microservices now would primarily increase complexity rather than solve an existing problem.

The project requirements explicitly identify advanced distributed architecture as unnecessary unless future requirements justify it.

### Why Modular Monolith?

It provides:

- Low deployment complexity
- Clear architectural boundaries
- Good maintainability
- Strong testability
- Reasonable scalability
- Lower infrastructure cost
- A practical foundation for future architectural evolution

This is particularly appropriate because the project is initially developed by a single developer and explicitly aims to avoid unnecessary operational complexity.

---

## 6. Consequences

### Positive Consequences

- One deployment unit
- Simple local development
- Simple CI/CD pipeline
- Lower infrastructure cost
- Clearer business boundaries
- Easier integration testing
- Easier debugging
- Easier operational management
- Architecture can evolve incrementally

The architecture also supports the project's goal of allowing future evolution without requiring a complete rewrite.

### Negative Consequences

- Modules share the same process
- A failure in the application can potentially affect multiple modules
- Independent scaling of modules is not initially possible
- Module boundaries must be actively protected
- Future service extraction may require additional work

These are accepted trade-offs because they are appropriate for the current scale and requirements.

---

## 7. Evolution Strategy

The Modular Monolith is not considered the final architecture for all future system states.

If requirements change, the architecture will be reassessed.

Potential future drivers could include:

```text
Higher Traffic
      ↓
Scalability Requirements
      ↓
Independent Scaling
      ↓
Service Extraction
```

or:

```text
Analytics / Notifications
          ↓
Asynchronous Processing
          ↓
Message Broker
          ↓
Event-Driven Components
```

or:

```text
Complex Read Workloads
          ↓
CQRS Requirement
          ↓
Separate Read/Write Models
```

Any future architectural evolution must be supported by:

1. A concrete requirement
2. A clearly identified problem
3. Analysis of alternatives
4. An architecture decision
5. An ADR
6. Implementation
7. Measurement/evaluation

This follows the project's architecture-evolution principle.

---

## 8. Related Decisions

This ADR establishes the **architectural style** only.

The following decisions remain separate and will be addressed through subsequent ADRs or architecture documents:

- Clean Architecture boundaries
- Domain-Driven Design
- Database technology
- Authentication mechanism
- Authorization model
- Caching strategy
- API versioning
- File storage
- Cloud platform
- Deployment architecture
- CI/CD architecture
- Observability architecture
- CQRS
- Event-driven architecture

These decisions should not be assumed simply because the Modular Monolith has been selected.

---

## 9. Decision Summary

| Decision | Result |
|---|---|
| Architectural Style | **Modular Monolith** |
| API Style | **REST** |
| Deployment Model | **Single Deployable Application** |
| Service Distribution | **Not initially distributed** |
| Database | To be decided |
| CQRS | Not initially required |
| Event-Driven Architecture | Not initially required |
| Microservices | Not initially required |
| Future Evolution | Requirement-driven |

---

## 10. Final Decision

> **Adopt a Modular Monolith with a REST API as the initial backend architectural style.**

The decision prioritizes maintainability, testability, operational simplicity, cost efficiency, and future evolution while avoiding distributed-system complexity that is not currently justified by the requirements.

The architecture will be revisited when new requirements, measurable system constraints, or business drivers make the current architectural style insufficient.

**Status: Accepted**