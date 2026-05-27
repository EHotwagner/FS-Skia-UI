# Feature Specification: Breakout Demo Feedback

**Feature Branch**: `022-breakout-demo-feedback`  
**Created**: 2026-05-27  
**Status**: Draft  
**Input**: User description: "Mailbox/2026-05-27T19-59-16+0200-breakout-demo-fs-skia-ui-feedback.md"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Generated Viewer Guidance Matches Reality (Priority: P1)

A generated game-app author follows the generated documentation and tests to launch a persistent viewer, and every referenced viewer entry point is available in the packaged surface used by the generated app.

**Independent Test**: Generate a fresh app, follow the generated launch guidance without editing placeholder strings or comments, and verify that the app starts persistently using only documented public names.

### User Story 2 - Simple Game Shapes Are First-Class (Priority: P1)

A generated game-app author renders common circular and elliptical entities, such as balls, bullets, selection handles, radial indicators, and data markers, without representing them as rectangles or inventing local approximations.

**Independent Test**: Build a small generated scene containing a filled circle and filled ellipse, then verify deterministic visual evidence identifies both shapes with the expected bounds, colors, and relative placement.

### User Story 3 - Screenshot Evidence Is Honest And Bounded (Priority: P2)

A generated app that needs visual evidence can request live viewer screenshot evidence and receive either a bounded screenshot artifact with machine-readable facts or an explicit unsupported result with a standard fallback path.

**Independent Test**: Run the generated screenshot evidence command on a supported desktop host and verify it records a bounded screenshot result; run the same command on an unsupported host and verify the report clearly states why screenshot capture is unsupported without claiming screenshot proof.

### User Story 4 - App Effects And Viewer Effects Are Distinct (Priority: P2)

A generated app author can tell where pure app transitions, app-level commands, viewer rendering, window behavior, and host-side effects belong, reducing confusion when adapting an Elmish-style app to the viewer host.

**Independent Test**: Review the generated source, tests, and docs for one complete example that keeps app update logic separate from viewer render effects, then verify guidance uses consistent names for each effect category.

### User Story 5 - Evidence Reports And Geometry Are Reusable (Priority: P3)

A generated app author can reuse standard geometry guidance and evidence report conventions instead of hand-rolling report files, stdout output, directory creation, status fields, and local geometry types with ambiguous field names.

**Independent Test**: Generate an app with multiple evidence commands and verify each command writes reports with consistent field ordering, status fields, unsupported-host fields, stdout echoing, and geometry guidance that avoids duplicate local shape records.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Generated viewer guidance, generated tests, quickstart text, and package-facing examples MUST reference one current persistent-launch contract that generated apps can actually use.
- **FR-002**: The framework MUST provide first-class public scene concepts for filled circles and filled ellipses suitable for common game, chart, and interaction markers.
- **FR-003**: Circle and ellipse scene concepts MUST participate in deterministic visual evidence so their bounds, fill, and placement can be verified without relying on a live desktop screenshot.
- **FR-004**: Generated apps MUST have a documented screenshot evidence path that returns machine-readable success facts when live screenshot capture is available.
- **FR-005**: When live screenshot capture is unavailable, the screenshot evidence path MUST return an explicit unsupported result with the command, status, reason, and recommended deterministic fallback, and MUST NOT claim screenshot proof.
- **FR-006**: Generated-app guidance MUST clearly distinguish app transition commands from viewer rendering/window effects and host-side interpretation.
- **FR-007**: Generated examples MUST include one complete pattern where app update logic remains pure and viewer rendering is produced at the host boundary.
- **FR-008**: Generated game/app guidance MUST recommend reusable scene geometry concepts for layout evidence, collision bounds, containment checks, and rendering bounds when those concepts fit the app model.
- **FR-009**: Public or generated helpers MUST standardize key-value evidence report behavior, including parent directory creation, stable field ordering, stdout echoing, normalized statuses, and consistent unsupported-host fields.
- **FR-010**: Evidence commands in generated apps MUST use consistent exit behavior for success, unsupported environments, and failure cases so governance automation can classify results reliably.
- **FR-011**: The feature MUST preserve the distinction between deterministic render proof, live persistent-viewer proof, and live screenshot proof.
- **FR-012**: The feature MUST include tests or readiness checks that catch drift between generated guidance, generated tests, package surface, and evidence wording before downstream generated apps inherit the mismatch.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package contents and generated package consumers change because scene shape concepts, screenshot evidence behavior, generated guidance, and evidence report conventions affect the public package surface and generated app usage. Package identities do not change.
- **Public contract impact**: Public scene, viewer evidence, generated guidance, and evidence report contracts change. Signature files, documented public APIs, sample contracts, and surface baselines must be reviewed together.
- **State workflow impact**: Stateful workflow guidance changes by clarifying the boundary between app transition commands, viewer rendering/window effects, and host-side interpretation. Pure app reducers must remain free of rendering and I/O behavior.
- **Layout/rendering impact**: Rendering, deterministic visual evidence, screenshot evidence, and unsupported-environment diagnostics change. Layout evidence guidance changes to prefer shared scene geometry where it reduces ambiguity.
- **Evidence obligations**: Required real evidence paths are `specs/022-breakout-demo-feedback/readiness/generated-viewer-guidance.md`, `specs/022-breakout-demo-feedback/readiness/scene-shape-evidence.md`, `specs/022-breakout-demo-feedback/readiness/screenshot-evidence.md`, `specs/022-breakout-demo-feedback/readiness/effect-boundary-guidance.md`, and `specs/022-breakout-demo-feedback/readiness/evidence-report-conventions.md`.
- **Unsupported scope**: This feature does not add new game mechanics, rebuild the Breakout demo, guarantee screenshot capture on hosts that cannot expose it, migrate unrelated controls/chart/graph/DataGrid work, change release automation, or redefine persistent-launch evidence already covered by the active persistent-launch feature.
- **Build-target impact**: `Verify`, `TemplateCheck`, `GeneratedGuidanceCheck`, `EvidenceGraph`, and `EvidenceAudit` must validate the new guidance and evidence conventions. `Dev`, `Ci`, `PackLocal`, `DependencyReport`, and `TemplateDrift` change only if implementation alters aggregation, packaging, dependencies, or template drift coverage.

