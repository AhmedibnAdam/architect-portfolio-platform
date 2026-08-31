# API Contract Design — Architect Portfolio Platform

**System:** Architect Portfolio Platform  
**Document:** API Contract Design  
**Status:** Draft  
**Date:** 2026-08-31  
**API Style:** REST  
**Backend:** ASP.NET Core Modular Monolith

---

## 1. Purpose

This document defines the external API contract between the Architect Portfolio Platform backend and its client applications.

The API provides a unified interface for:

- Portfolio Website
- Admin Dashboard
- iOS Application

The API is responsible for exposing application use cases without exposing internal domain models or infrastructure implementation details.

The API follows the architectural principle:

> Clients communicate with the Application layer through API contracts. Domain entities are never exposed directly.

---

# 2. API Consumers

The backend API is consumed by three client applications.

```text
                     ┌──────────────────────┐
                     │   Portfolio Website  │
                     │      Public          │
                     └──────────┬───────────┘
                                │
                                │
                     ┌──────────▼───────────┐
                     │      REST API        │
                     │                      │
                     │  Architect Portfolio │
                     │      Platform        │
                     └──────────▲───────────┘
                                │
              ┌─────────────────┼─────────────────┐
              │                 │                 │
              │                 │                 │
       ┌──────▼──────┐   ┌──────▼──────┐   ┌──────▼──────┐
       │    Admin    │   │    iOS      │   │   External  │
       │  Dashboard  │   │    App      │   │  Consumers  │
       └─────────────┘   └─────────────┘   └─────────────┘
```

### Client responsibilities

| Client | Purpose | Authentication |
|---|---|---|
| Portfolio Website | Public portfolio consumption | No authentication for public content |
| Admin Dashboard | Portfolio and platform management | Required |
| iOS App | Native portfolio consumption | No authentication for public content |
| External Consumers | Optional future API consumers | Depends on API |

---

# 3. API Design Principles

The API follows these principles:

1. RESTful resource-oriented endpoints.
2. JSON request and response bodies.
3. HTTPS only.
4. Versioned API contracts.
5. DTOs instead of domain entities.
6. Consistent error responses.
7. Authentication and authorization for administrative operations.
8. Pagination for collection endpoints.
9. Filtering and sorting where required.
10. Idempotent operations where appropriate.
11. Proper HTTP status codes.
12. Backward-compatible API evolution.

---

# 4. Base URL

The initial API version is:

```text
/api/v1
```

Example:

```text
GET /api/v1/projects
```

Administrative endpoints are separated from public endpoints:

```text
/api/v1/projects
/api/v1/articles

/api/v1/admin/projects
/api/v1/admin/articles
```

This makes the security boundary explicit.

---

# 5. Authentication

Administrative operations require authentication.

The initial design uses:

```text
Authorization: Bearer <access-token>
```

Example:

```http
Authorization: Bearer eyJhbGciOi...
```

Public read operations do not require authentication.

---

# 6. Authorization

Authentication answers:

> Who are you?

Authorization answers:

> What are you allowed to do?

The Administration bounded context provides RBAC-based authorization.

Example permissions:

```text
profile:read
profile:write

projects:read
projects:write
projects:publish
projects:delete

articles:read
articles:write
articles:publish
articles:delete

users:read
users:write

roles:read
roles:write
```

Example:

```text
SuperAdmin
    └── All permissions

Editor
    ├── projects:write
    ├── projects:publish
    ├── articles:write
    └── articles:publish

Viewer
    └── Read-only permissions
```

---

# 7. Portfolio API

## 7.1 Profile

### Get Profile

```http
GET /api/v1/profile
```

Authentication:

```text
Public
```

Response:

```json
{
  "id": "profile-id",
  "name": "Architect Name",
  "headline": "Software Architect",
  "bio": "Professional biography...",
  "email": "contact@example.com",
  "location": "Cairo, Egypt"
}
```

Response status:

```text
200 OK
```

---

### Update Profile

```http
PUT /api/v1/admin/profile
```

Authentication:

```text
Required
```

Permission:

```text
profile:write
```

Request:

```json
{
  "name": "Architect Name",
  "headline": "Software Architect",
  "bio": "Updated biography...",
  "email": "contact@example.com",
  "location": "Cairo, Egypt"
}
```

Response:

```text
200 OK
```

---

# 8. Experience API

## List Experience

