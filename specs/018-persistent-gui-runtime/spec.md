# Feature Specification: Persistent GUI Runtime

**Feature Branch**: `018-persistent-gui-runtime`  
**Created**: 2026-05-26  
**Status**: Draft  
**Input**: User description: "Create specs from `Mailbox/2026-05-26-1949-tetris-persistent-gui-runtime-analysis.md`"

## Clarifications

### Session 2026-05-26

- Q: What visual proof should count as authoritative generated game readiness evidence? → A: Screenshot preferred; pixel-readback fallback.
- Q: What should a normal interactive launch do when no usable graphical session is present? → A: Fail fast with diagnostic.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Play a Generated Game Interactively (Priority: P1)

A developer runs a generated Tetris application from a local checkout and expects a graphical game window to remain available for keyboard play until the user closes it. The launch must not end merely because the first visual frame was shown or because launch evidence was collected.

**Independent Test**: In an environment with a valid desktop session, launch the generated game and verify that the window remains open after the first frame, accepts user input, and closes only after an explicit user or host close action.

**Failure Test**: In an environment without a usable graphical session, launch the generated game through the normal interactive path and verify that it fails before mode switching with a desktop-session diagnostic.

### User Story 2 - Collect Launch Evidence Explicitly (Priority: P1)

A maintainer needs bounded launch evidence for CI or readiness review without confusing that evidence run with an interactive game session. The run outcome clearly states whether it was an interactive session or a bounded evidence session.

**Independent Test**: Run the generated app in evidence mode and verify that the result reports first-frame presentation, window availability, input-dispatch status, and self-close-for-evidence status without claiming that the user received an ongoing interactive session.

### User Story 3 - Diagnose Desktop Session Problems Before App Debugging (Priority: P2)

A developer launches the container from a desktop host and receives a clear readiness diagnostic when display, runtime directory, session bus, socket, or permission prerequisites are missing. The diagnostic distinguishes host session setup problems from generated app lifecycle behavior.

**Independent Test**: Run the container launch readiness check with missing or invalid runtime/display settings and verify that it reports the exact missing prerequisite before the generated app is launched.

### User Story 4 - Verify Generated App Dependencies and Tests (Priority: P2)

A maintainer verifies a generated game and needs confidence that the requested framework packages, generated tests, visual evidence, and readiness contracts were actually checked. Package fallback warnings and placeholder verification targets must not be treated as authoritative success.

**Independent Test**: Run generated verification on a project whose requested framework package version cannot be resolved exactly and verify that the workflow fails with actionable package source and resolved-version information. In a supported graphical environment, verify that game readiness captures a screenshot; if screenshot capture is unavailable but rendering can still be inspected, verify that pixel-readback evidence is captured instead.

### Edge Cases

