# Feature Specification: Fix Refactor Process Reliability

**Feature Branch**: `012-fix-refactor-process`  
**Created**: 2026-05-17  
**Status**: Draft  
**Input**: User description: "fix the problems described here: https://github.com/EHotwagner/FS-Skia-UI/blob/master/docs/controls-boundary-refactor-process-report.md"

## Clarifications

### Session 2026-05-17

- Q: How should broad aggregate targets behave when the runner environment prevents authoritative product validation? → A: Environment failures fail the aggregate, with an explicit environment-failure verdict.
- Q: What should process-health preflight do when runner health is clearly insufficient for broad verification? → A: Preflight fails fast on clearly insufficient runner health.
- Q: What is required for final readiness after a broad aggregate environment failure? → A: Require a fresh healthy broad pass before final readiness.
- Q: How should preflight decide that runner health is clearly insufficient? → A: Use repo-owned default thresholds with explicit overrides.
- Q: What stale-reference scope should block active package-boundary readiness? → A: Scan active-tree ownership docs, metadata, source, tests, templates, and generated guidance.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Trust Broad Verification Results (Priority: P1)

As a framework maintainer, I want broad verification runs to report whether a
failure is caused by product behavior or by the local runner environment, so
that I do not mistake process exhaustion, startup failures, or missing runner
dependencies for a broken feature.

**Independent Test**: On a constrained runner, a broad verification attempt
records process-health diagnostics before heavy work starts, classifies
infrastructure startup failures as environment failures, and names the exact
health signals that made the broad result non-authoritative. The broad
aggregate returns a failing result for the environment failure while clearly
separating that verdict from a product failure. When preflight diagnostics show
clearly insufficient runner health, the broad path stops before high-pressure
work starts. On a healthy runner, the same broad verification path proceeds
normally and reports product failures only when product checks actually execute
and fail.

### User Story 2 - Keep Focused Gates Actionable (Priority: P1)

As a maintainer working through a large refactor, I want focused validation
gates to run without unnecessary aggregate prerequisites, so that I can isolate
real product or governance failures even when a broad local runner is under
pressure.

**Independent Test**: Each focused gate used by the Controls boundary refactor
can be invoked directly, reports its own duration and verdict, and does not
depend on broad aggregate work unless that prerequisite is explicitly justified.
Contract checks fail if a focused gate is accidentally recoupled to broad work
or relies on stale build/restore assumptions that make its result misleading.

### User Story 3 - Reduce Governance False Positives (Priority: P2)

As a maintainer reviewing governance evidence, I want dependency, template, and
generated-product checks to distinguish intended content from real violations,
so that reports identify boundary problems rather than scanner weaknesses.

**Independent Test**: The known false-positive cases from the Controls boundary
refactor are represented as validation scenarios: package metadata containing
ordinary words must not be reported as forbidden dependencies, generated
sample-pack content must be allowed for the sample-pack profile while still
rejecting copied framework projects elsewhere, and generated inventories must
include product source and tests when a check claims generated products
exercise public guidance.

### User Story 4 - Catch Stale Boundary Evidence Earlier (Priority: P2)

As a maintainer preparing merge evidence, I want stale boundary scans to cover
active-tree ownership documentation, source, tests, package metadata,
capabilities, templates, and generated guidance before late audit tasks are
marked complete, so that removed packages and old ownership promises cannot
remain in active ownership paths unnoticed.

**Independent Test**: A staged stale-reference scenario is detected before the
final evidence audit. The report names every active stale reference, separates
expected migration guidance from forbidden active ownership, and requires
deletion or update evidence for removed package source, tests, package
references, and capability entries.

### Edge Cases

- Broad verification may fail before tests start because the local runner
  cannot create required processes, threads, sockets, or runtime instances; the
  aggregate must fail with an environment-failure verdict in this case.
- Process-health preflight may detect clearly insufficient runner health before
  broad verification starts; the aggregate must fail fast with an
  environment-failure verdict in this case.
- Runner environments may need explicit threshold overrides; readiness evidence
  must report the default threshold, override value, and reason for each
  overridden preflight rule.
- Repeated runner warnings may appear before successful target execution and
  must not obscure later actionable failures.
- Missing runner bootstrap packages may block validation before feature checks
  can start.
- Focused gates may need a small prerequisite build, but the dependency must be
  intentional, visible, and tested.
- Generated sample-pack profiles may intentionally include sample content that
  ordinary generated products must reject.
- Migration guidance may mention removed legacy packages, while active source,
  tests, metadata, generated guidance, and architecture promises must not keep
  treating those packages as current ownership.
