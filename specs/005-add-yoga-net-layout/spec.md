# Feature Specification: Yoga.Net Layout for UI Elements and Widgets

**Feature Branch**: `005-add-yoga-net-layout`  
**Created**: 2026-05-14  
**Status**: Draft  
**Input**: User description: "add https://github.com/chenrensong/Yoga.Net for ui elements and widgets layout"

## Clarifications

### Session 2026-05-14

- Q: What layout semantics should v1 expose? → A: Flex-style row/column/wrap layout only for v1; absolute/overlay positioning remains outside automatic layout
- Q: How should v1 size text and custom-drawn content? → A: Support custom content measurement callbacks in v1 for text and custom elements
- Q: How should layout invalidation behave after changes? → A: Only affected subtrees must be invalidated and re-evaluated after relevant changes
- Q: How should recoverable layout problems behave at runtime? → A: Return structured diagnostics and safe fallback bounds for recoverable layout problems
- Q: Which geometry units should automatic layout use? → A: Layout uses logical UI coordinates; rendering and hit testing apply deterministic pixel snapping

## Change Classification

- **Tier**: Tier 1 contracted change.
- **Public API Impact**: Adds or changes public layout-facing UI element and widget capabilities so applications can request automatic layout instead of manually calculating every child position.
- **Dependency Impact**: Adds Yoga.Net as the layout engine dependency for automatic element and widget layout.
- **Verification Approach**: Validate public layout contracts through `.fsi` surface evidence, semantic layout tests, widget integration tests, sample smoke evidence, and package surface-area baseline updates.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Arrange Child Elements Automatically (Priority: P1)

As an application author, I want UI elements to arrange their children through standard layout rules so I can build screens without manually computing each child rectangle.

**Why this priority**: Manual positioning does not scale beyond simple demos. Automatic child layout is the baseline capability needed before higher-level widgets can be composed reliably.

**Independent Test**: Can be tested by creating a container with multiple children, assigning layout intent such as direction, size, margin, padding, alignment, and spacing, then verifying that each child receives the expected measured bounds.

**Acceptance Scenarios**:

1. **Given** a container with three visible child elements and available space, **When** layout is evaluated, **Then** each child receives a non-overlapping bounds rectangle inside the parent bounds.
2. **Given** a child with margin and the parent with padding, **When** layout is evaluated, **Then** the child bounds respect both the parent padding and child margin.
3. **Given** a row or column layout with alignment settings, **When** child sizes differ, **Then** children are positioned according to the requested main-axis and cross-axis alignment.

---

### User Story 2 - Compose Standard Widgets (Priority: P2)

As a UI library consumer, I want common widgets to participate in the same layout system as lower-level elements so mixed screens remain predictable.

**Why this priority**: Widgets become hard to use when each one follows a separate sizing model. A shared layout contract lets applications combine panels, controls, overlays, and custom elements.

**Independent Test**: Can be tested by composing multiple standard widgets and custom elements in the same parent, resizing the parent, and verifying that every participant updates consistently.

**Acceptance Scenarios**:

1. **Given** a parent widget that contains standard controls and custom elements, **When** the parent is measured and arranged, **Then** all children participate in one layout pass and receive consistent bounds.
2. **Given** an application resizes a host window, **When** available space changes, **Then** affected widgets update their positions and sizes without stale bounds or overlap.
3. **Given** a widget declares fixed, flexible, or content-driven sizing intent, **When** layout is evaluated, **Then** the widget receives bounds that match that intent within parent constraints.

---

### User Story 3 - Diagnose Layout Problems (Priority: P3)

As an application author, I want layout failures and constraint conflicts to be visible and testable so I can correct UI definitions quickly.

**Why this priority**: Automatic layout hides positioning details unless diagnostics expose why a result differs from intent. Clear feedback is necessary for adoption in real applications.

**Independent Test**: Can be tested by defining impossible or conflicting layout constraints and verifying that the system reports actionable diagnostics while preserving a safe renderable result.

**Acceptance Scenarios**:

1. **Given** a child requests more space than the parent can provide, **When** layout is evaluated, **Then** the system produces bounded child placement and reports the constraint conflict.
2. **Given** invalid layout values are supplied, **When** the layout is prepared, **Then** the system rejects or normalizes those values with an actionable diagnostic.
3. **Given** a layout tree contains hidden or collapsed elements, **When** layout is evaluated, **Then** visible siblings still receive stable bounds and diagnostics distinguish hidden elements from layout failures.

### Edge Cases

