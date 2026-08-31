# Persistence Model — Architect Portfolio Platform

**System:** Architect Portfolio Platform  
**Document:** Persistence Model  
**Status:** Draft  
**Date:** 2026-08-31  
**Database:** Relational Database  
**ORM:** Entity Framework Core  

---

## 1. Purpose

This document defines the persistence architecture for the Architect Portfolio Platform. It describes:

* Persistent aggregates and entities
* Database tables
* Relationships
* Primary keys
* Foreign keys
* Unique constraints
* Indexes
* Audit information
* Concurrency strategy
* Soft-delete strategy
* Database boundaries
* Entity Framework Core mapping responsibilities

The persistence model supports the Domain Model without exposing persistence concerns to the Domain layer.

---

## 2. Persistence Architecture

The backend uses a relational database accessed through Entity Framework Core.

The dependency direction remains:

```
API
 │
 ▼
Application
 │
 ▼
Domain
 │
 ▲
 │
Infrastructure
 │
 ▼
Database
```

The Domain layer does not depend on Entity Framework Core.

```
Domain
  │
  ├── Entities
  ├── Aggregates
  ├── Value Objects
  ├── Domain Events
  └── Business Rules

Infrastructure
  │
  ├── EF Core
  ├── DbContext
  ├── Configurations
  ├── Repositories
  └── Database
```

---

## 3. Persistence Strategy

The initial system uses:
* One relational database for the Modular Monolith.
* All modules share the same database infrastructure while maintaining logical ownership of their data.

```
                    Database
                       │
        ┌──────────────┼──────────────┐
        │                             │
   Portfolio Data             Administration Data
        │                             │
   ┌────┴─────┐                 ┌─────┴─────┐
   │           │                 │           │
Profile    Projects           Users        Roles
Experience Articles           Permissions
Skills      SocialProfiles
```

This approach provides:
* Transactional consistency
* Simpler deployment
* Lower operational complexity
* Easier reporting
* Easier development for the initial platform

The architecture should still preserve module ownership so that future extraction remains possible if justified.

---

## 4. Aggregate Persistence

The primary aggregates identified in the DDD model are:

**Portfolio Context**
* ArchitectProfile
* Project
* Article

**Administration Context**
* User
* Role

Additional entities such as Experience, Skill, and SocialProfile are persisted according to their relationship with their owning aggregate/module.

---

## 5. Portfolio Tables

### 5.1 ArchitectProfile

**Table:** `ArchitectProfiles`

| Column | Type | Constraints |
| :--- | :--- | :--- |
| `Id` | UUID | PK |
| `Name` | varchar | NOT NULL |
| `Headline` | varchar | NOT NULL |
| `Bio` | text | NOT NULL |
| `Email` | varchar | NOT NULL |
| `Location` | varchar | NULL |
| `Slug` | varchar | UNIQUE |
| `CreatedAt` | datetime | NOT NULL |
| `UpdatedAt` | datetime | NOT NULL |
| `RowVersion` | rowversion / byte[] | Concurrency |

**Indexes:**
* `UNIQUE(Slug)`

The profile is expected to have a single active portfolio identity for the initial system.

---

## 6. Experience

**Table:** `Experiences`

| Column | Type | Constraints |
| :--- | :--- | :--- |
| `Id` | UUID | PK |
| `ProfileId` | UUID | FK |
| `Company` | varchar | NOT NULL |
| `Position` | varchar | NOT NULL |
| `Description` | text | NOT NULL |
| `StartDate` | date | NOT NULL |
| `EndDate` | date | NULL |
| `IsCurrent` | boolean | NOT NULL |
| `DisplayOrder` | int | NOT NULL |
| `CreatedAt` | datetime | NOT NULL |
| `UpdatedAt` | datetime | NOT NULL |

**Relationship:**
```
ArchitectProfile
       │
       │ 1
       │
       ▼
   Experiences
       │
       │ *
```

