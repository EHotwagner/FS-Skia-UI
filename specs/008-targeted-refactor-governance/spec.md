# Feature Specification: Targeted Refactor and Governance Diagnostics

**Feature Branch**: `008-targeted-refactor-governance`  
**Created**: 2026-05-15  
**Status**: Draft  
**Input**: User description: "Refactoring would be useful, but it should be targeted. A broad rewrite would be wasteful because the current structure is already understandable and tested. High-value refactors: split src/Lib/Library.fs internally while keeping src/Lib/Library.fsi stable; introduce small internal resource helpers for Vulkan handles; flatten deep bind nesting in VulkanHost.run with a result computation expression or initialization pipeline; split build.fsx by concern if FAKE loading supports it cleanly, otherwise group into named sections; convert GeneratedGuidanceCheck from substring checks to structured section checks; make TemplateDrift in scripts/template-drift.fsx more semantic; add diagnostics for Yoga fallback; review public record invariants and decide where helper constructors or validation-first APIs should be recommended."

## Change Classification

**Tier**: Tier 1 governance and observable diagnostics change with a constrained Tier 2 internal runtime refactor.

**Public API Impact**: The public `src/Lib/Library.fsi` signature is expected to remain stable. Any proposed public record constructor, validation API, or signature change discovered during the invariant review is out of scope for this feature unless it is captured as a separate follow-up specification. Existing public diagnostics may gain additional structured cases or messages for fallback reporting only if they can be expressed through the current public surface.

**Verification Approach**: Preserve existing public surface and package baselines, prove existing semantic behavior remains intact, add focused failure and governance tests for resource cleanup, startup failure paths, structured guidance checks, semantic drift alignment, Yoga fallback diagnostics, and public record invariant recommendations.

## Clarifications

### Session 2026-05-15

- Q: For native startup failure coverage, what test strategy should the spec require for proving cleanup after staged acquisition failures? → A: Mixed strategy: deterministic injectable acquisition tests for each resource category plus existing real native smoke coverage where available.
- Q: For Yoga fallback diagnostics, what should happen if the current public diagnostic surface cannot carry the required structured fallback information without changing `Library.fsi`? → A: Treat the public-surface gap as a blocker for Yoga diagnostic implementation and record a separate follow-up API proposal.
- Q: For the public record invariant review, what artifact should count as authoritative evidence that every exposed record was reviewed? → A: Structured readiness inventory with record name, invariant, decision, rationale, and follow-up ID when needed.
- Q: For semantic `TemplateDrift` validation, what should count as acceptable alignment evidence for a template-owned path change? → A: Same-diff alignment files plus active feature spec/plan/readiness evidence that names the changed path or affected feature area.
- Q: For `build.fsx` organization, what should be the acceptance rule for physically splitting the FAKE script by concern? → A: Attempt physical split, accept it only if `Dev`, `Verify`, and `Ci` load cross-platform; otherwise use named sections in one script with fallback evidence.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Review Runtime Internals Safely (Priority: P1)

As a library maintainer, I need the large runtime implementation to be separated into clear internal responsibility areas while preserving the public library contract, so native lifetime and rendering changes are easier to review without broad rewrite risk.

**Why this priority**: The current implementation is understandable and tested, but concentrated runtime responsibilities increase review risk around native resources and rendering. The first deliverable must reduce that risk without changing what consumers can call.

**Independent Test**: Can be tested by comparing public surface evidence before and after the change, running the existing semantic and smoke evidence, and confirming reviewers can inspect scene model, diagnostics, drawing, native resource, frame, screenshot, and viewer-host responsibilities independently.

**Acceptance Scenarios**:

1. **Given** an existing application uses the published library surface, **When** the refactor is complete, **Then** the same public contract remains available with no required application code changes.
2. **Given** a reviewer inspects a rendering or native lifetime change, **When** they open the affected implementation area, **Then** unrelated scene modeling, drawing, frame, screenshot, and host responsibilities are not mixed into the same review unit.
3. **Given** the public surface baseline is checked, **When** verification runs after the refactor, **Then** any change to the public contract is reported as a defect unless separately specified.

---

### User Story 2 - Audit Native Resource Startup and Cleanup (Priority: P2)

As a maintainer of the native rendering host, I need startup ordering and cleanup ownership to be explicit and testable, so failed initialization does not leak handles or hide which resource owns cleanup.

**Why this priority**: Native resource failure paths are costly to reason about when ownership is spread across nested startup code. Small ownership helpers and flatter startup flow make failure behavior auditable.

