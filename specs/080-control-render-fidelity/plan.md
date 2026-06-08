# Implementation Plan: Faithful Control Preview Rendering

**Branch**: `080-control-render-fidelity` | **Date**: 2026-06-08 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/080-control-render-fidelity/spec.md`

## Summary

Feature 079 shipped "demonstrative" control previews that are really uniform
label-on-a-box schematics: `Control.render` draws every control as one filled
rect plus one clipped text label, so charts/collections/value controls/`image`/
`icon` render as a single word, and the readiness prose claimed per-control
content that was never viewed. This feature implements report **Option B + the
§8 decoded-content gate** with honest evidence:

1. **Faithful renderer** — replace the uniform `renderNode` primitive with
   per-control geometry lowered to *existing* Scene primitives (polyline `Path`
   for line, `Rectangle`s for bars, `Arc`s for pie, `Points`/`Circle` for
   scatter, item rows for collections, track+thumb/tick/toggle/tab-strip chrome
   for value controls, a framed placeholder for `image`, a font-safe glyph for
   `icon`), all laid out **within the preview canvas bounds**. Fix
   `chartValues` to read the structured `ChartSeries list` / `ChartPoint list`
   the typed controls actually store (today it reads a flat `float list` and
   silently yields `[]`).
2. **Sample data** — author representative, font-safe sample data for every
   control that lacks it so faithful geometry has something to draw (FR-014).
3. **Pixel-decoding fidelity gate** — a new render-capable governance target
   (`ControlFidelityCheck`) that decodes each committed PNG and asserts a
   **per-control content signature** (lit-pixel coverage outside the title band
   + required `Scene.describe` primitive kinds), **fails closed** for any
   catalog control lacking both a signature and an `Unsupported` status, and is
   guarded by a **retained committed fixture set** (079-style label-on-box
   PNGs asserted to fail, faithful PNGs asserted to pass). It stays separate
   from the SkiaSharp-free `ControlsCatalogDocsCheck` byte-floor currency gate.
4. **Honest evidence** — regenerate every preview, regenerate the catalog
   detail-page prose, and correct the per-control claims so each matches
   decoded image content; controls that cannot be depicted stay honestly
   `Unsupported`.

**Technical approach grounding (verified during planning):**

- Charts store `UntypedValue(ChartSeries list)` under `"series"` and
  `UntypedValue(ChartPoint list)` under `"values"` (`src/Controls/Charts.fs:23-25`);
  `Control.fs:159` `chartValues` matches only `float list`/`float array`/`FloatValue`
  → never matches → `[]`. Root cause confirmed.
- Every chart shape can be drawn from primitives that **already exist** in the
  `SceneNode` union (`Path`, `Rectangle`/`PaintedRectangle`, `Points`, `Arc`,
  `Circle`, `Line`, `TextRun`) and that the painter already rasterizes
  (`SceneRenderer.paintNode`, proven by `tests/SkiaViewer.Tests/Feature063RendererTests.fs`).
  **No new Scene node is required** — charts lower to existing primitives within
  bounds, so the opaque `Chart` node (and its fixed-coordinate `chartTop=180`
  off-canvas painter at `SceneRenderer.fs:394`) is no longer emitted on the
  preview path. This keeps the public `Scene` `.fsi` surface unchanged.
- The render-capable path already exists: `ControlsPreview.Harness`
  (`tests/ControlsPreview.Harness/`) references SkiaViewer/SkiaSharp and renders
  via `Viewer.captureScreenshotEvidence` (`PreviewRender.fs`). The fidelity gate
  runs here (it can decode pixels); the governance build (`FS.Skia.UI.Build`)
  stays SkiaSharp-free and shells out to the harness, exactly like the existing
  `SkiaViewer.Tests` `-- --sequenced` pattern (`Update.fs:61`).

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: SkiaSharp 4 preview (already pinned; used only in the
render-capable harness for rendering + the new pixel decode). No new dependency.
**Testing**: Expecto (in `ControlsPreview.Harness`, render-capable, `-- --sequenced`),
governance tests (`Governance.Tests`), FAKE targets, the render-only evidence path.
**Target Platform**: Windows and Linux. The fidelity gate requires a render-capable
host (native Skia); the SkiaSharp-free currency gate continues to run in GPU-free CI.
**Change classification**: **Tier 1 (contracted change)** — adds a governance/build
contract surface (a new build target + routing rule + `validation.contract.yml`
regeneration) and alters observable `Scene.describe` output for chart/collection/
value controls. Escalated to the **`maintainer-verify`** serialized path per the
spec's Public-contract impact. Public `src/**/*.fsi` are expected to be **unchanged**
(charts reuse existing primitives; `Control.render` signature is stable) — confirmed
below; if a delta is discovered during implementation it returns to planning.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Principle I (Spec→FSI→Tests→Impl):** Followed. The only new public-ish surface
is internal to the render-capable harness (the `ContentSignature` type and the
gate). It is sketched in `contracts/content-signature.contract.md` and exercised
by the harness fidelity tests before the renderer bodies are written. The renderer
change is behind the stable `Control.render` signature.

**Principle II (Visibility in `.fsi`):** No new public package `.fsi` is expected.
`Control.fs` renderer changes are internal (`ControlInternals`); the `Scene` `.fsi`
is untouched because charts lower to existing primitives. If implementation finds a
genuinely needed public symbol, its `.fsi` and per-package baseline are updated in
the same change (`PerPackageSurface.captureCurrent`).

**Principle III (Idiomatic simplicity):** Per-control geometry is plain functions
returning `SceneNode` groups; one `match control.Kind with` dispatch. A `mutable`
pixel-coverage accumulator over the decoded bitmap row buffer is allowed on the
decode hot path with a `// mutable: hot path` note (Principle III). No SRTP,
reflection, providers, or exotic CEs.

