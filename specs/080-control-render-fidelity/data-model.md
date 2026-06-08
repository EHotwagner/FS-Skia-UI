# Phase 1 Data Model — Faithful Control Preview Rendering

This feature is rendering + governance; the "entities" are the value types the
renderer reads, the fidelity declaration the gate asserts, and the report it
emits. No persistent/stateful model (no MVU boundary — see plan Principle IV).

## Existing types reused (no change)

### `ChartPoint` / `ChartSeries` — `src/Controls/Charts.fs(.fsi)`
```fsharp
type ChartPoint  = { X: float; Y: float; Label: string option }
type ChartSeries = { Name: string; Points: ChartPoint list }
```
- Stored on chart controls as `UntypedValue(ChartSeries list)` under `"series"`
  (line/bar/scatter) and `UntypedValue(ChartPoint list)` under `"values"` (pie).
- **Validation rule (FR-002)**: extraction (`Control.fs:chartValues`) MUST read
  these shapes; empty `Points` → honest empty state (FR-011), never off-canvas.

### `SceneNode` / `SceneElementKind` — `src/Scene/Scene.fs(.fsi)` (unchanged)
- Faithful geometry lowers to existing primitives: `Path`, `Rectangle`/
  `PaintedRectangle`, `Points`, `Arc`, `Circle`, `Line`, `TextRun`, `Group`,
  `ClipNode`. `Scene.describe : Scene -> SceneElementKind list` is the structural
  signal the gate's primitive-kind check reads (output changes; type unchanged).

### `ControlRenderResult<'msg>` / `Control.render` — `src/Controls/Types.fs`, `Control.fs:324` (signature unchanged)

## New types (render-capable harness — `tests/ControlsPreview.Harness/`)

### `ContentSignature`
Per-control criteria the fidelity gate asserts (FR-007). Lives in the harness.
```fsharp
type PixelSignature =
    { /// Min fraction of non-background pixels OUTSIDE the title band [0..1].
      MinCoverageOutsideTitleBand: float
      /// Min count of distinct non-background colors outside the title band.
      MinDistinctColors: int
      /// Title-band height in px reserved for the label (top strip; e.g. 28).
      TitleBandHeight: int }

type PrimitiveSignature =
    { /// SceneElementKinds that MUST be present in the live render's describe output.
      RequiredKinds: SceneElementKind list
      /// Optional minimum repeat counts (e.g. RectangleElement >= 3 for rows/bars).
      MinKindCounts: (SceneElementKind * int) list }

type ContentSignature =
    { Pixel: PixelSignature
      /// None when no live scene is checked (e.g. raw fixtures use Pixel only).
      Primitive: PrimitiveSignature option }
```
- **Validation rules**:
  - `MinCoverageOutsideTitleBand` ∈ (0,1]; a label-on-box scores near 0 outside
    the band → fails (SC-003).
  - `RequiredKinds` is control-family-specific: line→`PathElement`;
    bar→`RectangleElement` (MinKindCounts ≥ #points); pie→`ArcElement`;
    scatter→`PointsElement`/`CircleElement`; collection→`RectangleElement`/
    `TextRunElement` (≥ #rows); value controls→ their chrome kinds.

### `FidelityDeclaration` (fail-closed; D5/FR-013)
```fsharp
type FidelityDeclaration =
    | Signature of ContentSignature      // Demonstrative: must be present
    | UnsupportedNoPreview               // honest no-image (custom-control, …)
```
- Added to `ControlSampleDefinition` as a required field, unifying with
  `SampleKind`: `Demonstrative` ⇒ `Signature _`; `Unsupported` ⇒
  `UnsupportedNoPreview`. The type makes a Demonstrative-without-signature
  unrepresentable (compile-time fail-closed).

### `FidelityVerdict` (gate output → `readiness/control-fidelity.md`)
```fsharp
type FidelityVerdict =
    { ControlId: string
      Declared: FidelityDeclaration
      CoverageOutsideTitleBand: float option   // None for Unsupported
      DistinctColors: int option
      PresentKinds: SceneElementKind list
      Passed: bool
      /// Human-readable miss when failed: names control + missing signature part.
      FailureReason: string option }
```
- **Validation rule (FR-007/Principle VII)**: a failing verdict MUST set
  `FailureReason` naming the control and the missing component (kind absent /
  coverage below threshold / no committed PNG / catalog id lacking declaration).

## Entity relationships

```
CatalogGen.catalogFacts (governance)  ──totality──▶  PreviewSamples.samples (harness)
        │ id, category                                    │ Id, Kind, Build, Canvas, Fidelity
        │                                                 ▼
        │                                   Demonstrative ⇒ Build () : Widget
        │                                                 ▼ Control.render Theme.light
        │                                            SceneNode ──Scene.describe──▶ SceneElementKind list
        │                                                 ▼ captureScreenshotEvidence
        │                                   docs/img/controls/<id>.png (committed)
        ▼                                                 ▼
  fail-closed: every fact id          ControlFidelityCheck decodes PNG + checks
  has a Demonstrative+Signature        ContentSignature (Pixel always; Primitive
  or Unsupported declaration           on live render) ⇒ FidelityVerdict
                                                          ▼
                                   fixtures/fidelity/{lowfi,faithful}/<id>.png
                                   (lowfi MUST fail, faithful MUST pass)
```

## State transitions

None — the renderer is a pure transform and the gate is a pure decode-and-assert.
The only "transition" is the failing-first → passing demonstration (SC-003),
which is a test-suite state, not a runtime model.