**Independent Test**: Can be tested by using deterministic injectable acquisition failures for each native resource category and confirming every acquired resource is cleaned up exactly once, startup reports the failing stage, successful startup still reaches the same rendered-frame behavior, and existing real native smoke coverage continues where available.

**Acceptance Scenarios**:

1. **Given** a native resource is acquired and a later startup step fails, **When** the host exits the failed startup path, **Then** the acquired resource is released exactly once and the failure names the stage that failed.
2. **Given** startup completes successfully, **When** the host renders and shuts down, **Then** normal frame and shutdown behavior remains equivalent to the pre-refactor behavior.
3. **Given** a reviewer traces startup, **When** they inspect the startup flow, **Then** initialization order, failure propagation, and cleanup obligations are readable without following deeply nested success/failure branches.

---

### User Story 3 - Strengthen Build and Template Governance Checks (Priority: P3)

As a repository maintainer, I need build-script organization, generated-guidance validation, and template-drift validation to describe their responsibilities semantically, so governance failures point to the missing alignment work rather than shallow text matches.

**Why this priority**: V2 template governance relies on generated guidance and drift checks. Shallow substring checks and path-only drift signals are useful but do not give enough confidence that future features will preserve required prompts and alignment obligations.

**Independent Test**: Can be tested with passing and failing governance fixtures that omit required sections, move deferred-scope content, break active/preset parity, change a template-owned path without same-diff alignment files or active feature spec/plan/readiness evidence naming the changed path or affected feature area, and prove any physical FAKE script split loads `Dev`, `Verify`, and `Ci` cross-platform or records fallback evidence for named sections in one script.

**Acceptance Scenarios**:

1. **Given** generated guidance is missing a required section or prompt, **When** guidance validation runs, **Then** it fails with the missing section or prompt named.
2. **Given** active guidance and preset guidance drift apart, **When** guidance validation runs, **Then** it reports the parity mismatch and the affected generated artifact class.
3. **Given** a template-owned path changes without same-diff alignment files, active feature spec/plan/readiness evidence naming the changed path or affected feature area, or an accepted deferral, **When** drift validation runs, **Then** it reports the changed path class and the required alignment class.
4. **Given** a physical concern-level FAKE script split is attempted, **When** `Dev`, `Verify`, and `Ci` fail to load cross-platform, **Then** the build entry script remains a single canonical script with path model, effects, interpreter, validation, governance, guidance, and target graph concerns grouped under clear section boundaries and fallback evidence recorded.

---

### User Story 4 - Diagnose Fallbacks and Public Record Invariants (Priority: P4)

As a library consumer and maintainer, I need fallback layout behavior and public record construction guidance to be explicit, so recoverable Yoga failures and invalid record states are visible instead of becoming silent runtime surprises.

**Why this priority**: The Yoga fallback path keeps rendering safe, but using it silently hides the reason layout changed. Public records are convenient, but maintainers need a clear decision on where free construction remains intended and where validation-first usage should be recommended.

**Independent Test**: Can be tested by forcing Yoga execution to fail, asserting that fallback layout still returns safe bounds with a structured diagnostic when the existing public diagnostic surface can carry it, verifying implementation stops at a follow-up API proposal when it cannot, and checking a structured readiness inventory that lists every public record with invariant, decision, rationale, and follow-up ID when needed.

**Acceptance Scenarios**:

1. **Given** Yoga execution fails for a recoverable layout case and the existing public diagnostic surface can carry fallback details, **When** layout falls back to the pure path, **Then** the caller can observe a structured diagnostic that identifies fallback use and the affected layout context.
2. **Given** fallback layout succeeds, **When** rendering continues, **Then** the UI receives safe deterministic bounds rather than an unreported failure.
3. **Given** a public record can be freely constructed, **When** maintainers review its invariant needs, **Then** the structured readiness inventory records the record name, invariant, decision, rationale, and follow-up ID when helper construction or a future validation-first public API proposal is recommended.

### Edge Cases

