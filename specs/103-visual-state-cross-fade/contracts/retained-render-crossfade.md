# Contract: RetainedRender Visual-State Cross-Fade (internal)

**Scope.** Internal to `FS.Skia.UI.Controls` (`src/Controls/RetainedRender.fs/.fsi`). The
**public** consumer surface (`runInteractiveApp`, the `ControlRenderResult` shape) is **unchanged**.
This contract governs the cross-fade overlay on the live retained render path; it is exercised in
tests via `RetainedRender.step` under `<InternalsVisibleTo>` (the same seam used by features 099/101).

## Inputs

- `clock : AnimationClock` — active clock for an identity, carrying `Anim` (opacity tween),
  `Elapsed` (injected), `Target`, and **`From`** (the prior state's static own-scene snapshot).
- `nextOwn : Scene list` — this frame's static own-scene snapshot for the same identity
  (`RetainedNode.Fragment.OwnScene`, painted at the now-stamped visual state by `Style.resolve`).
- The previous-frame retained tree `prev.Root` — source of the `From` snapshot, matched by
  `RetainedId` at transition start.

## Operation

`sampleOnPaint` composites two opacity-driven layers via the **public** `Animation.applyAt`:

```
priorLayer = applyAt clock.Elapsed (opacity 1.0 → 0.0, Duration, EaseOut) (Scene.group clock.From)
nextLayer  = applyAt clock.Elapsed clock.Anim                              (Scene.group nextOwn)
result     = [ { Nodes = [ priorLayer; nextLayer ] } ]      // prior under, next over
```

- When `clock.From = []` (first entry, or the prior node had no own paint) the prior layer is empty
  and the result is the existing next-fades-in behavior — a safe degenerate case, not a special path.
- The assemble walk only enters this branch when `clockActive clock` is true. A settled clock uses
  `ownStatic` verbatim (no composite), preserving INV-2 with **no change** to the settle path.

## Guaranteed invariants (must hold for the contract to be satisfied)

1. **At-rest byte-identity** (INV-1): with no active clock the assembled scene equals the cached
   `SubtreeScene`; no animation attribute is emitted.
2. **Final-frame byte-identity** (INV-2): once `Elapsed ≥ Duration` the node paints `ownStatic`; the
   frame equals `Control.renderTree`'s static paint of the new state for every channel.
3. **Mid-flight strictly-between** (INV-3): at an intermediate `Elapsed`, a both-states-painted region
   composites to a color strictly between the prior and next endpoint colors (a genuine cross-fade,
   not a fade-in from transparent).
4. **Determinism** (INV-4): the result is a pure function of `(clock, nextOwn)`; identical injected
   deltas ⇒ identical frames; a non-positive delta is a no-op; a past-duration delta settles
   canonically.
5. **Retarget continuity** (INV-5): a mid-flight state change re-seeds `From` from the previous target
   snapshot and resets `Elapsed`; no snap to a stale endpoint.
6. **Scoped repaint of a held state** (INV-6): a settled, held state stays a `Keep`.
7. **Doc agreement** (INV-7): the reconciled `AnimationClock` doc names exactly the driven channels
   (opacity tween + snapshot composite); the standalone `Color`-tween claim is removed.

## Non-goals (contract boundary)

- No `FS.Skia.UI.Scene` source/`.fsi` change (the public `applyAt` is reused, not extended).
- No consumer-facing transition/animation authoring, easing, or duration surface.
- No transform-channel animation on a state change (opacity-driven snapshot blend only).
- No animated channel beyond what `Style.resolve` already produces in the two snapshots.
