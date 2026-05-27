# Feature Specification: Fix Window Visibility

**Feature Branch**: `019-fix-window-visibility`  
**Created**: 2026-05-27  
**Status**: Draft  
**Input**: User description: `Mailbox/2026-05-27-005619-asteroids-demo-windowing-report.md`

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Launch A Usable Game Window (Priority: P1)

A developer launches a generated graphical game from the command line and receives an accessible desktop window that can be seen, focused, resized where allowed, and closed by the user.

**Independent Test**: Launch a generated graphical game on a supported desktop session. Verify that the app remains open after the first frame, the game surface is visible and accessible from the desktop, the user can close the window, and the reported outcome identifies that close as a user close.

### User Story 2 - Diagnose Taskbar-Only Or Invisible Windows (Priority: P1)

A developer launches a generated graphical game in an environment where the desktop host creates a process or taskbar entry but no usable window surface appears. The launch result clearly distinguishes a hidden, minimized, off-screen, unmapped, unsupported, or otherwise inaccessible window from a successful visible launch.

**Independent Test**: Run a launch check in a desktop session that cannot present a usable window. Verify that the result does not report success as a normal interactive launch and includes actionable environment and window-state diagnostics.

### User Story 3 - Capture Inspectable Visual Evidence (Priority: P2)

A developer requests generated app visual evidence and receives an artifact that can be opened and visually inspected as an image, instead of a text hash mislabeled as a screenshot.

**Independent Test**: Run the generated app visual-evidence command. Verify that a requested image artifact is an actual image file, that metadata/hash evidence remains clearly labeled as metadata when produced, and that the evidence result states whether it proves scene rendering, desktop window visibility, or both.

### User Story 4 - Configure Expected Window Behavior (Priority: P2)

A generated app author sets expected desktop-window behavior such as resize availability, maximize availability, startup window state, startup position, and rendering backend preference. The launched window honors supported settings or reports why a setting could not be honored.

**Independent Test**: Launch a generated app with each supported window behavior setting. Verify that supported settings are reflected in the observable window behavior and unsupported settings produce explicit diagnostics without being silently ignored.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST keep normal interactive generated game launches open until a user close, an explicit app close request, or an unrecoverable launch failure occurs.
- **FR-002**: The system MUST NOT close a normal interactive generated game launch automatically after presenting the first frame.
- **FR-003**: The system MUST keep bounded first-frame or evidence-only launch behavior separate from normal interactive launch behavior.
- **FR-004**: The system MUST report distinct close reasons for user close, app-requested close, evidence-requested close, framework-requested close, and failure-driven close.
- **FR-005**: The system MUST NOT report `user close observed` when the user did not actually close the window.
- **FR-006**: The system MUST classify a launch with only a process or taskbar entry and no accessible visible window as a failed or degraded interactive launch, not as a fully successful visible launch.
- **FR-007**: The system MUST report enough window-state diagnostics to determine whether the window was initialized, visible, focused or focusable, closing, minimized or maximized where observable, sized as requested, attached to a renderable surface, and connected to available input devices.
- **FR-008**: The system MUST report enough desktop-session diagnostics to separate environment/session failures from application lifecycle failures.
- **FR-009**: The system MUST allow generated graphical apps to request common window behavior: resize policy, maximize policy, initial window state, initial position when supported, and backend preference.
- **FR-010**: The system MUST state when requested window behavior cannot be honored by the current host environment.
- **FR-011**: The system MUST produce an actual inspectable image when the user requests screenshot or image evidence.
- **FR-012**: The system MUST label deterministic hash or metadata evidence as metadata/hash evidence rather than screenshot evidence.
- **FR-013**: The system MUST distinguish visual evidence that proves scene rendering from evidence that proves native desktop-window visibility.
- **FR-014**: The system MUST preserve generated app testability for non-interactive evidence runs in environments where no supported desktop window can be shown.
- **FR-015**: The system MUST include generated-app validation that covers normal interactive persistence, bounded evidence behavior, close-reason reporting, visible-window diagnostics, window behavior options, and real image evidence.
- **FR-016**: The system MUST fail validation when generated app package resolution, generated tests, or visual evidence claims are misleading or incomplete.

### Edge Cases

- The host creates a taskbar entry but no active or visible window can be selected.
- The window is created but minimized, off-screen, zero-sized, hidden behind unsupported compositor behavior, or missing a usable rendering surface.
- A desktop screenshot facility requires interactive approval and cannot be used during automated validation.
- A graphical backend warning is emitted but the launch can still produce a usable visible window.
- Input-dispatch verification is requested while the user expects the app to remain available for manual testing.
- Image evidence cannot prove desktop visibility because it was captured from an off-screen scene path.

### Synthetic Evidence Disclosure

