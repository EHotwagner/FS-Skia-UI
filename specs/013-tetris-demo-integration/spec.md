# Feature Specification: Tetris Demo Integration Improvements

**Feature Branch**: `013-tetris-demo-integration`  
**Created**: 2026-05-18  
**Status**: Draft  
**Input**: User description: "create specs for Mailbox/tetris-demo-integration-analysis.md"

## Clarifications

### Session 2026-05-18

- Q: What renderer parity is required for headless visual evidence? → A: Headless evidence may use a deterministic scene-level renderer, while real viewer startup remains separate required evidence.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Start and Control a Generated Graphical App (Priority: P1)

As an app developer using the generated graphical app path, I want viewer
keyboard events to arrive as stable, documented app input values, so that the
rendered app can be started, navigated, played, paused, restarted, and tested
without hand-written string matching for each windowing backend.

**Independent Test**: A generated graphical app starts from its initial screen
using a viewer key event, navigates an options screen, performs a primary
interaction in the main experience, and restarts from an end screen using the
same documented input values that the live viewer delivers. The test fails if
the pure domain input path works while the viewer-event path leaves the visible
app unresponsive.

### User Story 2 - Prove Graphical Startup Without Manual Timeouts (Priority: P1)

As a maintainer validating generated graphical apps, I want an official bounded
graphical smoke mode that exits after visible rendering evidence is collected,
so that CI and local readiness scripts can distinguish first-frame success from
startup failure without relying on external process timeouts or log scanning.

**Independent Test**: A generated app can run a bounded graphical smoke command
that returns success after at least one rendered frame and returns a failure
with a clear diagnostic when a window, surface, renderer, or scene cannot be
created. The command includes enough evidence to show frame count, elapsed
time, initial output size, renderer path, and last meaningful diagnostic.

### User Story 3 - Debug Startup Without Frame Log Noise (Priority: P2)

As an app developer debugging viewer startup, I want diagnostics separated by
level and category, so that startup, input, renderer, screenshot, and frame-loop
messages can be enabled independently and logs remain useful during short
integration runs.

**Independent Test**: A startup-focused diagnostic run reports window and
renderer initialization milestones without repeated per-frame messages. A
frame-focused diagnostic run includes frame-loop messages only when that
category is explicitly enabled or sampled. Tests can capture diagnostics
without scraping process stderr.

### User Story 4 - Collect Visual Evidence When Desktop Windows Are Unavailable (Priority: P2)

As a maintainer running generated app validation in CI, I want a supported
offscreen or headless visual evidence path, so that generated apps can prove
scene output even when a desktop session or GPU-backed window is unavailable.

**Independent Test**: A generated app can render a representative scene to
deterministic visual evidence without opening a native window through a
scene-level evidence path. Real viewer startup remains separately validated by
bounded graphical smoke evidence. When the host cannot support the requested
renderer mode, validation returns an explicit unsupported-environment
diagnostic rather than an ambiguous app failure.

### User Story 5 - Reproduce Consumer Package Setup Reliably (Priority: P3)

As an external app author validating a generated consumer against local
packages, I want one documented local-integration command that reports package
versions, feed location, restore guidance, and consumer configuration snippets,
so that stale local package feeds are easy to identify and correct.

**Independent Test**: After local packages are produced, the command prints the
local feed path, all package identities and versions needed by a generated
consumer, a ready-to-use consumer package configuration snippet, and the restore
command. A stale or missing package feed is reported as setup drift, not as an
application build failure.

### Edge Cases

- The visible graphical window renders correctly but the initial screen ignores
  viewer key events; validation must fail the user-reachable flow.
- Different windowing backends or operating systems may report common keys with
  different raw names; documented input values must normalize these cases.
- Unknown keys, letters, digits, and unsupported keys must remain observable
  without crashing input handling.
- A bounded smoke run may render zero frames because window creation,
  renderer setup, surface creation, or scene drawing failed; the failure must
  name the blocked stage.
- A bounded smoke run may reach first frame while verbose frame diagnostics are
  enabled; diagnostic sampling must keep evidence readable.
- CI may lack a desktop session, display server, GPU, Vulkan support, or runtime
  directory; visual validation must separate unsupported host conditions from
  product defects.
