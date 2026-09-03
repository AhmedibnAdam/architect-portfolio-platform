# Git Branching Strategy — Architect Portfolio Platform

This document defines the branching model, naming conventions, and commit conventions used for this repository. It follows the same **Lean Initial Footprint** principle as the [solution structure](solution-structure.md): the lightest workflow that still gives history, review, and rollback safety — not a heavyweight process the project doesn't need yet.

---

## Branch Model

A **trunk-based** model with short-lived branches.

```
main                                   (always buildable, deployable)
 │
 ├── feature/domain-project-aggregate  (merged, deleted)
 ├── feature/application-projects      (in progress)
 ├── feature/application-articles
 ├── fix/project-metadata-validation
 └── docs/git-branching-strategy
```

- **`main`** is the only long-lived branch. It must always build and pass tests. Nothing is committed to it directly except trivial doc fixes — everything else lands via a branch merged in.
- No permanent `develop` branch. At this stage (pre-release, single contributor, no parallel release trains) a `develop` branch adds merge overhead without a benefit — it can be introduced later if/when there's a real need to stage multiple in-flight releases.
- Short-lived branches are created per unit of work and deleted after merge.

## Branch Naming

```
<type>/<scope>-<short-description>
```

| Type       | Used for                                              | Example                                   |
|------------|--------------------------------------------------------|--------------------------------------------|
| `feature/` | New capability (a domain aggregate, a use case, an endpoint) | `feature/application-projects-create-usecase` |
| `fix/`     | Bug fix                                                | `fix/project-metadata-validation`          |
| `refactor/`| Behavior-preserving restructuring                      | `refactor/domain-common-base-types`        |
| `docs/`    | Documentation only                                     | `docs/git-branching-strategy`              |
| `chore/`   | Tooling, CI, project scaffolding                       | `chore/add-application-project`            |
| `test/`    | Test-only changes (no production code)                 | `test/skill-update-coverage`               |

`<scope>` matches the module/layer being touched — `domain`, `application`, `infrastructure`, `api`, or a vertical slice name (`projects`, `articles`, `profile`, `experience`, `skills`, `administration`) — mirroring the folder names in [solution-structure.md](solution-structure.md).

## Workflow

1. Branch off `main`:
   ```
   git checkout main
   git pull
   git checkout -b feature/application-projects-create-usecase
   ```
2. Commit in small, reviewable increments (see commit convention below).
3. Push and open a PR into `main`.
4. Merge via **squash merge** — keeps `main` history one commit per unit of work, regardless of how many WIP commits happened on the branch.
5. Delete the branch after merge.

## Commit Message Convention

[Conventional Commits](https://www.conventionalcommits.org/), matching the convention already used in this repo's history:

```
<type>(<scope>): <summary>
```

| Type       | Meaning                                  |
|------------|-------------------------------------------|
| `feat`     | New behavior                              |
| `fix`      | Bug fix                                   |
| `refactor` | Restructuring without behavior change     |
| `test`     | Test-only change                          |
| `docs`     | Documentation only                        |
| `chore`    | Tooling/scaffolding, no source impact     |

Examples:
- `feat(domain): implement project aggregate and domain model`
- `feat(application): add CreateProjectCommand and handler`
- `fix(domain): correct self-assignment bug in Skill.Update`
- `docs: add git branching strategy`

## Layer Rollout Sequence

Branches should generally follow the build order already established: **Domain → Application → Infrastructure → API**, one vertical slice at a time (Projects, Articles, Profile, Experience, Skills, Administration). This keeps each branch small and reviewable instead of one branch spanning multiple layers.

## When to Revisit This Document

Introduce a `develop` branch, `release/*` branches, or a stricter gitflow only when a real need appears — e.g. multiple releases in flight at once, or a second contributor needing an integration branch separate from `main`. Until then, this lean model is the default.
