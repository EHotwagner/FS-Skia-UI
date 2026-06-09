# Contract: `FS.Skia.UI.Controls` renderTree + bounds (FR-007..FR-012)

Deltas to `src/Controls/Types.fsi`, `src/Controls/Control.fsi`, and the layout
**behavior** in `src/Controls/Control.fs`. The Feature-080 single-control preview
(`Control.render`/`Widget.render`) is **unchanged** (FR-010). Controls baselines
recapture after.

## `ControlRenderResult<'msg>` — add computed bounds (FR-011)

```fsharp
type ControlRenderResult<'msg> =
    { Scene: Scene
      Layout: LayoutNode                  // UNCHANGED — still the input tree (back-compat)
      Bounds: (ControlId * Rect) list     // NEW — evaluated absolute bounds per control
      Diagnostics: ControlDiagnostic list
      EventBindings: ControlEventBinding<'msg> list
      NodeCount: int }
```

- `Bounds` is populated from the `LayoutResult` `renderTree` already computes
  (`Control.fs:1067`) and currently discards. Keyed by `ControlId` so a host joins
  it with `EventBindings` (also keyed by `ControlId`).

## `Control.hitTest` — point → control (FR-012)

```fsharp
/// Resolve which rendered control (if any) contains the point (x, y), from the
/// public render result alone. `None` when the point lies in a gap. Layered over
/// Layout.hitTestComputed.
val hitTest : result: ControlRenderResult<'msg> -> x: float -> y: float -> ControlId option
```

## `Stack` horizontal orientation (FR-007)

```fsharp
module Stack =
    val create : Attr<'msg> list -> Control<'msg>
    val children : Control<'msg> list -> Attr<'msg>
    val orientation : StackOrientation -> Attr<'msg>   // NEW (or `horizontal`/`vertical`)
```

`directionOf` (internal) returns `Row` for documented horizontal kinds **or** when
`orientation = horizontal`; else `Column`.

## Behavioral laws (semantic tests)

1. **FR-007**: a horizontal `Stack` with two children lays them out along the row
   axis (distinct x, same-ish y), not stacked vertically.
2. **FR-008**: two structurally similar **unkeyed** same-kind siblings receive
   **distinct, non-overlapping** bounds at distinct x (the collision case).
3. **FR-009**: an explicit `Attr.width`/`Attr.height` on a container is reflected in
   that container's entry in `Bounds`.
4. **FR-011**: every laid-out control with a `ControlId` appears exactly once in
   `Bounds` with its evaluated box.
5. **FR-012**: a point inside control C's bounds → `Some C`; a point in a gap → `None`.
6. **FR-010**: `Control.render` / `Widget.render` output and the 080 preview goldens
   are byte-identical to pre-feature.