- Generated examples may include start, options, gameplay, pause, game-over, or
  restart screens; the input contract must cover every user-reachable screen
  provided by the template.
- Local package feeds may contain stale package versions; consumer setup
  guidance must make the intended feed and version set visible.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The framework MUST provide documented normalized viewer input
  values for common graphical app keys, including arrows, enter, space, escape,
  backspace, letters, digits, function keys, and unknown raw keys.
- **FR-002**: Viewer key-down and key-up events MUST be convertible to the
  normalized input values without requiring generated apps to compare raw
  backend-specific strings.
- **FR-003**: The normalized input behavior MUST treat common alternative raw
  names for the same key as the same documented input value where users would
  reasonably expect them to match.
- **FR-004**: Generated graphical app templates that include keyboard input MUST
  define user-reachable input flows for initial, options, main interaction,
  pause or escape/back behavior where present, and end or restart screens.
- **FR-005**: Generated graphical app tests MUST include at least one flow that
  starts the app from the initial screen through a viewer key event rather than
  only through a domain-specific test message.
- **FR-006**: Generated graphical app tests MUST cover options navigation and
  end-screen restart through viewer key events when those screens are generated.
- **FR-007**: The viewer MUST support a bounded execution mode that exits after
  first-frame evidence or after a requested frame count, without requiring an
  external timeout to represent success.
- **FR-008**: Bounded viewer execution MUST return structured evidence for
  successful runs, including frames rendered, elapsed time, initial output size,
  renderer mode, and the last relevant diagnostic summary.
- **FR-009**: Bounded viewer execution MUST return a structured failure when
  rendering cannot reach the requested evidence point, naming the blocked stage
  and whether the failure is an unsupported host condition or an app/rendering
  defect.
- **FR-010**: Viewer diagnostics MUST support independent level and category
  selection for startup, input, frame loop, renderer backend, scene drawing,
  swapchain or surface management, and screenshots or readback.
- **FR-011**: Viewer diagnostics MUST allow frame-loop messages to be disabled,
  sampled, or explicitly enabled so startup evidence is not drowned out by
  repeated frame messages.
- **FR-012**: Viewer diagnostics MUST be capturable by tests and app hosts
  without requiring process-level stderr scraping.
- **FR-013**: The framework MUST provide an official offscreen or headless
  scene-level evidence path that can produce deterministic visual evidence for
  generated apps without opening a desktop window.
- **FR-014**: The offscreen or headless evidence path MUST report explicit
  unsupported-environment diagnostics when the host cannot provide required
  rendering capabilities.
- **FR-014a**: Headless visual evidence MAY use deterministic scene-level
  rendering rather than the same renderer backend used by the live viewer, but
  it MUST NOT replace the separate bounded real-viewer startup evidence.
- **FR-015**: Generated graphical app quickstarts MUST document both the
  interactive run path and the bounded smoke or visual evidence path.
- **FR-016**: Local consumer integration guidance MUST provide one command or
  documented workflow that prints the local package feed, package identities,
  package versions, consumer package configuration, and restore command needed
  to validate a generated app against local packages.
- **FR-017**: Consumer setup validation MUST identify stale or missing local
  package-feed content separately from application source, input, or rendering
  defects.
- **FR-018**: The framework SHOULD provide an optional app-host convenience path
  for generated graphical apps that combines model initialization, update,
  view/scene production, normalized key mapping, ticking, diagnostics, and
  bounded smoke execution while preserving access to lower-level viewer
  primitives.
- **FR-019**: All new graphical integration failures MUST identify the affected
  app flow, input value, screen, rendering stage, diagnostic category, package
  identity, or evidence path so users can act without reverse-engineering the
  viewer lifecycle.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package identities are expected to remain stable. Package
  contents may change for viewer input, diagnostics, visual evidence helpers,
  generated templates, generated tests, and local consumer guidance. Generated
  package consumers may change to exercise the new graphical app input and
  smoke evidence paths. Controls, charts, graph views, and DataGrid ownership
  remain on the active Controls package path; legacy Charts migration is out of
  scope.
- **Public contract impact**: Public viewer input, diagnostics, bounded smoke,
  visual evidence, generated template, and sample contracts may change.
  Signature files, documentation, and surface baselines must be updated when a
  public contract is added or changed.
