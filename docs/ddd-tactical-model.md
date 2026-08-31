# DDD Tactical Model: Architect Portfolio Platform

This document outlines the Domain-Driven Design (DDD) Tactical Model for the **Architect Portfolio Platform**, structured according to the core subdomains and bounded contexts identified in the system architecture.

---

## 1. Bounded Contexts / Modules

The system is partitioned into two primary Bounded Contexts to isolate distinct business domain logic, lifecycle rules, and administrative capabilities.

```
Architect Portfolio Platform
│
├── Portfolio Bounded Context
│   ├── Profile
│   ├── Experience
│   ├── Skills
│   ├── Projects
│   ├── Articles
│   └── Social Profiles
│
└── Administration Bounded Context
    ├── Users
    ├── Roles
    └── Permissions
```

### 1.1 Portfolio Bounded Context
- **Purpose**: Manages the public-facing and content-creation lifecycle of the architect's professional identity, showcase work, and published thought leadership.
- **Modules**:
  - `Profile`: Personal details, biography, summary statement, contact details.
  - `Experience`: Work history, roles, employment dates, achievements.
  - `Skills`: Technical proficiency, design methodologies, certifications, domain expertise.
  - `Projects`: Architecture/design portfolio items, project metadata, galleries, client details, technologies used.
  - `Articles`: Technical publications, blog posts, design studies, draft/published status.
  - `Social Profiles`: External profile links (e.g., GitHub, LinkedIn, Behance, Twitter).

### 1.2 Administration Bounded Context
- **Purpose**: Handles authentication, authorization, RBAC (Role-Based Access Control), system configuration, and audit auditing for platform management.
- **Modules**:
  - `Users`: System user accounts, credentials, identity state, profile linkages.
  - `Roles`: Access control groups defining system capabilities (e.g., `SuperAdmin`, `Editor`, `Viewer`).
  - `Permissions`: Granular access flags associated with specific actions across contexts.

---

## 2. Aggregates

Aggregates represent consistency boundaries within which domain invariants must be maintained at all times.

### Portfolio Context Aggregates
- **`ArchitectProfile` Aggregate**:
  - **Aggregate Root**: `ArchitectProfile`
  - **Encapsulated Entities/Value Objects**: `ContactInformation`, `SocialLink`, `Bio`, `SkillSet`
  - **Invariants**: Must maintain at least one primary email contact; custom URL handle must be unique.
- **`Project` Aggregate**:
  - **Aggregate Root**: `Project`
  - **Encapsulated Entities/Value Objects**: `ProjectImage` (Entity), `ProjectMetadata` (VO), `TechnologyTag` (VO), `ProjectDuration` (VO)
  - **Invariants**: Published projects must have a non-empty thumbnail image and valid title; project end date cannot precede start date.
- **`Article` Aggregate**:
  - **Aggregate Root**: `Article`
  - **Encapsulated Entities/Value Objects**: `ArticleContent` (VO), `Slug` (VO), `PublishStatus` (VO)
  - **Invariants**: Published articles must possess a valid, non-empty slug and reading time calculation.

### Administration Context Aggregates
- **`User` Aggregate**:
  - **Aggregate Root**: `User`
  - **Encapsulated Entities/Value Objects**: `UserId` (VO), `EmailAddress` (VO), `PasswordHash` (VO), `UserRoleAssignment` (Entity)
  - **Invariants**: A user must always possess at least one active Role assignment.
- **`Role` Aggregate**:
  - **Aggregate Root**: `Role`
  - **Encapsulated Entities/Value Objects**: `RoleId` (VO), `PermissionSet` (VO)
  - **Invariants**: System default roles cannot be deleted or have critical platform management permissions detached.

---

## 3. Entities

Entities possess a explicit identity that persists across time and state mutations.

