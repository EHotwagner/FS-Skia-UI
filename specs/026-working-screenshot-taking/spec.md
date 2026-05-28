# Feature Specification: Working Screenshot Taking

**Feature Branch**: `026-working-screenshot-taking`  
**Created**: 2026-05-28  
**Status**: Draft  
**Input**: User description: "implement actual working screenshot taking capability in working code"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Capture A Real Rendered Screenshot (Priority: P1)

A maintainer or generated-app author runs the screenshot readiness workflow for a viewer-backed graphical app and receives an actual image file captured from the rendered app, not only scene metadata, layout facts, logs, or simulated evidence.

**Independent Test**: On a supported desktop host, run the screenshot readiness workflow for a known graphical sample. The workflow opens or renders the app through the supported viewer path, captures a non-empty image file, records the capture artifact path, and verifies that the image dimensions and visible content match the expected rendered scene.

### User Story 2 - Use Screenshots As Reviewable Visual Evidence (Priority: P1)

A reviewer evaluates a feature that changes rendering, layout, controls, game output, or generated app visuals and can inspect a concrete screenshot artifact that proves what appeared on screen or in the supported capture surface.

**Independent Test**: Given a completed visual feature readiness package, a reviewer can open the screenshot evidence, confirm that it is an image produced by the working code path, and trace it to the command, host facts, sample, and capture result without relying on manual descriptions alone.

### User Story 3 - Diagnose Capture Failures Honestly (Priority: P2)

A developer runs screenshot evidence on a host where a window, capture surface, native graphics dependency, or pixel readback may be unavailable. The result explains whether the app failed to render, the host was unsupported, capture failed after rendering, or the screenshot was blank or invalid.

**Independent Test**: Run the screenshot workflow in a host missing required desktop or graphics prerequisites and verify that the readiness output records a failed or unsupported result with blocked stage, reason, command, host facts, and missing evidence fields instead of producing a synthetic placeholder or passing with metadata-only proof.

### User Story 4 - Keep Screenshot Evidence Separate From Other Evidence (Priority: P3)

A maintainer continues using structural layout evidence, deterministic scene reports, bounded launch evidence, and persistent-launch evidence, while screenshot capture remains a distinct visual artifact with its own acceptance rules.

**Independent Test**: Run visual readiness checks for a feature that has layout and launch evidence but no screenshot file. The evidence audit identifies the missing screenshot artifact when the feature requires screenshot proof and does not accept structural metadata as a substitute.

## Requirements *(mandatory)*

### Change Classification

This is a **Tier 1 (contracted change)** because it adds observable viewer/testing capability, capture evidence, generated guidance expectations, and merge-readiness behavior for visual features.

Public API and package surface changes must update `.fsi` signatures, semantic tests, FSI transcripts where applicable, surface baselines, documentation, generated template content, and package/version review together.

### Functional Requirements