**Foreign key:**
* `Experiences.ProfileId` → `ArchitectProfiles.Id`

---

## 7. Skills

**Tables:**
* `Skills`
* `SkillCategories`

### Skills

| Column | Type | Constraints |
| :--- | :--- | :--- |
| `Id` | UUID | PK |
| `ProfileId` | UUID | FK |
| `Name` | varchar | NOT NULL |
| `ProficiencyLevel` | varchar | NOT NULL |
| `DisplayOrder` | int | NOT NULL |
| `CreatedAt` | datetime | NOT NULL |
| `UpdatedAt` | datetime | NOT NULL |

**Relationship:**
```
ArchitectProfile
       │
       ▼
     Skills
```
A skill belongs to the portfolio profile.

---

## 8. Projects

**Table:** `Projects`

| Column | Type | Constraints |
| :--- | :--- | :--- |
| `Id` | UUID | PK |
| `Title` | varchar | NOT NULL |
| `Slug` | varchar | NOT NULL |
| `Summary` | varchar | NOT NULL |
| `Description` | text | NOT NULL |
| `StartDate` | date | NOT NULL |
| `EndDate` | date | NULL |
| `IsFeatured` | boolean | NOT NULL |
| `Status` | varchar | NOT NULL |
| `PublishedAt` | datetime | NULL |
| `CreatedAt` | datetime | NOT NULL |
| `UpdatedAt` | datetime | NOT NULL |
| `RowVersion` | rowversion / byte[] | Concurrency |

**Indexes:**
* `UNIQUE(Slug)`
* `INDEX(Status)`
* `INDEX(IsFeatured)`
* `INDEX(PublishedAt)`

**Project status:**
* Draft
* Published
* Archived

The database should not allow duplicate project slugs.

---

## 9. Project Images

**Table:** `ProjectImages`

| Column | Type | Constraints |
| :--- | :--- | :--- |
| `Id` | UUID | PK |
| `ProjectId` | UUID | FK |
| `StorageKey` | varchar | NOT NULL |
| `Url` | varchar | NOT NULL |
| `Caption` | varchar | NULL |
| `DisplayOrder` | int | NOT NULL |
| `IsHero` | boolean | NOT NULL |
| `CreatedAt` | datetime | NOT NULL |

**Relationship:**
```
Project
   │
   │ 1
   ▼
ProjectImages
   │
   │ *
```

**Foreign key:**
* `ProjectImages.ProjectId` → `Projects.Id`

---

## 10. Articles

**Table:** `Articles`

| Column | Type | Constraints |
| :--- | :--- | :--- |
| `Id` | UUID | PK |
| `Title` | varchar | NOT NULL |
| `Slug` | varchar | NOT NULL |
| `Summary` | varchar | NOT NULL |
| `Content` | text | NOT NULL |
| `Status` | varchar | NOT NULL |
| `ReadingTimeMinutes` | int | NOT NULL |
| `PublishedAt` | datetime | NULL |
| `CreatedAt` | datetime | NOT NULL |
| `UpdatedAt` | datetime | NOT NULL |
| `RowVersion` | rowversion / byte[] | Concurrency |

**Indexes:**
* `UNIQUE(Slug)`
* `INDEX(Status)`
* `INDEX(PublishedAt)`

---

## 11. Article Categories

**Tables:**
* `ArticleCategories`
* `ArticleCategoryMappings`

### ArticleCategories

| Column | Type | Constraints |
| :--- | :--- | :--- |
| `Id` | UUID | PK |
| `Name` | varchar | NOT NULL |
| `Slug` | varchar | NOT NULL |

**Constraints:**
* `UNIQUE(Name)`
* `UNIQUE(Slug)`

### ArticleCategoryMappings

| Column | Type | Constraints |
| :--- | :--- | :--- |
| `ArticleId` | UUID | PK / FK |
| `CategoryId` | UUID | PK / FK |