- A valid display variable exists, but the runtime directory or display socket is absent.
- A private runtime directory fallback exists, but no real host desktop session is available.
- Normal interactive launch is requested without a usable graphical session; the launch fails fast with a desktop-session diagnostic and does not silently switch to evidence or text-only behavior.
- The first frame is presented successfully, but no user input has been dispatched yet.
- Evidence mode intentionally closes after collecting bounded evidence.
- Interactive mode presents a frame and remains open without a close event.
- Requested framework packages are unavailable from configured package sources.
- A generated verification target completes without running the generated test project.
- Screenshot evidence cannot be captured but rendered pixels can be inspected; pixel-readback evidence is acceptable as the fallback.
- Neither screenshot nor pixel-readback evidence can be captured because the host lacks graphical support; the unsupported-host diagnostic must be explicit.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST provide an interactive generated-app launch path that keeps the graphical window available until the user or host explicitly closes it.
- **FR-002**: The system MUST prevent first-frame presentation alone from ending an interactive generated-app session.
- **FR-003**: The system MUST provide an explicit bounded evidence launch path for first-frame, input-dispatch, and launch-readiness checks.
- **FR-004**: The system MUST report launch outcomes using unambiguous user-facing fields that distinguish interactive sessions from evidence sessions.
- **FR-005**: The system MUST disclose whether a launch self-closed for evidence, whether a user close was observed, whether a first frame was presented, and whether input dispatch was verified or not required.
- **FR-006**: Generated game entry points MUST default to the interactive launch path for normal user runs.
- **FR-007**: Evidence-oriented launches MUST require an explicit user, command, or workflow choice and MUST NOT be the default normal run behavior.
- **FR-008**: The system MUST include a regression check proving that the interactive launch path does not self-close after the first frame when no close action occurred.
- **FR-009**: Container launch readiness MUST validate runtime directory presence, ownership suitability, permission suitability, display availability, and display socket availability before app lifecycle failures are diagnosed.
- **FR-010**: Normal interactive launches MUST fail fast with a desktop-session diagnostic when no usable graphical session is present.
- **FR-011**: Normal interactive launches MUST NOT silently switch to bounded evidence mode, text-only metadata, or private runtime fallback when the user requested interactive play.
- **FR-012**: Container launch readiness MUST support real desktop session integration when the host provides runtime, display, session bus, and socket values.
- **FR-013**: Container launch readiness MUST provide a clearly labeled fallback for diagnostic or evidence workflows on hosts without a real runtime directory and MUST state that the fallback is not equivalent to a full desktop session.
- **FR-014**: Generated verification MUST fail when requested framework package versions are not resolved exactly.
- **FR-015**: Generated verification MUST record requested package versions, resolved package versions, and package source information as readiness evidence.
- **FR-016**: Generated verification MUST run the generated test project when a generated test project exists.
- **FR-017**: Generated verification MUST label placeholder or non-authoritative targets as non-authoritative readiness evidence.
- **FR-018**: Generated game readiness MUST capture screenshot evidence that the game surface is readable and interactive when screenshot capture is available in a supported graphical environment.
- **FR-019**: Generated game readiness MUST accept pixel-readback evidence as the fallback only when screenshot capture is unavailable but rendered pixels can still be inspected.
- **FR-020**: Generated game readiness MUST provide an explicit unsupported-host diagnostic when neither screenshot nor pixel-readback evidence can be captured.
- **FR-021**: Generated game examples MUST demonstrate a board/grid presentation, side information, keyboard-driven updates, and time-based game progression at a level sufficient for a playable demo.
- **FR-022**: Readiness guidance MUST make required evidence files, required evidence content, and acceptance keywords explicit before implementation begins.
- **FR-023**: Task workflow guidance MUST support recorded implementation batches with named tasks, shared evidence, and before/after graph validation when one cohesive change completes multiple tasks.
- **FR-024**: Task workflow guidance MUST provide a red-green evidence log format for related test clusters, including failing assertion, command, change reference, and final passing command.
- **FR-025**: Synthetic error-handling guidance MUST provide a pre-implementation validation path that identifies missing accepted synthetic-error metadata before coding starts.
- **FR-026**: Local generated projects MUST either include the package source configuration required for local framework packages or use only package versions available from configured sources.
- **FR-027**: User-facing diagnostics MUST separate environment/session failures, package-resolution failures, verification-depth failures, and app lifecycle failures.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package identities and package contents may change for the viewer package that owns generated app launch behavior. Package versions must be bumped if packable project contents change. Generated package consumers are impacted because generated games must resolve exact requested framework packages. Controls, chart, graph, and DataGrid authoring are not in scope.
- **Public contract impact**: Public launch contracts may change to separate interactive launch behavior from evidence launch behavior. `.fsi` signatures, documented public APIs, sample contracts, and surface baselines must be reviewed and updated if the launch surface changes.
- **State workflow impact**: Stateful workflow changes are expected for generated app lifecycle, close handling, input-dispatch observation, and evidence-session outcome reporting. I/O and command behavior change only where launch and verification workflows expose these states.
- **Layout/rendering impact**: Game rendering expectations change for generated Tetris-style apps because readiness must prove a readable visual board or provide an unsupported-host diagnostic. General controls, charts, DataGrid behavior, Vulkan behavior, and unrelated rendering output are out of scope.
- **Evidence obligations**: Required real evidence paths include `specs/018-persistent-gui-runtime/readiness/interactive-lifecycle.md`, `specs/018-persistent-gui-runtime/readiness/evidence-launch-mode.md`, `specs/018-persistent-gui-runtime/readiness/container-session-diagnostics.md`, `specs/018-persistent-gui-runtime/readiness/package-resolution.md`, `specs/018-persistent-gui-runtime/readiness/generated-verify.md`, `specs/018-persistent-gui-runtime/readiness/game-visual-evidence.md`, `specs/018-persistent-gui-runtime/readiness/task-workflow-guidance.md`, and `specs/018-persistent-gui-runtime/readiness/evidence-audit.md`.
- **Unsupported scope**: This feature does not require a new game engine, unrelated chart/control migrations, broad platform expansion, release automation, marketplace distribution, or changes to non-game generated applications beyond launch and verification contracts they share.
- **Build-target impact**: `Verify`, generated `Test`, package verification, generated guidance checks, `EvidenceGraph`, and `EvidenceAudit` must be reviewed for changes. `Dev`, `Ci`, `PackLocal`, `TemplateCheck`, `DependencyReport`, and `TemplateDrift` may change only if they aggregate or validate the affected workflows.