**Principle IV (MVU boundary):** **N/A — no stateful or I/O-bearing workflow.**
Rendering is a pure `Control -> Scene` transform; the gate is a pure decode +
assert over committed bytes. No `Model`/`Msg`/`Effect`/interpreter is introduced.

**Principle V (Synthetic evidence):** The fidelity **fixtures** are deliberately
synthetic *inputs* (079-style label-on-box PNGs + faithful PNGs) used to prove the
gate's discrimination — they are gate test vectors, not product evidence, and are
disclosed as a retained fixture set, not passed off as live renders. They are **not**
`[S]` product evidence: the catalog previews themselves are real renders. No mocks/
fakes substitute for product capability. No `[S]`/`[SEH]` anticipated; if any task's
real evidence proves infeasible it is disclosed at all five surfaces.

**Principle VI (Test evidence):** Failing-first is structural to the feature
(SC-003): the gate must be demonstrated **red** on the pre-fix previews/fixtures
before green. Tests: extraction tests (`chartValues` now yields the series),
renderer tests (per-family geometry present in `Scene.describe`), gate tests
(signature pass/fail + fixtures + fail-closed), recaptured `Scene.describe`/
screenshot baselines.

**Principle VII (Observability):** The gate fails with an actionable message
**naming the control and the missing signature** (FR-007/FR-013). Render-capable
host warnings are classified benign vs blocking via the `fs-skia-evidence-mode`
discipline. The renderer emits no silent empty-state — empty/missing data renders
a recognizable honest empty state or is `Unsupported` (FR-011).

### Repository Governance Decisions

- **Template ownership**: No `.template.config/template.json` change. The
  renderer (`src/Controls`, `src/SkiaViewer`), the catalog facts/docs generators
  (`build/Governance`), and the preview harness (`tests/ControlsPreview.Harness`)
  are not template-content-selected surfaces; the new gate is a repo governance
  target, not a generated-project capability. Generated products do not ship the
  catalog-preview pipeline. **Deferral rationale**: template posture is exercised
  on the escalated path (`TemplateCheck`/`GeneratedProductCheck`) but no template
  edit is required. The packable libraries (`FS.Skia.UI.Controls`,
  `FS.Skia.UI.SkiaViewer`, `FS.Skia.UI.Build`, `FS.Skia.UI.Scene`) and every other
  packable project bump to the next `-preview` on merge per convention, and
  `FS.Skia.UI.Build` is packed alongside the `src` libs (build-package-version
  drift gotcha).
- **Dependency impact**: **N/A — no dependency change.** SkiaSharp is already
  pinned in `Directory.Packages.props` and already referenced by the render-capable
  harness; the pixel decode uses `SKBitmap`/`SKImage` from the existing package.
  No `docs/dependencies.md` or `DependencyReport` change.
