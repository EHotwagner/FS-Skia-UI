---
name: controls-namespace-du-case-collisions
description: New public DUs in namespace FS.Skia.UI.Controls collide with existing case names (ButtonIntent, VisualState) — use [<RequireQualifiedAccess>]
metadata:
  type: project
---

When adding a new public discriminated union to `namespace FS.Skia.UI.Controls`,
its bare case names pollute the whole namespace and collide with existing cases
compiled later in `Controls.fsproj` (the error surfaces in unrelated files like
`Widgets/Input.fs`, not your new file).

Observed in feature 075 (`Pointer.fs`):
- `PointerButton = Primary | Secondary | Middle` collided with
  `ButtonIntent = Primary | Secondary | Danger | Ghost` (used bare in
  `Widgets/Input.fs`).
- `PointerPhase = Moved | Pressed | Released | Wheel | Exited` collided with
  `VisualState` (cases `Moved`/`Pressed`/…), surfacing as `error FS0001 ... but
  here has type 'VisualState'` when a consumer constructed `{ Phase = Moved }`.

**Why:** F# resolves a bare case name against the whole namespace; existing
non-RQA DUs in `Controls` own common names (`Primary`, `Pressed`, …).

**How to apply:** mark the new DU `[<RequireQualifiedAccess>]` (consumers then
write `PointerButton.Primary`, `PointerPhase.Moved`) and qualify your own match
arms. Pattern-match scrutinees on the new type still resolve bare cases fine; only
*construction* and cross-type ambiguity break. The longer `PointerInteraction` /
`PointerMsg` case names (`HoverEnter`, `WheelMsg`, …) did NOT collide and were left
unqualified. See [[per-package-baseline-not-in-refresh-target]] for the surface
baseline regen that follows a new `.fsi`.
