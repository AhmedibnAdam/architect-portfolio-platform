# Domain Model — Architect Portfolio Platform

**System:** Architect Portfolio Platform  
**Document:** Domain Model  
**Status:** Draft  
**Date:** 2026-08-30  

---

## 1. Purpose
This document defines the conceptual domain model of the Architect Portfolio Platform.
It identifies:

- Core domain concepts
- Entities
- Value Objects
- Aggregates
- Business invariants
- Relationships
- Domain boundaries
- Cross-module dependencies
- Potential domain events

The purpose is to establish a stable business model before defining detailed implementation structures.  
This document does **not** prescribe specific frameworks or persistence technologies.

---

## 2. Domain Overview
The Architect Portfolio Platform represents and manages the professional identity and public portfolio of an architect/developer.

The primary domain areas are:

```
                    Portfolio Platform
                           │
       ┌───────────────────┼───────────────────┐
       │                   │                   │
       ▼                   ▼                   ▼
   Professional        Portfolio           Content
     Identity           Assets             Publishing
       │                   │                   │
       ├── Profile         ├── Projects        └── Articles
       ├── Experience      ├── Skills
       └── Education*      └── CV
```

*\*Education is currently considered a potential future extension rather than a confirmed MVP domain module.*

---

## 3. Core Domain Concepts
The initial domain contains the following concepts:

| Concept | Type | Module | Purpose |
| :--- | :--- | :--- | :--- |
| **Profile** | Entity / Aggregate Root | Profile | Represents professional identity |
| **Experience** | Entity | Experience | Represents professional work history |
| **Skill** | Entity | Skills | Represents a professional capability |
| **Skill Category** | Entity | Skills | Groups related skills |
| **Project** | Entity / Aggregate Root | Projects | Represents a portfolio project |
| **Technology** | Value Object / Reference | Projects | Represents technology used by a project |
| **Article** | Entity / Aggregate Root | Articles | Represents published content/reference |
| **Social Link** | Value Object | Profile | Represents an external professional link |
| **CV** | Entity / Aggregate Root | Profile | Represents the active CV/document |
| **Administrator** | Entity | Administration | Represents a user authorized to manage the portfolio |

*The exact entity/value-object classification may evolve during implementation if additional business rules are discovered.*

---

## 4. Profile Module

### 4.1 Purpose
The Profile module represents the professional identity of the portfolio owner.  
It contains information that describes who the portfolio owner is professionally.

### 4.2 Profile
**Type:** Aggregate Root  

**Conceptual model:**
```
Profile
 ├── Name
 ├── ProfessionalTitle
 ├── Summary
 ├── Location
 ├── Email
 ├── Phone
 ├── SocialLinks
 └── CV
```

#### Responsibilities
The Profile aggregate is responsible for:
- Maintaining professional identity
- Maintaining public profile information
- Managing associated social links
- Identifying the active CV

#### Invariants
Examples:
- A profile must have a valid professional name.
- Public contact information must satisfy validation rules.
- Social links must contain valid URLs.
- Only one CV should be considered the active/default CV at a time.

---

## 5. Experience Module

### 5.1 Purpose
Represents the portfolio owner's professional career history.

### 5.2 Experience
**Type:** Entity  

**Conceptual model:**
```
Experience
 ├── Company
 ├── Position
 ├── Description
 ├── Responsibilities
 ├── Achievements
 ├── StartDate
 ├── EndDate
 └── IsCurrent
```

#### Invariants
- `StartDate` must precede `EndDate` when an end date exists.
- A current experience should not have a completed end date.
- Required professional information must not be empty.

---

## 6. Skills Module

### 6.1 Purpose
Represents technical and professional capabilities.

### 6.2 Skill Category
Examples:
- Programming Languages
- Backend
- Architecture
- Cloud
- DevOps
- Databases
- Mobile
- Testing

A category groups related skills.

### 6.3 Skill
**Conceptual model:**
```
Skill
 ├── Name
 ├── Category
 ├── Proficiency
 └── Description
```

#### Invariants
- Skill name must be unique within its applicable scope.
- A skill must belong to a valid category.
- Proficiency must use an accepted representation if proficiency is enabled.

---

## 7. Projects Module

### 7.1 Purpose
The Projects module represents portfolio projects that demonstrate professional experience and technical capabilities.  
This is one of the most important portfolio domains.

### 7.2 Project
**Type:** Aggregate Root  

**Conceptual model:**
```
Project
 ├── Title
 ├── Description
 ├── Technologies
 ├── RepositoryUrl
 ├── DemoUrl
 ├── Image
 ├── Status
 ├── StartDate
 └── EndDate
```

#### Responsibilities
The Project aggregate manages:
- Project identity
- Project description
- Technology associations
- External references
- Visibility/status
- Project lifecycle information

#### Invariants
Examples:
- A project must have a title.
- A project must have a meaningful description.
- URLs must be valid when provided.
- Project dates must be logically ordered.
- A project cannot be published unless required publication information is valid.

---