- Historical specs and intentional migration documents may mention removed
  packages without blocking readiness, provided active ownership docs,
  metadata, source, tests, templates, and generated guidance no longer treat
  those packages as current.
- A fresh runner may be required to make the final broad aggregate result
  authoritative after local process-health degradation; final readiness must
  wait for a healthy broad pass after an aggregate environment failure.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Broad verification MUST record a process-health preflight before
  starting high-pressure aggregate work, including available memory, process
  count, zombie process count, thread limits, file descriptor limits, and other
  runner signals needed to judge whether the environment can produce an
  authoritative result.
- **FR-001a**: If process-health preflight identifies clearly insufficient
  runner health for broad verification, the aggregate MUST stop before
  high-pressure work starts and return an environment-failure verdict.
- **FR-001b**: Process-health preflight MUST use repository-owned default
  thresholds for fail-fast decisions and MUST report any explicit threshold
  override with the overridden rule, override value, and reason.
- **FR-002**: Broad verification MUST distinguish product failures from
  environment failures. A failure that occurs while starting test
  infrastructure MUST fail the aggregate with an environment-failure verdict
  and MUST be reported as non-authoritative product evidence unless a product
  check actually ran and failed.
- **FR-003**: Broad verification logs MUST include a concise verdict category,
  the failing stage, relevant health diagnostics, and a recommended re-run
  environment when the result is degraded by local runner conditions.
- **FR-004**: A lightweight bootstrap validation MUST confirm that runner
  dependencies required for local validation are present and restorable before
  feature gates depend on them.
- **FR-005**: Repeated runner bootstrap warnings MUST be classified separately
  from target failures so that successful target execution is not hidden behind
  non-actionable warning noise.
- **FR-006**: Focused validation gates MUST remain first-class entry points for
  package surface, transcript, catalog, interaction, rendering, dependency,
  template, generated-product, generated-guidance, template-drift, evidence
  graph, and evidence audit checks.
- **FR-007**: Focused validation gates MUST NOT depend on broad aggregate
  targets unless that dependency is explicitly documented and protected by a
  command-contract test.
- **FR-008**: Focused gate diagnostics MUST make stale build or restore
  assumptions visible, including the affected gate and the action needed to
  produce a valid result.
- **FR-009**: Governance dependency checks MUST inspect structured dependency
  declarations or anchored dependency syntax rather than arbitrary substring
  matches for package or project names.
- **FR-010**: Generated product scanners MUST be profile-aware, allowing
  intended sample-pack content for sample-pack profiles while rejecting copied
  framework implementation projects in ordinary generated products.
- **FR-011**: Generated product inventories MUST include the generated product
  source and tests needed to prove that public guidance is exercised, not only
  the outer file list.
- **FR-012**: Stale boundary scans MUST cover active-tree ownership
  documentation, governance memory, architecture documentation, active source
  files, active tests, package metadata, capability metadata, generated
  guidance, template fragments, and readiness evidence before final audit
  completion is accepted.
- **FR-012a**: Stale boundary scans MUST distinguish forbidden active ownership
  references from intentional historical or migration references to removed
  packages.
- **FR-013**: Removed package checks MUST require explicit evidence that active
  source files, tests, package references, capability entries, and public
  ownership claims have been deleted or updated.
- **FR-014**: Evidence reports MUST separate environment failures from product
  failures, preserve focused passing evidence, and avoid presenting degraded
  broad aggregate runs as successful product proof.
- **FR-015**: Validation failures MUST identify the affected target, rule,
  generated profile, package reference, stale file, or readiness evidence path
  so maintainers can act without reverse-engineering the scanner.
- **FR-016**: The follow-up MUST avoid changing Controls product behavior,
  public control APIs, or package ownership unless such a change is directly
  required to make process evidence accurate.
- **FR-017**: Final readiness guidance MUST state when maintainers should rerun
  broad verification in a fresh shell, fresh container, or CI runner before
  treating aggregate evidence as final.
- **FR-018**: After any broad aggregate environment failure, final readiness
  MUST require a subsequent healthy broad aggregate pass. Focused passing
  evidence MAY support diagnosis but MUST NOT replace that final broad signal.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identity, product package content, package
  version, or active Controls ownership change is intended. Generated-product
  validation may change to improve evidence accuracy, but generated consumers
  must continue to treat Controls as the active path for controls, charts,
  graph views, and DataGrid, with legacy Charts only appearing in migration or
  deletion evidence.
- **Public contract impact**: No `.fsi` or product public API change is
  expected. Documentation, sample contracts for generated validation, readiness
  reports, and command-contract evidence are in scope.
- **State workflow impact**: Product state workflows are out of scope. Validation
  workflow and runner verdict state are in scope, including explicit
  environment-failure classification.
