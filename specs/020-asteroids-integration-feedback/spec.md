# Feature Specification: Asteroids Integration Feedback

**Feature Branch**: `020-asteroids-integration-feedback`  
**Created**: 2026-05-27  
**Status**: Draft  
**Input**: User description: `Mailbox/2026-05-27-112035-asteroids-demo-fs-skia-ui-integration-analysis.md`

## Clarifications

### Session 2026-05-27

- Q: Should the feature include a repo-local layout/evidence capability skill for generated game HUD/readability work? -> A: Yes; add a required layout/evidence skill for planning, task generation, and implementation work that touches generated game layout readability or scene evidence.
- Q: Should layout evidence be a public framework contract or generated-sample guidance only? -> A: Public framework contract for layout evidence and generated-sample validation.
- Q: What repo-local skill id should task metadata use for layout/evidence work? -> A: `fs-skia-layout-evidence`.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Keep Game HUD Readable (Priority: P1)

A generated game author or sample consumer runs a playable graphical demo and sees score, lives, wave, status, and gameplay content arranged so that important text remains readable during normal play and after window resizing.

**Independent Test**: Run a generated or sample game scene at standard and small supported window sizes. Verify that HUD/status text occupies a reserved region, active gameplay entities stay outside that region unless intentionally layered, and the player can read score/status information without overlap.

### User Story 2 - Discover Public Scene And Host Contracts (Priority: P1)

A library consumer adds explicit public signatures or tests around a generated graphical app and can identify the intended scene-returning type, generated host type, and update entry points without inspecting compiled package metadata.

**Independent Test**: Starting from generated guidance and public docs only, write a small app-owned signature and test surface that exposes a scene-producing view, the generated app host, and the app update function. Verify the names are unambiguous and the guidance prevents accidental binding to similarly named framework helpers.

### User Story 3 - Validate Layout-Sensitive Scene Evidence (Priority: P2)

A framework maintainer or generated app author captures evidence through the public framework contract for a graphical sample and can tell whether the evidence proves only deterministic rendering metadata or also proves readable, non-overlapping layout for HUD and gameplay regions.

**Independent Test**: Produce evidence for a generated or sample game scene with HUD text and gameplay entities. Verify that the evidence explicitly reports HUD region, gameplay region, and text bounds or equivalent layout facts, and that a scene with HUD/content overlap fails the layout-readability check.

### User Story 4 - Separate Benign Host Warnings From Real Failures (Priority: P3)

A developer launching a graphical demo sees non-fatal desktop host warnings classified as environment noise when the app remains usable, while real launch, layout, or rendering failures remain visible and actionable.

**Independent Test**: Launch a graphical demo in an environment that emits known non-fatal host module warnings while still presenting a usable window. Verify that readiness output identifies the warnings as non-fatal and does not hide true layout, launch, or rendering failures.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Generated game samples and guidance MUST reserve a dedicated HUD/status region separate from the primary gameplay region.
- **FR-002**: Generated game samples and guidance MUST define how gameplay entities remain within the gameplay region when a HUD/status region is present.
- **FR-003**: Generated or sample game validation MUST include at least one small-window or constrained-size layout scenario that checks HUD readability.
- **FR-004**: Layout validation MUST detect when score, lives, wave, status text, or equivalent HUD information overlaps each other or overlaps active gameplay content.
- **FR-005**: Public guidance MUST consistently name the consumer-facing scene-returning type for functions that expose rendered scene values.
- **FR-006**: Public guidance MUST consistently name the intended generated app host type when app authors expose host values from app-owned signatures.
- **FR-007**: Test guidance MUST show how to avoid ambiguous unqualified update calls when framework namespaces and app modules expose common reducer names.
- **FR-008**: Scene evidence guidance MUST distinguish deterministic metadata/hash evidence from evidence that proves readable, non-overlapping user-facing layout.
- **FR-009**: The framework MUST expose or document a public layout-evidence contract sufficient to report approximate bounds for text and primary visual regions used by generated game samples.
- **FR-010**: Public layout evidence MUST report the expected HUD region, gameplay region, and relevant text bounds or an explicit unsupported reason.
- **FR-011**: Generated validation MUST fail when a sample claims layout readability while required HUD/gameplay bounds are missing, unsupported without disclosure, or overlapping.
- **FR-012**: Readiness output MUST classify known non-fatal desktop host warnings separately from launch, rendering, layout, and package failures.
- **FR-013**: Readiness guidance MUST state that benign host warnings do not by themselves invalidate a successful usable launch.
- **FR-014**: Existing deterministic rendering evidence MUST remain usable for render consistency, while not being treated as proof of readable HUD layout unless layout facts are included.
- **FR-015**: The feature MUST preserve current generated-game playability and evidence workflows while adding layout-readability checks and clearer guidance.
- **FR-016**: The project MUST include a repo-local layout/evidence capability skill named `fs-skia-layout-evidence` for generated game HUD readability, scene layout evidence, public contract guidance, and host-warning classification work.
- **FR-017**: Planning and task generation MUST require `fs-skia-layout-evidence` in task metadata for any task that changes generated game layout, HUD readability validation, scene evidence claims, generated guidance for scene/host/update names, or benign host-warning classification.

