# Contract: Interaction, Text, Accessibility, And Rendering Validation

## Purpose

Controls must be validated as user-facing widgets, not just as compiled
constructors. This contract defines the required behavioral and evidence checks.

## Semantic Behavior

Required readiness path:

```text
specs/010-skia-controls-library/readiness/semantic-tests.md
```

Validation must prove that representative controls can be constructed through
the public `.fsi` surface, composed in a view function, rendered to scene/layout
output, and updated from changed model values.

## Interaction Dispatch

Required readiness path:

```text
specs/010-skia-controls-library/readiness/interaction-tests.md
```

For every supported interactive control category, tests must cover:

- pointer activation where applicable
- keyboard activation where applicable
- disabled and read-only suppression
- focus traversal
- selected/checked/toggled/value changes
- exactly-once message dispatch for the exercised action
- stale handler prevention after model changes

95% of catalog interaction tests must dispatch exactly the expected message and
payload for the exercised user action before readiness approval.

## Text Entry

Text entry validation must cover:

- single-line entry
- multi-line entry
- cursor movement
- text selection
- clipboard commands
- validation feedback
- committed value changes
- cancellation or rejection of invalid input
- environment-aware IME/composition diagnostics

Unsupported environment diagnostics must state the missing platform capability
and must not be reported as a passing implementation path unless the feature
explicitly marks the test skipped with rationale.

## Accessibility

Required evidence is included in:

```text
specs/010-skia-controls-library/readiness/control-catalog.md
specs/010-skia-controls-library/readiness/layout-rendering.md
```

Every supported interactive control must declare:

- role
- accessible name source
- state metadata
- focus order
- keyboard operation behavior
- contrast evidence

Validation must fail when metadata is missing, focus order is unreachable,
keyboard-only operation is not supported where required, or contrast checks
fail.

## Layout And Rendering

Required readiness path:

```text
specs/010-skia-controls-library/readiness/layout-rendering.md
```

Validation must cover:

- layout participation with sizing, alignment, margins, padding, ordering, and
  clipping
- nested containers
- zero-size children
- overlapping children and boundary hit testing
- three viewport sizes
- two DPI scale factors
- supported themes or color modes chosen by implementation
- no unintended text clipping, uncontrolled overlap, or missing visual states

Environment-aware visual validation may distinguish unsupported GPU, font, or
window-system conditions from implementation defects, but it must record that
distinction explicitly.

## Large Data Controls

List and table-like controls must validate:

- 10,000 item data sets
- visible range calculation or equivalent bounded rendering
- responsive scrolling evidence
- single and multiple selection where applicable
- empty state
- item update behavior while the model changes

The validation report must name observed item counts, viewport size, selected
range, and update path.

## Diagnostics

Control diagnostics must identify:

- missing required attributes
- unsupported state combinations
- missing keys where transient state requires stable identity
- failed hit testing
- layout conflicts
- missing accessibility metadata
- contrast failures
- unsupported environment conditions
- stale generated package/skill/template references

Failures must include control id, control kind, source evidence path, and an
actionable message.
