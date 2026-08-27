# Quality Attribute Scenarios

**Project:** Architect Portfolio Platform  
**Document:** Quality Attribute Scenarios  
**Version:** 1.0  
**Status:** Draft  
**Previous Document:** `02-architecture-drivers.md`  
**Next Document:** `04-architecture-options.md`

---

## 1. Purpose

This document defines the measurable **quality attribute scenarios** that will guide architectural decisions [cite: source].

The scenarios translate non-functional requirements into concrete situations that an architecture must handle [cite: source].

They will later be used to:

- Compare architecture options objectively [cite: source].
- Identify architectural trade-offs [cite: source].
- Evaluate whether an architectural decision satisfies the system's requirements [cite: source].
- Define measurable acceptance criteria [cite: source].
- Prevent premature adoption of architectural patterns or technologies [cite: source].
- Provide evidence for future Architecture Decision Records (ADRs) [cite: source].

The scenarios follow the standard quality attribute scenario structure:

- Source [cite: source]
- Stimulus [cite: source]
- Environment [cite: source]
- Artifact [cite: source]
- Response [cite: source]
- Response Measure [cite: source]

---

## 2. Quality Attribute Scenario Structure

Each scenario consists of six elements [cite: source].

| Element | Description |
| :--- | :--- |
| **Source** | Who or what generates the stimulus [cite: source] |
| **Stimulus** | The event or condition that triggers the scenario [cite: source] |
| **Environment** | The operating condition under which the stimulus occurs [cite: source] |
| **Artifact** | The system component affected by the stimulus [cite: source] |
| **Response** | What the system should do [cite: source] |
| **Response Measure** | How the response is measured [cite: source] |

The **Response Measure** is particularly important because it transforms a qualitative expectation into something that can be evaluated [cite: source].

---

## 3. Performance

Performance describes how quickly and efficiently the system responds to requests and processes workloads [cite: source].

### PERF-01 — API Response Time

- **Source:** User or client application [cite: source].
- **Stimulus:** The client sends a normal API request [cite: source].
- **Environment:** Normal production workload [cite: source].
- **Artifact:** API endpoint and associated application services [cite: source].
- **Response:** The system processes the request and returns a response [cite: source].
- **Response Measure:**
  - At least 95% of requests should complete within 500 ms [cite: source].
  - At least 99% should complete within 1 second [cite: source].
  - Performance targets should be measured excluding network latency outside the system's control [cite: source].

---

### PERF-02 — Database Query Performance

- **Source:** Application service [cite: source].
- **Stimulus:** A database query is executed as part of a user request [cite: source].
- **Environment:** Normal production workload [cite: source].
- **Artifact:** Database and data-access layer [cite: source].
- **Response:** The database executes the query and returns the required data [cite: source].
- **Response Measure:**
  - 95% of normal queries should complete within 200 ms [cite: source].
  - Queries exceeding 1 second should be considered performance anomalies and observable through monitoring [cite: source].

---

### PERF-03 — Peak Request Latency

- **Source:** Multiple concurrent users [cite: source].
- **Stimulus:** Request traffic increases significantly above the normal baseline [cite: source].
- **Environment:** Peak expected production traffic [cite: source].
- **Artifact:** API/application layer [cite: source].
- **Response:** The system continues processing requests without unacceptable latency degradation [cite: source].
- **Response Measure:**
  - 95% of requests should remain below 1 second [cite: source].
  - 99% should remain below 2 seconds [cite: source].
  - Error rate should remain below 1% [cite: source].

---

### PERF-04 — Resource Efficiency

- **Source:** Production workload [cite: source].
- **Stimulus:** The system processes sustained traffic [cite: source].
- **Environment:** Normal and peak production workload [cite: source].
- **Artifact:** Application infrastructure [cite: source].
- **Response:** The system processes requests without excessive CPU, memory, database connections, or network consumption [cite: source].
- **Response Measure:**
  - CPU utilization should normally remain below 70% [cite: source].
  - Memory utilization should remain below 80% [cite: source].
  - No sustained resource exhaustion should occur during expected workload [cite: source].

---

## 4. Security

Security protects data, APIs, infrastructure, and users from unauthorized access or malicious activity [cite: source].

### SEC-01 — Authentication

