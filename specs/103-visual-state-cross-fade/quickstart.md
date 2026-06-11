# Quickstart: Observing the Visual-State Cross-Fade (R6)

This shows how to drive a `Normal → Hover` transition deterministically and observe the interpolated
**color** mid-flight — the proof that the new appearance no longer merely fades in from transparent.
It runs against the internal `RetainedRender.step` surface (the seam features 099/101 test through via
`<InternalsVisibleTo>`); injected `TimeSpan` deltas are the sole time coordinate (no wall-clock).

## 1. Build a control whose Hover style differs in a paint channel

Pick a migrated control (E3/feature 093) whose `Style.resolve` output differs between `Normal` and
`Hover` in a token-derived color field (e.g. background or accent). Stamp the visual state via the
`visualState` attribute (`ControlInternals.visualStateOf` reads it).

## 2. Frame 0 — at rest (no clock)

Step the retained render with the control in `Normal`. Assert:
- the assembled scene is **byte-identical** to `Control.renderTree theme size control` (INV-1 / SC-002);
- no animation attribute is present.

## 3. Frame 1 — flip to Hover (transition starts)

Re-stamp the control as `Hover` and `step` with a **zero** prior delta, then advance with a fixed
sequence of injected deltas (e.g. `[50ms; 50ms; 50ms]` against a 150 ms default duration). The clock's
`From` is captured from the previous frame's `Normal` own-scene snapshot.

## 4. Mid-flight — sample the interpolated color

At an intermediate elapsed (e.g. 75 ms, eased `t ≈ 0.5`), read the composited color of a region painted
in **both** states. Assert it lies **strictly between** the `Normal` and `Hover` endpoint colors
(INV-3 / SC-001) — *not* the `Hover` color fading up from transparent. Compare with `Animation.lerpColor`
endpoints as the strictly-between reference; exact over-composite ratio need not equal 0.5 (mid-flight is
animation, not golden).

## 5. Final frame — settle

Advance past the duration (a large injected delta). The clock settles → the node paints `ownStatic`.
Assert the frame is **byte-identical** to the snapped static `Hover` render for every channel
(INV-2 / SC-003). For a `Hover → Normal` transition the settled clock is **dropped** and output returns
to the at-rest `Normal` byte-identity.

## 6. Determinism

Replay the same injected-delta sequence in a fresh run; assert the sampled frame sequence is identical
(INV-4 / SC-004). A non-positive delta must be a no-op (never rewinds).

## 7. Held state

Hold `Hover` across several frames after settle; assert a single scoped repaint / `Keep` (no per-frame
repaint) — the `VisualStateValue` equality case (feature 099) stays intact (INV-6 / SC-006).

## Gates

Run the gates `./fake.sh build -t Route` prints for this `controls-public-surface`-escalated diff
(sequentially, deterministic order), then `EvidenceGraph` and `EvidenceAudit`. Recapture the per-package
surface baseline (`PerPackageSurface.captureCurrent`) after the internal `AnimationClock` field/doc edit
— `RefreshSurfaceBaselines` does not regenerate per-package snapshots.