### Key Concepts

- **Generated Viewer Contract**: The documented persistent-viewer entry point and related guidance that generated apps are expected to use.
- **Scene Shape Primitive**: A public scene concept for simple geometric output, including circles and ellipses, that can be used in rendering and deterministic evidence.
- **Screenshot Evidence Result**: A machine-readable result that either records live screenshot facts or explains why screenshot capture is unsupported.
- **Effect Boundary Guidance**: Generated-app documentation and examples that separate app transitions, app commands, viewer effects, and host interpretation.
- **Evidence Report Convention**: A reusable structure for generated evidence reports, including standard status, command, output, and unsupported-host fields.

### Assumptions

- Generated apps should continue to support deterministic pixel/readback evidence even when live screenshot capture is unsupported.
- The current persistent-launch evidence work remains the source of truth for real persistent-window proof; this feature only adds Breakout-derived improvements and guidance alignment.
- Circle and ellipse support should cover filled shapes first; painted or styled variants may be included if they follow the same public model and evidence expectations.
- Unsupported screenshot capture is acceptable when clearly reported and paired with deterministic fallback evidence.

### Edge Cases

- A generated app runs on a desktop host where persistent launch succeeds but screenshot capture is unavailable.
- A generated app has multiple evidence commands that all write to the same output directory.
- Shape evidence includes circles or ellipses partially outside the visible scene bounds.
- Generated source imports viewer and app capability namespaces that contain similarly named command or effect concepts.
- App-owned geometry names overlap with scene geometry field names and could confuse readers or type inference.

## Success Criteria *(mandatory)*

- **SC-001**: A freshly generated game-style app can follow generated launch guidance and complete a persistent viewer launch without any stale or unavailable viewer contract references.
- **SC-002**: At least three representative generated visual examples can render circular or elliptical entities without rectangle substitutions.
- **SC-003**: Deterministic visual evidence can verify circle and ellipse output by bounds, color, and placement in under 5 seconds for a standard generated scene.
- **SC-004**: Screenshot evidence commands produce either a bounded screenshot result or an explicit unsupported result with no ambiguous or missing status fields in 100% of tested supported and unsupported host scenarios.
- **SC-005**: Generated guidance reviewers can identify where app commands and viewer effects belong in under 2 minutes using the generated example alone.
- **SC-006**: Generated evidence reports from at least three commands share the same required status, command, output, and unsupported-host conventions.
- **SC-007**: Governance checks detect stale generated guidance or evidence wording before release when a referenced public contract is missing or renamed.