- **Source:** Unauthenticated client [cite: source].
- **Stimulus:** The client attempts to access a protected resource [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** API authentication and authorization layer [cite: source].
- **Response:** The system rejects the request unless valid authentication credentials are provided [cite: source].
- **Response Measure:**
  - Unauthorized requests must receive an appropriate 401/403 response [cite: source].
  - No protected data may be returned [cite: source].
  - Authentication failures must be logged without exposing credentials or secrets [cite: source].

---

### SEC-02 — Authorization

- **Source:** Authenticated user [cite: source].
- **Stimulus:** The user attempts to access a resource they are not authorized to access [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** Authorization layer and protected resource [cite: source].
- **Response:** The system denies access [cite: source].
- **Response Measure:**
  - Unauthorized access must never expose protected data [cite: source].
  - The request must be rejected consistently [cite: source].
  - The security event must be observable through security/audit logs where appropriate [cite: source].

---

### SEC-03 — Data Protection

- **Source:** Attacker or unauthorized user [cite: source].
- **Stimulus:** Attempts to intercept or access sensitive information [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** Network communication and data storage [cite: source].
- **Response:** Sensitive information is protected both in transit and at rest [cite: source].
- **Response Measure:**
  - Sensitive network traffic must use encrypted communication [cite: source].
  - Sensitive stored data must use appropriate encryption where required [cite: source].
  - Secrets must not be stored in source control [cite: source].
  - Credentials must never appear in application logs [cite: source].

---

### SEC-04 — Malicious Input

- **Source:** Malicious client [cite: source].
- **Stimulus:** The client submits malformed or intentionally malicious input [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** API/application boundary [cite: source].
- **Response:** The system validates and rejects unsafe input without compromising system integrity [cite: source].
- **Response Measure:**
  - Invalid input must not cause application crashes [cite: source].
  - No unauthorized data access should occur [cite: source].
  - Security validation failures should be observable [cite: source].

---

### SEC-05 — Secret Management

- **Source:** Developer or deployment process [cite: source].
- **Stimulus:** Application requires access to credentials or sensitive configuration [cite: source].
- **Environment:** Development, staging, or production [cite: source].
- **Artifact:** Configuration and secret-management mechanism [cite: source].
- **Response:** The application retrieves secrets through an approved secure mechanism [cite: source].
- **Response Measure:**
  - No production secret should be committed to source control [cite: source].
  - Secrets must be independently rotatable [cite: source].
  - Secret rotation should not require rebuilding application source code [cite: source].

---

## 5. Availability

Availability describes the system's ability to remain accessible when users need it [cite: source].

### AVAIL-01 — Normal Availability

- **Source:** User [cite: source].
- **Stimulus:** User attempts to access the system [cite: source].
- **Environment:** Normal production operation [cite: source].
- **Artifact:** Complete production system [cite: source].
- **Response:** The system accepts and processes the request [cite: source].
- **Response Measure:**
  - Target availability: 99.9% monthly [cite: source].
  - Planned maintenance must be excluded from availability calculations where formally scheduled [cite: source].

---

### AVAIL-02 — Component Failure

- **Source:** Infrastructure or external dependency [cite: source].
- **Stimulus:** A non-critical component becomes unavailable [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** Affected service and dependent components [cite: source].
- **Response:** The system continues providing available functionality where possible [cite: source].
- **Response Measure:**
  - Failure of a non-critical dependency must not bring down the entire system [cite: source].
  - Degraded functionality should be clearly observable [cite: source].
  - Recovery should occur automatically where feasible [cite: source].

---

### AVAIL-03 — Dependency Failure

- **Source:** External service [cite: source].
- **Stimulus:** An external dependency becomes unavailable [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** Integration layer [cite: source].
- **Response:** The system handles the failure gracefully [cite: source].
- **Response Measure:**
  - Requests should fail predictably rather than causing cascading failures [cite: source].
  - Appropriate timeout and retry policies should be applied [cite: source].
  - The failure must be observable [cite: source].
  - Recovery should occur automatically when the dependency becomes available [cite: source].

---

## 6. Reliability

Reliability describes the system's ability to perform correctly and consistently over time [cite: source].

### REL-01 — Transient Failure Recovery

- **Source:** Infrastructure or network [cite: source].
- **Stimulus:** A temporary failure occurs while processing a request [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** Application service [cite: source].
- **Response:** The system retries or recovers when retrying is safe [cite: source].
- **Response Measure:**
  - Transient failures should not unnecessarily become user-visible failures [cite: source].
  - Retry policies must use bounded retries [cite: source].
  - Retry behavior must not create request storms [cite: source].

---

### REL-02 — Data Consistency

- **Source:** User or system process [cite: source].
- **Stimulus:** An operation modifies persistent data [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** Application and persistence layer [cite: source].
- **Response:** The system maintains the defined consistency guarantees [cite: source].
- **Response Measure:**
  - Business invariants must not be violated [cite: source].
  - Failed operations must not leave invalid partial state [cite: source].
  - Consistency guarantees must be explicitly documented for distributed operations [cite: source].

---

### REL-03 — Duplicate Requests

- **Source:** Client [cite: source].
- **Stimulus:** The same operation is submitted more than once because of retries or network uncertainty [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** API/application layer [cite: source].
- **Response:** The system prevents unintended duplicate business effects where the operation requires idempotency [cite: source].
- **Response Measure:**
  - Idempotent operations must produce the same business result when safely repeated [cite: source].
  - Duplicate processing must not create unintended duplicate records or transactions [cite: source].

---

## 7. Scalability

Scalability describes how the system behaves as workload and data volume increase [cite: source].

### SCALE-01 — Increased Concurrent Users

- **Source:** Growing user population [cite: source].
- **Stimulus:** Concurrent users increase significantly [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** Application and infrastructure [cite: source].
- **Response:** The system scales to accommodate additional demand [cite: source].
- **Response Measure:**
  - System capacity should increase without unacceptable architectural changes [cite: source].
  - Performance targets defined in the performance section should remain satisfied [cite: source].
  - Scaling should not require modifying business logic [cite: source].

---

### SCALE-02 — Increased Data Volume

- **Source:** Business growth [cite: source].
- **Stimulus:** Persistent data grows significantly [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** Database and data-access layer [cite: source].
- **Response:** The system continues operating within acceptable performance limits [cite: source].
- **Response Measure:**
  - Query performance should remain within defined thresholds [cite: source].
  - Storage growth must not cause application instability [cite: source].
  - Database capacity must be independently scalable where required [cite: source].

---

### SCALE-03 — Horizontal Scaling

- **Source:** Infrastructure/platform [cite: source].
- **Stimulus:** Additional application instances are started [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** Application services [cite: source].
- **Response:** Requests can be distributed across multiple instances [cite: source].
- **Response Measure:**
  - Application instances should not require local persistent state for correctness unless explicitly designed [cite: source].
  - Adding instances should increase capacity approximately proportionally within the expected scaling range [cite: source].

---

## 8. Maintainability

Maintainability describes how easily the system can be understood, changed, debugged, and evolved [cite: source].

### MAINT-01 — Feature Modification

- **Source:** Development team [cite: source].
- **Stimulus:** A business requirement requires modification of an existing feature [cite: source].
- **Environment:** Normal development [cite: source].
- **Artifact:** Affected application module [cite: source].
- **Response:** Developers can modify the feature without unnecessary changes to unrelated modules [cite: source].
- **Response Measure:**
  - Changes should remain localized where architectural boundaries permit [cite: source].
  - Unrelated modules should not require modification without a dependency reason [cite: source].
  - Automated tests should identify regressions [cite: source].

---

### MAINT-02 — Defect Investigation

- **Source:** Development team [cite: source].
- **Stimulus:** A production defect is reported [cite: source].
- **Environment:** Production incident investigation [cite: source].
- **Artifact:** Application code, logs, metrics, and traces [cite: source].
- **Response:** Developers can identify the affected component and root cause [cite: source].
- **Response Measure:**
  - Common production defects should be diagnosable within 30 minutes [cite: source].
  - Relevant logs and traces should provide enough context to reproduce the execution path [cite: source].

---

### MAINT-03 — Dependency Upgrade

- **Source:** Development team [cite: source].
- **Stimulus:** A framework or third-party dependency requires an upgrade [cite: source].
- **Environment:** Development [cite: source].
- **Artifact:** Dependency and affected modules [cite: source].
- **Response:** The dependency can be upgraded without widespread architectural changes [cite: source].
- **Response Measure:**
  - Dependency changes should be isolated behind appropriate abstractions where justified [cite: source].
  - Automated tests must detect compatibility regressions [cite: source].

---

## 9. Testability

Testability describes how easily system behavior can be verified automatically [cite: source].

### TEST-01 — Unit Testing

- **Source:** Development team [cite: source].
- **Stimulus:** A developer needs to test business logic [cite: source].
- **Environment:** Development/CI [cite: source].
- **Artifact:** Application business logic [cite: source].
- **Response:** Business logic can be tested independently of infrastructure [cite: source].
- **Response Measure:**
  - Core business rules should be testable without requiring a real database, network, or external service [cite: source].
  - Unit tests should execute quickly enough to run on every CI build [cite: source].

---

### TEST-02 — Integration Testing

- **Source:** Development team [cite: source].
- **Stimulus:** A change affects communication between components [cite: source].
- **Environment:** CI/staging [cite: source].
- **Artifact:** Service and integration boundaries [cite: source].
- **Response:** Automated integration tests verify the interaction [cite: source].
- **Response Measure:**
  - Critical integrations must have automated integration coverage [cite: source].
  - Integration tests must detect contract-breaking changes [cite: source].

---

### TEST-03 — Regression Testing

- **Source:** Development team [cite: source].
- **Stimulus:** A new feature or bug fix is introduced [cite: source].
- **Environment:** CI [cite: source].
- **Artifact:** Application [cite: source].
- **Response:** Automated tests execute before deployment [cite: source].
- **Response Measure:**
  - Critical business flows must have automated regression coverage [cite: source].
  - A failed critical test must prevent deployment to production [cite: source].

---

## 10. Observability

Observability describes the ability to understand internal system behavior from external outputs [cite: source].

### OBS-01 — Request Tracing

- **Source:** User/client [cite: source].
- **Stimulus:** A request enters the system [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** Application request path [cite: source].
- **Response:** The request receives a correlation/trace identifier that can follow its execution [cite: source].
- **Response Measure:**
  - Requests should be traceable across relevant application components [cite: source].
  - Logs, metrics, and traces should be correlatable [cite: source].

---

### OBS-02 — Error Detection

- **Source:** Application component [cite: source].
- **Stimulus:** An unexpected error occurs [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** Affected component [cite: source].
- **Response:** The system records sufficient diagnostic information and generates an appropriate alert [cite: source].
- **Response Measure:**
  - Critical errors must be detected automatically [cite: source].
  - Alerts should be generated within 5 minutes of detection [cite: source].
  - Logs must contain enough contextual information for investigation without exposing sensitive data [cite: source].

---

### OBS-03 — Performance Monitoring

- **Source:** Application [cite: source].
- **Stimulus:** Request latency or resource consumption deviates from expected behavior [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** Application/infrastructure [cite: source].
- **Response:** Monitoring detects the degradation [cite: source].
- **Response Measure:**
  Monitoring should expose at minimum [cite: source]:
  - Request rate [cite: source]
  - Error rate [cite: source]
  - Latency [cite: source]
  - CPU utilization [cite: source]
  - Memory utilization [cite: source]
  - Database performance [cite: source]
  - Dependency failures [cite: source]

---

## 11. Deployability

Deployability describes how safely and efficiently the system can be released [cite: source].

### DEPLOY-01 — Standard Deployment

- **Source:** Development/CI pipeline [cite: source].
- **Stimulus:** A validated version is ready for release [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** Application [cite: source].
- **Response:** The system is deployed automatically through the deployment pipeline [cite: source].
- **Response Measure:**
  - Deployment should not require manual modification of production servers [cite: source].
  - Deployment should be repeatable [cite: source].
  - Deployment should produce an auditable release record [cite: source].

---

### DEPLOY-02 — Deployment Failure

- **Source:** Deployment pipeline [cite: source].
- **Stimulus:** A deployment step fails [cite: source].
- **Environment:** Production release [cite: source].
- **Artifact:** Deployment environment [cite: source].
- **Response:** The system prevents an incomplete release from becoming the active production version [cite: source].
- **Response Measure:**
  - Failed deployments must not leave the system in an unknown state [cite: source].
  - The previous stable version should remain available or recovery should be automated [cite: source].

---

### DEPLOY-03 — Rollback

- **Source:** Operations/deployment system [cite: source].
- **Stimulus:** A production release introduces a critical defect [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** Deployed application [cite: source].
- **Response:** The system is reverted to the last known stable version [cite: source].
- **Response Measure:**
  - Rollback should be executable through an automated or well-defined process [cite: source].
  - Target rollback time: less than 15 minutes [cite: source].

---

### DEPLOY-04 — Configuration Change

- **Source:** Operations [cite: source].
- **Stimulus:** A runtime configuration value needs to change [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** Application configuration [cite: source].
- **Response:** The configuration changes without requiring unnecessary source-code changes [cite: source].
- **Response Measure:**
  - Configuration should be externally managed where appropriate [cite: source].
  - Sensitive configuration must use secure secret management [cite: source].
  - Configuration changes must be auditable [cite: source].

---

## 12. Extensibility

Extensibility describes how easily new capabilities can be added without destabilizing existing functionality [cite: source].

### EXT-01 — New Business Feature

- **Source:** Product/business team [cite: source].
- **Stimulus:** A new business capability is requested [cite: source].
- **Environment:** Development [cite: source].
- **Artifact:** Application architecture [cite: source].
- **Response:** The capability can be added while minimizing changes to unrelated functionality [cite: source].
- **Response Measure:**
  - New functionality should primarily affect its relevant module/components [cite: source].
  - Existing critical functionality should continue passing regression tests [cite: source].
  - Architectural changes should be required only when justified by the new capability [cite: source].

---

### EXT-02 — New External Integration

- **Source:** Business requirement [cite: source].
- **Stimulus:** A new third-party provider must be integrated [cite: source].
- **Environment:** Development/production [cite: source].
- **Artifact:** Integration boundary [cite: source].
- **Response:** The new provider can be introduced without coupling the core business logic directly to provider-specific implementation details [cite: source].
- **Response Measure:**
  - Provider-specific code should remain isolated [cite: source].
  - Replacing the provider should not require rewriting unrelated business logic [cite: source].
  - Integration tests should validate the provider boundary [cite: source].

---

### EXT-03 — New Client Application

- **Source:** Business/product team [cite: source].
- **Stimulus:** A new client such as web or mobile is introduced [cite: source].
- **Environment:** Development/production [cite: source].
- **Artifact:** API/application boundary [cite: source].
- **Response:** The system supports the new client without duplicating core business rules unnecessarily [cite: source].
- **Response Measure:**
  - Core business logic should remain reusable [cite: source].
  - Client-specific presentation concerns should remain separated from core business rules [cite: source].
  - Existing clients must continue functioning [cite: source].

---

## 13. Usability

Usability describes how effectively users can understand and operate the system [cite: source].

### USE-01 — Normal User Flow

- **Source:** End user [cite: source].
- **Stimulus:** The user performs a common business operation [cite: source].
- **Environment:** Normal system operation [cite: source].
- **Artifact:** User interface and supporting API [cite: source].
- **Response:** The system provides clear feedback and completes the operation [cite: source].
- **Response Measure:**
  - Common operations should complete within the defined performance thresholds [cite: source].
  - Users should receive clear success/failure feedback [cite: source].
  - Validation errors should identify the problem and appropriate corrective action [cite: source].

---

### USE-02 — Error Recovery

- **Source:** End user [cite: source].
- **Stimulus:** The user enters invalid data or encounters a recoverable error [cite: source].
- **Environment:** Normal production operation [cite: source].
- **Artifact:** User interface [cite: source].
- **Response:** The system explains the error and allows the user to recover without unnecessarily restarting the workflow [cite: source].
- **Response Measure:**
  - Error messages should be understandable to the target user [cite: source].
  - Recoverable errors should not cause loss of already entered valid information [cite: source].
  - The user should be able to continue or retry the operation [cite: source].

---

### USE-03 — Slow/Degraded Dependency

- **Source:** External dependency [cite: source].
- **Stimulus:** A dependency becomes slow or temporarily unavailable [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** User-facing workflow [cite: source].
- **Response:** The application communicates the degraded state appropriately rather than appearing frozen or failing silently [cite: source].
- **Response Measure:**
  - The user should receive feedback when an operation cannot complete immediately [cite: source].
  - Requests must respect defined timeout limits [cite: source].
  - The UI should not wait indefinitely for an unavailable dependency [cite: source].

---

## 14. Cross-Cutting Quality Scenarios

Some scenarios affect several quality attributes simultaneously [cite: source].

### CROSS-01 — Cascading Failure Prevention

- **Source:** External dependency [cite: source].
- **Stimulus:** A dependency becomes unavailable or significantly slower [cite: source].
- **Environment:** Production under normal or elevated traffic [cite: source].
- **Artifact:** Application and dependency integration layer [cite: source].
- **Response:** The system isolates the failure and prevents it from propagating through unrelated components [cite: source].
- **Response Measure:**
  - No cascading application-wide failure should occur [cite: source].
  - Timeouts must be bounded [cite: source].
  - Retries must be bounded [cite: source].
  - Failure must be observable [cite: source].
  - Unaffected functionality should remain available where possible [cite: source].
- **Quality Attributes Affected:** Availability, Reliability, Performance, Observability, Scalability [cite: source].

---

### CROSS-02 — Production Incident

- **Source:** Production system [cite: source].
- **Stimulus:** A critical service begins returning errors [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** Affected service and supporting infrastructure [cite: source].
- **Response:** The team detects, investigates, mitigates, and recovers from the incident [cite: source].
- **Response Measure:**
  - Detection within 5 minutes [cite: source].
  - Diagnosis target within 30 minutes [cite: source].
  - Recovery target within 60 minutes for a critical incident [cite: source].
  - Logs, metrics, and traces must support investigation [cite: source].
- **Quality Attributes Affected:** Observability, Reliability, Availability, Maintainability, Deployability [cite: source].

---

### CROSS-03 — Safe Release

- **Source:** Development team [cite: source].
- **Stimulus:** A new version is released [cite: source].
- **Environment:** Production [cite: source].
- **Artifact:** Application and deployment infrastructure [cite: source].
- **Response:** The release is validated and can be safely rolled back if necessary [cite: source].
- **Response Measure:**
  - Automated tests must pass before release [cite: source].
  - Deployment must be repeatable [cite: source].
  - Health checks must validate the deployed version [cite: source].
  - Rollback must be possible within 15 minutes [cite: source].
- **Quality Attributes Affected:** Deployability, Reliability, Availability, Testability, Observability [cite: source].

---

## 15. Scenario Priority

Not every scenario has equal architectural importance [cite: source].  
Each scenario should eventually receive a priority based on [cite: source]:

| Priority | Meaning |
| :--- | :--- |
| **Critical** | Failure directly threatens business continuity, security, or core functionality [cite: source] |
| **High** | Significant impact on users or operational cost [cite: source] |
| **Medium** | Important but tolerable degradation [cite: source] |
| **Low** | Desirable improvement [cite: source] |

The initial priority should be refined after reviewing the business requirements and system constraints [cite: source].

---

## 16. Scenario Summary

| ID | Quality Attribute | Scenario |
| :--- | :--- | :--- |
| **PERF-01** | Performance | API response time [cite: source] |
| **PERF-02** | Performance | Database query performance [cite: source] |
| **PERF-03** | Performance | Peak request latency [cite: source] |
| **PERF-04** | Performance | Resource efficiency [cite: source] |
| **SEC-01** | Security | Authentication [cite: source] |
| **SEC-02** | Security | Authorization [cite: source] |
| **SEC-03** | Security | Data protection [cite: source] |
| **SEC-04** | Security | Malicious input [cite: source] |
| **SEC-05** | Security | Secret management [cite: source] |
| **AVAIL-01** | Availability | Normal availability [cite: source] |
| **AVAIL-02** | Availability | Component failure [cite: source] |
| **AVAIL-03** | Availability | Dependency failure [cite: source] |
| **REL-01** | Reliability | Transient failure recovery [cite: source] |
| **REL-02** | Reliability | Data consistency [cite: source] |
| **REL-03** | Reliability | Duplicate requests [cite: source] |
| **SCALE-01** | Scalability | Increased concurrent users [cite: source] |
| **SCALE-02** | Scalability | Increased data volume [cite: source] |
| **SCALE-03** | Scalability | Horizontal scaling [cite: source] |
| **MAINT-01** | Maintainability | Feature modification [cite: source] |
| **MAINT-02** | Maintainability | Defect investigation [cite: source] |
| **MAINT-03** | Maintainability | Dependency upgrade [cite: source] |
| **TEST-01** | Testability | Unit testing [cite: source] |
| **TEST-02** | Testability | Integration testing [cite: source] |
| **TEST-03** | Testability | Regression testing [cite: source] |
| **OBS-01** | Observability | Request tracing [cite: source] |
| **OBS-02** | Observability | Error detection [cite: source] |
| **OBS-03** | Observability | Performance monitoring [cite: source] |
| **DEPLOY-01** | Deployability | Standard deployment [cite: source] |
| **DEPLOY-02** | Deployability | Deployment failure [cite: source] |
| **DEPLOY-03** | Deployability | Rollback [cite: source] |
| **DEPLOY-04** | Deployability | Configuration change [cite: source] |
| **EXT-01** | Extensibility | New business feature [cite: source] |
| **EXT-02** | Extensibility | New external integration [cite: source] |
| **EXT-03** | Extensibility | New client application [cite: source] |
| **USE-01** | Usability | Normal user flow [cite: source] |
| **USE-02** | Usability | Error recovery [cite: source] |
| **USE-03** | Usability | Slow/degraded dependency [cite: source] |
| **CROSS-01** | Cross-cutting | Cascading failure prevention [cite: source] |
| **CROSS-02** | Cross-cutting | Production incident [cite: source] |
| **CROSS-03** | Cross-cutting | Safe release [cite: source] |

---

## 17. How These Scenarios Will Be Used

These scenarios are **not architecture decisions** [cite: source].  
They are **constraints and evaluation criteria** [cite: source].

The next architecture-design steps should use them to compare possible solutions [cite: source]:

```text
Architecture Option A
          ↓
Evaluate against quality scenarios
          ↓
Architecture Option B
          ↓
Evaluate against quality scenarios
          ↓
Compare trade-offs
          ↓
Select architecture
          ↓
Document decision in ADR
```

An architecture should therefore not be selected because [cite: source]:

> *"Clean Architecture is better."*  
> or:  
> *"Microservices are more scalable."* [cite: source]

Instead, the decision should be based on evidence such as [cite: source]:

> **Requirement:** High availability during dependency failure [cite: source].  
> **Option A:** Monolith with synchronous dependency calls [cite: source].  
> **Option B:** Modular monolith with asynchronous processing [cite: source].  
> **Option C:** Distributed services with asynchronous messaging [cite: source].  
> **Evaluation:** Option C provides stronger isolation but introduces operational and consistency complexity [cite: source].  
> **Decision:** Choose the simplest option that satisfies the required availability and reliability scenarios [cite: source].

This preserves the principle established earlier:

> **Architecture should emerge from requirements and architectural drivers, not from architectural fashion.** [cite: source]

---

## 18. Relationship With Previous Documents

The architecture documentation sequence is now [cite: source]:

```text
01 — SRS
    ↓
02 — Architecture Requirements
    ↓
02 — Architecture Drivers
    ↓
03 — Quality Attribute Scenarios
    ↓
Architecture Options
    ↓
Architecture Trade-off Analysis
    ↓
Architecture Decision Records
    ↓
Target Architecture
```

The most important transition is now [cite: source]:

> **Requirements → Drivers → Measurable Scenarios → Architecture Options → Trade-offs** [cite: source]

This gives us a defensible basis for deciding whether we actually need things such as [cite: source]:

- Clean Architecture [cite: source]
- DDD [cite: source]
- CQRS [cite: source]
- Event-driven architecture [cite: source]
- Modular monolith [cite: source]
- Microservices [cite: source]
- Message brokers [cite: source]
- Distributed caching [cite: source]
- API gateways [cite: source]
- Service decomposition [cite: source]

...rather than introducing them simply because they are considered "good architecture." [cite: source]

---

## 19. Next Document

The next document is **`04-architecture-options.md`**.

---

## 20. Document Status

- **Version:** 1.0  
- **Status:** Complete & Ready for Architecture Options  
- **Previous Document:** `02-architecture-drivers.md`  
- **Next Document:** `04-architecture-options.md`