- Synthetic malformed-input/error-path evidence is approved only for validation of malformed readiness rows, invalid evidence command arguments, corrupt image metadata records, missing required generated-validation fields, and hostile artifact paths. This maps to T014 and uses `specs/019-fix-window-visibility/readiness/logs/t014-synthetic-error-evidence.txt` as the real-evidence tracking path for validator/audit command output. It does not replace supported-host visible-window evidence, real image evidence, generated validation evidence, or any user-story readiness artifact.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package contents and generated package consumers may change because generated graphical app behavior, visual evidence artifacts, and public viewer capabilities are affected. Package identities do not change unless planning finds an unavoidable packaging boundary issue.
- **Public contract impact**: Public signatures, documented public APIs, generated sample contracts, and surface baselines are expected to change for window behavior options, launch outcomes, close reasons, and visual evidence naming.
- **State workflow impact**: Stateful workflow, I/O, commands, effects, subscriptions, and interpreter behavior change because launch lifecycle, close reason tracking, window diagnostics, input observation, and evidence capture are affected.
- **Layout/rendering impact**: Rendering, screenshots, visual output, backend selection, and unsupported environment diagnostics change. Chart, DataGrid, and unrelated layout scope do not change.
- **Evidence obligations**: Required real evidence paths are `specs/019-fix-window-visibility/readiness/interactive-visible-window.md`, `specs/019-fix-window-visibility/readiness/close-reason-separation.md`, `specs/019-fix-window-visibility/readiness/window-state-diagnostics.md`, `specs/019-fix-window-visibility/readiness/window-options.md`, `specs/019-fix-window-visibility/readiness/real-image-evidence.md`, `specs/019-fix-window-visibility/readiness/generated-validation.md`, and `specs/019-fix-window-visibility/readiness/evidence-audit.md`.
- **Unsupported scope**: Out of scope are a new game engine, new generated game mechanics, unrelated control/chart/DataGrid authoring changes, release automation, marketplace distribution, and guarantees for unsupported desktop sessions beyond clear diagnostics and fallback evidence.
- **Build-target impact**: `Verify`, generated `Test`, generated product checks, `GeneratedGuidanceCheck`, `TemplateCheck`, `DependencyReport`, `EvidenceGraph`, and `EvidenceAudit` may need updates. `Dev`, `Ci`, `PackLocal`, and `TemplateDrift` may change only where they aggregate or validate the affected workflows.

### Key Entities

- **Interactive Launch Outcome**: The user-facing result of a normal graphical launch, including mode, visibility, close reason, first-frame status, input status, diagnostics, and failure class.
- **Window Behavior Request**: The requested desktop-window behavior for a generated app, including resize policy, maximize policy, startup state, startup position, and backend preference.
- **Window State Diagnostic**: Observable facts about native window creation, visibility, focusability, size, rendering surface availability, backend selection, and input device availability.
- **Visual Evidence Artifact**: A generated artifact that proves scene rendering, native desktop visibility, or metadata/hash consistency with an explicit evidence type.

### Assumptions

- A supported desktop session is one where the host environment can present a native window without requiring privileged or unavailable compositor actions.
- Real image evidence means an artifact that common image viewers can open as an image, not a text file containing a hash.
- Desktop-window visibility evidence may require supported-host/manual evidence when automated screenshot capture is blocked by compositor permissions.
- Metadata/hash evidence remains valuable as deterministic scene evidence when it is labeled accurately.

## Success Criteria *(mandatory)*

- **SC-001**: In a supported desktop session, 100% of normal generated game launches remain open past the first rendered frame until the user or app explicitly closes them.
- **SC-002**: In validation runs, 0 launch outcomes incorrectly report a user close when the close was requested by the framework, evidence path, app logic, or failure handling.
- **SC-003**: A generated graphical app launched on a supported desktop produces a visible, focusable game window in at least 95% of repeated launch attempts across the supported host matrix, with every exception classified by failure reason.
- **SC-004**: 100% of taskbar-only, unmapped, hidden, minimized-only, or inaccessible-window launches are reported as degraded or failed interactive launches with actionable diagnostics.
- **SC-005**: A developer can identify from one launch report whether the failure class is environment/session, package/verification, visual-evidence, or app lifecycle without inspecting source code.
- **SC-006**: 100% of requested image evidence artifacts are valid image files, and 100% of metadata/hash artifacts are labeled so they cannot be mistaken for screenshots.
- **SC-007**: Generated app validation completes the required launch, diagnostics, close-reason, window-option, and visual-evidence checks in under 5 minutes on a prepared supported host.
- **SC-008**: Manual interactive testing of a generated game can begin within 30 seconds of command launch on a supported desktop, with no environment variable required to prevent first-frame auto-close.
