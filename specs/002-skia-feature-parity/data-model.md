# Data Model: Skia Feature Parity

## ParityBaseline

Represents the fixed external capability target.

**Fields**:

- `Repository`: source repository identifier.
- `Commit`: exact baseline revision.
- `ReviewedOn`: date the baseline was inspected.
- `CapabilityAreas`: list of grouped capability areas.

**Validation Rules**:

- `Commit` must be non-empty and immutable for this feature.
- Future upstream changes must not alter this baseline unless the spec is explicitly revised.

## CapabilityArea

Groups related parity behavior.

**Fields**:

- `Id`: stable identifier such as `core-scene`, `charts`, `layout`, `graph`, `screenshots`, `diagnostics`.
- `Name`: human-readable area name.
- `PackageBoundary`: core viewer, charts/data grid, or layout/graph.
- `Capabilities`: capability evidence items in this area.

**Relationships**:

- Belongs to one `ParityBaseline`.
- Contains many `ParityEvidenceItem` records.

## ParityEvidenceItem

Tracks whether one baseline capability is complete.

**Fields**:

- `CapabilityId`: stable capability identifier.
- `Description`: observable baseline behavior.
- `Status`: `Supported`, `Adapted`, `IntentionallyExcluded`, or `NotYetSupported`.
- `ConstraintReason`: reason when status is `Adapted` or `IntentionallyExcluded`.
- `EvidenceType`: semantic test, screenshot test, smoke test, package test, manual visual review, or documentation review.
- `EvidenceCommand`: command or procedure proving the result.
- `EvidencePath`: file, report, screenshot, or log path.
- `Notes`: short explanation.

**Validation Rules**:

- Completion requires every non-conflicting capability to be `Supported` or `Adapted`.
- `ManualVisualReview` is allowed only when deterministic graphics comparison is impractical.
- `NotYetSupported` is not allowed in merge-ready evidence for non-conflicting capabilities.

## CapabilityPackage

Represents one independently referenceable consumer package.

**Fields**:

- `PackageId`: package identifier.
- `Boundary`: `CoreViewer`, `ChartsDataGrid`, or `LayoutGraph`.
- `PublicModules`: modules exposed through `.fsi`.
- `Dependencies`: package and project dependencies.
- `SurfaceBaselinePath`: automated public surface baseline.

**Validation Rules**:

- Every public module requires a matching `.fsi`.
- Package dependencies must be pinned and justified.

## Scene

Root immutable description of a frame.

**Fields**:

- `Background`: optional background color.
- `Elements`: ordered list of scene elements.
- `Metadata`: optional diagnostics or test labels.

**Relationships**:

- Produced by the consumer `view` function.
- Consumed by the Vulkan renderer.

## SceneElement

Declarative visual element.

**Fields**:

- `Kind`: rectangle, ellipse, line, text, image, path, group, points, vertices, arc, picture, text runs, chart output, layout output, or graph output.
- `Bounds`: optional allocated bounds.
- `Paint`: optional visual style.
- `Transform`: optional transform.
- `Clip`: optional clipping region.
- `Children`: nested scene elements for groups/layouts.

**Validation Rules**:

- Elements render in declaration order unless an element explicitly defines child ordering.
- Invalid resource references produce diagnostics rather than unhandled failures.

## PaintStyle

Visual style applied to scene elements.

**Fields**:

- `Fill`, `Stroke`, `Opacity`, `Antialias`, `StrokeCap`, `StrokeJoin`, `StrokeMiter`.
- `BlendMode`, `Shader`, `ColorFilter`, `MaskFilter`, `ImageFilter`, `PathEffect`.
- `Font`: optional font specification for text.

**Validation Rules**:

- Unsupported device-specific effects produce capability diagnostics.
- Defaults must be deterministic and documented.

## ChartProps

View-layer projection passed to chart components.

**Fields**:

- `Config`: title, axes, labels, legend, palette, bounds, and formatting choices.
- `Series`: immutable numeric/category data.
- `Interaction`: optional projected interaction state such as selected point or visible range.

**Relationships**:

- Derived from the consumer Elmish `Model`.
- Passed to a pure chart builder in `view`.
- Produces `SceneElement` values.

**Validation Rules**:

- Chart components do not own application state.
- 100,000-point datasets must be accepted by scale tests.
- Empty and invalid values must produce readable fallback output or diagnostics.

## DataGridProps

View-layer projection passed to DataGrid components.

**Fields**:

- `Columns`: column definitions, value kind, width hints, sortability.
- `Rows`: immutable cell values.
- `Viewport`: projected scroll offset and visible row range.
- `SortState`: projected sort column and direction.

**Relationships**:

- Derived from the consumer Elmish `Model`.
- Passed to a pure DataGrid builder in `view`.

**Validation Rules**:

- DataGrid components do not own sort or scroll state.
- 10,000-row datasets must be accepted by scale tests.
- Fixed headers remain visible while vertically scrolling.

## LayoutDefinition

Declarative layout composition.

**Fields**:

- `Kind`: horizontal stack, vertical stack, or dock.
- `Spacing`, `Padding`, `Alignment`, `Sizing`.
- `Children`: layout children with desired sizing.

**Validation Rules**:

- Negative or zero bounds must not crash layout.
- Resize recalculates layout deterministically.

## GraphDefinition

Declarative graph input.

**Fields**:

- `Kind`: directed acyclic or undirected.
- `Nodes`: node identifiers, labels, and style.
- `Edges`: source, target, weight, label, and style.
- `LayoutOptions`: direction, spacing, and bounds.

**Validation Rules**:

- Directed acyclic graphs must detect cycles.
- Duplicate IDs, missing endpoints, and invalid self-loop usage produce validation results.
- Disconnected components remain visible.

## ViewerProgram

Elmish application boundary for running a viewer.

**Fields**:

- `Configuration`: title, initial size, frame target, diagnostics.
- `Init`: creates initial model and commands.
- `Update`: pure model transition.
- `View`: pure model-to-scene projection.
- `Subscriptions`: optional subscriptions.
- `EventMapping`: maps viewer events to messages.
- `EffectMapping`: maps messages or effects to viewer edge effects.

**State Rules**:

- `Model` owns application state, including chart selection, grid sort/scroll, graph focus, and screenshot requests.
- `View` builds scene and component props from the model.
- Renderer and filesystem work happens only at the interpreter edge.

## ViewerDiagnostic

Structured diagnostic emitted by startup, frame rendering, screenshots, and shutdown.

**Fields**:

- `Severity`: info, warning, error, fatal.
- `Stage`: platform, Vulkan instance/device/surface/swapchain, Skia context, frame, screenshot, shutdown.
- `Message`: user-readable summary.
- `Cause`: optional underlying detail.
- `Capability`: optional capability/effect involved.

**Validation Rules**:

- Fatal startup failures occur before presenting a partially functional viewer.
- Recoverable frame errors are reported and followed by a valid frame when available.

## State Transitions

```text
NotStarted -> Starting -> Running -> ShuttingDown -> Stopped
Starting -> FailedStartup
Running -> RecoveringFrame -> Running
Running -> ScreenshotPending -> Running
Running -> FailedRuntime (fatal unrecoverable only)
```

Application component interaction state follows the consumer Elmish model:

```text
Model -> view projection -> component props -> scene elements -> viewer events -> Msg -> update -> Model
```