- **Command-surface impact**: **Yes.** A new FAKE target `ControlFidelityCheck`
  is added: `Targets.Target` DU case + `allTargets` + `name`/`spec`/timeout/cost/
  owner + `AgentValidation.knownGates` + a `StartTarget` case in
  `Engine/Update.fs` that shells out `dotnet run --project
  tests/ControlsPreview.Harness -- --fidelity` (render-capable, sequential,
  SkiaSharp-free build side). It is wired into the `controls-catalog-docs` routing
  rule's `RequiredGates` so preview-asset changes require it (FR-012).
  `validation.contract.yml` is **regenerated from `Routing.fs`** via
  `RefreshSurfaceBaselines`; `TargetMetadataDrift` enforces currency. FAKE-backed
  targets run sequentially in the deterministic order. Example order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t ControlFidelityCheck`
  (full escalated order in **Validation** below).
- **Generated project impact**: **N/A — no generated-project content change.**
  The faithful renderer is framework-internal; generated products consume
  `Control.render` unchanged (same signature, richer output). No change to
  default/minimal contents, selected-Controls guidance, local skills, validation
  logs, placeholder/excluded-history scans, or generated `Dev`.
- **Evidence paths**:
  - Real per-control PNGs: `docs/img/controls/<id>.png` (regenerated; one per
    Demonstrative catalog control; no image for `Unsupported`).
  - Fidelity fixtures (retained): `tests/ControlsPreview.Harness/fixtures/fidelity/`
    (`lowfi/<id>.png` label-on-box, `faithful/<id>.png` counterparts).
  - Gate report: `readiness/control-fidelity.md` (decoded-content report: per
    control, signature, pass/fail, fixture results).
  - Recaptured baselines: `Scene.describe` snapshots and screenshot baselines
    that move (chart/collection/value controls) under `tests/**`; per-package
    surface snapshots only if a `.fsi` actually changes.
  - Corrected prose: regenerated `docs/controls/<id>.md` Preview sections;
    `specs/080-control-render-fidelity/readiness/real-image-evidence.md` and
    `usage-coherence.md` authored against decoded images; a correction note on
    the 079 readiness overclaims pointing to 080.
  - FSI transcripts: `chartValues`/`Control.render` exercised through the packed
    library where `FsiTranscripts` applies.
- **`.fsi` / contract impact**: Public package `.fsi` expected **unchanged**
  (charts reuse existing `Scene` primitives; `Control.render`/`Widget.render`
  signatures stable). The **contract surface that does change** is governance:
  the new build target + routing rule + regenerated `validation.contract.yml`.
  The behavioral contract `Scene.describe` output changes for chart/collection/
  value controls (snapshots recaptured). Compatibility note: no source-breaking
  change to consumers; preview output is strictly richer.
- **MVU/effect boundary**: **N/A — no stateful or I/O-bearing work.** Pure
  `Control -> Scene` rendering and a pure decode-and-assert gate (file reads at
  the gate's edge only, no workflow state).
- **Synthetic evidence**: The retained **fidelity fixtures** are synthetic gate
  *test vectors* (079-style label-on-box + faithful), disclosed as a fixture set
  in the spec (Key Entities) and a `(* SYNTHETIC FIXTURE: ... *)` banner in the
  gate test file. They prove gate discrimination and are **not** product evidence;
  the catalog previews are real renders, so no product task is `[S]`. No
  `--accept-synthetic` anticipated.
- **Test evidence**: Failing-first (SC-003): commit fixtures + assert the gate
  fails the low-fi set and 079 previews before the renderer fix lands, then green
  after. Semantic tests: `chartValues` extraction, per-family geometry in
  `Scene.describe`, signature pass/fail, fail-closed (a catalog id with no
  signature/Unsupported fails), totality over `catalogFacts`. Governance tests:
  `ControlFidelityCheck` registered, routing rule present, `validation.contract.yml`
  current (`TargetMetadataDrift`). Host smoke: render-capable harness runs.
- **Observability**: Gate message names the control + missing signature
  component (kind absent / coverage below threshold / off-title-band empty).
  Report fields in `readiness/control-fidelity.md`: control id, declared
  signature, decoded coverage, present primitive kinds, verdict, fixture matrix.
  Missing-artifact-class failure: a Demonstrative control with no committed PNG,
  or a catalog id with neither signature nor `Unsupported`, fails loudly.
  Unsupported-environment message: when native Skia is absent the gate reports a
  classified blocking host warning (cannot decode) rather than silently passing.
- **Deferred scope**: Out of scope (bounded follow-ups, not this feature):
  pixel-perfect design-system styling parity; interactive widget toolkit; GPU/
  Vulkan dependency for the gate; new runtime control behavior; extending the
  fidelity gate to non-catalog images. `custom-control` stays `Unsupported`.

**Gate evaluation:** PASS. No unjustified violations. Principle IV and the
Dependency/Generated-project/Template areas are N/A with rationale; all other
areas carry concrete decisions. Re-evaluated post-Phase-1: unchanged (no new
public `.fsi`, no MVU surface, no new dependency surfaced by the design).

## Project Structure

```
specs/080-control-render-fidelity/
├── spec.md
├── plan.md                      # this file
├── research.md                  # Phase 0
├── data-model.md                # Phase 1
├── quickstart.md                # Phase 1
├── contracts/
│   ├── content-signature.contract.md   # per-control ContentSignature shape + fail-closed
│   └── fidelity-gate.contract.md        # ControlFidelityCheck target + report + fixtures
└── readiness/                   # (authored during implementation)
    ├── control-fidelity.md
    ├── real-image-evidence.md
    └── usage-coherence.md

Code touched:
src/Controls/Control.fs                  # chartValues fix + per-control faithful geometry (renderNode)
src/Controls/Charts.fs(.fsi)             # (read-only ref; series shapes already correct)
src/SkiaViewer/SceneRenderer.fs          # stop emitting opaque Chart on preview path / bounds-safe (charts now primitives)
tests/ControlsPreview.Harness/
  ├── PreviewSamples.fs                  # FR-014 sample data + per-control ContentSignature field
  ├── PreviewRender.fs                   # unchanged path; regenerates faithful PNGs
  ├── PreviewHarnessTests.fs             # totality/explicitness/idempotence (retained)
  ├── Fidelity.fs (new)                  # decode + signature check + fixtures (--fidelity mode)
  └── fixtures/fidelity/{lowfi,faithful} # retained committed fixture set
build/Governance/
  ├── Targets.fs(.fsi)                   # + ControlFidelityCheck case/registry/name/spec
  ├── Routing.fs                         # + ControlFidelityCheck in controls-catalog-docs RequiredGates/Paths
  ├── AgentValidation.fs                 # + "ControlFidelityCheck" in knownGates
  ├── Engine/Update.fs                   # + StartTarget ControlFidelityCheck shellout
  └── CatalogDocsGen.fs                  # regenerated detail-page Preview prose (corrected)
validation.contract.yml                  # regenerated from Routing.fs (RefreshSurfaceBaselines)
docs/img/controls/*.png                  # regenerated faithful previews
docs/controls/*.md                       # regenerated corrected Preview prose
```

## Validation (escalated `maintainer-verify` serialized path)

FAKE-backed targets are run **sequentially** in deterministic order (shared
`.fake` state). After authoring, run `./fake.sh build -t Route --enforce` first
to confirm the tier/gate list and that escalated evidence artifacts are present,
then the serialized order, inserting the new render-capable gate:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t ControlFidelityCheck`  *(render-capable; the new gate)*
3. `./fake.sh build -t GeneratedGuidanceCheck`
4. `./fake.sh build -t TemplateCheck`
5. `./fake.sh build -t GeneratedProductCheck`  *(known non-authoritative local env failure — see memory)*
6. `./fake.sh build -t EvidenceGraph`
7. `./fake.sh build -t EvidenceAudit`

Plus the controls-public-surface gates the route prints
(`ControlsCatalogDocsCheck`, `ControlsCatalogCheck`, `ControlsRenderingCheck`,
`PackageSurfaceCheck`, `FsiTranscripts`) and the render-capable harness suite
(`dotnet run --project tests/ControlsPreview.Harness -- --sequenced`).
`TargetMetadataDrift` confirms `validation.contract.yml` was regenerated, not
hand-edited.

## Phase 0 / Phase 1 outputs

- Phase 0: [research.md](./research.md) — resolves the renderer, gate-placement,
  signature-shape, and fail-closed decisions.
- Phase 1: [data-model.md](./data-model.md), [contracts/](./contracts/),
  [quickstart.md](./quickstart.md). Agent context (`AGENTS.md` SPECKIT block)
  updated to point at this plan.