## Success Criteria *(mandatory)*

- **SC-001**: In a valid desktop session, 100% of sampled normal generated game launches remain open for at least 30 seconds after first-frame presentation unless an explicit close action occurs.
- **SC-002**: 100% of bounded evidence launches report whether they self-closed for evidence and are not labeled as successful interactive play sessions.
- **SC-003**: At least 95% of invalid container desktop-session configurations fail at readiness validation with a specific missing prerequisite before app launch.
- **SC-004**: 100% of generated verification runs fail when requested framework package versions are not resolved exactly.
- **SC-005**: 100% of generated game verification runs include generated test execution when a generated test project exists.
- **SC-006**: Readiness review can distinguish environment failure, package-resolution failure, verification-depth failure, and app lifecycle failure from the recorded evidence without rerunning the investigation.
- **SC-007**: At least one generated game readiness path provides screenshot proof of a readable game board in supported graphical environments with screenshot capture.
- **SC-008**: 100% of generated game readiness runs that cannot capture screenshots but can inspect rendered pixels provide pixel-readback evidence instead of text-only scene metadata.
- **SC-009**: Maintainers can identify required readiness files and required acceptance content before implementation begins, with zero audit-only discoveries for this feature's planned evidence files.

## Assumptions

- Generated Tetris is the representative game workload for this feature, but the launch and verification expectations apply to generated graphical games generally.
- Normal user launches should prioritize interactive play; bounded launch evidence remains valuable but must be opt-in.
- A host without a usable graphical session may still produce non-visual evidence, but that evidence must be labeled as an unsupported-host diagnostic rather than visual proof.
- Existing synthetic evidence governance remains in force; this feature only adds diagnostics and pre-checks that make approved synthetic error-handling easier to validate before implementation.

## Key Entities

- **Generated Game Application**: A generated graphical app intended for direct user interaction, keyboard input, and visible game-state progression.
- **Interactive Launch Session**: A normal run that keeps the window open until an explicit close action.
- **Evidence Launch Session**: A bounded run that collects launch, frame, and input evidence and may close automatically after evidence conditions are met.
- **Launch Outcome**: The recorded result of a launch, including mode, first-frame status, close source, input-dispatch status, and diagnostic information.
- **Desktop Session Diagnostic**: A readiness result describing runtime directory, display, socket, session bus, and permission status.
- **Package Resolution Evidence**: Recorded requested versions, resolved versions, configured sources, and failure details for generated app framework packages.
- **Visual Game Evidence**: Screenshot proof, or pixel-readback fallback proof, that the generated game surface is readable and interactive in a supported environment.
- **Red-Green Evidence Log**: A readiness record that preserves failing-first and passing-after-change evidence for related test clusters.
