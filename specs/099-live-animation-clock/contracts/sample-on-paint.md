# Contract: Sample-on-Paint, Identity-at-Rest, Scoped Repaint

**Scope**: `src/Controls/RetainedRender.fs[i]` paint pass (`init`/`step`), reusing
feature-073 `FS.Skia.UI.Scene.Animation.applyAt`. Internal surface only.

## C1 — Active clocks are sampled into the paint

In the retained paint pass, when an identity's `StateByIdentity` entry has an **active**
clock, the painted node for that identity is derived through
`Animation.applyAt clock.Elapsed clock.Anim`, so the rendered opacity / transform / color
reflects the in-progress sampled value for **that** frame (FR-002).

- Sampling is pure: same `(Elapsed, Anim)` ⇒ same node (FR-006).
- The sampled value is fed into the **paint** layer only — never into layout/measure
  (paint-level transform/opacity/color, not flex geometry), so R2 incremental measure is
  untouched (FR-010, Assumptions).

## C2 — Identity-at-rest is byte-identical

An identity with **no** active clock (`Animation = None`, including a settled-and-dropped
clock) is painted exactly as the pre-R4 static render — **no** animation attribute is
emitted, and the frame is **byte-identical** to the pre-R4 golden (FR-005 / SC-003).

- `Animation.applyAt`'s identity-at-rest lowering guarantees a settled animation lowers to
  the same node as the static widget, so the converged final frame reaches *exactly* the
  snapped target appearance (resolves the FR-003 vs FR-005 interaction).
- The at-rest count (recompute / animation-output) for a frame with no active clock is
  **zero**.

## C3 — Repaint stays scoped to the animating subtree

Advancing and sampling an animating identity's clock contributes a per-frame change scoped
to **that identity's own subtree**; it does **not** force a whole-tree repaint or
re-measure. The presence of one active animation does not invalidate the at-rest fast path
for other identities (FR-002 vs FR-005/FR-010 resolution; SC-006). The work-reduction
metric shows animation-driven repaint bounded to active identities.

## Verification fixed points

- **animates-vs-snaps (SC-001)**: across consecutive frames during a hover/focus
  transition, at least one **intermediate** sampled appearance is present before the
  target; a no-seam build snaps in one step and fails.
- **at-rest (SC-003)**: a frame with no active clock equals the pre-R4 golden, zero count.
- **determinism (SC-004)**: two runs over an identical injected-delta sequence produce
  identical sampled output.
- **scoped repaint (SC-006)**: the work-reduction metric attributes repaint only to active
  identities; no whole-tree invalidation.
