# Contract: published interactive pointer/host surface

**Goal (FR-005)**: a consumer recovers the pointer/host DU shapes from `docs/api-surface/`
— not by reflecting over compiled assemblies. These types are **already public** in their
`.fsi`; this contract publishes and drift-guards them. No code/signature change.

## Types to publish

### `PointerButton` — `src/Controls/Pointer.fsi`
```fsharp
[<RequireQualifiedAccess>]
type PointerButton =
    | Primary
    | Secondary
    | Middle
```

### `PointerInteraction` — `src/Controls/Pointer.fsi`
```fsharp
type PointerInteraction =
    | HoverEnter of control: ControlId * x: float * y: float
    | HoverLeave of control: ControlId
    | PressedDown of control: ControlId * button: PointerButton * x: float * y: float
    | ReleasedUp of control: ControlId * button: PointerButton * x: float * y: float
    | Click of control: ControlId * button: PointerButton * x: float * y: float
    | DragBegin of control: ControlId * button: PointerButton * startX: float * startY: float
    | DragMove of control: ControlId * button: PointerButton * x: float * y: float
    | DragEnd of control: ControlId * button: PointerButton * x: float * y: float
    | DragCancelled of control: ControlId option
    | Scroll of control: ControlId * deltaX: float * deltaY: float * x: float * y: float
    | FocusMovedByPointer of control: ControlId
    | Diagnostic of PointerDiagnostic
```

### `ViewerPointerPhaseKind` — `src/SkiaViewer/SkiaViewer.fsi`
```fsharp
[<RequireQualifiedAccess>]
type ViewerPointerPhaseKind =
    | Moved
    | Pressed
    | Released
    | Wheel
    | Exited
```

## Folding contract note (host seam)

`InteractiveAppHost` exposes the pointer/key fallback seams an authored binding defers to:
```fsharp
MapPointer  : PointerInteraction -> 'msg option
MapKeyChord : ViewerKey -> KeyModifiers -> 'msg option
```
An authored control binding wins and consumes the interaction; `MapPointer` is the
fallback. Document this so consumers know where unrouted pointer interactions land.

## Drift guard

The published `docs/api-surface/` entry for these types must match their `.fsi`. Wire a
check (or extend an existing api-surface/doc check) that fails if the published shape and
the `.fsi` diverge, so the surface cannot silently rot.
</content>