```http
GET /api/v1/experience
```

Response:

```json
{
  "items": [
    {
      "id": "experience-id",
      "company": "Company",
      "position": "Software Architect",
      "description": "Responsibilities and achievements...",
      "startDate": "2022-01-01",
      "endDate": null,
      "isCurrent": true
    }
  ]
}
```

---

## Create Experience

```http
POST /api/v1/admin/experience
```

Permission:

```text
experience:write
```

Request:

```json
{
  "company": "Company",
  "position": "Software Architect",
  "description": "Responsibilities...",
  "startDate": "2022-01-01",
  "endDate": null
}
```

Response:

```text
201 Created
```

---

## Update Experience

```http
PUT /api/v1/admin/experience/{id}
```

Response:

```text
200 OK
```

---

## Delete Experience

```http
DELETE /api/v1/admin/experience/{id}
```

Response:

```text
204 No Content
```

---

# 9. Skills API

## List Skills

```http
GET /api/v1/skills
```

---

## Create Skill

```http
POST /api/v1/admin/skills
```

Request:

```json
{
  "name": "Software Architecture",
  "category": "Architecture",
  "proficiencyLevel": "Expert"
}
```

---

## Update Skill

```http
PUT /api/v1/admin/skills/{id}
```

---

## Delete Skill

```http
DELETE /api/v1/admin/skills/{id}
```

---

## Reorder Skills

```http
PUT /api/v1/admin/skills/order
```

Request:

```json
{
  "items": [
    {
      "id": "skill-1",
      "order": 1
    },
    {
      "id": "skill-2",
      "order": 2
    }
  ]
}
```

---

# 10. Projects API

Projects are one of the primary portfolio resources.

## List Published Projects

```http
GET /api/v1/projects
```

Optional query parameters:

```text
?page=1
&pageSize=10
&category=architecture
&technology=.NET
&featured=true
&sort=publishedAt
&order=desc
```

Example:

```http
GET /api/v1/projects?page=1&pageSize=10&featured=true
```

Response:

```json
{
  "items": [
    {
      "id": "project-id",
      "title": "Architect Portfolio Platform",
      "slug": "architect-portfolio-platform",
      "summary": "Architecture portfolio platform...",
      "thumbnailUrl": "/media/projects/portfolio.jpg",
      "featured": true,
      "publishedAt": "2026-08-31"
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 1,
  "totalPages": 1
}
```

---

## Get Project

```http
GET /api/v1/projects/{slug}
```

Example:

```http
GET /api/v1/projects/architect-portfolio-platform
```

Response:

```json
{
  "id": "project-id",
  "title": "Architect Portfolio Platform",
  "slug": "architect-portfolio-platform",
  "summary": "Architecture portfolio platform...",
  "description": "Detailed project description...",
  "technologies": [
    ".NET",
    "ASP.NET Core",
    "Swift",
    "React"
  ],
  "images": [
    {
      "url": "/media/projects/portfolio-1.jpg",
      "caption": "System architecture"
    }
  ],
  "startDate": "2026-01-01",
  "endDate": null,
  "featured": true,
  "publishedAt": "2026-08-31"
}
```

---

## Create Project

```http
POST /api/v1/admin/projects
```

Permission:

```text
projects:write
```

Request:

```json
{
  "title": "Architect Portfolio Platform",
  "summary": "Architecture portfolio platform...",
  "description": "Detailed description...",
  "technologies": [
    ".NET",
    "ASP.NET Core",
    "Swift"
  ],
  "startDate": "2026-01-01"
}
```

Response:

```text
201 Created
```

---

## Update Project

```http
PUT /api/v1/admin/projects/{id}
```

---

## Delete Project

```http
DELETE /api/v1/admin/projects/{id}
```

---

## Publish Project

```http
POST /api/v1/admin/projects/{id}/publish
```

Response:

```text
200 OK
```

This operation triggers:

```text
ProjectPublished
```

domain event.

---

## Unpublish Project

```http
POST /api/v1/admin/projects/{id}/unpublish
```

---

## Feature Project

```http
POST /api/v1/admin/projects/{id}/feature
```

---

## Unfeature Project

```http
POST /api/v1/admin/projects/{id}/unfeature
```

---

# 11. Project Images API

## Add Project Image

```http
POST /api/v1/admin/projects/{projectId}/images
```

Request:

```text
multipart/form-data
```

