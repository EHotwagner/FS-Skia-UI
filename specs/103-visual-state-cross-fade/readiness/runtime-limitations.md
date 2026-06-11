# Runtime limitations + permanent non-goals — feature 103 (R6, T002/T003)

## Supported runtime

R6 touches the framework wherever it runs: a **.NET 10 desktop** host rendering through **Vulkan**
via the **SkiaSharp preview** native binding, on Windows and Linux desktop (`net10.0`).
**unsupported macOS/mobile/browser** — there is **no software-renderer fallback**; these are out of
scope for the framework and therefore for this feature. R6's proof is GPU-free: the cross-fade is
deterministic scene assembly (two cached own-scene snapshots composited by the public opacity
sampler), exercised through `RetainedRender.step` with injected `TimeSpan` deltas — no live window,
no GPU, no wall-clock.

## What R6 adds (and what it deliberately does not)

R6 makes a live visual-state transition a **genuine cross-fade**: the prior state's cached own-scene
snapshot fades OUT under the next state's own-scene fading IN, both driven by the **public**
`Animation.applyAt` opacity tween. The single new piece of state is the internal `AnimationClock.From`
prior-snapshot field. The decisive grounding fact: `Animation.applyAt` **never applies the `Color`
tween** (it samples opacity/transform only), and a single `Color` tween cannot represent the
multi-channel `Foreground`/`Fill`/`Stroke` paint `Style.resolve` produces — so the roadmap's loose
"feed the style delta into `applyAt`" is not a real path, and the `AnimationClock` doc is reconciled to
match the shipped snapshot-composite mechanism (FR-009).

## Out of scope / permanent non-goals (deferred scope)

- **Consumer-facing transition authoring API**, configurable **easing/duration** knobs, or any open
  per-property animation surface — the animated quantity is the node's own painted appearance, closed
  and token-derived upstream by `Style.resolve` (FR-003).
- **Extending `FS.Skia.UI.Scene` `applyAt`** to drive the `Color` tween (a second-package Tier-1
  blast radius) — rejected in research; R6 reuses the public opacity sampler unchanged.
- **Transform-channel animation** on a state change (opacity-driven snapshot blend only).
- Animating any channel `Style.resolve` does not already produce in the two snapshots.
- Default arrow-key routing for `Chart`/`Graph`/`Progress` (a separate R8-noted decision).
- **Permanent roadmap non-goals preserved**: no data binding, no dependency/attached properties, no
  CSS selectors, no lookless template engine. R6 is the LAST roadmap rung; no successor.

## Failure diagnostics

No new runtime failure path is introduced. The cross-fade is an assembly-time overlay gated to active
(mid-flight) clocks only; at rest and at settle the fast/settle path is **unchanged**, so the two
stable points stay byte-identical to the static render. The existing R1/R2/R4/R5/R7 property and unit
suites stay green and byte-identical, which is the evidence that the overlay never perturbs the at-rest
or settled output.