**Relationship:**
```
Article
   │
   │ *
   ▼
ArticleCategoryMappings
   ▲
   │ *
Category
```
An article can belong to multiple categories.

---

## 12. Article Tags

**Tables:**
* `Tags`
* `ArticleTags`

### Tags

| Column | Type | Constraints |
| :--- | :--- | :--- |
| `Id` | UUID | PK |
| `Name` | varchar | NOT NULL |
| `Slug` | varchar | NOT NULL |

### ArticleTags

| Column | Type | Constraints |
| :--- | :--- | :--- |
| `ArticleId` | UUID | PK / FK |
| `TagId` | UUID | PK / FK |

**Relationship:**
```
Article
   │
   ├──── Tag
   ├──── Tag
   └──── Tag
```

**Unique constraint:**
* `UNIQUE(ArticleId, TagId)`

---

## 13. Social Profiles

**Table:** `SocialProfiles`

| Column | Type | Constraints |
| :--- | :--- | :--- |
| `Id` | UUID | PK |
| `ProfileId` | UUID | FK |
| `Platform` | varchar | NOT NULL |
| `Url` | varchar | NOT NULL |
| `DisplayOrder` | int | NOT NULL |
| `IsVisible` | boolean | NOT NULL |
| `CreatedAt` | datetime | NOT NULL |
| `UpdatedAt` | datetime | NOT NULL |

**Relationship:**
```
ArchitectProfile
       │
       ▼
SocialProfiles
```

---

## 14. CV / Documents

The CV itself should not be stored as binary data in the relational database initially.

Instead:
```
Database
   │
   └── CV metadata

Object/File Storage
   │
   └── Actual PDF
```

**Table:** `Documents`

| Column | Type | Constraints |
| :--- | :--- | :--- |
| `Id` | UUID | PK |
| `Type` | varchar | NOT NULL |
| `FileName` | varchar | NOT NULL |
| `StorageKey` | varchar | NOT NULL |
| `ContentType` | varchar | NOT NULL |
| `FileSize` | bigint | NOT NULL |
| `IsActive` | boolean | NOT NULL |
| `UploadedAt` | datetime | NOT NULL |

**Example:**
```
Documents
   │
   └── Type = CV
```
The database stores metadata while Infrastructure manages physical storage.

---

## 15. Administration Tables

### 15.1 Users

**Table:** `Users`

| Column | Type | Constraints |
| :--- | :--- | :--- |
| `Id` | UUID | PK |
| `Email` | varchar | NOT NULL |
| `PasswordHash` | varchar | NOT NULL |
| `IsActive` | boolean | NOT NULL |
| `CreatedAt` | datetime | NOT NULL |
| `UpdatedAt` | datetime | NOT NULL |
| `LastLoginAt` | datetime | NULL |
| `RowVersion` | rowversion / byte[] | Concurrency |

**Constraint:**
* `UNIQUE(Email)`

---

## 16. Roles

**Table:** `Roles`

| Column | Type | Constraints |
| :--- | :--- | :--- |
| `Id` | UUID | PK |
| `Name` | varchar | NOT NULL |
| `IsSystemRole` | boolean | NOT NULL |
| `CreatedAt` | datetime | NOT NULL |
| `UpdatedAt` | datetime | NOT NULL |

**Constraint:**
* `UNIQUE(Name)`

---

## 17. User Roles

**Table:** `UserRoles`

| Column | Type | Constraints |
| :--- | :--- | :--- |
| `UserId` | UUID | PK / FK |
| `RoleId` | UUID | PK / FK |
| `AssignedAt` | datetime | NOT NULL |

**Relationship:**
```
User
 │
 ├──── Role
 ├──── Role
 └──── Role
```
This represents a many-to-many relationship.

---

## 18. Permissions

**Table:** `Permissions`

| Column | Type | Constraints |
| :--- | :--- | :--- |
| `Id` | UUID | PK |
| `Key` | varchar | NOT NULL |
| `Description` | varchar | NULL |

