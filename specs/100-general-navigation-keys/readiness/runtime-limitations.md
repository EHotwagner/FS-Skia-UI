# Runtime limitations + permanent non-goals — feature 100 (R5, T008)

## Supported runtime

General navigation runs wherever the framework runs: a **.NET 10 desktop** host rendering through
**Vulkan** via the **SkiaSharp preview** native binding. Targets are Windows and Linux desktop
(`net10.0`). **unsupported macOS/mobile/browser** — there is **no software-renderer fallback**; these
are out of scope for the framework and therefore for this feature. Navigation itself is pure data
routing and is platform-independent; only the live evidence host depends on the desktop runtime.

## Scope handling (Out of Scope / Assumptions, FR-001…FR-010)

- **No consumer-facing custom key-binding / remapping API** and **no free-form per-key handler
  surface** — both would drift toward the rejected routed-event system. Navigation flows only from the
  declared role + `NavigationKeys`/`NavRange` metadata and the closed `NavIntent`/`NavPayload` set.
- **No authored navigation DSL** and no type-ahead / incremental-search selection.
- **Single-selection moves only** — no multi-select range extension (Shift-arrow), no drag-reorder.
- **No focus-traversal (Tab/Shift-Tab) or activation (Space/Enter) change** — those are E4 (feature
  094), unchanged. R5 extends only the `Navigate` arm.
- **Boundary policy defaults to clamp** — wrap is opt-in metadata, not shipped here; a boundary move is
  a designed **no-op with no spurious dispatch** (FR-009).
- **Representative coverage** — a value role (slider), a linear-selection role (radio-group/tab), and a
  grid role (data-grid). Full-52-control navigation coverage beyond these is a later fitness pass.
- **Closed-model permanence** — CSS selectors, attached/dependency properties, lookless templates, and
  data binding remain permanent roadmap non-goals; navigation does not introduce any of them.
- This is the **final** roadmap remediation (R1–R5) — **no successor**.

## Failure diagnostics

No new failure path is introduced. `Focus.route` and the host per-intent resolver are pure, total
functions of the declared role + `NavigationKeys`/`NavRange` metadata and the live selection/value
model. Every honest failure mode is normal control flow, not an error: no matching `NavigationKeys` ->
`Fallthrough`; an empty selection group or an unresolvable current index -> no dispatch; a boundary
clamp (first/last item, min/max value, grid edge) -> no dispatch past the bound. These are asserted as
verified outcomes ("nothing happened" is a tested result, not a swallowed error). Missing-artifact
failures are the existing readiness-gate classes (a required readiness file absent or malformed -> the
owning gate reports it). The actionable signal for the live path is the responds-vs-renders evidence
primitive: a pre-R5 / un-wired build dispatches nothing on a focused radio-group arrow and fails
`responds-vs-renders.md`. `Accessibility.validate` continues to flag a focusable control with no
operable key set.
