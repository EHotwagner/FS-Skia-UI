# Feature Specification: Persistent Launch Evidence

**Feature Branch**: `021-persistent-launch-evidence`  
**Created**: 2026-05-27  
**Status**: Draft  
**Input**: User description: "Mailbox/2026-05-27-155722-space-invaders-demo-persistent-launch-and-evidence-report.md"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Prove Persistent GUI Launch (Priority: P1)

A generated game maintainer runs the generated app's readiness workflow on a supported desktop host and receives a machine-readable persistent-launch artifact that proves the real interactive window opened, reached a presented frame, accepted the expected close path, and records whether input dispatch was verified.

**Independent Test**: On a supported desktop host, run the generated readiness workflow for a graphical game sample and verify that it creates an accepted persistent-launch readiness artifact with `status=ok`, `mode=interactive-window`, `window-opened=true`, a recorded input-dispatch value, `exit-path=true`, and no unsupported-host classification when the window is actually visible.

### User Story 2 - Diagnose Capture Failures Honestly (Priority: P1)

A framework maintainer investigates a case where a user can see the persistent app window but automated observation fails, and the evidence report clearly distinguishes a launch failure, a render failure, an input verification gap, and a window-observation or capture failure.

**Independent Test**: Simulate or reproduce an observation failure while the app process stays alive, then verify that readiness evidence reports the blocked stage as observation or capture, preserves the command and host facts, and does not classify the environment as headless-only unless desktop prerequisites are absent or launch is impossible.

**Synthetic Evidence Disclosure**: Parser and classifier error-handling tests may use approved synthetic malformed persistent-launch artifacts for missing fields, invalid values, and contradictory pass claims. These fixtures validate rejection behavior only; they cannot satisfy supported-host persistent-launch readiness. The real-evidence replacement path is `specs/021-persistent-launch-evidence/readiness/window-observation-diagnostics.md`, backed by real launch, host, viewer, observation, and missing-fact diagnostics where available.

### User Story 3 - Keep Benign Host Warnings Non-Blocking (Priority: P2)

A generated app maintainer sees common desktop module warnings during launch, but the readiness result still passes when the app opens and all required launch evidence is present.

**Independent Test**: Run the persistent-launch readiness workflow with known non-fatal desktop warning messages present and verify that the artifact records the warnings as benign noise without converting them into unsupported-host or failed-launch evidence.

### User Story 4 - Avoid Generated App Naming Collisions (Priority: P2)

A generated game developer opens framework capability namespaces in tests or samples and can still call the app-owned scene, host, and update functions unambiguously.

**Independent Test**: Review generated guidance and sample tests for a game app that imports framework capabilities, then verify that app-owned reducer, scene, and host references are qualified or named clearly enough to avoid collisions with framework functions.

## Requirements *(mandatory)*

### Change Classification

This is a **Tier 1 (contracted change)** because it adds or changes public viewer/testing contracts, generated template behavior, governance evidence requirements, and observable generated app readiness behavior.

Public API and package surface changes must update `.fsi` signatures, semantic tests, FSI transcripts, surface baselines, documentation, generated template content, and package/version review together.

### Functional Requirements

