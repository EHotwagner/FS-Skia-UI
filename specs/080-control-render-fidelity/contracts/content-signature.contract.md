# Contract — Per-Control Content Signature (Fidelity)

**Surface**: render-capable harness (`tests/ControlsPreview.Harness/`). This is
**not** a packable public `.fsi` surface; it is the in-repo contract the fidelity
gate enforces. Co-located with the single per-control sample source.

## What the signature asserts

For each catalog control, the gate decides "faithful vs label-on-a-box" against a
**per-control** `ContentSignature` (never a single uniform threshold). FR-007.

### Pixel signature (universal — applies to any committed PNG, including fixtures)
- The PNG is decoded (SkiaSharp `SKBitmap`).
- The top `TitleBandHeight` px is the **title band** (the label strip). Pixels
  there do NOT count toward fidelity.
- Outside the title band the gate computes:
  - `coverage` = (non-background pixels) / (pixels outside band), must be
    `>= MinCoverageOutsideTitleBand`.
  - `distinctColors` = distinct non-background quantized colors, must be
    `>= MinDistinctColors`.
- A 079 label-on-box has near-zero coverage and ~1 distinct color outside the
  band → **fails** (SC-003 fixture requirement).

### Primitive-kind signature (structural — applies to the live render)
- The control is rendered (`Control.render Theme.light`), `Scene.describe` is
  taken, and `RequiredKinds` MUST all be present; `MinKindCounts` enforce repeat
  minima (e.g. `RectangleElement >= 3` for bars/rows).
- This makes a chart drawn as a box fail **even at high raw pixel coverage**,
  because `PathElement`/`ArcElement`/`PointsElement` would be absent.

## Per-family signature guidance (authored in `PreviewSamples.fs`)

| Family | RequiredKinds (outside title band) | Pixel intent |
|--------|-----------------------------------|--------------|
| line-chart | `PathElement` | polyline lit across plot area |
| bar-chart | `RectangleElement` (≥ #points) | multiple bars |
| pie-chart | `ArcElement` (≥ #slices) | colored arcs |
| scatter-plot | `PointsElement` or `CircleElement` (≥ #points) | scattered marks |
| graph-view | `CircleElement` + `LineElement` | nodes + edges |
| collections (list/tree/combo/multi-select) | `TextRunElement`/`RectangleElement` (≥ #rows) | ≥3 distinct rows |
| slider | `RectangleElement` (track) + `CircleElement`/`RectangleElement` (thumb) | track + thumb |
| progress-bar | ≥2 `RectangleElement` (track + fill) | partial fill |
| radio-group | `CircleElement` (≥ #options) | circles, one marked |
| tabs | `RectangleElement` (≥ #tabs) | tab strip, active tab |
| switch / check-box | `RectangleElement`/`CircleElement` + tick/toggle | chrome + state |
| image | framed `RectangleElement` chrome | placeholder frame, not path text |
| icon | `PathElement`/`TextRunElement` glyph | font-supported glyph, no .notdef box |

Layout containers (stack/grid/dock/…) and text controls (button/label/text-box/…)
keep text-carries-meaning signatures (their 079 renders are already faithful):
title-band-exclusive content is acceptable where the control IS text, but the
pixel signature still requires content beyond a single token where chrome exists.

## Fail-closed rule (FR-013)

- `ContentSignature` is a **required** field on every Demonstrative sample
  (`FidelityDeclaration.Signature`). A Demonstrative sample without one does not
  compile.
- The gate asserts the sample set is **total** over `CatalogGen.catalogFacts`:
  any catalog id with neither a `Signature` nor `UnsupportedNoPreview` fails the
  gate with a message naming the control. A control added later cannot pass until
  an author declares one.

## Determinism

Signatures are fixed literals (no clock/randomness/environment), matching the
existing `PreviewSamples.fs` determinism convention, so the gate is reproducible.