## 8. Technology
Technology requires special consideration.  
For the initial system, a technology such as:
- C#
- ASP.NET Core
- Swift
- PostgreSQL
- Docker
- AWS

does not necessarily require its own independent lifecycle.  
Therefore the initial model treats technology as a **Value Object or lightweight reference** rather than immediately creating a complex Technology aggregate.

**Conceptually:**
```
Project
   │
   ├── Technology("C#")
   ├── Technology("ASP.NET Core")
   └── Technology("PostgreSQL")
```

If future requirements require technology management, searching, categorization, or reuse across multiple domains, this decision can be revisited.

---

## 9. Articles Module

### 9.1 Purpose
Represents articles presented by the portfolio.  
Articles may exist on external publishing platforms such as Medium.  
The portfolio platform should therefore distinguish between:
- **Article Metadata**  
and:
- **External Article Content**

### 9.2 Article
**Type:** Aggregate Root  

**Conceptual model:**
```
Article
 ├── Title
 ├── Description
 ├── PublishedDate
 ├── Platform
 ├── ExternalUrl
 ├── Tags
 └── Status
```

#### Responsibilities
- Maintain article metadata
- Control publication state
- Maintain external article reference
- Provide article information to portfolio clients

#### Invariants
- Published articles must have a valid external URL when the article is externally hosted.
- Publication dates must be valid.
- An article must have a title.
- The external platform remains authoritative for externally hosted article content.

---

## 10. Social Links
Social links represent external professional identities.  
Examples:
- GitHub
- LinkedIn
- Medium
- Personal Website
- Other Professional Platform

**Conceptually:**
```
SocialLink
 ├── Platform
 └── Url
```

This is initially modeled as a **Value Object** because the link itself does not require an independent lifecycle.

#### Invariants
- URL must be valid.
- Platform must be recognized or explicitly supported.
- Duplicate links for the same platform should be prevented where appropriate.

---

## 11. CV
The CV represents the portfolio owner's current professional résumé/document.

**Conceptually:**
```
CV
 ├── FileName
 ├── StorageReference
 ├── Version
 ├── UploadedAt
 └── IsActive
```

The actual binary file should be treated as a storage concern.  
The domain should represent the CV's business identity and state rather than the physical storage mechanism.

```
Domain
  │
  ▼
 CV
  │
  └── StorageReference
          │
          ▼
     File Storage
```

---

## 12. Administration Module

### 12.1 Administrator
The Administrator represents a user who is authorized to manage portfolio content.

**Conceptually:**
```
Administrator
 ├── Identity
 ├── Status
 └── Permissions
```

The domain model should distinguish **Authentication** from **Authorization**.  
Authentication technology remains an infrastructure/security decision.  
Authorization policies determine what administrative operations an administrator may perform.

---

## 13. Aggregate Boundaries
The initial aggregate boundaries are:

```
Profile
 └── SocialLinks
 └── CV

Experience
 └── Experience data

Skills
 └── Skill Categories
 └── Skills

Project
 └── Technologies

Article
 └── Article metadata

Administration
 └── Administrator
```

The key principle is:  
> **An aggregate should protect business invariants, not simply mirror database tables.**  

We should therefore avoid creating an aggregate for every database entity.

---

## 14. Aggregate Relationships
Aggregates should reference other aggregates by identity where possible.  
For example:

```
Project
   │
   └── SkillId / Technology reference
```
rather than:
```
Project
   │
   └── Entire Skill aggregate
          │
          └── Other objects
```
This reduces coupling between aggregates.

---

## 15. Domain Relationships
A simplified conceptual relationship model is:

```
                    ┌─────────────┐
                    │   Profile   │
                    └──────┬──────┘
                           │
                ┌──────────┼──────────┐
                │          │          │
                ▼          ▼          ▼
          Social Links    CV       Experience


                    ┌─────────────┐
                    │   Project   │
                    └──────┬──────┘
                           │
                           ▼
                      Technologies


                    ┌─────────────┐
                    │    Skill    │
                    └──────┬──────┘
                           │
                           ▼
                    Skill Category


                    ┌─────────────┐
                    │   Article   │
                    └──────┬──────┘
                           │
                           ▼
                    External Platform
```

These relationships represent conceptual associations and do not necessarily imply direct object references in code.

---

## 16. Module Dependency Rules
The modules should remain loosely coupled.

**Preferred:**
```
Profile
   │
   └── publishes information through contracts

Projects
   │
   └── references required information through IDs/contracts
```

**Avoid:**
```
Projects
   │
   └──────▶ Profile internal implementation
```
or:
```
Articles
   │
   └──────▶ Projects database tables
```

Each module should own its internal business rules.

---

## 17. Domain Invariants
The initial domain invariants include:

- **Profile:** Profile must have valid professional identity information.
- **Experience:** `StartDate <= EndDate` when an end date exists.
- **Project:** Title is required; Description is required; External URLs must be valid when provided.
- **Article:** Published article requires valid publication information.
- **Social Link:** URL must be valid.
- **CV:** Only one CV may be active at a time.
- **Administration:** Only authorized administrators may modify portfolio content.

