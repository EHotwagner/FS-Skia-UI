# Feature Specification: Bomberman Demo Feedback Follow-ups

**Feature Branch**: `029-bomberman-demo-feedback`  
**Created**: 2026-05-29  
**Status**: Draft  
**Input**: User description: "Mailbox/2026-05-29T11-24-45+0200-bomberman-demo-fs-skia-ui-feedback.md"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Reliable Evidence Commands (Priority: P1)

As a generated-app maintainer, I need evidence graph and verification commands to produce clean, reviewable results from a normal checkout so that readiness artifacts are not blocked by file permissions or corrupted text logs.

**Independent Test**: From a fresh generated app checkout, run the documented evidence graph and verification workflows. The graph workflow completes without requiring manual file-mode repair, and the verification log is valid readable text without embedded NUL bytes.

### User Story 2 - Truthful Screenshot Evidence (Priority: P1)

As a reviewer, I need generated screenshot evidence commands to attempt the real screenshot capability before reporting unsupported status so that working host support is not hidden by a fallback report.

**Independent Test**: Run a generated screenshot evidence command on a host where screenshot capture is available. The resulting report proves that a real capture was attempted and, when successful, records a non-blank screenshot artifact. Unsupported results include evidence that the real capability path was probed first.

### User Story 3 - Easier Generated Game Wiring (Priority: P2)

As a generated game app author, I need a standard way to connect pure application state transitions to viewer events and effects so that generated apps avoid repetitive host adapter code and keep pure app behavior separate from viewer or file work.

**Independent Test**: Create or inspect a generated game app that uses the common wiring path. The app can launch persistently, process key and tick events, render frames, and keep application effects distinct from host-side work.

### User Story 4 - Clearer Scene and Layout Authoring (Priority: P3)

As an app developer writing scene and layout evidence, I need common record-heavy authoring points to be easy to disambiguate so that examples and generated code avoid surprising type inference errors.

**Independent Test**: Follow the generated-app guidance for scene and layout helpers. The examples show clear construction or annotation patterns for overlapping fields such as coordinates, dimensions, diagnostics, state, and positions.

### Edge Cases

- Evidence graph commands are invoked from a checkout where executable file modes were not preserved.
- Verification output is redirected to a readiness log and later reviewed as text.
- Screenshot capture is unavailable on the host after the real capability path is attempted.
- A generated app has pure gameplay effects and viewer effects with similar names or timing.
- Scene, layout, and evidence records share common field names in nearby helper code.

## Requirements *(mandatory)*

### Change Classification

- **Tier**: Tier 1 (contracted change)
- **Reason**: This feature may add or refine public `.fsi` surfaces in `FS.Skia.UI.SkiaViewer`, `FS.Skia.UI.Testing`, `FS.Skia.UI.Elmish`, `FS.Skia.UI.Scene`, or `FS.Skia.UI.Layout`, and changes generated consumer behavior.
- **Required evidence**: `.fsi` contract review, FSI transcript evidence, semantic tests, package surface baselines for public additions, generated product validation, and named readiness artifacts.

### Functional Requirements

