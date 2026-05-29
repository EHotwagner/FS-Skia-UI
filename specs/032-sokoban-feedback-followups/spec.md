# Feature Specification: Sokoban Feedback Follow-ups

**Feature Branch**: `032-sokoban-feedback-followups`  
**Created**: 2026-05-29  
**Status**: Draft  
**Input**: User description: "Mailbox/2026-05-29T21-05-37+0200-sokoban-demo-fs-skia-ui-feedback.md"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Readable Default Text Evidence (Priority: P1)

A generated-app author captures evidence screenshots for a demo that uses default text nodes for HUD labels, status messages, or instructions. The captured output shows recognizable glyphs instead of solid blocks or placeholder boxes, so reviewers can inspect the screenshot without requiring custom font setup.

**Independent Test**: Create an evidence screenshot containing default text, inspect the captured image, and confirm the text contains visible glyph-shaped coverage rather than only filled rectangles or tofu-style placeholder boxes.

### User Story 2 - Prove Persistent Interactive Launch Without Manual Closing (Priority: P1)

A generated-app author needs to satisfy the persistent interactive-window evidence gate in an automated or agent-run environment. They can use documented generated-app behavior to launch the real persistent host, request a close through the app workflow, and record an accepted interactive-window exit result without substituting a bounded evidence-only run.

**Independent Test**: Run a generated demo through its persistent launch evidence workflow and verify the resulting evidence record reports a real interactive-window launch, opened window, dispatched or emitted close action, clean exit path, and accepted status.

### User Story 3 - Discover Consumer API Shape Before Coding (Priority: P2)

A consumer author building an FS.Skia.UI demo can find a compact reference for keyboard keys, viewer host responsibilities, viewer effects, and scene node construction without reflecting over installed assemblies or searching framework internals.

**Independent Test**: Starting from generated consumer guidance only, identify the supported key cases, host callbacks, viewer effects, and common scene nodes needed for a simple keyboard-controlled demo.

### User Story 4 - Prepare Readiness Evidence Before Audit Failures (Priority: P2)

A Spec Kit feature author can see the required readiness files, required terms, and correct feature-scoped directory before running the evidence audit, reducing trial-and-error cycles caused by missing or misplaced evidence files.

**Independent Test**: During task planning for a generated-app feature, locate a readiness contract that names each required evidence file, its required content terms, and the directory the audit reads.

### User Story 5 - Avoid Task Graph Validator Gotchas (Priority: P3)

A Spec Kit task author can avoid known task-title trigger phrases and dependency-file formatting mistakes before running the graph validator.

**Independent Test**: Read the task-generation guidance and identify at least two examples of title wording or dependency formatting that would create unintended validation failures.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Default text in evidence screenshot captures MUST render as recognizable glyphs on supported Linux desktop hosts with common installed Latin fonts.
- **FR-002**: The framework MUST provide a deterministic fallback behavior for default text when no matching system font is available, so default HUD text does not become solid blocks or unreadable placeholder boxes.
- **FR-003**: Validation MUST include a screenshot-based capability check that fails when default text produces no glyph-shaped coverage in the capture path.

For default-text screenshot validation, "recognizable glyph coverage" means the captured text region contains non-rectangular foreground coverage with multiple interior/background transitions and is not classified as solid-block, tofu-box-only, or undecodable screenshot output by the validation helper. Exact pixel thresholds are owned by the default-text glyph capture contract and must be reported in readiness evidence.

