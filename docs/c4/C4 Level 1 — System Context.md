# C4 Level 1 — System Context

**System:** Architect Portfolio Platform  
**Diagram Level:** C4 Level 1 — System Context  
**Status:** Draft  
**Date:** 2026-08-30

---

## 1. Purpose

The System Context diagram provides the highest-level view of the Architect Portfolio Platform.

It identifies:

- The system being designed
- The people who interact with it
- External systems it depends on
- The major relationships between these entities

Internal implementation details are intentionally excluded.

---

# 2. System Context

```text
                         ┌──────────────────────┐
                         │      Portfolio       │
                         │        Owner         │
                         │    Administrator     │
                         └──────────┬───────────┘
                                    │
                         Manage portfolio content
                                    │
                                    ▼
┌──────────────┐          ┌──────────────────────────┐
│              │          │                          │
│   Visitor    │─────────▶│   Architect Portfolio   │
│              │  Browse  │         Platform        │
└──────────────┘          │                          │
                          └────────────┬─────────────┘
                                       │
                          ┌────────────┼────────────┐
                          │            │            │
                          ▼            ▼            ▼
                   ┌────────────┐ ┌────────────┐ ┌────────────┐
                   │   Public   │ │    Admin   │ │    iOS     │
                   │  Website   │ │  Dashboard │ │ Application│
                   └────────────┘ └────────────┘ └────────────┘


                 External Professional Platforms

        ┌─────────────┐   ┌─────────────┐   ┌─────────────┐
        │   GitHub    │   │  LinkedIn   │   │   Medium    │
        └──────┬──────┘   └──────┬──────┘   └──────┬──────┘
               │                 │                 │
               └─────────────────┼─────────────────┘
                                 │
                         External references /
                         future integrations
                                 │
                                 ▼
                    ┌──────────────────────────┐
                    │   Architect Portfolio    │
                    │         Platform         │
                    └──────────────────────────┘
```

---

# 3. People

## 3.1 Visitor

**Description:**  
An unauthenticated public user who visits the portfolio.

**Interactions:**

- View professional profile
- View experience
- View skills
- View projects
- View articles
- Access CV
- Access external professional profiles
- Access social links

The Visitor represents the primary public-facing user of the platform.

---

## 3.2 Portfolio Owner / Administrator

**Description:**  
The portfolio owner who manages the professional information presented by the platform.

**Interactions:**

- Authenticate
- Manage profile
- Manage experience
- Manage skills
- Manage projects
- Manage articles
- Manage social links
- Manage CV
- Publish/update portfolio content

The current MVP assumes a single portfolio owner and primary administrator.

---

# 4. External Systems

## 4.1 GitHub

**Description:**  
External professional platform containing repositories and development-related information.

**Relationship:**  

The portfolio may provide links or references to GitHub resources.

GitHub remains the authoritative platform for its own content.

---

## 4.2 LinkedIn

**Description:**  
External professional networking platform.

**Relationship:**  

The portfolio provides a link/reference to the portfolio owner's LinkedIn presence.

LinkedIn remains authoritative for its own professional-networking content.

---

## 4.3 Medium

**Description:**  
External publishing platform containing articles written by the portfolio owner.

**Relationship:**  

The portfolio may store references to external Medium articles rather than duplicating their complete content.

---

# 5. Client Applications

The Portfolio Platform is consumed through multiple client applications.

## Public Website

Provides the public portfolio experience for visitors.

Primary capabilities:

- Profile
- Experience
- Skills
- Projects
- Articles
- CV
- Social links
- Contact

---

## Administration Dashboard

Provides the authenticated management experience for the portfolio owner.

Primary capabilities:

- Authentication
- Authorization
- Portfolio management
- Content CRUD
- Validation
- Publishing

---

## iOS Application

Provides a mobile representation of the portfolio.

The iOS application consumes the same backend API as the website and administration client.

The iOS application is currently classified as a **Should Have** capability rather than an MVP Must Have.

---

# 6. System Boundary

The **Architect Portfolio Platform** owns:

```text
Portfolio data
Professional profile
Experience
Skills
Projects
Article references
Social links
CV metadata
Administration
Authentication/authorization
Business rules
REST API
```

The platform does **not** own the complete content of external platforms.

For example:

```text
Portfolio Platform
        │
        │ reference
        ▼
     Medium
        │
        └── owns article content
```

This separation prevents the portfolio platform from unnecessarily duplicating external systems.

---

# 7. Key Relationships

| From | To | Relationship |
|---|---|---|
| Visitor | Portfolio Platform | Browses public portfolio |
| Portfolio Owner | Portfolio Platform | Authenticates and manages content |
| Public Website | Portfolio Platform | Retrieves public portfolio data |
| Admin Dashboard | Portfolio Platform | Manages portfolio content |
| iOS Application | Portfolio Platform | Retrieves portfolio data |
| Portfolio Platform | GitHub | Provides references/links |
| Portfolio Platform | LinkedIn | Provides references/links |
| Portfolio Platform | Medium | Provides references to articles |

---

# 8. Important Architectural Boundaries

At C4 Level 1, the following boundaries are established:

### Internal

```text
Portfolio Platform
```

The platform owns the core portfolio domain and its business rules.

### External

```text
Visitors
Portfolio Owner
GitHub
LinkedIn
Medium
```

External systems and users interact through defined interfaces.

### Client Boundary

```text
Public Website
Admin Dashboard
iOS Application
```

These are clients of the platform rather than independent sources of portfolio truth.

---

# 9. Architectural Implications

The System Context establishes several important architectural consequences.

### 9.1 Multiple clients require a centralized API

Because the website, dashboard, and iOS application consume the same backend, the backend API becomes the central application interface.

```text
                    Portfolio Platform
                           │
                       REST API
                           │
              ┌────────────┼────────────┐
              ▼            ▼            ▼
            Web          Admin         iOS
```

This supports the requirement that multiple clients consume the same backend API.

### 9.2 External platforms should remain isolated

GitHub, LinkedIn, and Medium are external dependencies.

Their integration should therefore be isolated behind appropriate boundaries rather than spreading external API knowledge throughout the system.

### 9.3 Authentication is primarily an administrative concern

Public visitors do not require authentication for normal portfolio browsing.

Authentication and authorization primarily protect administrative capabilities.

### 9.4 The context does not require distributed architecture

Nothing at the system-context level currently requires:

- Microservices
- Kubernetes
- Message brokers
- Service mesh
- Distributed databases

Those technologies may become appropriate if future requirements introduce new architectural drivers.

---

# 10. C4 Level 1 Scope Rules

The following are intentionally **not represented** at this level:

- Controllers
- Application services
- Domain services
- Repositories
- EF Core
- Database tables
- Redis
- Message brokers
- Internal modules
- Classes
- Framework-specific implementation details

Those belong to lower architectural levels.

---

# 11. Next C4 Level

The next step is:

> **C4 Level 2 — Container Diagram**

The Container Diagram will zoom into the **Architect Portfolio Platform** and show its major deployable/runtime containers.

The next diagram will answer:

> **What are the major building blocks inside the Portfolio Platform, and how do they communicate?**

Expected initial containers:

```text
                   Architect Portfolio Platform
                              │
             ┌────────────────┼────────────────┐
             │                │                │
             ▼                ▼                ▼
        REST API         Authentication      Database
             │
             │
       ┌─────┴──────────────────────────┐
       │                                │
       ▼                                ▼
 Portfolio Management              External Integrations
       │                                │
       └──────────────┬─────────────────┘
                      ▼
                File Storage
```

The exact containers will be determined during the next architectural analysis rather than assumed prematurely.