Fields:

```text
file
caption
displayOrder
```

---

## Remove Project Image

```http
DELETE /api/v1/admin/projects/{projectId}/images/{imageId}
```

---

## Reorder Project Images

```http
PUT /api/v1/admin/projects/{projectId}/images/order
```

Request:

```json
{
  "items": [
    {
      "id": "image-1",
      "order": 1
    },
    {
      "id": "image-2",
      "order": 2
    }
  ]
}
```

---

# 12. Articles API

## List Published Articles

```http
GET /api/v1/articles
```

Query parameters:

```text
?page=1
&pageSize=10
&category=architecture
&tag=ddd
```

---

## Get Article

```http
GET /api/v1/articles/{slug}
```

---

## Create Article

```http
POST /api/v1/admin/articles
```

Request:

```json
{
  "title": "Designing a Modular Monolith",
  "summary": "A practical architecture study...",
  "content": "Article content...",
  "categories": [
    "Architecture",
    "DDD"
  ],
  "tags": [
    "modular-monolith",
    "ddd"
  ]
}
```

---

## Update Article

```http
PUT /api/v1/admin/articles/{id}
```

---

## Delete Article

```http
DELETE /api/v1/admin/articles/{id}
```

---

## Publish Article

```http
POST /api/v1/admin/articles/{id}/publish
```

Triggers:

```text
ArticlePublished
```

---

## Unpublish Article

```http
POST /api/v1/admin/articles/{id}/unpublish
```

---

# 13. Social Profiles API

## Get Social Profiles

```http
GET /api/v1/social-profiles
```

---

## Add Social Profile

```http
POST /api/v1/admin/social-profiles
```

Request:

```json
{
  "platform": "LinkedIn",
  "url": "https://example.com/profile",
  "displayOrder": 1
}
```

---

## Update Social Profile

```http
PUT /api/v1/admin/social-profiles/{id}
```

---

## Delete Social Profile

```http
DELETE /api/v1/admin/social-profiles/{id}
```

---

## Reorder Social Profiles

```http
PUT /api/v1/admin/social-profiles/order
```

---

# 14. CV API

## Get Current CV

```http
GET /api/v1/cv
```

---

## Download CV

```http
GET /api/v1/cv/download
```

---

## Upload CV

```http
POST /api/v1/admin/cv
```

Content type:

```text
multipart/form-data
```

---

## Replace CV

```http
PUT /api/v1/admin/cv
```

---

## Remove CV

```http
DELETE /api/v1/admin/cv
```

---

# 15. Administration API

Administration endpoints are private.

## Authentication

### Login

```http
POST /api/v1/auth/login
```

Request:

```json
{
  "email": "admin@example.com",
  "password": "********"
}
```

Response:

```json
{
  "accessToken": "token",
  "expiresIn": 3600
}
```

---

## Users

### List Users

```http
GET /api/v1/admin/users
```

### Get User

```http
GET /api/v1/admin/users/{id}
```

### Create User

```http
POST /api/v1/admin/users
```

### Update User

```http
PUT /api/v1/admin/users/{id}
```

### Activate User

```http
POST /api/v1/admin/users/{id}/activate
```

### Deactivate User

```http
POST /api/v1/admin/users/{id}/deactivate
```

---

# 16. Roles

### List Roles

```http
GET /api/v1/admin/roles
```

### Create Role

```http
POST /api/v1/admin/roles
```

### Update Role

```http
PUT /api/v1/admin/roles/{id}
```

### Delete Role

```http
DELETE /api/v1/admin/roles/{id}
```

---

# 17. Permissions

### List Permissions

```http
GET /api/v1/admin/permissions
```

### Assign Role

```http
POST /api/v1/admin/users/{userId}/roles
```

Request:

```json
{
  "roleId": "editor-role-id"
}
```

### Remove Role

```http
DELETE /api/v1/admin/users/{userId}/roles/{roleId}
```

---

# 18. Standard Error Contract

All API errors should follow a consistent structure.

Example:

```json
{
  "type": "https://api.example.com/errors/validation",
  "title": "Validation failed",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "instance": "/api/v1/admin/projects",
  "errors": {
    "title": [
      "Title is required."
    ]
  },
  "traceId": "00-abc123"
}
```

The API should follow the Problem Details standard.

---

# 19. HTTP Status Codes