- **FR-001**: Generated graphical app readiness MUST include a persistent-launch evidence workflow that attempts the real interactive viewer path on supported desktop hosts and writes a machine-readable readiness artifact.
- **FR-002**: The persistent-launch artifact MUST include status, mode, command, window-opened result, first-frame or presented-frame result, input-dispatch result, exit-path result, blocked stage, classification, category, and message fields.
- **FR-003**: A passing supported-host persistent-launch artifact MUST indicate that the interactive window opened, a controlled exit path was exercised, and input dispatch was either verified or explicitly recorded as not verified without hiding that limitation.
- **FR-004**: The persistent-launch workflow MUST distinguish desktop prerequisite failures, process launch failures, first-frame or render failures, external observation failures, input verification failures, and controlled-exit failures.
- **FR-005**: Readiness diagnostics MUST NOT classify a host as headless-only solely because external title or window search tools fail to observe a window that the user can see.
- **FR-006**: Viewer diagnostics MUST state whether they describe generic host prerequisites, synthetic or probe facts, or a real attempted persistent launch.
- **FR-007**: Generated app guidance MUST keep deterministic gameplay/layout evidence separate from persistent-window launch evidence and MUST NOT present structural layout evidence as screenshot or visible-window proof.
- **FR-008**: Known benign desktop module warnings MUST be recorded in readiness output without failing the persistent-launch gate when required launch, render, and exit facts pass.
- **FR-009**: Generated app tests and documentation MUST use unambiguous app-owned names for the app scene, generated host, and reducer/update function when framework capability namespaces are also in scope.
- **FR-010**: Evidence audit guidance MUST make required readiness contract files and persistent-launch artifacts discoverable before the final audit task.
- **FR-011**: If a persistent-launch artifact cannot be produced automatically on a host with present desktop prerequisites, the readiness output MUST identify the missing observable facts and the exact blocked stage instead of reporting a generic failure.
- **FR-012**: The generated readiness workflow MUST preserve normal interactive app behavior; evidence-mode launch and controlled close behavior must be separate from the default user launch.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package contents may change for viewer, testing, generated template, and governance assets. Package identities should remain unchanged. Package versions must be reviewed if public contracts or packaged template content change.
- **Public contract impact**: Public viewer or testing contracts may change to expose persistent-launch evidence, viewer-native window facts, warning classification, or generated validation results. Any `.fsi` signatures, documented public APIs, sample contracts, and surface baselines affected by those outcomes must be updated together.
- **State workflow impact**: Generated apps must keep pure gameplay state workflows separate from viewer/evidence effects. Any persistent-launch evidence workflow that opens, observes, dispatches input, or closes a window must be represented as an explicit evidence operation rather than folded into ordinary game update behavior.
- **Layout/rendering impact**: Rendering and visual readiness diagnostics change because the feature adds supported-host persistent-window proof and clearer first-frame/window-observation classification. Deterministic layout evidence remains structural proof, not visible-window proof.
- **Evidence obligations**: Required real evidence paths are `specs/021-persistent-launch-evidence/readiness/persistent-launch-evidence.md`, `specs/021-persistent-launch-evidence/readiness/window-observation-diagnostics.md`, `specs/021-persistent-launch-evidence/readiness/host-warning-classification.md`, `specs/021-persistent-launch-evidence/readiness/generated-guidance.md`, and `specs/021-persistent-launch-evidence/readiness/evidence-audit.md`.
- **Unsupported scope**: This feature does not add new game mechanics, rewrite generated game reducers, guarantee automated visibility proof on hosts that cannot expose required window facts, introduce release automation, or change unrelated controls, charts, graph, or DataGrid behavior.
- **Build-target impact**: `Verify`, generated `Test`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateCheck`, `EvidenceGraph`, and `EvidenceAudit` must verify the new persistent-launch evidence contract or its generated guidance. `Dev`, `Ci`, `PackLocal`, `DependencyReport`, and `TemplateDrift` change only if aggregation, packaging, dependency, or template-drift checks require it.

## Success Criteria *(mandatory)*

- **SC-001**: On a supported desktop host where the generated app window is visible, the readiness workflow produces an accepted persistent-launch artifact in at least 95% of repeated runs without requiring manual file editing.
- **SC-002**: 100% of persistent-launch readiness artifacts include the required status, mode, command, window-opened, first-frame or presented-frame, input-dispatch, exit-path, blocked-stage, classification, category, and message fields.
- **SC-003**: In observation-failure cases with desktop prerequisites present and a process that remains alive, diagnostics identify observation or capture as the blocked stage instead of labeling the host headless-only.
- **SC-004**: Known benign desktop module warnings do not fail readiness when launch, first-frame/window, and exit facts pass.
- **SC-005**: Generated documentation and tests consistently use unambiguous app-owned scene, host, and update names, with zero unresolved naming-collision examples in generated guidance checks.
- **SC-006**: Final evidence audit passes only when persistent-launch evidence, layout evidence, warning classification, generated guidance, and required readiness contract files are present and internally consistent.

## Assumptions

- A supported desktop host has the session prerequisites needed to open a graphical window for the generated app.
- Some hosts may expose visible windows inconsistently to external observation tools; the readiness workflow should prefer viewer-owned facts where available and report unsupported facts explicitly where unavailable.
- Manual human observation can inform diagnosis, but merge readiness requires a structured artifact unless a later approved governance decision defines a manual-observation artifact format.

## Key Entities

- **Persistent Launch Artifact**: Machine-readable readiness record describing a real interactive viewer launch attempt, observed window facts, input-dispatch status, controlled-exit status, classification, and message.
- **Window Observation Result**: Evidence describing whether the app window was observed through reliable viewer or host facts, including missing facts and blocked stage when observation fails.
- **Host Warning Classification**: Readiness record that separates benign desktop warning noise from warnings that indicate launch, render, input, or package failure.
- **Generated App Naming Guidance**: Public guidance and sample expectations for app-owned scene, host, and update names when framework capability modules are in scope.
