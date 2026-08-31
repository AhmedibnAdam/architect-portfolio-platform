# Application Use Case Model

## 1. Overview & Architectural Intent
The **Application Use Case Model** establishes the behavioral boundary of the system, defining what each actor can execute against the backend services. Moving beyond domain taxonomy ("What exists in the domain?"), this document answers: **"What can each actor do with the system?"**

This layer forms the application service boundary (CQRS Handlers / Use Cases) positioned between the entry points (API Controllers) and core business logic (Domain Aggregates).

```
                      ┌──────────────────┐
                      │  Portfolio Web   │
                      └────────┬─────────┘
                               │
                      ┌────────▼─────────┐
                      │ Admin Dashboard  │
                      └────────┬─────────┘
                               │
                      ┌────────▼─────────┐
                      │     iOS App      │
                      └────────┬─────────┘
                               │
                               ▼
                      ┌──────────────────┐
                      │    REST API      │
                      └────────┬─────────┘
                               │
                               ▼
                      ┌──────────────────┐
                      │   Application    │
                      │                  │
                      │ Commands/Queries │
                      └────────┬─────────┘
                               │
                               ▼
                      ┌──────────────────┐
                      │     Domain       │
                      └──────────────────┘
```

---

## 2. Actor Profiles & Security Contexts

| Actor | Description | Security / Auth Context | Key Permissions |
| :--- | :--- | :--- | :--- |
| **Public Visitor** | Unauthenticated user consuming public content across Web/iOS. | Anonymous / Public | Read-only access to published content & public CV download. |
| **Portfolio Owner** | Content creator and manager of personal professional data. | Authenticated (`Bearer Token`, Role: `Owner`) | Full CRUD on Profile, Experience, Skills, Projects, Articles, Social Profiles, CV. |
| **Administrator** | Platform engineer/admin managing system access & roles. | Authenticated (`Bearer Token`, Role: `Admin`) | Full management of Users, Roles, Permissions, and System Settings. |

---

## 3. Actor-Centric Use Case Hierarchies

### 3.1 Public Visitor Use Cases
```
Public Visitor
├── View Profile
├── View Experience
├── View Skills
├── Browse Projects
├── View Project Details
├── Browse Articles
├── Read Article
├── View Social Profiles
└── Download CV
```

### 3.2 Portfolio Owner Use Cases
```
Portfolio Owner
├── Profile
│   └── Update Profile
├── Experience
│   ├── Create Experience
│   ├── Update Experience
│   └── Delete Experience
├── Skills
│   ├── Create Skill
│   ├── Update Skill
│   ├── Delete Skill
│   └── Reorder Skills
├── Projects
│   ├── Create Project
│   ├── Update Project
│   ├── Delete Project
│   ├── Publish Project
│   ├── Unpublish Project
│   ├── Feature Project
│   ├── Unfeature Project
│   └── Manage Project Images
├── Articles
│   ├── Create Article
│   ├── Update Article
│   ├── Delete Article
│   ├── Publish Article
│   ├── Unpublish Article
│   └── Manage Article Categories
├── Social Profiles
│   ├── Add Social Profile
│   ├── Update Social Profile
│   ├── Remove Social Profile
│   └── Reorder Social Profiles
└── CV
    ├── Upload CV
    ├── Replace CV
    └── Remove CV
```

### 3.3 Administrator Use Cases
```
Administrator
├── Users
│   ├── Create User
│   ├── Update User
│   ├── Deactivate User
│   └── Activate User
├── Roles
│   ├── Create Role
│   ├── Update Role
│   └── Delete Role
└── Permissions
    ├── Assign Role
    ├── Remove Role
    └── Manage Permissions
```

---

## 4. Mapping Use Cases to Bounded Contexts

```
                       Application Layer
                               │
          ┌────────────────────┴────────────────────┐
          ▼                                         ▼
   Portfolio Context                      Administration Context
   ├── Profile                            ├── Users
   ├── Experience                         ├── Roles
   ├── Skills                             └── Permissions
   ├── Projects
   ├── Articles
   ├── Social Profiles
   └── CV
```