- A refactor accidentally changes `src/Lib/Library.fsi` or a surface baseline while intending to be internal only.
- A resource acquisition succeeds and the immediately following startup step fails before ownership is transferred.
- Cleanup is requested after partial initialization, after successful initialization, and after a repeated shutdown attempt.
- Startup failure reporting must not swallow the original native or host error.
- Build script concern splitting must not make FAKE loading brittle or create multiple competing entry points.
- Physical FAKE script splitting must be accepted only when `Dev`, `Verify`, and `Ci` load cross-platform; otherwise named sections in one canonical script are required with fallback evidence.
- Guidance validation must detect renamed sections, missing prompts, prompts placed only in deferred scope, and active/preset parity mismatches.
- Drift validation must distinguish a source-only change with same-diff active feature spec/plan/readiness evidence or an accepted deferral from an untracked template-owned change.
- Yoga fallback diagnostics must be emitted for recoverable Yoga execution failure without breaking pure fallback layout when the existing public diagnostic surface can carry them.
- If Yoga fallback diagnostics require a `Library.fsi` change, implementation must stop at safe fallback behavior and record a separate public API follow-up proposal instead of changing the signature in this feature.
- Public record invariant review must not turn documentation recommendations into accidental breaking API changes.
- Public record invariant evidence must fail review if any public record is missing from the structured readiness inventory or if a helper/validation-first recommendation lacks a follow-up ID.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The feature MUST preserve the public library signature and surface baseline unless a separately specified follow-up feature authorizes a public contract change.
- **FR-002**: The feature MUST separate the runtime implementation into clear internal responsibility areas for scene state, diagnostics, drawing, native resources, frame flow, screenshots, and viewer hosting.
- **FR-003**: The feature MUST keep existing consumer behavior, samples, and semantic tests equivalent for scenarios unaffected by the new diagnostics and governance checks.
- **FR-004**: The feature MUST introduce explicit ownership helpers or equivalent scoped ownership rules for native instance, device, surface, swapchain, command pool, fence, and staging-buffer resources.
- **FR-005**: The feature MUST prove through deterministic injectable acquisition failures that each native resource category releases all resources acquired before failure exactly once, with existing real native smoke coverage retained where available.
- **FR-006**: The feature MUST make native host startup order, failure propagation, and cleanup obligations readable as a sequence of named initialization stages.
- **FR-007**: The feature MUST attempt physical FAKE script organization by concern and accept it only if `Dev`, `Verify`, and `Ci` load cross-platform; otherwise it MUST organize one canonical build script with named concern sections and fallback evidence.
- **FR-008**: The feature MUST validate generated guidance by required section names, required prompts, deferred-scope placement, and active/preset parity instead of relying only on substring presence.
- **FR-009**: The feature MUST make template drift validation map changed path classes to required alignment classes and verify same-diff alignment files plus active feature spec/plan/readiness evidence that mentions either the changed path or the affected feature area.
- **FR-010**: The feature MUST report a structured diagnostic when recoverable Yoga execution failure causes fallback layout to be used and the existing public diagnostic surface can carry the diagnostic without changing `src/Lib/Library.fsi`.
- **FR-011**: The feature MUST preserve safe fallback layout bounds when Yoga fallback diagnostics are emitted.
- **FR-012**: The feature MUST record a separate follow-up API proposal if Yoga fallback diagnostics cannot be implemented through the existing public diagnostic surface without changing `src/Lib/Library.fsi`.
- **FR-013**: The feature MUST review every public record exposed by the library in a structured readiness inventory that records record name, invariant, decision, rationale, and follow-up ID when helper construction or validation-first public API work is recommended.
- **FR-014**: The feature MUST document any recommended follow-up public API work separately from this refactor so it does not enter the implementation by accident.
- **FR-015**: The feature MUST record real verification evidence for public surface stability, semantic behavior, native failure cleanup, guidance validation, drift validation, Yoga fallback diagnostics or follow-up deferral, and public record invariant review.

### Framework Governance Prompts *(mandatory for this repository)*

