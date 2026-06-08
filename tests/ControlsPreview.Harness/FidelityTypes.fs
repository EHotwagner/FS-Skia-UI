module ControlsPreview.FidelityTypes

// Feature 080 (T007, FR-007/FR-013, D5) — the per-control fidelity contract the gate asserts.
// Lives in the render-capable harness (NOT a packable public `.fsi`). The signature is the one
// reviewable answer to "why is this preview faithful and not a label-on-a-box": a pixel
// criterion (coverage + colour outside the title band) plus a structural criterion (the
// `Scene.describe` primitive kinds the live render must contain). `FidelityDeclaration` makes a
// Demonstrative-without-signature unrepresentable (fail-closed): `ControlSampleDefinition`
// carries one as a required field, so a Demonstrative sample with no signature does not compile.

open FS.Skia.UI.Scene

/// Pixel criterion — applies to any committed PNG (including fixtures). The top
/// `TitleBandHeight` px is the label strip and does NOT count toward fidelity, so a 079
/// label-on-box (its only content is the title) scores ~0 coverage / ~1 colour and fails.
type PixelSignature =
    { /// Min fraction of non-background pixels OUTSIDE the title band, in (0, 1].
      MinCoverageOutsideTitleBand: float
      /// Min count of distinct non-background quantized colours outside the title band.
      MinDistinctColors: int
      /// Title-band height in px reserved for the label (top strip; e.g. 28).
      TitleBandHeight: int }

/// Structural criterion — applies to the live render (`Control.render Theme.light` →
/// `Scene.describe`). A chart drawn as a box fails even at high pixel coverage because its
/// `PathElement`/`ArcElement`/`PointsElement` would be absent.
type PrimitiveSignature =
    { /// Kinds that MUST all be present in the live render's `describe` output.
      RequiredKinds: SceneElementKind list
      /// Optional minimum repeat counts (e.g. `RectangleElement >= 3` for bars/rows).
      MinKindCounts: (SceneElementKind * int) list }

/// The full per-control criterion: pixel always; primitive when a live scene is checked.
type ContentSignature =
    { Pixel: PixelSignature
      Primitive: PrimitiveSignature option }

/// Fail-closed declaration (D5/FR-013): a Demonstrative control MUST carry a `Signature`;
/// `UnsupportedNoPreview` is the honest no-image declaration (e.g. `custom-control`).
type FidelityDeclaration =
    | Signature of ContentSignature
    | UnsupportedNoPreview

/// Gate output for one control → a row in `readiness/control-fidelity.md`.
type FidelityVerdict =
    { ControlId: string
      Declared: FidelityDeclaration
      CoverageOutsideTitleBand: float option
      DistinctColors: int option
      PresentKinds: SceneElementKind list
      Passed: bool
      /// On failure, names the control + the missing component (kind absent / coverage below
      /// threshold / no committed PNG / catalog id lacking a declaration).
      FailureReason: string option }

// ---- signature builders (fixed literals; keep determinism, FR-008) ---------------------

/// A pixel-only signature with the default 28-px title band.
let pixelOnly (minCoverage: float) (minColors: int) : ContentSignature =
    { Pixel =
        { MinCoverageOutsideTitleBand = minCoverage
          MinDistinctColors = minColors
          TitleBandHeight = 28 }
      Primitive = None }

/// A pixel + structural signature (the common case for chart/value/collection chrome).
let withKinds (minCoverage: float) (minColors: int) (required: SceneElementKind list) (minCounts: (SceneElementKind * int) list) : ContentSignature =
    { Pixel =
        { MinCoverageOutsideTitleBand = minCoverage
          MinDistinctColors = minColors
          TitleBandHeight = 28 }
      Primitive = Some { RequiredKinds = required; MinKindCounts = minCounts } }