**Constraint:**
* `UNIQUE(Key)`

**Examples:**
* `projects:read`, `projects:write`, `projects:publish`
* `articles:read`, `articles:write`, `articles:publish`
* `users:read`, `users:write`

---

## 19. Role Permissions

**Table:** `RolePermissions`

| Column | Type | Constraints |
| :--- | :--- | :--- |
| `RoleId` | UUID | PK / FK |
| `PermissionId` | UUID | PK / FK |

**Relationship:**
```
Role
 │
 ├──── Permission
 ├──── Permission
 └──── Permission
```

---

## 20. Audit Information

Administrative changes should be auditable.  
For mutable entities, the initial model includes:
* `CreatedAt`
* `UpdatedAt`

For security-sensitive operations, a dedicated audit log should be introduced.

**Table:** `AuditLogs`

| Column | Type | Constraints |
| :--- | :--- | :--- |
| `Id` | UUID | PK |
| `UserId` | UUID | FK |
| `Action` | varchar | NOT NULL |
| `EntityType` | varchar | NOT NULL |
| `EntityId` | UUID | NOT NULL |
| `Timestamp` | datetime | NOT NULL |
| `Metadata` | JSON | NULL |

**Examples:**
* `UserRoleAssigned`
* `ProjectPublished`
* `ArticlePublished`
* `UserDeactivated`

Audit logging belongs to Infrastructure/Application concerns rather than the core Domain model.

---

## 21. Soft Delete Strategy

Soft delete should **not** be applied to every entity automatically.

For content where recovery is useful:
* `IsDeleted`
* `DeletedAt`

may be introduced.

However, for the initial version:
* **Projects:** Soft delete
* **Articles:** Soft delete
* **Users:** Deactivate rather than delete
* **Roles:** System roles cannot be deleted
* **Audit logs:** Never delete

The exact retention policy can evolve with business requirements.

---

## 22. Concurrency

Mutable aggregates should use optimistic concurrency.

The persistence model includes:
* `RowVersion`

**Example:**
```
Project
 ├── Id
 ├── Title
 ├── Status
 ├── ...
 └── RowVersion
```

If two administrators update the same project simultaneously:
```
Admin A ─────── Update ───────┐
                              │
Admin B ─────── Update ───────┤
                              ▼
                       Concurrency Check
                              │
                    ┌─────────┴─────────┐
                    │                   │
                  Success             Conflict
```

The API should return `409 Conflict` when a concurrency conflict is detected.

---

## 23. Referential Integrity

Foreign keys should enforce valid relationships.

**Examples:**
* `Experiences.ProfileId` → `ArchitectProfiles.Id`
* `ProjectImages.ProjectId` → `Projects.Id`
* `UserRoles.UserId` → `Users.Id`
* `UserRoles.RoleId` → `Roles.Id`

Deletion behavior must be explicitly configured.

For example:
* Deleting a `Project` may cascade-delete its `ProjectImages` metadata.
* However, deleting a `User` should **not** cascade-delete `AuditLogs` history.

---

## 24. Indexing Strategy

Indexes should be driven by actual query patterns.

Initial indexes include:
* **Projects:** `Slug`, `Status`, `PublishedAt`, `IsFeatured`
* **Articles:** `Slug`, `Status`, `PublishedAt`
* **Users:** `Email`
* **Roles:** `Name`
* **Categories:** `Slug`

Indexes should be reviewed after observing real production query behavior.

---

## 25. Domain Model vs Persistence Model

The Domain Model and Database Model are intentionally different.

```
Domain Model
     │
     │ Domain concerns
     ▼
Aggregate
Entity
Value Object
Business Rule
Domain Event
     │
     │ Mapping
     ▼
Persistence Model
     │
     ▼
Database Tables
```

For example:

**Domain:**
```
Project
 ├── ProjectId
 ├── ProjectMetadata
 ├── ProjectDuration
 ├── TechnologyTag
 └── Publish()
```

may be persisted as:

**Projects Table:**
```
Projects
 ├── Id
 ├── Title
 ├── Summary
 ├── Description
 ├── StartDate
 ├── EndDate
 ├── Status
 └── PublishedAt
```

The database does not need to mirror the Domain object structure.

---

## 26. Entity Framework Core Mapping

EF Core configurations belong in Infrastructure.

Example conceptual structure:
```
Infrastructure/
│
├── Persistence/
│   ├── PortfolioDbContext.cs
│   │
│   ├── Configurations/
│   │   ├── ArchitectProfileConfiguration.cs
│   │   ├── ProjectConfiguration.cs
│   │   ├── ProjectImageConfiguration.cs
│   │   ├── ArticleConfiguration.cs
│   │   ├── ExperienceConfiguration.cs
│   │   ├── SkillConfiguration.cs
│   │   ├── SocialProfileConfiguration.cs
│   │   ├── UserConfiguration.cs
│   │   ├── RoleConfiguration.cs
│   │   └── PermissionConfiguration.cs
│   │
│   └── Migrations/
```

The Domain project remains free from EF Core-specific configuration.

---

## 27. Repository Boundaries

Repositories should be defined around Aggregate Roots rather than every database table.

**Examples:**
* `IProjectRepository`
* `IArticleRepository`
* `IArchitectProfileRepository`
* `IUserRepository`
* `IRoleRepository`

Avoid creating repositories such as:
* `IProjectImageRepository`
* `IArticleTagRepository`
* `IUserRoleRepository`

when those entities are internal to their aggregate. The Aggregate Root controls access to its internal entities.

---

## 28. Transaction Boundary

A single application command should normally operate within one transaction.

**Example:**
```
PublishProjectCommand
        │
        ▼
Project Aggregate
        │
        ▼
Database Transaction
        │
        ├── Update Project
        │
        └── Store Domain Event
```

Cross-module operations should be minimized. If an operation eventually requires asynchronous processing, domain/integration events can be used.

---

## 29. Persistence Flow

The complete request flow becomes:

```
Client
  │
  ▼
REST API
  │
  ▼
Application Use Case
  │
  ▼
Domain Aggregate
  │
  ▼
Repository Interface
  │
  ▼
Infrastructure
  │
  ▼
EF Core
  │
  ▼
Relational Database
```

The dependency direction remains inward.

---

## 30. Initial Database Schema

The initial logical schema is:

```
Portfolio
│
├── ArchitectProfiles
│   │
│   ├── Experiences
│   ├── Skills
│   └── SocialProfiles
│
├── Projects
│   └── ProjectImages
│
├── Articles
│   ├── ArticleCategories
│   ├── ArticleCategoryMappings
│   ├── Tags
│   └── ArticleTags
│
└── Documents

Administration
│
├── Users
│   └── UserRoles
│
├── Roles
│   └── RolePermissions
│
├── Permissions
│
└── AuditLogs
```

---

## 31. Architectural Constraints

The following constraints apply:

1. Domain projects must not reference EF Core.
2. Domain projects must not reference database-specific APIs.
3. Infrastructure owns persistence implementation.
4. Repositories are defined around aggregate roots.
5. Database foreign keys enforce referential integrity.
6. Unique constraints enforce critical uniqueness requirements.
7. Optimistic concurrency protects mutable aggregates.
8. File content is stored outside the relational database.
9. Public API DTOs are independent from persistence entities.
10. Database schema should not dictate domain design.

---

## 32. Summary

The persistence architecture uses a relational database with EF Core inside the Infrastructure layer.

The design follows:

```
Domain
   ↑
Application
   ↑
API

Infrastructure
   │
   ├── EF Core
   ├── Repositories
   ├── Database
   └── External Storage
```

The system starts with a **single database** while preserving logical ownership between the Portfolio and Administration bounded contexts. This provides a pragmatic persistence model for the Modular Monolith without prematurely introducing distributed database complexity.
