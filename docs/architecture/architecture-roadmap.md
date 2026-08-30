# Architecture Roadmap

Below is a Mermaid diagram version of the roadmap (editable) plus a brief description. Use the SVG for presentation-quality rendering; the Mermaid block is easy to modify.

```mermaid
flowchart LR
  subgraph Business
    R[Requirements]\nStakeholders, Goals, Constraints
    A[Analysis & Specs]\nUser Stories, Acceptance Criteria
    S[Solution Design]\nHigh-level Modules, APIs
    Arch[Architecture & Tech]\nSystem Diagrams, Non-func Req
    TI[Technical Implementation]\nCode, CI/CD, Tests
  end

  R --> A --> S --> Arch --> TI

  subgraph Technical
    Tr[Traces]\nUse Cases & Flows
    DM[Data Model]\nEntities & Schemas
    C[Components]\nModules & Interfaces
    I[Infra]\nEnvironments & CI/CD
    Impl[Implementation]\nCode, Tests, Pipelines
  end

  Tr --> DM --> C --> I --> Impl

  R -.-> Tr
  A -.-> DM
  S -.-> C
  Arch -.-> I
  TI -.-> Impl
```

**Notes**
- Top lane follows product/business progression from `Requirements` to `Technical Implementation`.
- Bottom lane lists technical artifacts and areas that map to each stage.
- Use the SVG `architecture-roadmap.svg` for slides or documentation.

Generated files:
- [docs/architecture/architecture-roadmap.svg](docs/architecture/architecture-roadmap.svg)
- [docs/architecture/architecture-roadmap.md](docs/architecture/architecture-roadmap.md)
