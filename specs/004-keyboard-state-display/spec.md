# Feature Specification: Keyboard State Display Element

**Feature Branch**: `004-keyboard-state-display`  
**Created**: 2026-05-13  
**Status**: Draft  
**Input**: User description: "add a standard skia ui element displaying the current keyboard state with additional useful information like state of permanent layouts/stack if nested layouts..... ki can show you current layer/layout. something similar but more sophisticated. https://ki-editor.org/docs/cheatsheet"

## Clarifications

### Session 2026-05-13

- Q: What information must compact mode always prioritize when space is limited? → A: Active layout, active top context, condensed stack, and state
- Q: How should the display handle diagnostics without becoming a log viewer? → A: Show only the most recent actionable diagnostic
- Q: Which key labels count as current-context hints? → A: Only bindings available in the active top context
- Q: What should render when layout information is unavailable or invalid? → A: Available partial state and the most recent actionable diagnostic
- Q: What should the feature expose for applications and tests? → A: Pure display model plus the standard rendered UI element

## User Scenarios & Testing *(mandatory)*

### User Story 1 - See Current Keyboard Context (Priority: P1)

As a keyboard-driven application user, I want a standard on-screen keyboard state element so I can always see the active layout, active mode, and current layer context before pressing the next key.

**Why this priority**: The primary value is orientation. Modal and layered keyboard systems become hard to use when users cannot see which context is currently active.

**Independent Test**: Can be tested by displaying the element in a sample application, changing layouts and modes through keyboard input, and verifying that the element updates immediately with the current active context.

**Acceptance Scenarios**:

1. **Given** a keyboard input configuration with an active base layout, **When** the application displays the keyboard state element, **Then** the element shows the active layout name, active mode, and current state if one exists.
2. **Given** the user switches to another layout, **When** the keyboard state changes, **Then** the element updates to show the new active layout without requiring application-specific drawing code.
3. **Given** the user enters a popup or held layer, **When** that layer is active, **Then** the element shows the full active stack from base context to top context.

---

### User Story 2 - Understand Nested Layers and Permanent Contexts (Priority: P2)

As a power user, I want the keyboard state element to distinguish permanent layouts, stateful modes, popup layers, held layers, and nested stacks so I can understand exactly why a key will behave differently.

**Why this priority**: Advanced modal workflows need more than a single layout label. Users need to see the persistent base context and temporary overlays at the same time.

**Independent Test**: Can be tested by activating a stateful base mode, pushing nested popup and held layers, and verifying that the display distinguishes each layer type and the active top layer.

**Acceptance Scenarios**:

1. **Given** a stateful permanent mode with a selected state, **When** the element renders, **Then** it shows the stateful mode and its selected state as persistent context.
2. **Given** a temporary held layer is active above the permanent mode, **When** the element renders, **Then** it identifies the held layer separately from permanent context.
3. **Given** a popup layer is nested above another layer, **When** the element renders, **Then** it shows the ordered stack and highlights the active top layer.

---

### User Story 3 - Learn Available Keys in the Current Context (Priority: P3)

As a new user or application author, I want the element to optionally show useful current-context hints so the display can act like a richer version of a current layer indicator without becoming a full cheat sheet.

**Why this priority**: Hints make layered input discoverable, but the core orientation display must work even when hints are disabled or space is limited.

**Independent Test**: Can be tested by enabling hints in a sample application and verifying that the element lists relevant labels, pending sequences, and recently resolved commands for the current context.

**Acceptance Scenarios**:

1. **Given** display hints are enabled, **When** a layout is active, **Then** the element can show visible key labels for bindings available in the active top context.
2. **Given** the user has begun a key sequence, **When** a pending sequence exists, **Then** the element shows the pending sequence and whether a timeout or disambiguation is active.
3. **Given** a command was just resolved, **When** the element updates, **Then** it can show the most recent command or diagnostic for short-term feedback.

### Edge Cases