- **FR-001**: The system MUST provide a screenshot-taking capability that produces an actual image artifact from rendered app output for supported viewer-backed graphical apps.
- **FR-002**: A passing screenshot artifact MUST be a readable image file with non-zero dimensions, non-empty pixel content, and a recorded association with the app, command, host, capture mode, and timestamp.
- **FR-003**: Screenshot capture MUST exercise working product or supported evidence code paths rather than writing placeholders, static fixtures, structural layout reports, scene descriptions, or synthetic image files as proof.
- **FR-004**: Screenshot readiness evidence MUST record status, command, app or sample identity, host facts, capture mode, evidence kind, artifact path, image dimensions, pixel-content validation result, capture source, proves-screenshot, blocked stage, classification, category, message, timestamp, and diagnostics when present.
- **FR-005**: Screenshot readiness MUST distinguish app launch failure, render failure, first-frame or presented-frame failure, pixel readback failure, file write failure, blank or invalid image output, and unsupported host prerequisites.
- **FR-006**: Screenshot capture MUST NOT classify a workflow as successful when only metadata, logs, structural layout facts, or manual descriptions are present.
- **FR-007**: Supported generated graphical app templates and sample guidance MUST expose a repeatable screenshot evidence workflow when the profile declares screenshot-ready visual output.
- **FR-008**: Screenshot evidence MUST be usable by reviewers without local reruns by storing the captured image and a concise machine-readable or structured evidence record together in the feature readiness package.
- **FR-009**: Evidence audit MUST reject screenshot-required visual features when the required screenshot file is missing, unreadable, blank, synthetic, or not traceable to a working code path.
- **FR-010**: The screenshot workflow MUST preserve normal interactive app behavior; evidence-mode capture and controlled close or cleanup behavior must be separate from the default user launch path.
- **FR-011**: Unsupported-host outcomes MUST be recorded as real negative evidence and MUST NOT be replaced with synthetic screenshots or accepted as successful screenshot proof.
- **FR-012**: Existing persistent-launch, bounded smoke, deterministic scene, and layout evidence workflows MUST remain available and clearly separate from screenshot evidence.
- **FR-013**: Documentation and generated guidance MUST explain when screenshot evidence is required, where artifacts are stored, and why structural or launch evidence does not substitute for a captured image.
- **FR-014**: Completion evidence for this feature MUST include at least one supported-host screenshot artifact produced from working code, plus diagnostic evidence for unsupported or failure cases where available.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package contents may change for viewer, testing, generated template, and governance assets. Package identities should remain unchanged. Package versions must be reviewed if public screenshot capture contracts or packaged template content change.
- **Public contract impact**: Public viewer or testing contracts may change to expose screenshot capture, capture results, image validation, or generated readiness output. Any `.fsi` signatures, documented public APIs, sample contracts, and surface baselines affected by those outcomes must be updated together.
- **State workflow impact**: Generated apps must keep ordinary product state workflows separate from screenshot evidence effects. Any workflow that launches, renders, captures pixels, writes files, or closes a viewer must be represented as an explicit evidence operation rather than folded into ordinary app update behavior.
- **Layout/rendering impact**: Rendering and visual readiness diagnostics change because the feature adds real screenshot artifacts and blank-image detection. Layout and scene evidence remain structural proof, not screenshot proof.
- **Evidence obligations**: Required real evidence paths are `specs/026-working-screenshot-taking/readiness/screenshot-capture-evidence.md`, `specs/026-working-screenshot-taking/readiness/screenshot-artifacts.md`, `specs/026-working-screenshot-taking/readiness/capture-failure-diagnostics.md`, `specs/026-working-screenshot-taking/readiness/generated-guidance.md`, `specs/026-working-screenshot-taking/readiness/package-surface-baseline.md`, `specs/026-working-screenshot-taking/readiness/evidence-graph.md`, and `specs/026-working-screenshot-taking/readiness/evidence-audit.md`.
- **Unsupported scope**: This feature does not require new game mechanics, renderer redesign, new desktop platform support, browser/mobile capture, visual design changes, package publishing, release automation, or replacing persistent-launch evidence with screenshots.
- **Build-target impact**: `Verify`, generated `Test`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateCheck`, `EvidenceGraph`, and `EvidenceAudit` must verify screenshot evidence requirements or generated guidance. `Dev`, `Ci`, `PackLocal`, `DependencyReport`, and `TemplateDrift` change only if aggregation, packaging, dependency, or template-drift checks require it.

## Success Criteria *(mandatory)*

- **SC-001**: On a supported desktop host, the screenshot readiness workflow produces an accepted, readable screenshot artifact from working code in at least 95% of repeated runs for a stable graphical sample.
- **SC-002**: 100% of accepted screenshot evidence records include status, command, app or sample identity, host facts, capture mode, evidence kind, artifact path, image dimensions, pixel-content validation, capture source, proves-screenshot, blocked stage, classification, category, message, timestamp, and diagnostics when present.
- **SC-003**: 0 screenshot-required visual readiness packages pass evidence audit when they contain only metadata, structural layout evidence, launch evidence, synthetic placeholders, or manual descriptions instead of a readable captured image.
- **SC-004**: Blank, unreadable, missing, or zero-dimension screenshot artifacts are rejected in all covered validation cases.
- **SC-005**: Unsupported-host and capture-failure diagnostics identify the blocked stage and actionable reason clearly enough that a reviewer can classify the issue in under 2 minutes.
- **SC-006**: Existing launch, bounded smoke, deterministic scene, and layout evidence workflows continue to pass their own checks without being reclassified as screenshot evidence.

## Key Entities

- **Screenshot Artifact**: A captured image file produced from rendered app output, with readable dimensions and non-empty pixel content.
- **Screenshot Evidence Record**: A structured readiness record that links the screenshot artifact to the command, app or sample, host facts, capture mode, validation result, status, classification, and message.
- **Capture Mode**: The supported way the screenshot was obtained, such as interactive-window capture or supported offscreen/render-target capture, as selected during planning.
- **Pixel Content Validation**: A readiness check proving the image is readable, has non-zero dimensions, and is not blank according to documented acceptance rules.
- **Capture Failure Diagnostic**: Evidence that records why screenshot capture could not produce an accepted artifact, including blocked stage and host facts.
- **Generated Screenshot Guidance**: Template and documentation expectations that tell generated app users how to produce and evaluate screenshot evidence.

For this feature, `capture-source=live-viewer-window` means screenshot pixels came from the supported live viewer render path, including the planned viewer-owned render-target/pixel-readback capture mode. It does not include deterministic scene metadata, static fixtures, manual screenshots, or fallback-only diagnostics.

## Assumptions

- Screenshot capture is intended for viewer-backed graphical apps and generated visual samples that already have or are expected to have a supported render path.
- A supported desktop host has the prerequisites needed to render the app and capture pixels through the approved capture mode.
- Some hosts may remain unable to capture screenshots automatically; those outcomes are valid diagnostics but not successful screenshot proof.
- Planning may choose the exact capture mode and public contract shape, but the user-visible outcome must be an actual reviewable image generated by working code.
- Synthetic malformed evidence may be used only to test rejection behavior and cannot satisfy screenshot readiness.

## Edge Cases

- The app launches but never presents a frame; screenshot evidence must fail at the render or presentation stage.
- A captured file exists but is empty, unreadable, zero-dimension, fully transparent, or visually blank; screenshot validation must reject it.
- The host can render a window but does not expose an external capture mechanism; diagnostics must record capture as the blocked stage rather than claiming launch failure.
- The workflow lacks permission to write the artifact; diagnostics must identify file output as the blocked stage.
- A feature is intentionally headless or non-visual; it must declare that scope and must not be required to produce screenshot evidence.
- A user manually captures a screenshot outside the workflow; it may support investigation but does not satisfy readiness unless planning defines a structured manual-observation artifact and approval path.
