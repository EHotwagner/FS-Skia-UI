# Feature Specification: Racer Feedback Follow-Ups

**Feature Branch**: `024-racer-feedback-followups`  
**Created**: 2026-05-28  
**Status**: Draft  
**Input**: User description: "Mailbox/2026-05-28T07-40-55+0200-top-down-racer-fs-skia-ui-feedback.md"

## Clarifications

### Session 2026-05-28

- Q: What platform scope should the screenshot capture capability implementation target? → A: Supported Windows and Linux desktop hosts, with explicit unsupported results only when launch or capture capability is genuinely unavailable.
- Q: What artifact format counts as successful screenshot evidence? → A: A PNG file with reported artifact path and positive width and height.
- Q: What capture source counts as successful screenshot evidence? → A: The live viewer window after first-frame presentation; deterministic scene rendering remains only fallback or diagnostic evidence.
- Q: What platform evidence is required for acceptance if both supported desktop OSes are not available? → A: Real screenshot success on at least one supported Windows or Linux desktop host, plus explicit capability or deferral evidence for the other supported OS if unavailable.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Avoid Scene Naming Collisions (Priority: P1)

A developer generating a game or visual sample needs guidance that avoids app-domain names that collide with common scene/layout concepts. They can start from generated guidance or sample conventions and see domain-specific names that keep scene coordinates, world geometry, and layout evidence easy to distinguish.

**Independent Test**: Review generated sample guidance and generated code-facing examples for geometry naming. Confirm they avoid generic app-domain names such as `Rect`, `Point`, and `Size` when scene/layout concepts with the same names are in scope, and confirm the guidance recommends domain-specific alternatives such as `WorldRect`, `WorldPoint`, or `TrackBounds`.

### User Story 2 - Capture Honest Screenshot Evidence (Priority: P1)

A developer collecting readiness evidence on a supported Windows or Linux desktop host needs live screenshot capture from the viewer window after first-frame presentation to produce an auditable PNG screenshot artifact. On hosts where launch or capture capability is genuinely unavailable, the result remains explicit and audit-friendly. They can distinguish successful screenshot proof, unsupported screenshot capture, and successful deterministic render evidence, and they can tell whether the viewer failed to open or opened but could not be captured.

**Independent Test**: Run evidence collection on a supported Windows or Linux desktop host and confirm the result reports screenshot evidence as successful with a real PNG screenshot artifact path, positive width and height, and a live-window capture source after first-frame presentation. Run evidence collection in a screenshot-unsupported environment and confirm the result reports screenshot evidence as unsupported, preserves the deterministic evidence fallback distinction, and includes a capability detail that separates launch/open status from capture availability.

### User Story 3 - Classify Benign Host Warnings (Priority: P2)

A developer launching a generated visual app on a desktop host may see host decoration/module warnings even when first-frame evidence succeeds. They need generated readiness reporting to avoid treating known benign host warnings as application or framework failures while still preserving the warnings for audit review.

**Independent Test**: Collect launch evidence from a host that emits the known GTK module warning messages. Confirm readiness output records first-frame success, classifies those warnings as benign host warnings, and does not mark the launch as failed solely because those warnings appeared.

### User Story 4 - Use Reliable Detached GUI Launch Guidance (Priority: P2)

A developer running generated GUI apps in the background on Linux needs guidance that uses a reliable detached launch pattern and does not promise that a simple terminal detachment will work for GUI startup.