| Status | Usage |
|---|---|
| `200 OK` | Successful read/update/action |
| `201 Created` | Resource successfully created |
| `202 Accepted` | Asynchronous operation accepted |
| `204 No Content` | Successful operation with no response body |
| `400 Bad Request` | Invalid request |
| `401 Unauthorized` | Authentication required/invalid |
| `403 Forbidden` | Authenticated but insufficient permissions |
| `404 Not Found` | Resource does not exist |
| `409 Conflict` | Business or uniqueness conflict |
| `422 Unprocessable Entity` | Domain validation failure |
| `429 Too Many Requests` | Rate limit exceeded |
| `500 Internal Server Error` | Unexpected server failure |

---

# 20. Pagination Contract

Collection endpoints should support pagination.

Request:

```http
GET /api/v1/projects?page=1&pageSize=20
```

Response:

```json
{
  "items": [],
  "page": 1,
  "pageSize": 20,
  "totalCount": 100,
  "totalPages": 5
}
```

The API should enforce a maximum page size to prevent excessive queries.

Example:

```text
Default page size: 20
Maximum page size: 100
```

---

# 21. Filtering and Sorting

Where appropriate, collection endpoints support:

```text
?page=1
&pageSize=20
&sort=publishedAt
&order=desc
```

Filtering examples:

```text
?featured=true
?category=architecture
?technology=.NET
?tag=ddd
```

Filtering capabilities should be introduced only where justified by actual client requirements.

---

# 22. API → Application Mapping

The API layer does not contain business logic.

The request flow is:

```text
HTTP Request
     │
     ▼
┌─────────────┐
│ API         │
│ Controller  │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│ Application │
│ Use Case    │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│ Domain      │
│ Aggregate   │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│Infrastructure│
│ Persistence │
└─────────────┘
```

For example:

```text
POST /api/v1/admin/projects/{id}/publish
                │
                ▼
       PublishProjectCommand
                │
                ▼
        Project.Publish()
                │
                ▼
       ProjectPublished
                │
                ▼
       Infrastructure handlers
```

---

# 23. API Contract Ownership

The API contract belongs to the API boundary.

The Application layer owns:

- Commands
- Queries
- Use cases
- Application DTOs
- Application interfaces

The Domain layer owns:

- Entities
- Aggregates
- Value Objects
- Domain Events
- Business Rules

Infrastructure owns:

- Database
- File storage
- External services
- Identity implementation
- Message infrastructure

Therefore:

```text
API
 │
 │ HTTP Contracts
 ▼
Application
 │
 │ Use Cases
 ▼
Domain
 │
 │ Business Rules
 ▼
Infrastructure
```

---

# 24. API Security Boundaries

Public resources:

```text
GET /api/v1/profile
GET /api/v1/projects
GET /api/v1/projects/{slug}
GET /api/v1/articles
GET /api/v1/articles/{slug}
GET /api/v1/skills
GET /api/v1/experience
GET /api/v1/social-profiles
GET /api/v1/cv
```

Protected resources:

```text
/api/v1/admin/*
```

The backend must enforce authorization independently of the client UI.

The Admin Dashboard must not be considered a security boundary.

---

# 25. API Evolution

The API should be designed for backward-compatible evolution.

Initial version:

```text
/api/v1
```

Future breaking changes may introduce:

```text
/api/v2
```

Non-breaking changes should generally include:

- Adding optional response properties
- Adding new endpoints
- Adding optional request properties
- Adding new resources

Breaking changes should require a new API version.

---

# 26. API Contract Summary

The initial API is organized around the following resources:

```text
/api/v1
│
├── auth
│
├── profile
├── experience
├── skills
├── projects
├── articles
├── social-profiles
└── cv
    │
    └── admin
        ├── profile
        ├── experience
        ├── skills
        ├── projects
        ├── articles
        ├── social-profiles
        ├── cv
        ├── users
        ├── roles
        └── permissions
```

The API provides a stable boundary between the three clients and the modular monolith backend.

---

## 27. Architectural Decision

The platform will use a **versioned REST API** as the primary communication mechanism between:

```text
Portfolio Website
        │
Admin Dashboard
        │
     iOS App
        │
        ▼
   REST API
        │
        ▼
Modular Monolith
```

The API will expose **application capabilities**, not domain entities.

Business rules remain inside the Domain layer, while orchestration remains inside the Application layer.

This preserves the dependency direction established by the target architecture.