### Edge Cases

- HUD text fits at the default size but overlaps gameplay after the window is made smaller.
- Long status text wraps or expands into the gameplay region.
- Multiple HUD labels are individually present but collide with each other.
- Gameplay entities wrap around the full scene instead of the gameplay-only region.
- Evidence can render a deterministic scene hash while the real viewer still shows unreadable text overlap.
- A host emits non-fatal desktop module warnings during a successful launch.
- A consumer opens a framework namespace that exports a common reducer name and accidentally calls the wrong update function.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package contents and generated package consumers may change because generated sample guidance, validation evidence, and consumer-facing docs are affected. Package identities are not expected to change.
- **Public contract impact**: Documented public APIs, sample contracts, generated app signatures, and surface baselines must change or be explicitly reviewed because layout evidence is a public framework-facing contract. Existing public names for scene values and generated app host values must be reviewed for guidance consistency.
- **State workflow impact**: Stateful gameplay behavior is not the primary target, but validation workflows and evidence production may change to include layout regions, text bounds, and warning classification.
- **Layout/rendering impact**: Layout, rendering evidence, screenshots or visual-readback claims, and unsupported environment diagnostics are affected. Chart, DataGrid, and unrelated controls are out of scope.
- **Evidence obligations**: Required real evidence paths are `specs/020-asteroids-integration-feedback/readiness/hud-layout-readability.md`, `specs/020-asteroids-integration-feedback/readiness/public-contract-guidance.md`, `specs/020-asteroids-integration-feedback/readiness/layout-evidence.md`, `specs/020-asteroids-integration-feedback/readiness/host-warning-classification.md`, `specs/020-asteroids-integration-feedback/readiness/generated-validation.md`, and `specs/020-asteroids-integration-feedback/readiness/evidence-audit.md`.
- **Unsupported scope**: Out of scope are rewriting the Asteroids game mechanics, adding a new game engine, changing unrelated controls/chart/DataGrid behavior, release automation, marketplace distribution, and guaranteeing layout proof where the host cannot expose required facts beyond explicit unsupported diagnostics.
- **Build-target impact**: `Verify`, generated `Test`, generated product checks, `GeneratedGuidanceCheck`, `TemplateCheck`, `EvidenceGraph`, and `EvidenceAudit` may need updates. `Dev`, `Ci`, `PackLocal`, `DependencyReport`, and `TemplateDrift` change only if they aggregate or validate the affected workflows. Task metadata validation must recognize `fs-skia-layout-evidence` when applicable.

### Key Entities

- **HUD Region**: A reserved visual area containing score, lives, wave, status, and similar game state text that must remain readable.
- **Gameplay Region**: The area where active game entities move, wrap, collide, and render without unintentionally covering HUD information.
- **Scene Layout Evidence**: Public framework evidence that reports layout facts such as HUD region, gameplay region, text bounds, overlap status, and unsupported facts.
- **Public Contract Guidance**: Consumer-facing documentation and generated examples that name intended scene, host, and update entry points.
- **Host Warning Classification**: A readiness classification that separates benign environment warnings from actionable launch, rendering, layout, or package failures.
- **Layout Evidence Skill**: The repo-local `fs-skia-layout-evidence` capability guide that implementation agents must load for generated game layout readability, scene layout evidence, public contract guidance, and benign host-warning classification tasks.

### Assumptions

- The initial motivating sample is a generated or consumer-owned Asteroids-style game, but the framework outcome should apply to generated graphical game samples generally.
- Default supported layout validation size is 1280x720. Constrained small-window validation size is 640x480 unless a generated profile documents a stricter supported minimum in its template guidance.
- Approximate bounds are sufficient for layout-readability validation when exact font metrics are unavailable, provided the approximation is deterministic and conservative.
- Metadata/hash evidence remains valuable for deterministic rendering but is not sufficient by itself to prove readable text layout.
- Known non-fatal host warnings may be documented by message class rather than treated as errors when a usable window and valid evidence are produced.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of generated game layout-readability checks fail when HUD/status text overlaps gameplay content or other HUD/status text in covered validation scenarios.
- **SC-002**: Generated game samples keep HUD/status text readable at the default supported size and at one documented small-window size in every supported host validation run.
- **SC-003**: A consumer can identify the intended scene type, generated host type, and app update qualification guidance from public docs or generated examples in under 10 minutes without inspecting compiled package metadata.
- **SC-004**: 100% of public layout evidence artifacts for generated game samples state whether they prove text/layout readability, deterministic rendering only, or an unsupported layout-inspection condition.
- **SC-005**: 100% of generated validation reports include HUD region, gameplay region, and relevant text-bound status when layout readability is claimed.
- **SC-006**: Non-fatal host warning classifications reduce false readiness failures for known benign desktop module warnings to 0 while preserving failures for real launch, rendering, layout, and package issues.
- **SC-007**: The added layout-readability and guidance validation completes as part of generated validation in under 5 minutes on a prepared supported host.
- **SC-008**: 100% of tasks that modify generated game layout readability, layout evidence, public scene/host/update guidance, or host-warning classification list `fs-skia-layout-evidence` in their task metadata.
