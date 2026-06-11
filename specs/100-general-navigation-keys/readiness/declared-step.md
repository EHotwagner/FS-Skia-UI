# Declared-step value move + non-regressive golden (feature 100, R5, T014)

evidence-kind=declared-step
status=observed
seam=runInteractiveApp (real RetainedRender.init/step + ControlsElmish.routeFocusedKey)

## Non-default-step slider steps by its DECLARED step

A focused slider declared with a **non-default** `NavRange { Step = 5.0; Min = 0.0; Max = 100.0 }`
(value 50), routed through the real seam:

| Pressed key | Current value | Dispatched value | Closed Nav payload |
|-------------|---------------|------------------|--------------------|
| ArrowRight | 50.0 | 55.0 | `SteppedValue 55.0` |
| ArrowLeft | 50.0 | 45.0 | `SteppedValue 45.0` |

The move is **exactly** the declared step (5.0), **not** the pre-R5 hardcoded 0.1 — a pre-R5 build
steps by 0.1 regardless and fails. Min/max clamp is a verified no-op: at Max (100) + ArrowRight and at
Min (0) + ArrowLeft dispatch **nothing** (FR-009).

## Default-step slider is byte-identical to the pre-R5 numeric path (FR-007 golden)

A focused **default-step** slider (`Accessibility.defaultFor "slider"` → `NavRange { 0.1; 0.0; 1.0 }`),
value 0.5, ArrowRight:

- pre-R5 reference (recomputed with the SAME operations): `Math.Clamp(0.5 + 0.1, 0.0, 1.0)` and its
  `InvariantCulture` string.
- observed dispatched `Payload` string equals that pre-R5 reference **byte-for-byte**, and
  `Nav = Some (SteppedValue (Math.Clamp(0.5 + 0.1, 0.0, 1.0)))`.

The golden compares against the pre-R5 *formula*, not a literal, so floating-point representation is
identical by construction (the resolver applies the same `current + Step` clamp the pre-R5
`steppedValue` did).

## Source

`tests/Elmish.Tests/Feature100NavigationTests.fs` —
`100 US2 declared-step value move at the host seam (SC-002)`.
