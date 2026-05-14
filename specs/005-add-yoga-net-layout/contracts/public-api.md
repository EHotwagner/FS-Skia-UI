# Public API Contract: Yoga.Net Layout

This contract describes the intended public `.fsi` additions before implementation. Exact names may be refined during FSI sketching, but the implementation must preserve these capabilities through the public `FS.Skia.UI.Layout` surface.

## Package Dependency Contract

- `src/Layout/Layout.fsproj` references `Yoga.Net` version `3.2.3`.
- Yoga.Net types are not exposed in public `.fsi` signatures.
- The FS-Skia-UI layout package remains packable and updates the `FS.Skia.UI.Layout` surface-area baseline.

## Types Surface

```fsharp
namespace FS.Skia.UI.Layout

open FS.Skia.UI

type LayoutNodeId = string

type MeasureMode =
    | Undefined
    | Exactly
    | AtMost

type LayoutDirection =
    | Row
    | Column

type LayoutWrap =
    | NoWrap
    | Wrap

type LayoutAlign =
    | Auto
    | Start
    | Center
    | End
    | Stretch
    | SpaceBetween
    | SpaceAround
    | SpaceEvenly

type LayoutVisibility =
    | Visible
    | Hidden
    | Collapsed

type LayoutSize =
    { Width: float option
      Height: float option }

type LayoutGap =
    { Row: float
      Column: float }

type DiagnosticSeverity =
    | Info
    | Warning
    | Error

type LayoutDiagnosticCode =
    | InvalidAvailableSpace
    | InvalidLayoutValue
    | UnsatisfiedConstraint
    | UnmeasurableContent
    | FallbackBoundsApplied

type LayoutDiagnostic =
    { NodeId: LayoutNodeId option
      Code: LayoutDiagnosticCode
      Severity: DiagnosticSeverity
      Message: string
      Constraint: string option
      FallbackApplied: bool }

type LayoutIntent =
    { Direction: LayoutDirection
      Wrap: LayoutWrap
      AlignItems: LayoutAlign
      AlignSelf: LayoutAlign option
      JustifyContent: LayoutAlign
      Padding: LayoutPadding
      Margin: LayoutPadding
      Gap: LayoutGap
      Size: LayoutSize
      MinSize: LayoutSize
      MaxSize: LayoutSize
      FlexGrow: float
      FlexShrink: float
      FlexBasis: float option }

type MeasureRequest =
    { AvailableWidth: float
      WidthMode: MeasureMode
      AvailableHeight: float
      HeightMode: MeasureMode }

type MeasureResponse =
    { Width: float
      Height: float
      Diagnostics: LayoutDiagnostic list }

type ContentMeasure = MeasureRequest -> MeasureResponse

type LayoutNode =
    { Id: LayoutNodeId
      Intent: LayoutIntent
      Visibility: LayoutVisibility
      Measure: ContentMeasure option
      Content: Scene option
      Children: LayoutNode list }

type AvailableSpace =
    { Width: float
      WidthMode: MeasureMode
      Height: float
      HeightMode: MeasureMode }

type ComputedBounds =
    { NodeId: LayoutNodeId
      Bounds: LayoutBounds
      Visibility: LayoutVisibility }

type LayoutResult =
    { Bounds: ComputedBounds list
      Diagnostics: LayoutDiagnostic list
      Invalidated: LayoutNodeId list
      Revision: int64 }

type SnapMode =
    | Floor
    | Round
    | Expand

type PixelSnapPolicy =
    { ScaleFactor: float
      Mode: SnapMode }
```

## Function Surface

```fsharp
module Defaults =
    val layoutIntent : LayoutIntent
    val layoutNode : id: LayoutNodeId -> LayoutNode
    val availableSpace : width: float -> height: float -> AvailableSpace
    val pixelSnapPolicy : scaleFactor: float -> PixelSnapPolicy

module Layout =
    val evaluate : available: AvailableSpace -> root: LayoutNode -> LayoutResult
    val evaluateIncremental :
        previous: LayoutResult ->
        changedNodeIds: LayoutNodeId list ->
        available: AvailableSpace ->
        root: LayoutNode ->
            LayoutResult

    val renderComputed : result: LayoutResult -> root: LayoutNode -> Scene
    val snapBounds : policy: PixelSnapPolicy -> bounds: LayoutBounds -> LayoutBounds
    val hitTestComputed :
        policy: PixelSnapPolicy ->
        result: LayoutResult ->
        x: float ->
        y: float ->
            LayoutNodeId option
```

## Behavioral Contract

- `evaluate` returns one bounded `ComputedBounds` record per node that participates in automatic layout.
- Hidden or collapsed nodes are distinguished from layout failures in diagnostics and visibility state.
- Valid flex-style configurations do not produce visible sibling overlap.
- Invalid numeric input, impossible constraints, invalid available space, and invalid measurement output return diagnostics and fallback bounds.
- `evaluateIncremental` may recompute ancestors needed for correctness, but unchanged sibling subtrees must keep stable computed bounds when their constraints do not change.
- `renderComputed` consumes computed bounds and does not recalculate child layout independently.
- `hitTestComputed` and rendering use the same `PixelSnapPolicy`.
- Existing manual `horizontalStack`, `verticalStack`, `dock`, graph layout, absolute, and overlay scene composition remain compatible.
