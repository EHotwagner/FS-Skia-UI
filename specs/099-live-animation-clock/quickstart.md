# Quickstart: A visual-state transition animates on the live host (R4)

**Goal**: with **zero animation code**, a consumer's interactive app animates its
visual-state transitions (hover / press / focus) instead of snapping — driven entirely by
the host advancing a per-control clock from the existing per-frame tick.

## What the consumer writes (unchanged)

Exactly the same MVU host as before R4 — `view : 'model -> Control<'msg>` returns the
usual migrated interactive controls; no animation API, no per-control knob:

```fsharp
let host : InteractiveAppHost<Model, Msg> =
    { Init = init
      Update = update
      View = fun _size model -> myView model          // ordinary R1-migrated controls
      Theme = Theme.light
      MapKey = mapKey
      MapPointer = mapPointer
      Tick = fun _delta -> None                         // consumer ignores the delta…
      Diagnostics = ViewerDiagnosticsOptions.Default }

ControlsElmish.runInteractiveApp options host
```

The consumer's `Tick` may stay `fun _ -> None`: the **host** consumes the injected delta
internally to advance the animation clocks; the consumer's tick message is still delivered
if they return one.

## What you observe

1. Hover (or focus) a migrated control. Its appearance **eases** into the hover/focus
   look — a focus-ring fades in, a press tint settles — rather than snapping in one frame.
2. Move focus away mid-transition: the control animates **back** toward its at-rest look
   and, once settled, is byte-identical to a non-animated build.
3. Trigger an unrelated state change that shifts sibling positions while a control is
   mid-transition: the animating control keeps its identity, its clock keeps advancing from
   where it was, and the transition **completes** — it is not reset or dropped.

## How to prove it (evidence, deterministic)

Drive the **real** `runInteractiveApp` seam with an explicit sequence of injected deltas
(no wall-clock) and capture consecutive frames:

```fsharp
// pseudo: feed a fixed delta sequence through the host's tick + render seam
let deltas = [ ms 16.0; ms 16.0; ms 16.0; ms 16.0; ms 16.0 ]   // ~5 frames @ 60fps
let frames = driveHostFrames host (hover migratedControlId) deltas
// Assert: at least one intermediate sampled appearance before the target (animates, not snaps)
// Assert: replaying `deltas` yields byte-identical frames (determinism)
// Assert: a frame with no active clock == pre-R4 golden (identity-at-rest)
```

- **No-seam build** (pre-R4): `frames` jump straight to the target — fails the
  intermediate-appearance assertion.
- **Survival**: start a tween, advance a few deltas, apply a sibling-shifting re-render,
  continue ticking → the same `RetainedId`'s clock reaches completion with the same final
  result as an un-shifted run (this replaces the hand-seeded
  `Feature092LiveSurvivalTests` PRECONDITION).

## Boundaries

- Animation is **paint-level** (opacity / transform / color) — never size/position reflow.
- The transition is a single framework default (short duration + standard easing); there is
  no consumer authoring API, keyframe surface, or timeline in this feature.
- If a reduced-motion / opt-out policy is later added, disabling it reverts to the pre-R4
  snap behavior; R4 does not require such a policy but does not preclude one.