- Parent available size is zero, negative, infinite, or otherwise invalid.
- A child has no explicit size and no measurable content.
- A child requests a size larger than the parent can provide.
- Deeply nested containers must remain deterministic and avoid accumulated rounding errors.
- Rendered and hit-tested geometry must remain aligned after deterministic pixel snapping.
- Hidden, collapsed, or disabled elements appear among visible siblings.
- Layout is re-evaluated repeatedly during resize or animation.
- Text or content measurement changes after the first layout pass.
- Layout diagnostics are produced while the UI still needs to render safe fallback bounds.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST provide a standard layout capability for UI elements and widgets that computes child bounds from declared layout intent and available parent space.
- **FR-002**: System MUST support flex-style container layout properties for direction, wrapping behavior, alignment, justification, padding, margin, gap or spacing, fixed size, minimum size, maximum size, and flexible growth or shrink behavior.
- **FR-003**: System MUST allow both built-in widgets and custom elements to participate in the same measure and arrange flow.
- **FR-004**: System MUST expose computed layout bounds as structured data that can be asserted by tests without requiring visual screenshot inspection.
- **FR-005**: System MUST keep rendering behavior separate from layout intent so element drawing consumes computed bounds rather than recalculating layout independently.
- **FR-006**: System MUST update affected layout bounds when parent size, child visibility, child layout intent, or content measurement changes.
- **FR-006a**: System MUST invalidate and re-evaluate only affected layout subtrees after relevant changes, while preserving stable computed bounds for unaffected siblings and ancestors.
- **FR-007**: System MUST preserve deterministic results for the same layout tree, available size, and measurement inputs.
- **FR-008**: System MUST prevent visible child overlap for valid flex-style layout configurations.
- **FR-009**: System MUST support non-fatal safe fallback behavior when layout cannot satisfy all requested constraints, including bounded placement and actionable diagnostics.
- **FR-010**: System MUST provide diagnostics for invalid layout values, unsatisfied constraints, unmeasurable content, and layout trees that cannot fit within available space.
- **FR-010a**: System MUST return structured diagnostics and safe fallback bounds for recoverable layout problems rather than raising runtime errors or silently clamping.
- **FR-011**: System MUST allow applications to inspect or log layout diagnostics without requiring a debug-only build.
- **FR-012**: System MUST include examples that demonstrate automatic layout for nested elements, mixed widgets, resizing, flexible sizing, hidden elements, and diagnostic scenarios.
- **FR-013**: System MUST maintain compatibility with existing manually positioned scene composition so current applications can migrate incrementally.
- **FR-014**: System MUST document how standard layout participates in pointer hit testing, keyboard focus regions, and visual bounds so interaction remains aligned with rendered geometry.
- **FR-015**: System MUST keep absolute-positioned and overlay composition outside the v1 automatic layout tree while preserving existing manual composition paths for those cases.
- **FR-016**: System MUST support custom content measurement callbacks for text and custom elements in v1 so content-driven widgets can report preferred sizes during layout.
- **FR-017**: System MUST compute layout in logical UI coordinates and apply deterministic pixel snapping at rendering and hit-test boundaries so visual and interaction geometry stay aligned.

### Key Entities

- **Layout Tree**: Hierarchical representation of elements and widgets participating in automatic layout.
- **Layout Node**: One element or widget in the layout tree, including declared layout intent, visibility state, measurement behavior, and computed bounds.
- **Layout Intent**: User-facing sizing and placement preferences such as direction, alignment, padding, margin, spacing, and flexible sizing.
- **Available Space**: The parent-provided constraints within which a layout node must measure and arrange itself.
- **Computed Bounds**: Final logical-coordinate rectangle assigned to an element or widget after layout evaluation.
- **Measured Content**: Preferred size reported by built-in content or custom element measurement callbacks before final arrangement.
- **Layout Diagnostic**: Actionable message describing invalid layout input, unsatisfied constraints, or fallback behavior.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Application authors can build a nested screen of at least 20 elements without writing manual child-coordinate calculations.
- **SC-002**: 100% of representative layout tests produce deterministic computed bounds across repeated runs with the same inputs.
- **SC-003**: Valid row, column, alignment, margin, padding, spacing, and flexible sizing scenarios produce no visible child overlap in automated tests.
- **SC-004**: Layout re-evaluation for a representative tree of 200 nodes completes within 16.7ms per resize evaluation on the documented development profile, with the measurement command and environment captured in readiness evidence.
- **SC-005**: Existing manually positioned scene examples continue to render successfully after the layout capability is added.
- **SC-006**: At least 90% of invalid or conflicting layout sample cases produce an actionable diagnostic that identifies the affected node or constraint.
- **SC-007**: A first-time application author can reproduce a nested widget layout from the examples in under 20 minutes.
- **SC-008**: 100% of representative text and custom element measurement tests produce computed bounds that reflect the supplied preferred sizes and parent constraints.
- **SC-009**: In representative invalidation tests, 100% of unchanged sibling subtrees keep identical computed bounds after an unrelated subtree changes.
- **SC-010**: 100% of recoverable invalid layout tests return structured diagnostics and bounded fallback geometry without terminating the render flow.
- **SC-011**: 100% of representative scale-factor tests keep rendered bounds and hit-test regions aligned after deterministic pixel snapping.

## Assumptions

- Yoga.Net is the chosen dependency for the underlying layout algorithm.
- The feature targets desktop Skia UI applications already supported by the project.
- Existing manual scene composition remains supported for applications that do not opt in to automatic layout.
- The first release focuses on layout calculation and widget participation, not a full visual designer.
- The first release exposes flex-style row, column, and wrap layout semantics only; absolute-positioned and overlay layout semantics are deferred.
- Accessibility metadata is outside the baseline scope, but focus regions and hit-test bounds must stay aligned with computed visual bounds.
- Layout diagnostics are runtime-visible structured data, not only exception messages; recoverable layout problems do not terminate normal rendering.
- Layout uses logical UI coordinates as its canonical geometry representation.
