# Phase 0 Research — Faithful Control Preview Rendering

All NEEDS CLARIFICATION from Technical Context are resolved below. Each decision
is grounded in verified source references.

## D1 — How to render faithful chart geometry

- **Decision**: Lower each chart to **existing** `SceneNode` primitives within
  the preview canvas bounds, computed in `Control.fs` `renderNode`:
  line → `Path` (polyline of `LineTo`), bar → one `Rectangle`/`PaintedRectangle`
  per point, pie → `Arc` per slice, scatter → `Points` (or `Circle` markers),
  graph → `Circle` nodes + `Line` edges. Do **not** emit the opaque `Chart` node
  on the preview path.
- **Rationale**: Every primitive already exists in the union
  (`src/Scene/Scene.fs:276-297`) and is already rasterized by
  `SceneRenderer.paintNode` and proven faithful by
  `tests/SkiaViewer.Tests/Feature063RendererTests.fs` (Line/Path/Points/Arc).
  Reusing them means **no public `Scene` `.fsi` change** and no new bounds-bearing
  node — the simplest path that satisfies FR-002 and avoids the
  `chartTop=180` off-canvas bug (`SceneRenderer.fs:394-411`) entirely, because the
  preview path no longer reaches that painter.
- **Alternatives considered**:
  - *Fix the opaque `Chart` painter to be bounds-aware* — rejected: keeps an
    opaque node that `Scene.describe` reports as a single `ChartElement`, so the
    fidelity gate's primitive-kind signature can't distinguish a real chart from
    a box; and it leaves a second, hard-coded layout path to maintain.
  - *Add a new bounds-bearing `Chart` node* — rejected: a public `Scene` `.fsi`
    addition (breaking surface change) for no benefit over existing primitives.

## D2 — Fixing chart data extraction

- **Decision**: Rewrite `chartValues` (`Control.fs:159`) to read the structured
  shapes the typed controls store: `UntypedValue(:? (ChartSeries list))` under
  `"series"` (line/bar/scatter) and `UntypedValue(:? (ChartPoint list))` under
  `"values"` (pie), preserving `X`/`Y`/`Label`. Keep the existing flat-list
  fallback for back-compat.
- **Rationale**: `src/Controls/Charts.fs:23-25` stores
  `Attr.create "series" Data (UntypedValue values)` where `values : ChartSeries list`;
  the current matcher only handles `float list`/`float array`/`FloatValue`, so it
  yields `[]` (post-mortem §3.2). The renderer needs `X`/`Y` to lay out geometry,
  not a flat magnitude list.
- **Alternatives**: Change how typed controls store series — rejected: would
  churn the typed front door and existing samples; the storage shape is correct,
  only the *extraction* is wrong.

## D3 — Where the fidelity gate executes (render-capable vs SkiaSharp-free)

- **Decision**: Implement the decode + signature check in the **render-capable
  `ControlsPreview.Harness`** project (it already references SkiaSharp via
  SkiaViewer) as a new `Fidelity.fs` reachable by a `-- --fidelity` mode and by
  the harness Expecto suite. Add a governance FAKE target `ControlFidelityCheck`
  whose `StartTarget` effect **shells out** `dotnet run --project
  tests/ControlsPreview.Harness -- --fidelity`, mirroring the existing
  `SkiaViewer.Tests -- --sequenced` shellout (`Update.fs:61`).
- **Rationale**: `FS.Skia.UI.Build` is SkiaSharp-free **by design** so the
  currency gate runs in GPU-free CI (post-mortem §5; `Update.fs:377` `validatePng`
  is signature/IHDR-only). Pixel decoding must live where native Skia is present.
  Shelling out keeps that separation intact (FR-008) and reuses the proven
  native-startup execution edge.
- **Alternatives**: Add SkiaSharp to `FS.Skia.UI.Build` — rejected: breaks the
  GPU-free guarantee for the whole governance build and the currency gate.

## D4 — Content-signature shape (what "faithful" asserts per control)

- **Decision**: A per-control `ContentSignature` with two complementary parts:
  1. **Pixel signature** (universal; applies to any committed PNG incl. fixtures):
     minimum lit-pixel coverage and minimum distinct non-background color count
     in the region **outside the title band** (top strip reserved for the label).
     This is what fails a label-on-box (its non-background pixels are
     concentrated in the title band).
  2. **Primitive-kind signature** (structural; applies to the live render where
     `Scene.describe` is available): the set of `SceneElementKind`s that MUST be
     present (e.g. `PathElement` for line-chart, `ArcElement` for pie,
     `PointsElement` for scatter, ≥N `RectangleElement` for bars/rows).