- Active layout information is unavailable or invalid; the element renders available partial state and the most recent actionable diagnostic.
- The mode stack is deeper than the available display width.
- A key sequence is pending and the top layer changes before timeout.
- Hints are disabled, but state and stack information must remain visible.
- The application has very small available display space.
- The active layout uses custom labels or symbols rather than alphabetic keys.
- Multiple held layers are active and one is released out of order.
- The keyboard state has diagnostics; the element shows only the most recent actionable diagnostic.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST provide a standard keyboard state display element that applications can use without writing their own state visualization.
- **FR-001a**: System MUST provide a pure keyboard state display model in addition to the standard rendered UI element.
- **FR-002**: System MUST show the active layout identifier and display name.
- **FR-003**: System MUST show the active mode stack in order from persistent base context to active top context.
- **FR-004**: System MUST distinguish permanent/stateful contexts from popup contexts and temporary held contexts.
- **FR-005**: System MUST show the active state for any stateful context when a state is present.
- **FR-006**: System MUST identify the active top context that will receive the next key input.
- **FR-007**: System MUST optionally show key labels only for bindings available in the active top context.
- **FR-008**: System MUST optionally show pending key sequences and disambiguation/timeout state when present.
- **FR-009**: System MUST optionally show the most recent resolved command and only the most recent actionable diagnostic.
- **FR-010**: System MUST support compact and expanded display modes so applications can use the element in small status areas or richer inspector panels; compact mode MUST prioritize active layout, active top context, condensed stack, and active state before optional hints.
- **FR-011**: System MUST remain readable when stack entries or labels are too long by truncating, wrapping, or otherwise preserving layout without overlap.
- **FR-012**: System MUST make hidden or omitted details discoverable in expanded mode when compact mode cannot show everything.
- **FR-013**: System MUST update when layout, mode stack, stateful mode state, held modes, pending sequence, resolved command, or diagnostic state changes.
- **FR-014**: System MUST support a disabled or hidden state so applications can opt out of rendering the element while still using keyboard input.
- **FR-015**: System MUST expose the pure display model as structured display data for tests and alternate application renderers to verify the same current keyboard state.
- **FR-016**: System MUST render available partial state and the most recent actionable diagnostic when active layout information is unavailable or invalid.

### Key Entities

- **Keyboard State Display Element**: Standard visual element that presents current keyboard input context.
- **Keyboard State Display Model**: Pure structured representation of the keyboard state display, used by the standard element, tests, and alternate renderers.
- **Keyboard State Snapshot**: The current active layout, mode stack, active stateful state, held layers, pending sequence, recent command, and recent diagnostic at a point in time.
- **Context Stack Entry**: One visible entry in the active stack, including its name, kind, state, and whether it is the active top context.
- **Layout Label Set**: Labels for physical keys that have bindings available in the active top context.
- **Display Mode**: The chosen presentation density, such as compact status display or expanded inspector display.
- **Context Hint**: Optional user-facing hint such as a pending sequence, recent command, or relevant key label.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: In a sample application, users can identify the active layout and top keyboard context within 2 seconds after any keyboard state change.
- **SC-002**: 100% of tested layout switches, popup layer pushes, held layer pushes/releases, state transitions, and focus-loss cleanup updates are reflected in the display element.
- **SC-003**: The compact display fits in a single status area without text overlap for stack depths up to 4 and layout labels up to 12 visible keys.
- **SC-004**: The expanded display shows full stack details, active state, pending sequence, recent command, and recent diagnostic for all representative test scenarios.
- **SC-005**: Applications can enable the standard element in an existing keyboard-driven sample without defining custom rendering logic for the state display.
- **SC-006**: At least 90% of first-time evaluators can explain which layout and layer are active after using the sample for one minute.

## Assumptions

- The feature builds on the existing keyboard input framework and its current state snapshot concepts.
- The primary display is an in-application visual element, not a console log or external debug window.
- Applications should be able to choose whether the element is hidden, compact, or expanded.
- The element should be useful as a Ki-style current layer/layout indicator, but richer than a single mode label.
- Full keymap cheat-sheet browsing is out of scope for this feature; only current-context hints are included.
- Visual styling should be suitable for desktop Skia UI applications and remain adaptable by consuming applications.

## Change Classification

- **Tier**: Tier 1 contracted change.
- **Public API Impact**: Extends `FS.Skia.UI.KeyboardInput` in `src/Lib/KeyboardInput.fsi` with keyboard state display options, structured display model types, and standard scene-rendering functions.
- **Compatibility Impact**: Existing `layoutState`, `renderLayoutState`, and `renderLayoutStateAt` remain available. Existing render functions may delegate internally only if observable behavior stays compatible.
- **Verification Approach**: Validate the `.fsi` surface through FSI/prelude evidence, semantic tests through the public API, surface-area baseline refresh, sample smoke evidence, and evidence audit.
