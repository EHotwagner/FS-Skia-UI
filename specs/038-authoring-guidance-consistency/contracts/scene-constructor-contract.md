# Contract: Consistent Scene Constructors (US6)

**Satisfies:** FR-010 · SC-006 · Tier 1 contract change (additive)

## Surface delta (additive only)

In `src/Scene/Scene.fs` + `.fsi`, add self-describing constructors/helpers
consistent with the existing `Rect`-based pattern, **without removing** any
existing constructor:

- A `Rect`-based / named-argument constructor for `Rectangle` (today
  `Rectangle of (float*float*float*float)*Color`, `Scene.fsi:322`).
- A `Rect`-based / named-argument constructor for `Text` (today
  `Text of (float*float)*string*Color`, `Scene.fsi:332`).
- Consistent with `PaintedRectangle of Rect*Paint` (`Scene.fsi:325`),
  `rectangleWithPaint: Rect -> Paint -> Scene` (`Scene.fsi:412`), and
  `text: position -> text -> color -> Scene` (`Scene.fsi:430`).

Helper functions are preferred over new DU cases (smaller surface, no
match-exhaustiveness churn); named-field DU additions are acceptable when a
matchable constructor is required. Both are additive.

## Rules

1. A consistent, self-describing way to construct each of
   `Rectangle`/`PaintedRectangle`/`Text` exists, so an arity slip is prevented or
   yields a clear error (no more "tuple of length 5").
2. **No removals.** Existing positional `Rectangle`/`Text` constructors and the
   `Scene.rectangle`/`Scene.text` helpers still compile — existing generated code
   is unaffected.

## Process

- Update `.fs` + `.fsi`; refresh `readiness/surface-baselines/FS.Skia.UI.Scene.txt`
  and merged `FS.Skia.UI.txt`; version bump on merge.

## Evidence

`readiness/fsi/` — one FSI fixture compiling both the existing positional forms
and the new self-describing forms.