- **Package impact**: No package identity or package version change is expected. Package contents may change only through internal source organization and governance script organization, with generated consumer behavior preserved.
- **Public contract impact**: `src/Lib/Library.fsi`, documented public APIs, samples, and surface baselines are expected to remain stable. Yoga fallback diagnostics may add observable diagnostic content only through the existing diagnostic surface. If fallback diagnostics require a signature change, the change is deferred to a separate public API follow-up. Any new constructor or validation API recommendation from the public record review is also deferred to a separate feature.
- **State workflow impact**: The native viewer startup workflow changes internally to expose named initialization stages, explicit failure propagation, and cleanup ownership. Build workflow organization may change internally while preserving the same user-facing entry commands; physical FAKE script splitting is accepted only when `Dev`, `Verify`, and `Ci` load cross-platform.
- **Layout/rendering impact**: Rendering behavior should remain equivalent. Layout diagnostics change when Yoga execution fails and pure fallback layout is used only if the current public diagnostic surface can expose the structured fallback information without a signature change. Vulkan native resource ownership and frame startup/shutdown paths are in scope for refactor and failure evidence.
- **Evidence obligations**: Required evidence includes public surface baseline output, semantic test logs, native startup failure cleanup evidence, viewer smoke evidence where available, generated guidance check logs, template drift logs, Yoga fallback diagnostic tests or a follow-up API deferral record, and the structured public record invariant inventory under the feature readiness path.
- **Unsupported scope**: Broad rewrites, public API redesign, new rendering capabilities, new template packaging behavior, release validation, external repository split, and new distribution automation are out of scope.
- **Build-target impact**: `Dev`, `Verify`, and `Ci` must continue to pass and must load cross-platform if FAKE script concerns are physically split. If physical splitting is rejected, fallback evidence must show named sections in the single canonical script. `GeneratedGuidanceCheck` and `TemplateDrift` behavior must change. `TemplateCheck`, `DependencyReport`, `EvidenceGraph`, and `EvidenceAudit` should only change if needed to preserve the existing governance workflow after organization changes.

### Key Entities

- **Internal Responsibility Area**: A reviewable implementation concern such as scene state, diagnostics, drawing, native resources, frame flow, screenshot capture, or viewer hosting.
- **Native Resource Ownership Rule**: The documented owner, acquire point, transfer point, cleanup action, and failure behavior for one native handle or dependent resource group.
- **Startup Stage**: A named step in native viewer initialization with defined inputs, outputs, failure diagnostics, and cleanup obligations.
- **Guidance Contract**: The set of required generated-specification and generated-plan sections, prompts, deferred-scope placement rules, and active/preset parity expectations.
- **Drift Alignment Rule**: A mapping from a changed path class to same-diff alignment files and active feature spec/plan/readiness evidence that names the changed path or affected feature area, or to an accepted deferral record.
- **Fallback Diagnostic**: Structured information emitted when Yoga execution fails recoverably and the pure fallback layout path is used.
- **Record Invariant Decision**: The structured readiness inventory entry for a public record, including record name, invariant, decision, rationale, and follow-up ID when helper construction or a validation-first API should be specified later.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of public surface baseline checks either remain unchanged or identify only separately approved public-contract changes.
- **SC-002**: 100% of existing semantic tests and non-visual smoke checks that passed before the refactor continue to pass after the refactor.
- **SC-003**: Deterministic injectable native startup failure tests cover every owned resource category and show each acquired resource is released exactly once on failure and shutdown, with existing real native smoke evidence retained where available.
- **SC-004**: Review evidence maps every native startup stage to its acquired resources, cleanup owner, failure diagnostic, and transfer point.
- **SC-005**: Generated guidance validation catches 100% of seeded missing-section, missing-prompt, deferred-scope-placement, and active/preset-parity failures in governance fixtures.
- **SC-006**: Template drift validation catches 100% of seeded path-class changes that lack same-diff alignment files, active feature spec/plan/readiness evidence naming the changed path or affected feature area, or accepted deferral records.
- **SC-007**: 100% of recoverable Yoga execution failure tests return safe fallback bounds and either include a structured fallback diagnostic through the existing public surface or record a separate follow-up API proposal when that surface is insufficient.
- **SC-008**: 100% of public records exposed by the library appear in the structured readiness inventory with record name, invariant, decision, rationale, and follow-up ID when helper construction or public API changes are recommended.
- **SC-009**: Repository users retain one documented build entry command for fast, full, and automation verification, and any physical FAKE script split is accepted only when `Dev`, `Verify`, and `Ci` load cross-platform; otherwise fallback evidence documents named sections in one canonical script.
- **SC-010**: No feature evidence required by this specification is synthetic-only unless it is explicitly marked and justified under the repository synthetic evidence policy.

## Assumptions

- The current implementation and tests are valuable and should be preserved; this feature targets specific risk areas rather than replacing the runtime architecture.
- `src/Lib/Library.fsi` stability is a hard constraint for this feature.
- Existing diagnostics provide a suitable public path for reporting Yoga fallback use; if they do not, the new public API work will be deferred.
- FAKE script loading may make physical file splitting awkward; clearly named sections inside the entry script are acceptable only after a physical split attempt fails `Dev`, `Verify`, or `Ci` cross-platform loading and fallback evidence is recorded.
- Template packaging behavior from the V2 feature remains the baseline and is not expanded here.
- Real verification evidence is expected for changed behavior; synthetic-only evidence is not acceptable for merge readiness unless separately justified.