- **State workflow impact**: Generated app state workflows may change only to
  carry complete user-reachable input flows for start, options, main
  interaction, pause/back, and restart screens. Viewer lifecycle commands,
  subscriptions, diagnostics, and bounded execution behavior are in scope.
- **Layout/rendering impact**: Core layout behavior is not expected to change.
  Rendering evidence, screenshots or readback, real graphical viewer startup,
  unsupported environment diagnostics, and generated visual smoke paths are in
  scope.
- **Evidence obligations**: Required real evidence paths include
  `specs/013-tetris-demo-integration/readiness/normalized-viewer-input.md`,
  `specs/013-tetris-demo-integration/readiness/bounded-viewer-smoke.md`,
  `specs/013-tetris-demo-integration/readiness/diagnostics.md`,
  `specs/013-tetris-demo-integration/readiness/headless-scene-evidence.md`,
  `specs/013-tetris-demo-integration/readiness/generated-template-input-flows.md`,
  `specs/013-tetris-demo-integration/readiness/local-consumer-packages.md`,
  `specs/013-tetris-demo-integration/readiness/generated-consumer-validation.md`,
  `specs/013-tetris-demo-integration/readiness/evidence-graph.md`, and
  `specs/013-tetris-demo-integration/readiness/evidence-audit.md`.
- **Unsupported scope**: Replacing the renderer backend, changing game-specific
  Tetris rules, redesigning all generated app visuals, guaranteeing native
  window support on every CI host, publishing packages to a remote feed, and
  migrating external applications automatically are out of scope.
- **Build-target impact**: `Verify`, `Ci`, `PackLocal`,
  `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateCheck`,
  `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit` may change.
  `DependencyReport` may change only if local consumer package guidance needs
  additional package inventory evidence. `Dev` should change only if required
  for the interactive generated app workflow.

## Success Criteria *(mandatory)*

- **SC-001**: A generated graphical app can complete an initial-screen start
  flow through a viewer key event in automated validation with no raw key
  string comparisons in generated app code.
- **SC-002**: Normalized input validation covers at least arrows, enter, space,
  escape, backspace, letters, digits, function keys, common alternate raw names,
  and unknown keys.
- **SC-003**: A bounded graphical smoke run exits successfully after at least
  one rendered frame and records frame count, elapsed time, output size, and
  renderer mode in readiness evidence.
- **SC-004**: A forced pre-frame rendering failure produces a structured
  diagnostic naming the blocked stage for covered window, surface, renderer,
  swapchain, scene, readback, app, timeout, and unknown-stage failures.
- **SC-005**: Startup-only diagnostics for a short graphical smoke run contain
  no repeated per-frame swapchain or scene-drawing messages unless frame
  diagnostics are explicitly enabled.
- **SC-006**: Diagnostic capture tests can assert startup, input, and frame
  categories without reading process stderr.
- **SC-007**: Headless or offscreen scene-level visual evidence can be produced
  for at least one generated graphical app scene in an environment without
  opening a native desktop window, or reports an explicit
  unsupported-environment diagnostic when the host cannot support it.
- **SC-008**: Generated template validation covers start, options, primary
  interaction, and restart or exit flows through viewer key events for every
  generated screen that supports those actions.
- **SC-009**: Local consumer package guidance prints the feed path, package
  identities, package versions, consumer configuration snippet, and restore
  command in a single documented workflow.
- **SC-010**: A stale local package feed is reported as setup drift before
  generated consumer build or rendering failures are attributed to app code.
- **SC-011**: A maintainer can run the generated consumer graphical validation
  path from fresh package output to first-frame or visual evidence in under 10
  minutes on a supported local development machine.

## Assumptions

- The Tetris demo integration analysis is the source of truth for this feature.
- The feature targets reusable framework and generated-template improvements,
  not Tetris-specific game behavior.
- Common generated graphical apps need keyboard-only start, options, main
  interaction, and restart flows unless a template explicitly opts out.
- Real graphical viewer startup and deterministic visual evidence are both
  valuable; neither fully replaces the other.
- Unsupported host rendering conditions should be visible and actionable rather
  than treated as successful product evidence.
