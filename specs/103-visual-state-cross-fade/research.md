# Phase 0 Research: True Visual-State Cross-Fade (R6)

All Technical-Context unknowns are resolved here. No `NEEDS CLARIFICATION` remain.

## Decisive finding: `applyAt` does not apply the `Color` tween

`FS.Skia.UI.Scene.Animation.applyAt` (`src/Scene/Animation.fs`) samples **opacity** and
**transform** only. The `Animation.Color : Tween<Color> option` channel is present on the
type and is counted by `isSettled`, but `applyAt` never recolors the target scene by it:

```fsharp
let applyAt elapsed animation target =
    let opacity = sampleOpacity elapsed animation
    let transform = sampleTransform elapsed animation
    if opacity = 1.0 && Transform.isIdentity transform then Lower.unwrap target
    else (* scale opacity, maybe PerspectiveNode for transform *) ...
    // NOTE: animation.Color is never read here.
```

Consequences that reshape R6's approach away from the roadmap's loose wording:

1. The roadmap §11.3 says "feed the style delta into feature-073's existing color-capable
   `applyAt` so paint channels interpolate." **`applyAt` is not color-capable today** — that
   path does not exist without a Scene-package change.
2. `ResolvedStyle` carries **three** color fields (`Foreground`, `Fill`, `Stroke`) plus
   `StrokeWidth`/font. A **single** `Tween<Color>` cannot represent a multi-channel paint
   cross-fade. So even if `applyAt` applied `Color`, one tween is the wrong shape for the job.

This makes the spec's explicit alternative the live one: *"or, if a channel is intentionally
out of scope, trim the `AnimationClock` doc to match — close the doc↔behavior gap explicitly"*
(FR-009). R6 realizes the cross-fade by a mechanism other than the standalone `Color` tween and
reconciles the doc accordingly.

## Design fork (the only real decision)

### Option A — Extend Scene `applyAt` to drive the `Color` tween
Make `applyAt` recolor the target by the sampled `Color`, and set the clock's `Color` tween from
the dominant style-color delta.
- **Rejected.** Touches the **`FS.Skia.UI.Scene` public package** surface (`applyAt` is public
  `.fsi` + cross-package baseline), a second-package Tier-1 blast radius for an internal Controls
  behavior. "Recolor a whole scene by one color" is semantically ambiguous (which of
  Foreground/Fill/Stroke?) and cannot express multi-channel paint. Highest risk, worst fit.

### Option B — Per-channel `ResolvedStyle` interpolation + re-paint
Carry the prior `ResolvedStyle` (or `VisualState`), and at each sampled frame re-paint the node's
own scene from `lerp(priorStyle, nextStyle, easedT)` per color field via `Animation.lerpColor`.
- **Rejected (as primary).** `ControlInternals.paintNode` resolves style internally from a
  `Control`'s attributes; there is no clean "paint own node from an arbitrary `ResolvedStyle`" seam,
  so this needs a new paint-from-style entry point — more surface and per-frame re-painting cost.
  It is the most literal reading of "interpolate the resolved style channels," but heavier than
  needed given Option C achieves the same observable result by reusing cached snapshots.

### Option C — Two-layer snapshot cross-fade *(CHOSEN)*
Composite the two **already-cached static own-scene snapshots** of the retained identity: the
*prior* state's `Fragment.OwnScene` (from the matched previous-frame retained node) **under** the
*next* state's `Fragment.OwnScene`, each driven by the **public** `Animation.applyAt` opacity tween
— prior fades `1 → 0`, next fades `0 → 1`.

```
sampleOnPaint clock priorOwn nextOwn =
    let fadeOut = { Animation.empty with Opacity = Some { Start=1.0; End=0.0; Duration; Easing=EaseOut } }
    let priorLayer = applyAt clock.Elapsed fadeOut (Scene.group priorOwn)   // prior fading out
    let nextLayer  = applyAt clock.Elapsed clock.Anim (Scene.group nextOwn) // next fading in (existing tween)
    [ { Nodes = [ priorLayer; nextLayer ] } ]
```

- **Decision: adopt Option C.**
- **Rationale.**
  - *Correct cross-fade, both directions.* For a region painted in both states, source-over of
    `prior·(1-t)` then `next·t` yields a displayed color **strictly between** the endpoints
    (SC-001). Growing paint (Normal→Hover gains a ring) fades the new region in; shrinking paint
    (Hover→Normal loses a ring) fades the old region out — both handled, unlike "next over
    transparent" (today's bug) which can only grow.
  - *Byte-identity for free at the stable points.* The change is confined to the **active-clock**
    branch of the assemble walk. At rest there is no clock → fast path returns the cached
    `SubtreeScene` (FR-004). At settle the clock is dropped / `clockActive` is false → the node uses
    `ownStatic` verbatim, so the **final frame is byte-identical to the snapped static render**
    (FR-005) with **no change to the settle path**. The composite recipe never has to itself be
    byte-identical at `t=1`.
  - *Reuses the public Scene API.* No `FS.Skia.UI.Scene` source/`.fsi` change; only the **internal**
    Controls `AnimationClock` gains a snapshot field. Smallest faithful surface.
  - *Closed channel set by construction.* The animated thing is the node's own painted appearance,
    whose colors are token-derived upstream by `Style.resolve`. No open per-property surface (FR-003).
- **Alternatives considered**: A and B above.

## Retarget on a second mid-flight state change (FR-007)

`updateClockForState` already retargets the *opacity* from the current sampled value (no snap). For
the prior snapshot under Option C, the chosen rule: **on a fresh transition, capture
`prior = matched previous retained node's `Fragment.OwnScene`** for that identity; **on a mid-flight
retarget (state changes again before settle), set the new `prior` to the *previous target's* own
snapshot** (the layer that was fading in becomes the layer that now fades out) and reset `Elapsed`.
This is the vector-scene analogue of "continue from what is displayed": the most-recently-targeted
appearance becomes the new "from," and the freshest target fades in. It avoids rasterizing the live
composite (scenes are vector lists, not bitmaps) while preserving "no hard snap to a stale endpoint."
A settled return-to-`Normal` clock is still **dropped** (existing rule), discarding any snapshot.

## Determinism & evidence strategy (FR-006, R4 parity)

The clock advances solely from **injected** `TimeSpan` deltas (no wall-clock); replaying a fixed delta
sequence reproduces frames exactly. Evidence is sampled at explicit elapsed points through
`RetainedRender.step`. The repo has **no `testProperty`** — use `Check.One` with a fixed generator/seq
as feature 099 did. Mid-flight frames are treated as *animation, not golden* (no golden churn budget for
intermediate frames); only the two stable points carry byte-identity obligations.

## Doc reconciliation (FR-009)

The `AnimationClock` `.fsi` doc currently advertises "a focus-ring fade (opacity) / **press tint
(color)** can be expressed … opacity/transform/color tweens, sampled by `Animation.applyAt`." Since the
cross-fade is realized by **compositing prior/next static snapshots under the opacity tween** — not by a
standalone `Color` tween that `applyAt` would sample — the doc is reconciled to state exactly that: the
opacity tween is the live channel; the paint cross-fade comes from the two-snapshot composite; the
standalone color-tween claim (which `applyAt` never honored) is dropped. This closes the same
doc-overstates-behavior gap R8 (feature 102) targeted, which R6 must not reopen.

## Open questions

None. The single fork is resolved (Option C); retarget, determinism, and doc reconciliation are decided.