| Context | Entity Name | Identity Field | Responsibilities & Description |
| :--- | :--- | :--- | :--- |
| **Portfolio** | `ArchitectProfile` | `ProfileId` | Represents the architect's central identity details and personal summary. |
| **Portfolio** | `Project` | `ProjectId` | Represents an individual portfolio piece with assets, timeline, and tech specs. |
| **Portfolio** | `ProjectImage` | `ImageId` | Represents a media asset assigned to a project gallery, tracking order and caption. |
| **Portfolio** | `Article` | `ArticleId` | Holds long-form blog posts or architectural case studies. |
| **Portfolio** | `Experience` | `ExperienceId` | Tracks professional employment, role duration, and highlighted contributions. |
| **Administration** | `User` | `UserId` | Represents an administrative identity capable of logging into the management panel. |
| **Administration** | `Role` | `RoleId` | Defines a set of permission rights assigned to system users. |

---

## 4. Value Objects

Value Objects are immutable, identity-less types defined strictly by their attribute values.

- **`Slug`**: Wraps a URL-friendly string representation (e.g., `modern-microservice-architecture`). Validates format upon instantiation.
- **`EmailAddress`**: Encapsulates email formatting, validation, and domain check logic.
- **`ProjectDuration`**: Holds `startDate` and optional `endDate`. Provides calculation methods like `.durationInMonths()` or `.isOngoing()`.
- **`SocialLink`**: Combines platform type (e.g., LinkedIn, GitHub), target URL, and display order.
- **`SkillRating`**: Encapsulates skill name, proficiency level (e.g., `Beginner`, `Expert`), and category tags.
- **`Permission`**: Immutable representation of a permission key (e.g., `projects:publish`, `users:write`).

---

## 5. Domain Services

Domain Services encapsulate domain logic and business operations that do not naturally belong to a single Entity or Aggregate Root.

- **`SlugGenerationService`** (`Portfolio` context):
  - *Responsibility*: Generates unique, SEO-compliant URL slugs for `Project` and `Article` titles, resolving duplicate slug collisions across the context repository.
- **`PortfolioPublishingService`** (`Portfolio` context):
  - *Responsibility*: Coordinates pre-publication validation across `ArchitectProfile`, `Project`, and `Skill` aggregates ensuring baseline completion requirements before public release.
- **`UserAccessControlService`** (`Administration` context):
  - *Responsibility*: Evaluates whether a target `User` possesses necessary permissions (combining assigned `Roles` and custom override permissions) to execute administrative commands across contexts.

---

## 6. Domain Events

Domain Events capture state-changing occurrences within the domain, enabling decoupled side-effects and cross-context communication.

- **`ProjectPublished`**: Emitted when a `Project` aggregate transitions from draft to published status. (Triggers search indexing / cache invalidation).
- **`ArticleDrafted`**: Emitted when a new `Article` is saved as a draft.
- **`ArticlePublished`**: Emitted when an `Article` is made publicly accessible.
- **`ProfileUpdated`**: Emitted when `ArchitectProfile` structural details or contact information change.
- **`UserRoleAssigned`**: Emitted in the Administration context when a `Role` is attached to a `User`.
- **`UserPermissionsRevoked`**: Emitted when access control attributes for a administrative user are restricted.

---

## 7. Business Rules

The system enforces several critical business invariants across aggregate boundaries:

1. **Uniqueness Rules**:
   - `Article` and `Project` slugs must be unique within their respective bounded context.
   - `User` email addresses must be unique platform-wide.
2. **Publishing Invariants**:
   - An `Article` cannot be published without a title, summary, non-empty slug, and at least one category tag.
   - A `Project` marked as `Featured` must have at least one high-resolution hero image assigned.
3. **Temporal Invariants**:
   - `Experience` start dates must be in the past or present.
   - `Experience` end date must be greater than or equal to the start date (if not ongoing).
4. **Access Invariants**:
   - Every `User` account must maintain at least one active administrative role.
   - The primary `SuperAdmin` user account cannot be deactivated or deleted.