**Independent Test**: Review generated guidance for background GUI launch. Confirm it recommends a known detached-session pattern for Linux, preserves log capture, redirects standard input away from the terminal, and avoids presenting unsupported detachment methods as reliable.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Generated sample guidance MUST warn developers not to use generic app-domain geometry names that are likely to collide with scene/layout concepts when scene primitives are in scope.
- **FR-002**: Generated sample guidance MUST provide at least three domain-specific naming examples for geometry or layout-related app records.
- **FR-003**: Generated samples and readiness examples MUST remain understandable without requiring developers to add extra type annotations solely to resolve naming ambiguity in common scenarios.
- **FR-004**: Screenshot evidence results MUST produce a real PNG screenshot artifact on supported Windows and Linux desktop hosts.
- **FR-005**: Screenshot evidence results MUST continue to distinguish successful screenshot proof, unsupported live screenshot capture, failed screenshot capture, and successful deterministic render evidence.
- **FR-006**: Screenshot evidence results MUST include a user-visible capability detail that distinguishes "viewer could not open" from "viewer opened but screenshot capture is unavailable" whenever that distinction can be determined.
- **FR-007**: Successful screenshot evidence results MUST report the PNG artifact path and positive width and height.
- **FR-008**: Successful screenshot evidence MUST capture the live viewer window after first-frame presentation; deterministic scene rendering MUST remain fallback or diagnostic evidence and MUST NOT be relabeled as screenshot proof.
- **FR-009**: Readiness diagnostics MUST classify the known GTK module messages for `colorreload-gtk-module` and `window-decorations-gtk-module` as benign host warnings when first-frame launch evidence succeeds.
- **FR-010**: Benign host-warning classification MUST preserve the original warning text in evidence output so reviewers can audit what occurred.
- **FR-011**: Generated guidance for Linux background GUI launch MUST recommend a detached-session pattern that captures logs and does not depend on an attached terminal for standard input.
- **FR-012**: Generated guidance MUST avoid presenting simple terminal detachment as the reliable default for GUI apps when a detached-session pattern is available.
- **FR-013**: Existing successful evidence paths for interactive launch, bounded first-frame launch, deterministic render evidence, successful screenshot reporting, and unsupported screenshot reporting MUST remain available to generated app consumers.
- **FR-014**: The feature MUST be accepted only with real evidence showing the guidance, diagnostics, screenshot artifact capture, and evidence-result behavior remain accurate.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package identities and generated package consumers are expected to remain stable. Package contents may change only as needed to implement screenshot capture, update generated guidance, or update host evidence reporting. Package versions are not part of this feature unless required by the repository release process.
- **Public contract impact**: Existing public contracts, documented public APIs, sample contracts, and surface baselines should remain stable except for additive evidence details needed to implement and explain screenshot capability. Any breaking public contract change is out of scope.
- **State workflow impact**: Generated app state workflows, commands, effects, subscriptions, and interpreter behavior must remain unchanged. This feature is limited to guidance, diagnostics, and evidence reporting clarity.
- **Layout/rendering impact**: Visual output and layout behavior must remain unchanged except for the addition of live screenshot artifact capture. Screenshot and unsupported-environment diagnostics may change to add successful screenshot proof, clearer capability detail, and benign warning classification.
- **Evidence obligations**: Required real evidence paths are `specs/024-racer-feedback-followups/readiness/baseline-status.md`, `specs/024-racer-feedback-followups/readiness/generated-guidance-validation.md`, `specs/024-racer-feedback-followups/readiness/screenshot-capability-detail.md`, `specs/024-racer-feedback-followups/readiness/screenshot-success-artifact.md`, `specs/024-racer-feedback-followups/readiness/host-warning-classification.md`, and `specs/024-racer-feedback-followups/readiness/detached-launch-guidance.md`. Screenshot acceptance requires real screenshot success on at least one supported Windows or Linux desktop host, plus explicit capability or deferral evidence for the other supported OS if that OS is not available in the validation environment.
- **Unsupported scope**: New desktop host support beyond supported Windows and Linux desktop hosts, renderer replacement, gameplay changes, generated game redesign, release automation redesign, and broad platform roadmap decisions are out of scope.
- **Build-target impact**: Existing user-facing build targets must remain available under the same names. `GeneratedGuidanceCheck`, `TemplateDrift`, `TemplateCheck`, `EvidenceGraph`, and `EvidenceAudit` may need expectation updates to validate the new guidance and evidence wording; `Dev`, `Verify`, `Ci`, `PackLocal`, and `DependencyReport` must not change semantics.

## Edge Cases

- Screenshot capture succeeds on a supported Windows or Linux desktop host and produces a readable screenshot artifact.
- Screenshot capture is unavailable before a viewer window can be opened.
- A viewer window opens successfully but the host cannot provide screenshot capture.
- Deterministic render evidence succeeds while live screenshot evidence is unsupported.
- Known benign GTK module warnings appear together with unrelated warnings that still require attention.
- Detached GUI launch starts but exits early without producing a useful log.
- Generated app domains naturally need rectangles or points and require guidance for unambiguous domain naming.

## Key Entities

- **Feedback Item**: A consumer-observed friction point with a summary, evidence, expected framework follow-up, and acceptance evidence.
- **Generated Guidance**: User-facing sample or readiness instructions that help generated app consumers avoid integration friction.
- **Screenshot Evidence Result**: Evidence output that records whether live screenshot proof succeeded, was unsupported, or failed, the PNG screenshot artifact path, dimensions, and live-window capture source when successful, and a capability detail when available.
- **Host Warning Classification**: A readiness diagnostic classification that separates benign host-environment warnings from application or framework failures.
- **Detached Launch Guidance**: Instructions for running generated GUI apps in the background while preserving logs and process/session behavior.

## Assumptions

- The feedback file represents accepted consumer evidence from a completed generated top-down racer integration.
- The primary user is a developer generating or validating FS.Skia.UI visual apps.
- The implementation should prefer additive screenshot capability, diagnostics, and guidance updates over breaking public contract changes.
- Unsupported screenshot capture remains a valid real host fact when it is explicitly reported for hosts where launch or capture capability is genuinely unavailable.

## Success Criteria *(mandatory)*

- **SC-001**: A reviewer can verify all four feedback follow-ups from the source feedback file against readiness evidence in under 10 minutes.
- **SC-002**: Generated guidance contains at least three explicit domain-specific geometry naming examples and zero recommended app-domain examples named only `Rect`, `Point`, or `Size`.
- **SC-003**: On at least one supported Windows or Linux desktop host, screenshot evidence produces `status=ok`, `evidence-kind=screenshot`, positive dimensions, a real PNG screenshot artifact path, and live-window capture source after first-frame presentation; if the other supported OS is unavailable for validation, readiness evidence records explicit capability or deferral status for that OS.
- **SC-004**: In screenshot-unsupported evidence, reviewers can identify whether launch/open status and capture availability are separate facts in 100% of applicable evidence records.
- **SC-005**: Known benign GTK module warnings are preserved in evidence output and classified as benign in 100% of first-frame-success launch records that contain only those warning messages.
- **SC-006**: Linux detached GUI launch guidance includes log capture and detached standard input handling, and no reviewed guidance presents simple terminal detachment as the preferred reliable method.
- **SC-007**: Existing generated app verification and evidence audit workflows continue to pass after the follow-ups are applied.