- **FR-001**: Documented evidence graph workflows MUST work from generated app checkouts without requiring users to manually repair script executable permissions.
- **FR-002**: Verification readiness logs MUST be reviewable as clean text artifacts and MUST NOT include embedded NUL byte blocks in normal passing or failing runs.
- **FR-003**: Generated screenshot evidence workflows MUST attempt real screenshot capture before reporting unsupported status.
- **FR-004**: Screenshot evidence reports MUST distinguish successful capture, unsupported host capability, and app-command implementation errors.
- **FR-005**: Unsupported screenshot reports MUST include reviewer-visible proof that the real capture capability was attempted or a clear rationale for why it could not be attempted.
- **FR-006**: Generated game app support MUST provide a standard pure-state-to-viewer wiring path that covers initialization, key mapping, tick mapping, scene rendering, host update adaptation, and persistent launch behavior.
- **FR-007**: The standard wiring path MUST preserve the boundary between pure application update results and host-side viewer, file, or native work.
- **FR-008**: Consumer guidance MUST show how to avoid ambiguous record inference around scene, layout, and evidence helper boundaries.
- **FR-009**: Common scene and layout authoring paths SHOULD offer discoverable construction or helper patterns for coordinates, dimensions, diagnostics, state, and positions.
- **FR-010**: Readiness evidence MUST allow reviewers to verify that the Bomberman feedback items were addressed without relying on synthetic success artifacts.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package identities are not expected to change. Package contents may change if generated-app wiring helpers or screenshot evidence helpers are added to the active framework packages, and generated package consumers are expected to change through template and guidance updates. No charts, graph, or DataGrid package migration is in scope.
- **Public contract impact**: Public signatures may change if a reusable generated-app wiring helper or scene/layout construction helpers are exposed. Documented sample contracts and generated guidance are expected to change. Surface baselines must be reviewed if any public helper is added.
- **State workflow impact**: Stateful workflow and interpreter behavior are in scope for generated-app wiring only. The feature must preserve pure application update behavior while keeping viewer, file, screenshot, and native work at host boundaries.
- **Layout/rendering impact**: Screenshot evidence behavior, visual evidence classification, and scene/layout authoring guidance are in scope. Renderer redesign, gameplay visuals, chart behavior, DataGrid behavior, Vulkan behavior, and new platform support are out of scope.
- **Evidence obligations**: Required real evidence paths are `specs/029-bomberman-demo-feedback/readiness/evidence-graph-invocation.md`, `specs/029-bomberman-demo-feedback/readiness/verify-log-cleanliness.md`, `specs/029-bomberman-demo-feedback/readiness/screenshot-evidence-probe.md`, `specs/029-bomberman-demo-feedback/readiness/generated-app-wiring.md`, and `specs/029-bomberman-demo-feedback/readiness/scene-layout-authoring.md`.
- **Unsupported scope**: New Bomberman gameplay features, release publishing, package distribution, browser or mobile screenshot capture, platform expansion, renderer replacement, and broad roadmap changes are out of scope.
- **Build-target impact**: `Verify`, `TemplateCheck`, `GeneratedGuidanceCheck`, `EvidenceGraph`, and `EvidenceAudit` may need changes. `Dev` may need generated-app behavior validation. `Ci` may aggregate the updated checks. `PackLocal`, `DependencyReport`, and `TemplateDrift` change only if package contents, dependencies, or template files are modified.

## Success Criteria *(mandatory)*

- **SC-001**: A fresh generated app can complete the documented evidence graph workflow with zero manual permission-fix steps.
- **SC-002**: At least three redirected `Verify` runs produce readiness logs that are valid text and contain zero embedded NUL bytes.
- **SC-003**: Screenshot evidence validation rejects generated commands that report unsupported status without first proving a real screenshot capability attempt.
- **SC-004**: On a host with screenshot support, a generated screenshot evidence command produces a non-blank screenshot artifact and a report identifying the capture source.
- **SC-005**: A generated game app can be wired through the standard host path with no app-specific boilerplate beyond pure initialization, update, view, key mapping, and tick mapping decisions.
- **SC-006**: Consumer-facing examples cover all identified ambiguous scene/layout record categories from the Bomberman feedback: coordinates, dimensions, diagnostics, state, and positions.
- **SC-007**: Reviewers can determine the status of each Bomberman feedback item from named readiness evidence in under 5 minutes.

## Assumptions

- The mailbox feedback describes framework and generated-app follow-up work for FS.Skia.UI, not changes to the Bomberman demo itself.
- Generated app support remains compatible with persistent interactive launch behavior.
- Unsupported screenshot status remains valid only when it follows a real capability probe or documented inability to probe.
- Real readiness evidence is required for successful validation; synthetic fixtures may be used only for negative rejection tests.

## Key Entities

- **Generated App**: A project created from the FS.Skia.UI template that consumes framework packages and readiness workflows.
- **Evidence Workflow**: A documented command path that produces reviewer-readable and machine-checkable readiness artifacts.
- **Screenshot Evidence Report**: A report that records screenshot capture status, capture source, artifact path when available, support classification, and proof of capability probing.
- **Viewer Host Wiring**: The reusable boundary that connects pure app state, messages, and scene rendering to viewer events, effects, and persistent launch behavior.
- **Scene/Layout Authoring Guidance**: Consumer-facing examples and helper patterns that reduce ambiguity around overlapping record fields.
