# Quickstart: Yoga.Net Layout

This quickstart is the target developer workflow for the implementation.

## Build and Test

```bash
dotnet restore
dotnet build
dotnet test
```

## FSI Contract Check

```bash
dotnet build src/Layout/Layout.fsproj
dotnet fsi scripts/layout-prelude.fsx
```

Expected evidence is captured under `specs/005-add-yoga-net-layout/readiness/fsi/yoga-layout-prelude.txt`.

## Example Usage

```fsharp
open FS.Skia.UI
open FS.Skia.UI.Layout

let labelMeasure text =
    fun request ->
        let preferredWidth = min request.AvailableWidth (float text.Length * 8.0)
        { Width = max 0.0 preferredWidth
          Height = 18.0
          Diagnostics = [] }

let child id text grow =
    { Defaults.layoutNode id with
        Intent = { Defaults.layoutIntent with FlexGrow = grow }
        Measure = Some(labelMeasure text)
        Content = Some(Scene.text (0.0, 0.0) text Colors.white) }

let root =
    { Defaults.layoutNode "root" with
        Intent =
            { Defaults.layoutIntent with
                Direction = Row
                Wrap = Wrap
                Padding = { Left = 12.0; Top = 12.0; Right = 12.0; Bottom = 12.0 }
                Gap = { Row = 8.0; Column = 8.0 } }
        Children =
            [ child "title" "Dashboard" 1.0
              child "status" "Ready" 0.0
              child "actions" "Save" 0.0 ] }

let result = Layout.evaluate (Defaults.availableSpace 640.0 240.0) root
let scene = Layout.renderComputed result root
let snapPolicy = Defaults.pixelSnapPolicy 1.0
let hit = Layout.hitTestComputed snapPolicy result 24.0 24.0
let focusRegion =
    result.Bounds
    |> List.find (fun item -> item.NodeId = "actions")
    |> fun item -> Layout.snapBounds snapPolicy item.Bounds
```

## Host Workflow Boundary

Host resize, widget updates, and content-measurement invalidation are modeled as messages and effects so layout I/O stays at the edge.

```fsharp
let model, effects = Layout.initWorkflow (Defaults.availableSpace 640.0 240.0) root

let resized, resizeEffects =
    Layout.updateWorkflow (LayoutHostResized(Defaults.availableSpace 480.0 240.0)) model

let completed =
    Layout.interpretWorkflowEffect resizeEffects.Head resized

let ready, _ = Layout.updateWorkflow completed resized
```

## Validation Expectations

- Bounds are asserted directly from `LayoutResult.Bounds`.
- Diagnostics are asserted directly from `LayoutResult.Diagnostics`.
- Rendering consumes computed bounds through `Layout.renderComputed`.
- Pointer hit testing uses `Layout.hitTestComputed` with the same `PixelSnapPolicy` as rendering.
- Keyboard focus regions are derived from computed visual bounds with `Layout.snapBounds`; they should not be calculated independently.
- Hidden nodes retain computed records for diagnostics and stability, but `Layout.hitTestComputed` does not return them.
- Collapsed nodes occupy zero size and keep visible siblings stable.
- Existing manual layout samples continue to build and smoke-test unchanged.
