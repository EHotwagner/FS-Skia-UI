# Runtime limitations + permanent non-goals — feature 099 (R4, T007)

## Supported runtime

Live animation runs wherever the framework runs: a **.NET 10 desktop** host rendering through
**Vulkan** via the **SkiaSharp preview** native binding. Targets are Windows and Linux desktop
(`net10.0`). **unsupported macOS/mobile/browser** — there is **no software-renderer fallback**; these
are out of scope for the framework and therefore for this feature.

## Scope handling (Out of Scope / Assumptions, FR-001…FR-010)

- **Injected-delta-only clock** — the sole time input is the host's per-frame `Tick` delta; there is
  **no** `Date.now` / wall-clock read anywhere on the advance/sample path (determinism; the
  environment has no wall-clock source anyway). A **non-positive** delta is a designed **no-op**
  (never rewinds), and a **very-large** delta **clamps** to the settled end (no overshoot).
- **Paint-level only** — animation targets opacity / transform / color via the feature-073
  `Animation` shape; there is **no layout geometry animation** (size/position reflow), so R2
  incremental measure and the scoped-repaint reduction are preserved and at-rest output stays
  byte-identical.
- **Single framework default transition** — a fixed 150 ms `EaseOut` opacity settle, applied to the
  representative R1-migrated interactive kinds. It is **not** a per-control consumer knob; full
  per-channel style-diff animation across all 52 controls is out of scope (tracked with E3/R1).
- **Reuses the feature-073 primitives** — `Animation`/`applyAt`/`isSettled` are consumed, **not**
  re-implemented; there is no new animation engine and no parallel identity scheme (the clock rides
  the existing `RetainedId`-keyed `StateByIdentity` map and its `liveIds` GC filter).
- **No consumer-facing animation authoring API** — no keyframes, timelines, per-control DSL,
  spring/physics, or easing models beyond the feature-073 set; no general animation scheduler beyond
  the per-frame tick advance. The `view : 'model -> Control<'msg>` consumer contract is unchanged.
- **Reduced-motion / opt-out** is **not required** by this feature but the design does **not preclude**
  one (snap = the pre-R4 path). CSS selectors, attached/dependency properties, lookless templates, and
  data binding remain permanent roadmap non-goals.

## Failure diagnostics

No new failure path is introduced. The clock advance/sample/retarget are pure, total functions of the
injected delta (a non-positive delta is normal control flow, not an error). Missing-artifact failures
are the existing readiness-gate classes (a required readiness file absent or malformed → the owning
gate reports it). The only actionable signal is the existing responds-vs-renders / animates-vs-snaps
evidence primitive: a no-seam build snaps and fails `us1-animates-vs-snaps.md`.