These invariants should be enforced as close to the domain/application boundary as practical rather than relying exclusively on client-side validation.

---

## 18. Domain Events — Candidates
Domain events are **not required** for the initial implementation.  
However, several business events could become useful in the future.

Potential candidates:
- `ProjectPublished`
- `ProjectUpdated`
- `ArticlePublished`
- `ProfileUpdated`
- `CVUploaded`
- `ExperienceAdded`

For example:
```
ProjectPublished
       │
       ├── Analytics
       ├── Search Index
       ├── Notification
       └── External Synchronization
```

These are currently **candidates only**.  
No message broker or event-driven architecture is introduced as part of this domain model.

---

## 19. Bounded Context Analysis
At the current scale, the modules do not necessarily justify separate bounded contexts.  
A preliminary grouping is:

```
┌─────────────────────────────────────────┐
│       Professional Identity             │
│                                         │
│ Profile                                 │
│ Experience                              │
│ Skills                                  │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│       Portfolio Content                 │
│                                         │
│ Projects                                │
│ Articles                                │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│       Administration                    │
│                                         │
│ Administrator                           │
│ Authorization                           │
└─────────────────────────────────────────┘
```

These boundaries are conceptual rather than independently deployed services.  
The current Modular Monolith allows these boundaries to exist inside a single application.

---

## 20. Core Domain vs Supporting Domain
The platform is primarily a professional portfolio system rather than a complex transactional business system.  
Therefore the domain can be categorized as:

### Core Domain
- Portfolio Presentation
- Professional Identity
- Projects
- Experience  
*(These directly support the primary purpose of the platform.)*

### Supporting Domain
- Skills
- Articles
- Social Links
- CV Management  
*(These enhance the portfolio experience but are not necessarily the central business differentiator.)*

### Generic Domain
- Authentication
- Authorization
- File Storage
- Logging  
*(These should generally use established technical solutions rather than custom domain complexity.)*

---

## 21. Domain Model Diagram

```
                         PORTFOLIO DOMAIN
                               │
             ┌─────────────────┼─────────────────┐
             │                 │                 │
             ▼                 ▼                 ▼
        Professional       Portfolio           Content
         Identity           Assets           Publishing
             │                 │                 │
      ┌──────┼──────┐      ┌───┴────┐           │
      ▼      ▼      ▼      ▼        ▼           ▼
   Profile Experience Skills   Project        Article
      │                       │
      ├── SocialLink          └── Technology
      │
      └── CV


                     ADMINISTRATION
                           │
                           ▼
                     Administrator
```

---

## 22. Domain Principles
The following principles govern the domain model:

- **Principle 1 — Business concepts over database tables:** The domain model represents business concepts rather than database schema.
- **Principle 2 — Aggregates protect invariants:** Aggregates exist where business consistency boundaries are required.
- **Principle 3 — Minimize coupling:** Modules should interact through explicit contracts.
- **Principle 4 — External systems remain external:** GitHub, LinkedIn, Medium, and storage providers should not become part of the core domain model.
- **Principle 5 — Avoid premature complexity:** DDD patterns should be introduced where they solve an actual domain problem.
- **Principle 6 — Domain independence:** The domain should not depend on ASP.NET Core, EF Core, SQL, HTTP, Cloud providers, External APIs, or File-storage implementations.

---

## 23. Initial Domain Structure
The conceptual implementation structure is:

```
Domain
│
├── Profile
│   ├── Profile
│   ├── SocialLink
│   └── CV
│
├── Experience
│   └── Experience
│
├── Skills
│   ├── Skill
│   └── SkillCategory
│
├── Projects
│   ├── Project
│   └── Technology
│
├── Articles
│   └── Article
│
└── Administration
    └── Administrator
```

This structure represents the current domain understanding and should be validated during implementation.

---

## 24. Future Domain Evolution
The domain may evolve as new requirements appear.  
Potential future concepts include:
- Education
- Certifications
- Testimonials
- Recommendations
- Achievements
- Analytics
- Notifications
- Search
- External Content Synchronization

These should not be added to the domain merely because they are technically possible.  
Each addition should be driven by an actual requirement.

---

## 25. Architectural Outcome
The domain model establishes the following initial business boundaries:
- Profile
- Experience
- Skills
- Projects
- Articles
- Administration

With primary aggregate roots:
- `Profile`
- `Project`
- `Article`

And supporting entities/value objects such as:
- `Experience`
- `Skill`
- `SkillCategory`
- `Technology`
- `SocialLink`
- `CV`
- `Administrator`

The model intentionally remains simple because the current business domain does not require a highly complex domain model.

---

## 26. Next Step
The next step is to translate this conceptual domain model into the **actual .NET solution architecture**.

The next design activity should define:
```
Solution
│
├── API
├── Application
├── Domain
└── Infrastructure
```
and then determine how the business modules are represented inside those boundaries.

The next deliverable should therefore be:  
`docs/architecture/solution-structure.md`

This will define the actual project/folder structure, dependency rules, project references, and module organization before implementation begins.
