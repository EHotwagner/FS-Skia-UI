# Data Model: Yoga.Net Layout for UI Elements and Widgets

## LayoutTree

Represents one automatic layout hierarchy.

- **Fields**: `Root: LayoutNode`; `Revision: int64`; `LastResult: LayoutResult option`
- **Relationships**: Owns the root `LayoutNode`; produces one `LayoutResult`
- **Validation rules**: Root id must be stable and unique within the tree; tree must not contain duplicate node ids; automatic layout tree excludes absolute and overlay composition in v1

## LayoutNode

Represents one element or widget that participates in automatic layout.

- **Fields**: `Id: string`; `Intent: LayoutIntent`; `Visibility: LayoutVisibility`; `Measure: ContentMeasure option`; `Content: Scene option`; `Children: LayoutNode list`
- **Relationships**: Parent owns ordered child nodes; leaf nodes may provide `Measure`; rendering consumes `Content` and computed bounds
- **Validation rules**: Id is non-empty and unique in the tree; collapsed or hidden nodes do not claim visible bounds; nodes with measurement callbacks may not rely on child measurement for preferred size
- **State transitions**: Intent, visibility, children, or content-measurement changes mark the node and affected ancestors dirty

## LayoutIntent

User-facing sizing and placement preferences.

- **Fields**: `Direction`; `Wrap`; `AlignItems`; `AlignSelf`; `JustifyContent`; `Padding`; `Margin`; `Gap`; `Size`; `MinSize`; `MaxSize`; `FlexGrow`; `FlexShrink`; `FlexBasis`
- **Relationships**: Applied to one `LayoutNode`; mapped to Yoga.Net styles by the adapter
- **Validation rules**: Numeric values must be finite and non-negative where CSS layout semantics require non-negative values; min size must not exceed max size without producing a diagnostic; v1 rejects absolute positioning and grid-only intent

## AvailableSpace

Parent-provided logical constraints for evaluation.

- **Fields**: `Width: float`; `Height: float`; `WidthMode: MeasureMode`; `HeightMode: MeasureMode`
- **Relationships**: Passed to root evaluation and measurement callbacks
- **Validation rules**: Invalid, negative, NaN, or infinite inputs are normalized to bounded fallback constraints with diagnostics

## ContentMeasure

Custom measurement behavior for text and custom-drawn elements.

- **Fields**: callback from `MeasureRequest` to `MeasureResponse`; optional stable cache key
- **Relationships**: Used by `LayoutNode` leaves; adapter maps it to Yoga.Net measurement callback
- **Validation rules**: Callback output must be finite and non-negative; invalid output becomes fallback size plus diagnostic

## ComputedBounds

Final logical rectangle assigned to a node after evaluation.

- **Fields**: `NodeId: string`; `X: float`; `Y: float`; `Width: float`; `Height: float`; `Visibility: LayoutVisibility`
- **Relationships**: One per evaluated node; rendering and hit testing consume this data
- **Validation rules**: Bounds are finite; width and height are non-negative; valid flex configurations must not overlap visible siblings

## PixelSnapPolicy

Deterministic conversion between logical bounds and device-pixel-aligned bounds.

- **Fields**: `ScaleFactor: float`; `Mode: SnapMode`
- **Relationships**: Applied after layout for rendering and hit testing
- **Validation rules**: Scale factor must be finite and positive; render and hit-test paths use the same policy for the same node

## LayoutDiagnostic

Structured runtime-visible diagnostic for invalid input, unsatisfied constraints, measurement failures, or fallback behavior.

- **Fields**: `NodeId: string option`; `Code: LayoutDiagnosticCode`; `Severity: DiagnosticSeverity`; `Message: string`; `Constraint: string option`; `FallbackApplied: bool`
- **Relationships**: Collected in `LayoutResult`; applications can inspect or log diagnostics
- **Validation rules**: Recoverable diagnostics do not terminate evaluation; fatal configuration defects are reported before render flow consumes the tree

## LayoutResult

Evaluation output for applications, tests, rendering, and hit testing.

- **Fields**: `Bounds: ComputedBounds list`; `Diagnostics: LayoutDiagnostic list`; `Invalidated: string list`; `Revision: int64`
- **Relationships**: Produced by `Layout.evaluate`; consumed by scene builders, hit testing, tests, and samples
- **Validation rules**: Contains bounded fallback geometry for recoverable failures; repeated evaluation with the same inputs returns deterministic bounds and diagnostics