- **FR-004**: Generated host guidance MUST describe how an app-level close request is translated into a real window close outcome for generated demos.
- **FR-005**: Generated app behavior MUST support a user-initiated or app-confirmed close path that can end a persistent interactive-window session cleanly.
- **FR-006**: The evidence guidance MUST distinguish accepted persistent interactive-window launch evidence from bounded screenshot or evidence-only substitutions.
- **FR-007**: Generated-app guidance MUST provide a CI-friendly recipe for proving persistent interactive launch, including first-frame confirmation, close request, exit-path evidence, and failure classification.
- **FR-008**: Consumer-facing guidance MUST include a compact API map covering keyboard key names, viewer host responsibilities, viewer effects, adapter commands, and common scene nodes needed for generated demos.
- **FR-009**: Consumer-facing guidance MUST warn authors to specify explicit fonts when they need brand or typography guarantees beyond the default text behavior.
- **FR-010**: Readiness guidance MUST name the feature-scoped readiness directory used by the authoritative audit and distinguish it from repository-level evidence output directories.
- **FR-011**: Readiness guidance MUST list the required readiness files and mandatory terms for governance risk levels, aggregate hang diagnostics, runtime limitations, and supported-host persistent launch evidence.
- **FR-012**: Task-generation guidance MUST document known task-title trigger phrases that can create unintended required-skill or graph-validation outcomes.
- **FR-013**: Task-generation guidance MUST document the required dependency-file shape and indentation rules so malformed dependency maps are caught before graph validation.
- **FR-014**: Follow-up guidance MUST classify each item as framework behavior, generated-app guidance, Spec Kit guidance, or consumer-author mistake so the backlog remains accurately scoped.
- **FR-015**: All updated guidance MUST be discoverable before implementation begins for a generated-app feature, not only after a failed validation or audit run.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package identities are expected to remain unchanged. Package contents may change if default text behavior, generated templates, or shipped consumer guidance are updated. Package versions may need normal preview bumps during release packaging. Generated package consumers are in scope because generated-app guidance and host close behavior are affected.
- **Public contract impact**: Public APIs are not expected to change for the baseline work because the feedback indicates close and input primitives already exist. If implementation adds a new public launch or cancellation contract, public signatures, sample contracts, and surface baselines become in scope.
- **State workflow impact**: Generated app state workflow and host-boundary effect interpretation are in scope where close-confirmed model state must lead to an actual window close effect. Core demo reducers should remain pure.
- **Layout/rendering impact**: Rendering and screenshot evidence are in scope for default text glyph readability in the capture path. Broader layout behavior, charting, DataGrid, Vulkan support expansion, and unsupported-platform behavior are out of scope except for documenting current limitations.
- **Evidence obligations**: Required real evidence paths include `specs/032-sokoban-feedback-followups/readiness/default-text-glyph-capture.md`, `specs/032-sokoban-feedback-followups/readiness/interactive-window-close-evidence.md`, `specs/032-sokoban-feedback-followups/readiness/consumer-guidance-scan.md`, `specs/032-sokoban-feedback-followups/readiness/readiness-contract-scan.md`, and `specs/032-sokoban-feedback-followups/readiness/task-guidance-scan.md`.
- **Unsupported scope**: Mobile, browser, macOS support expansion, release publishing, broad visual redesign, new game features, and replacing Spec Kit evidence governance are out of scope.
- **Build-target impact**: `Dev`, `Verify`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, and `EvidenceAudit` may need validation coverage. `PackLocal`, `Ci`, `DependencyReport`, and `TemplateDrift` should change only if touched artifacts require existing validation updates.

## Success Criteria *(mandatory)*

- **SC-001**: On a supported Linux desktop host with common Latin fonts, 100% of default-text screenshot capability checks show recognizable glyph coverage rather than solid blocks or placeholder-only text.
- **SC-002**: A generated demo can produce accepted persistent interactive-window launch evidence, including clean exit-path evidence, in under 60 seconds without manual window closing.
- **SC-003**: Guidance scans verify that the required readiness evidence files, required terms, and feature-scoped readiness directory are discoverable before the first evidence audit run.
- **SC-004**: Generated guidance contains a compact API map covering keyboard keys, host responsibilities, viewer effects, adapter commands, and common scene nodes for a simple generated demo.
- **SC-005**: Task-generation guidance documents at least two known task-title trigger pitfalls and the required dependency-file formatting rules before graph validation.
- **SC-006**: Guidance scans for generated apps and repository workflows find all five follow-up areas: default text, interactive close evidence, consumer API map, readiness contract, and task validator pitfalls.

## Assumptions

- The mailbox report accurately reflects behavior observed on a supported Linux desktop host during a full generated-app workflow.
- Existing input dispatch and close-window primitives are available for generated-app close evidence unless planning discovers a contract gap.
- The primary consumer audience is generated demo authors using FS.Skia.UI through the repository templates and Spec Kit workflow.
- Readability validation can use deterministic screenshot evidence rather than requiring subjective manual review for every run.