### 4.1 Portfolio Context Mapping
* **Profile**: `GetProfile`, `UpdateProfile`
* **Experience**: `GetExperience`, `CreateExperience`, `UpdateExperience`, `DeleteExperience`
* **Skills**: `GetSkills`, `CreateSkill`, `UpdateSkill`, `DeleteSkill`, `ReorderSkills`
* **Projects**: `GetProject`, `ListProjects`, `CreateProject`, `UpdateProject`, `DeleteProject`, `PublishProject`, `UnpublishProject`, `FeatureProject`, `UnfeatureProject`, `UploadProjectImage`, `DeleteProjectImage`
* **Articles**: `GetArticle`, `ListArticles`, `CreateArticle`, `UpdateArticle`, `DeleteArticle`, `PublishArticle`, `UnpublishArticle`, `ManageArticleCategories`
* **Social Profiles**: `GetSocialProfiles`, `AddSocialProfile`, `UpdateSocialProfile`, `RemoveSocialProfile`, `ReorderSocialProfiles`
* **CV**: `GetCV`, `UploadCV`, `ReplaceCV`, `RemoveCV`

### 4.2 Administration Context Mapping
* **Users**: `GetUser`, `ListUsers`, `CreateUser`, `UpdateUser`, `ActivateUser`, `DeactivateUser`
* **Roles**: `GetRole`, `ListRoles`, `CreateRole`, `UpdateRole`, `DeleteRole`
* **Permissions**: `AssignRoleToUser`, `RemoveRoleFromUser`, `UpdateRolePermissions`

---

## 5. Command / Query Separation (CQRS Taxonomy)

We segregate intent at the application boundary into **Commands** (mutations altering state) and **Queries** (read-only projections returning DTOs).

```
                    Application
                         │
              ┌──────────┴──────────┐
              │                     │
           Queries               Commands
              │                     │
          Read data             Change state
              │                     │
              ▼                     ▼
          Domain                  Domain
```

### 5.1 Portfolio Context

#### Queries
| Query Name | Input Contract DTO | Output DTO | Access Level |
| :--- | :--- | :--- | :--- |
| `GetProfileQuery` | `None` | `ProfileDto` | Public |
| `GetExperienceQuery` | `None` | `IReadOnlyCollection<ExperienceDto>` | Public |
| `GetSkillsQuery` | `CategoryFilter?` | `IReadOnlyCollection<SkillCategoryGroupDto>` | Public |
| `ListProjectsQuery` | `ProjectFilterParameters` | `PagedResult<ProjectSummaryDto>` | Public / Owner (Includes drafts) |
| `GetProjectBySlugQuery` | `Slug` | `ProjectDetailDto` | Public / Owner |
| `ListArticlesQuery` | `ArticleFilterParameters` | `PagedResult<ArticleSummaryDto>` | Public / Owner (Includes drafts) |
| `GetArticleBySlugQuery` | `Slug` | `ArticleDetailDto` | Public / Owner |
| `GetSocialProfilesQuery` | `None` | `IReadOnlyCollection<SocialProfileDto>` | Public |
| `GetLatestCVQuery` | `None` | `CVDownloadDto` | Public |

