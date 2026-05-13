# Research: Keyboard State Display Element

## Decision: Extend `FS.Skia.UI.KeyboardInput`

**Rationale**: The existing keyboard input framework already owns `InputRuntime`, `LayoutStateView`, `InputEffect`, diagnostics, and a basic `renderLayoutState` scene. A standard display element is a projection of that state, so placing it beside the runtime keeps applications from wiring a second model and keeps tests on the same public surface.

**Alternatives considered**:

- Add a separate `KeyboardStateDisplay` module. Rejected because it would duplicate keyboard state types or force a second public namespace for one tightly coupled projection.
- Keep the display sample-only. Rejected because FR-001 requires a standard reusable element.

## Decision: Provide a Pure Display Model Before Rendering

**Rationale**: FR-001a and FR-015 require structured display data for tests and alternate renderers. A pure model also keeps compact/expanded selection, top-context labels, pending sequence, diagnostics, and resolved-command feedback testable without scene inspection.

**Alternatives considered**:

- Return only `Scene`. Rejected because scene descriptions are too coarse for asserting display semantics.
- Expose only the existing `LayoutStateView`. Rejected because it lacks display density, condensed stack, recent command, actionable diagnostic, hidden mode, and explicit omission metadata.

## Decision: Use Display Density and Visibility Options

**Rationale**: The spec requires hidden, compact, and expanded behavior. A `KeyboardStateDisplayOptions` record with a visibility/density union keeps rendering configurable without adding optional arguments or multiple overloaded entry points.

**Alternatives considered**:

- Boolean flags only. Rejected because combinations like hidden plus expanded are unclear.
- Separate render functions for each density. Rejected because model generation should share ordering and omission rules.

## Decision: Compact Mode Prioritizes Orientation Fields

**Rationale**: Clarification requires active layout, active top context, condensed stack, and state to remain visible when space is limited. Compact model construction should mark lower-priority hints as omitted before it hides orientation data.

**Alternatives considered**:

- Always show all available fields and rely on clipping. Rejected because FR-011 forbids overlap and FR-010 defines compact priorities.
- Hide the stack first. Rejected because the stack explains nested popup/held behavior, one of the core user values.

## Decision: Active Labels Come Only From the Active Top Context

**Rationale**: Clarification and FR-007 restrict label hints to bindings available in the active top context. This avoids turning the element into a full cheat sheet and prevents misleading labels from lower stack frames.

**Alternatives considered**:

- Show all labels in the active layout. Rejected because it includes keys that may not resolve in the current mode.
- Show every binding from every stack frame. Rejected because it conflicts with current-context-only guidance.

## Decision: Diagnostic Display Selects the Most Recent Actionable Diagnostic

**Rationale**: FR-009 and clarification require the display to avoid becoming a log viewer. Actionable diagnostics are warnings, errors, and fatal diagnostics; informational stale/no-op entries can remain in runtime diagnostics but do not dominate the display when a stronger diagnostic is available.

**Alternatives considered**:

- Display all diagnostics. Rejected as log-viewer behavior.
- Display only errors. Rejected because warning-level recovery diagnostics such as focus-loss cleanup are actionable.

## Decision: Render Partial State for Invalid or Missing Layouts

**Rationale**: FR-016 requires available partial state and the most recent actionable diagnostic when layout information is unavailable or invalid. The display builder should not throw during rendering. It should preserve stack/state/diagnostic information and mark layout display as missing or invalid.

**Alternatives considered**:

- Fail scene construction. Rejected because UI failure would hide the diagnostic needed to recover.
- Substitute the first configured layout silently. Rejected because it can misrepresent the active layout.

## Decision: Render With Existing `Scene` Primitives

**Rationale**: The repository already represents UI as pure `Scene` values and tests can inspect `Scene.describe`. Using existing rectangles and text runs avoids new rendering dependencies and keeps the feature aligned with the package.

**Alternatives considered**:

- Introduce a widget framework abstraction. Rejected as unnecessary for this feature.
- Draw directly in sample host code. Rejected because applications would still need custom visualization.