- **Layout/rendering impact**: Product layout and rendering behavior are out of
  scope. Rendering-focused validation targets and unsupported environment
  diagnostics are in scope because they determine whether visual evidence is
  authoritative.
- **Evidence obligations**: Required real evidence paths include
  `specs/012-fix-refactor-process/readiness/process-health.md`,
  `specs/012-fix-refactor-process/readiness/focused-gates.md`,
  `specs/012-fix-refactor-process/readiness/governance-scanners.md`,
  `specs/012-fix-refactor-process/readiness/stale-boundary-scan.md`,
  `specs/012-fix-refactor-process/readiness/generated-product-validation.md`,
  `specs/012-fix-refactor-process/readiness/bootstrap-runner.md`,
  `specs/012-fix-refactor-process/readiness/verification-verdicts.md`,
  `specs/012-fix-refactor-process/readiness/evidence-graph.md`, and
  `specs/012-fix-refactor-process/readiness/evidence-audit.md`.
- **Unsupported scope**: Reworking the Controls boundary, restoring the legacy
  Charts package, adding release publishing automation, changing external CI
  providers, automatically migrating external applications, and broad runtime
  performance tuning are out of scope.
- **Build-target impact**: `Verify`, `Ci`, focused check targets,
  `DependencyReport`, `TemplateCheck`, `GeneratedProductCheck`,
  `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, and
  `EvidenceAudit` may change. `Dev` and `PackLocal` should change only if
  bootstrap validation or dependency reporting requires it.

## Success Criteria *(mandatory)*

- **SC-001**: A maintainer can see a process-health preflight summary for a
  broad verification run within 30 seconds of starting the run.
- **SC-001a**: When preflight detects clearly insufficient runner health, broad
  verification stops before launching high-pressure validation work and reports
  an environment-failure verdict.
- **SC-001b**: Every preflight fail-fast decision identifies the repository
  default threshold that failed or the explicit override that changed the
  decision.
- **SC-002**: When a broad run fails before product checks execute, the final
  report classifies the result as an environment failure and lists at least
  three concrete health or startup diagnostics.
- **SC-003**: At least 90% of focused validation gates used by the Controls
  boundary refactor can run without invoking a broad aggregate target.
- **SC-004**: The known dependency substring, sample-pack profile, and generated
  inventory false-positive cases are covered by validation scenarios that pass
  for intended content and fail for real boundary violations.
- **SC-005**: Generated-product validation reports include source and test
  evidence for every public behavior the report claims is exercised.
- **SC-006**: A seeded stale active-ownership reference in governance memory,
  architecture documentation, active source, active tests, package metadata,
  capability metadata, template fragments, or generated guidance is reported
  before final audit completion.
- **SC-007**: Removed package evidence identifies deletion or update status for
  source, tests, package references, capability entries, and active ownership
  documentation in one readiness report.
- **SC-008**: Final readiness output clearly states whether broad aggregate
  evidence is authoritative, failed because of environment conditions, or
  waiting for a fresh-run confirmation.
- **SC-009**: If broad aggregate verification previously failed for environment
  reasons, final readiness remains blocked until a later healthy broad run
  passes.

## Assumptions

- The local report is the source of truth for the follow-up problems because
  the linked GitHub `master` page was not accessible from this environment.
- The follow-up targets process reliability, validation accuracy, and readiness
  reporting rather than new user-facing control capabilities.
- Focused gates remain the preferred local isolation path, while broad
  aggregate verification remains required for final confidence on a healthy
  runner.
- Migration guidance may mention removed legacy packages, but active ownership
  claims must reflect current package boundaries.
- Historical and migration materials may retain removed-package references when
  they are clearly not active ownership guidance.

## Key Entities

- **Process Health Snapshot**: A preflight record that summarizes runner
  capacity and conditions before broad verification starts.
- **Verification Verdict**: The final classification for a validation run,
  including product failure, environment failure, success, or degraded result.
- **Focused Gate**: A directly invocable validation target with its own
  membership, prerequisites, duration, logs, and verdict.
- **Governance Scanner Rule**: A validation rule that inspects dependency,
  template, generated-product, or stale-boundary evidence and reports
  actionable violations.
- **Generated Product Profile**: A generated project shape with profile-specific
  expectations for allowed content, package references, source markers, and
  tests.
- **Stale Boundary Reference**: Any active source, test, ownership
  documentation, metadata, template, or generated-guidance reference that
  contradicts the intended package ownership boundary.
- **Removed Package Evidence**: Readiness evidence proving that a removed
  package is absent from active ownership paths while remaining migration
  guidance is intentional.