#### Commands
| Command Name | Input Payload | Side Effects / Domain Events | Access Level |
| :--- | :--- | :--- | :--- |
| `UpdateProfileCommand` | `ProfileData` | Emits `ProfileUpdatedEvent` | Owner |
| `CreateExperienceCommand` | `ExperienceData` | Emits `ExperienceAddedEvent` | Owner |
| `UpdateExperienceCommand` | `Id, ExperienceData` | Emits `ExperienceUpdatedEvent` | Owner |
| `DeleteExperienceCommand` | `Id` | Emits `ExperienceRemovedEvent` | Owner |
| `CreateSkillCommand` | `SkillData` | Emits `SkillCreatedEvent` | Owner |
| `UpdateSkillCommand` | `Id, SkillData` | Emits `SkillUpdatedEvent` | Owner |
| `DeleteSkillCommand` | `Id` | Emits `SkillDeletedEvent` | Owner |
| `ReorderSkillsCommand` | `List<SkillOrder>` | Updates display order sequence | Owner |
| `CreateProjectCommand` | `ProjectData` | Instantiates `ProjectAggregate` | Owner |
| `UpdateProjectCommand` | `Id, ProjectData` | Updates domain entity attributes | Owner |
| `DeleteProjectCommand` | `Id` | Purges aggregate & attached assets | Owner |
| `PublishProjectCommand` | `Id` | Emits `ProjectPublishedEvent` | Owner |
| `UnpublishProjectCommand` | `Id` | Changes visibility to draft | Owner |
| `FeatureProjectCommand` | `Id` | Flags project as featured | Owner |
| `UnfeatureProjectCommand` | `Id` | Removes featured flag | Owner |
| `UploadProjectImageCommand` | `Id, Stream, Meta` | Stores image & updates media collection | Owner |
| `CreateArticleCommand` | `ArticleData` | Instantiates `ArticleAggregate` | Owner |
| `UpdateArticleCommand` | `Id, ArticleData` | Updates content & slug | Owner |
| `DeleteArticleCommand` | `Id` | Removes article entity | Owner |
| `PublishArticleCommand` | `Id` | Emits `ArticlePublishedEvent` | Owner |
| `UnpublishArticleCommand` | `Id` | Reverts status to `Draft` | Owner |
| `UploadCVCommand` | `Stream, Meta` | Stores binary document, updates metadata | Owner |
| `ReplaceCVCommand` | `Id, Stream, Meta` | Overwrites active document version | Owner |
| `RemoveCVCommand` | `Id` | Deletes CV file resource | Owner |

---

### 5.2 Administration Context

#### Queries
| Query Name | Input Contract DTO | Output DTO | Access Level |
| :--- | :--- | :--- | :--- |
| `GetUserByIdQuery` | `UserId` | `UserDetailDto` | Admin |
| `ListUsersQuery` | `UserFilterParameters` | `PagedResult<UserSummaryDto>` | Admin |
| `ListRolesQuery` | `None` | `IReadOnlyCollection<RoleDto>` | Admin |
| `GetRolePermissionsQuery` | `RoleId` | `RolePermissionsDto` | Admin |

#### Commands
| Command Name | Input Payload | Side Effects / Domain Events | Access Level |
| :--- | :--- | :--- | :--- |
| `CreateUserCommand` | `UserData` | Emits `UserCreatedEvent` | Admin |
| `UpdateUserCommand` | `UserId, UserData` | Modifies user attributes | Admin |
| `ActivateUserCommand` | `UserId` | Sets user status `Active` | Admin |
| `DeactivateUserCommand` | `UserId` | Revokes tokens, sets status `Inactive` | Admin |
| `CreateRoleCommand` | `RoleData` | Creates security role entity | Admin |
| `UpdateRoleCommand` | `RoleId, RoleData` | Modifies role definition | Admin |
| `DeleteRoleCommand` | `RoleId` | Deletes unassigned role | Admin |
| `AssignRoleToUserCommand` | `UserId, RoleId` | Updates security claim bindings | Admin |
| `RemoveRoleFromUserCommand`| `UserId, RoleId` | Revokes claim binding | Admin |

---

## 6. Proposed Application Layer Directory Structure

The .NET Solution's `Application` project mirrors the CQRS layout divided by Bounded Context and Feature slice:

```
src/Backend/Application/
├── Common/
│   ├── Behaviors/             # Validation, Logging, UnitOfWork Pipeline Behaviors
│   ├── Exceptions/            # Application-level validation/not found exceptions
│   ├── Interfaces/            # ICurrentUserService, IDateTime, IStorageService
│   └── Models/                # PagedList, Result<T> wrappers
│
├── Portfolio/
│   ├── Profile/
│   │   ├── Commands/
│   │   │   └── UpdateProfile/
│   │   │       ├── UpdateProfileCommand.cs
│   │   │       ├── UpdateProfileCommandHandler.cs
│   │   │       └── UpdateProfileCommandValidator.cs
│   │   └── Queries/
│   │       └── GetProfile/
│   │           ├── GetProfileQuery.cs
│   │           ├── GetProfileQueryHandler.cs
│   │           └── ProfileDto.cs
│   │
│   ├── Experience/
│   │   ├── Commands/
│   │   │   ├── CreateExperience/
│   │   │   ├── UpdateExperience/
│   │   │   └── DeleteExperience/
│   │   └── Queries/
│   │       └── GetExperience/
│   │
│   ├── Skills/
│   │   ├── Commands/
│   │   │   ├── CreateSkill/
│   │   │   ├── UpdateSkill/
│   │   │   ├── DeleteSkill/
│   │   │   └── ReorderSkills/
│   │   └── Queries/
│   │       └── GetSkills/
│   │
│   ├── Projects/
│   │   ├── Commands/
│   │   │   ├── CreateProject/
│   │   │   ├── UpdateProject/
│   │   │   ├── DeleteProject/
│   │   │   ├── PublishProject/
│   │   │   ├── UnpublishProject/
│   │   │   ├── FeatureProject/
│   │   │   └── ManageProjectImages/
│   │   └── Queries/
│   │       ├── GetProjectBySlug/
│   │       └── ListProjects/
│   │
│   ├── Articles/
│   │   ├── Commands/
│   │   │   ├── CreateArticle/
│   │   │   ├── UpdateArticle/
│   │   │   ├── DeleteArticle/
│   │   │   ├── PublishArticle/
│   │   │   └── UnpublishArticle/
│   │   └── Queries/
│   │       ├── GetArticleBySlug/
│   │       └── ListArticles/
│   │
│   ├── SocialProfiles/
│   │   ├── Commands/
│   │   │   ├── AddSocialProfile/
│   │   │   ├── UpdateSocialProfile/
│   │   │   ├── RemoveSocialProfile/
│   │   │   └── ReorderSocialProfiles/
│   │   └── Queries/
│   │       └── GetSocialProfiles/
│   │
│   └── CV/
│       ├── Commands/
│       │   ├── UploadCV/
│       │   ├── ReplaceCV/
│       │   └── RemoveCV/
│       └── Queries/
│           └── GetLatestCV/
│
└── Administration/
    ├── Users/
    │   ├── Commands/
    │   │   ├── CreateUser/
    │   │   ├── UpdateUser/
    │   │   ├── ActivateUser/
    │   │   └── DeactivateUser/
    │   └── Queries/
    │       ├── GetUserById/
    │       └── ListUsers/
    │
    ├── Roles/
    │   ├── Commands/
    │   │   ├── CreateRole/
    │   │   ├── UpdateRole/
    │   │   └── DeleteRole/
    │   └── Queries/
    │       └── ListRoles/
    │
    └── Permissions/
        └── Commands/
            ├── AssignRoleToUser/
            ├── RemoveRoleFromUser/
            └── UpdateRolePermissions/
```

---

## 7. Next Steps & Architecture Artifact Roadmap

Having established the application core, we explicitly refrain from implementing controllers or repository concrete implementations at this stage.

```
Requirements                    ✅
Architecture Drivers            ✅
Quality Attributes              ✅
System Context                  ✅
Architecture Options            ✅
Architecture Decision           ✅
C4 Container Diagram            ✅
Domain Model                    ✅
.NET Solution Structure         ✅
Dependency Graph                ✅
DDD Tactical Model              ✅
Application Use Case Model      ✅ (THIS ARTIFACT)
API Contract Model              ← NEXT STEP
Database/Persistence Model      ← UPCOMING
Technical Implementation        ← THEN
```

### Next Milestone: API Contract / Interface Model
We will define:
* HTTP Methods & Route Specs (`GET /api/projects/{slug}`, `POST /api/admin/projects/{id}/publish`)
* Request / Response Payload Schemas
* Authentication & Role Authorization Requirements
* Pagination, Filtering, and Sorting parameters
* Standardized RFC 7807 Problem Details Error Contracts