- **Rationale**: FR-007 explicitly allows "required primitive kinds **and/or**
  lit-pixel coverage in regions outside the title band." Fixtures are raw PNGs
  with no scene, so the **pixel** part must stand alone for SC-003; the
  **primitive-kind** part adds a sharper structural assertion for live catalog
  renders and is what makes a chart-rendered-as-box fail even at high raw pixel
  count (spec FR-007 parenthetical).
- **Alternatives**: Single uniform pixel threshold — rejected by the spec
  clarification (a box can exceed any raw threshold). Primitive-kind only —
  rejected: cannot judge the retained fixture PNGs (no scene).

## D5 — Fail-closed enforcement (FR-013)

- **Decision**: Make the signature a **required field** on each Demonstrative
  sample in `PreviewSamples.fs` (`ControlSampleDefinition` gains
  `Fidelity: FidelityDeclaration` where the type forces either a `Signature` or
  an explicit `UnsupportedNoPreview`). Compile-time the type forbids a
  Demonstrative entry without a signature; runtime the existing **totality test**
  (sampleIds ≡ `CatalogGen.catalogFacts` ids) plus a new gate assertion fail any
  catalog id lacking both. A future control added to `catalogFacts` breaks
  totality until an author declares a signature or `Unsupported`.
- **Rationale**: Two independent fail-closed guarantees (type + totality) match
  FR-013's "cannot pass governance until an author declares." Co-locating the
  signature with the sample keeps the one reviewable per-control answer in one
  file (continuing the 079 single-source convention).
- **Alternatives**: Put the signature on `CatalogGen.TypedCatalogFact`
  (governance) — viable and also total, but the gate runs in the harness and
  would then need to import governance facts at runtime; co-location in the
  harness sample source is simpler and keeps the signature next to the sample it
  describes.

## D6 — Retained failing-first fixtures (SC-003)

- **Decision**: Commit `tests/ControlsPreview.Harness/fixtures/fidelity/lowfi/`
  (a small set of 079-style label-on-box PNGs, e.g. `line-chart`, `list-box`,
  `image`, `icon`) and `faithful/` (their regenerated faithful counterparts). The
  gate test asserts: every `lowfi` fixture **fails** its control's signature,
  every `faithful` fixture **passes**. Source the `lowfi` PNGs from the current
  `main` previews (pre-fix output of the schematic renderer) so they are genuine
  079 artifacts, not hand-drawn.
- **Rationale**: A retained committed set (clarification answer) makes the
  red→green transition durable and re-runnable rather than a one-time manual
  demonstration. Disclosed as a synthetic *fixture* set (Principle V) with a
  banner, distinct from product evidence.
- **Alternatives**: One-time manual red run — rejected by the clarification.

## D7 — Sample data for controls lacking it (FR-014)

- **Decision**: Author representative, font-safe sample data in
  `PreviewSamples.fs` for every Demonstrative control whose current sample yields
  empty geometry: collections get ≥3 items, charts already have `sampleSeries`
  (now reachable after D2), `icon` gets a glyph **verified present** in the
  rendering font (no missing-glyph box; replace `★` per post-mortem §3.3),
  `image` gets a framed placeholder (drawn chrome, not the path string),
  value/selection controls get explicit state (checked, mid-track, selected
  option/tab). Empty-state demonstrations (FR-011) are used only where an empty
  state is the point; `Unsupported` (FR-009) only where no authored data yields a
  recognizable depiction.
- **Rationale**: FR-014 makes authored data the default route to faithfulness;
  the font-safe constraint is from the `icon` missing-glyph failure. Determinism
  (fixed literals, no clock/randomness) is preserved per the existing
  `PreviewSamples.fs` convention.

## D8 — Public surface / escalation confirmation

- **Decision**: Treat the feature as escalated (`maintainer-verify`) because of
  the governance/build-target contract surface, **but** expect **no public
  package `.fsi` delta**: charts reuse existing `Scene` primitives; `Control.render`
  and `Widget.render` signatures are unchanged. `Scene.describe` *output* changes
  (behavioral) → snapshot baselines move, not `.fsi`/per-package surface. If
  implementation finds a needed public symbol, it returns to planning.
- **Rationale**: Grounded in the Scene/Controls `.fsi` review (no new node, no
  new constructor needed). Escalation is driven by the new gate + routing +
  `validation.contract.yml`, which are governance contract surfaces.
