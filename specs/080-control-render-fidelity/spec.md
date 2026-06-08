# Feature Specification: Faithful Control Preview Rendering

**Feature Branch**: `080-control-render-fidelity`  
**Created**: 2026-06-08  
**Status**: Draft  
**Input**: User description: "@docs/reports/2026-06-08-1255-control-preview-fidelity-analysis.md" — post-mortem of feature 079: the committed/deployed control preview images are uniform text-label schematics, not faithful depictions of each control; per-control evidence prose was overclaimed; no governance gate can detect low-fidelity-but-real images. Remediate per the report's recommendation (Option B + the decoded-content gate from §8, with honest evidence).

## Overview

The published control catalog shows a preview image for each control in `FS.Skia.UI.Controls`. Today those images are produced by a deliberately minimal schematic renderer that draws **every** control the same way — a filled rectangle plus one clipped text label. The label is the control's text content, or, when it has none, its kind id (e.g. `"line-chart"`). As a result, a large set of controls (all charts, `graph-view`, every attribute-backed collection, `separator`, `spinner`, `image`, `icon`) renders as a single word on a box — only marginally more informative than the near-blank images it replaced — while value/selection controls (`slider`, `progress-bar`, `radio-group`, `tabs`, `switch`, `check-box`) show a bare token rather than the widget. The readiness evidence additionally described per-control content that was never visually verified, so a deployed public site carries claims its images do not support.

This feature replaces the schematic preview path with a **faithful headless control renderer** so each preview visually depicts its control using real layout-aware geometry, regenerates every catalog preview from that renderer, corrects the evidence to match what the images actually show, and adds a **pixel-decoding fidelity gate** so a low-fidelity-but-real preview can no longer pass governance silently.

## Clarifications

### Session 2026-06-08

- Q: How should the fidelity gate define what counts as a faithful depiction — what content signature does it assert per preview? → A: Per-control signature — each control declares its own expected content signature (required primitive kinds and/or lit-pixel regions); the gate checks that control's specific signature, so a chart that rendered as a box fails even when its raw pixel count is high.
- Q: When a new catalog control is added later with no fidelity signature and no explicit Unsupported declaration, how should the gate behave? → A: Fail closed — any catalog control lacking both a fidelity signature and an explicit Unsupported status fails the gate, forcing the author to declare one.
- Q: For catalog controls that have no existing sample data, what is the intended behavior? → A: Author representative, font-safe sample data for any control missing it so it renders faithful control-specific geometry.
- Q: How should the failing-first red demonstration (gate must fail 100% of pre-fix 079 previews) be wired as a durable test input? → A: Retained fixture set — commit a small set of 079-style label-on-box previews as a permanent fixture; the gate test asserts it fails those and passes the regenerated faithful previews.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A reader recognizes a control from its preview (Priority: P1)

A developer browsing the published control catalog opens a control's page and looks at its preview image to understand what the control is and roughly what it looks like, without reading prose or running code.

**Why this priority**: This is the core promise of "demonstrative previews" that feature 079 made and did not keep. Without it the catalog previews carry little value over a blank box for a large share of controls.

**Independent Test**: Render the full catalog with the new renderer, open the previews for one control from each visual family (a chart, a collection, a slider, a checkbox, an icon, an image), and confirm each preview shows control-specific visual structure (e.g. a plotted series, item rows, a track-and-thumb, a tick in a box, a recognizable glyph, a framed image placeholder) rather than only a label on a box.

**Acceptance Scenarios**:

1. **Given** a chart control (`line-chart`, `bar-chart`, `pie-chart`, `scatter-plot`) with sample series data, **When** its preview is rendered, **Then** the image contains the corresponding chart geometry (a polyline / filled bars / pie arcs / plotted points) laid out inside the preview canvas bounds, not only a title label.
2. **Given** an attribute-backed collection control (`list-box`, `list-view`, `combo-box`, `tree-view`, `multi-select-list`), **When** its preview is rendered, **Then** the image shows multiple distinct item rows drawn from the control's sample items, not only the control's kind id.
3. **Given** a value/selection control (`slider`, `progress-bar`, `radio-group`, `tabs`, `switch`, `check-box`), **When** its preview is rendered, **Then** the image shows the control's chrome (track + thumb, filled progress track, radio circles with the selection marked, a tab strip with the active tab, a toggle in its state, a tick/box) in addition to any value text.
4. **Given** `image` and `icon`, **When** their previews are rendered, **Then** `image` shows a framed image placeholder (not the path string alone) and `icon` shows a glyph supported by the rendering font (not a missing-glyph box).

---

### User Story 2 - A maintainer trusts that "demonstrative" is enforced, not asserted (Priority: P1)

A maintainer reviewing a future change that regenerates previews wants assurance that a preview which is technically a valid PNG but does not actually depict its control will be rejected by the build, rather than relying on a byte-size floor and an author's prose description.

**Why this priority**: The root failure in 079 was that every gate was structural/byte-level/text-pattern based, so a real-but-inadequate image passed. Without a pixel-decoding fidelity check the same defect can recur silently.

**Independent Test**: Take a known low-fidelity preview (a label-on-a-box render of a chart) and a known faithful preview of the same control, run the fidelity gate against both, and confirm the gate fails the low-fidelity image and passes the faithful one, with a message naming the control and the missing content signature.

**Acceptance Scenarios**:

1. **Given** a preview image that contains only a title-band label and background, **When** the fidelity gate runs, **Then** it fails and names the control and the expected-but-absent content signature.
2. **Given** a preview image that contains the control's expected content (lit pixels / required primitive kinds outside the title band), **When** the fidelity gate runs, **Then** it passes.
3. **Given** the catalog currency check, **When** previews are regenerated, **Then** the fidelity gate is part of the evidence required for any change that touches preview assets, so faithful content is a release condition, not an optional review step.

---

### User Story 3 - The catalog and evidence stop overclaiming (Priority: P1)

A reader of the deployed docs and a reviewer of the feature's readiness evidence should find that every per-control claim matches what the corresponding image actually shows.

**Why this priority**: The genuine honesty failure in 079 was a false prose claim about real evidence, shipped publicly. Correcting it is required to restore the catalog's and the evidence regime's credibility.

**Independent Test**: For every control claim in the readiness/evidence files, decode the referenced image and confirm the described content is visibly present; for any control that cannot be rendered faithfully, confirm it is honestly marked unsupported rather than described as depicted.

**Acceptance Scenarios**:

1. **Given** the regenerated catalog, **When** any per-control evidence claim is checked against its image, **Then** the claim is supported by visible content in the image.
2. **Given** a control that the renderer cannot depict faithfully, **When** the catalog is published, **Then** that control is presented as honestly unsupported (no image plus an explicit unsupported status) rather than as a depicted preview.

---

### Edge Cases

- **Control with no sample data** (e.g. a chart whose series are empty): the preview MUST either render an honest empty-state that is still recognizable as that control, or be marked unsupported — it MUST NOT render off-canvas or as a bare label that the fidelity gate would (incorrectly) pass.
- **Glyph/font availability**: `icon` and any glyph-bearing control MUST only use sample glyphs present in the rendering font; a missing-glyph box does not satisfy fidelity.
- **Canvas bounds**: all control geometry MUST lay out within the preview canvas; the chart placeholder painter's fixed off-canvas coordinates (the 079 bug) must not recur — the fidelity gate's "content outside the title band" signature is the backstop.
- **`custom-control`**: remains honestly unsupported (no image), consistent with 079.
- **Fidelity gate in GPU-free CI**: the existing byte-floor/structural catalog check runs SkiaSharp-free by design; the new pixel-decoding fidelity check requires a render-capable step. The two MUST be separated so the GPU-free currency gate still runs everywhere while the fidelity gate runs in the render-capable harness.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST render each catalog control's preview using a faithful, layout-aware renderer that draws control-specific geometry from the control's own data/state, not a single uniform rectangle-plus-label primitive for all controls.
- **FR-002**: Chart controls MUST render their actual series data as chart geometry (polyline for line, filled bars for bar, arcs for pie, plotted points for scatter) laid out within the preview canvas; the data-extraction path MUST read the structured series shape the typed controls actually store (so extraction no longer silently yields empty data).
- **FR-003**: Attribute-backed collection controls MUST expand their sample items into multiple visible rows/nodes in the preview, rather than rendering only the container's kind id.
- **FR-004**: Value/selection controls MUST render their characteristic chrome and state (track+thumb, filled progress, radio circles with selection, tab strip with active tab, toggle state, tick/box), not only a value token.
- **FR-005**: `image` MUST render a framed image placeholder and `icon` MUST render a font-supported glyph; neither may render only a path/name string or a missing-glyph box.
- **FR-006**: Every catalog preview asset MUST be regenerated from the new renderer through the real render-only evidence path (a genuine decodable PNG of the documented dimensions).
- **FR-007**: The system MUST provide a pixel-decoding **fidelity gate** that validates a preview actually depicts its control by asserting a **per-control content signature** — each catalog control declares its own expected signature (required primitive kinds and/or lit-pixel coverage in regions outside the title band), and the gate checks that control's specific declared signature rather than a single uniform threshold (so a chart rendered as a box fails even when its raw non-background pixel count is high). The gate MUST fail with a message naming the control and the missing signature.
- **FR-008**: The fidelity gate MUST run in a render-capable harness and MUST remain separate from the existing SkiaSharp-free, GPU-free catalog currency/byte-floor check, so the currency check still runs in GPU-free CI while fidelity is enforced where decoding is possible.
- **FR-009**: For any control that the renderer cannot depict faithfully, the catalog MUST present it as honestly unsupported (no image plus an explicit unsupported status) rather than as a depicted preview; `custom-control` remains unsupported.
- **FR-010**: All per-control evidence and catalog prose MUST be corrected so each claim matches the content visibly present in its image; no per-control visual claim may be authored without viewing (or gate-verifying) the corresponding image.
- **FR-011**: Empty-data or missing-data controls MUST render a recognizable honest empty state within canvas bounds or be marked unsupported; they MUST NOT render off-canvas or as a bare label.
- **FR-012**: The fidelity requirement MUST be enforced as a release condition for any change that touches preview assets (the gate is part of the required evidence), so a low-fidelity-but-real preview cannot pass governance silently.
- **FR-013**: The fidelity gate MUST **fail closed**: any catalog control that has neither a declared per-control fidelity signature nor an explicit `Unsupported` status fails the gate with a message naming the control. A control newly added later therefore cannot pass governance until an author either declares its signature or marks it unsupported — the 079 silent-pass failure mode cannot recur for future controls.
- **FR-014**: For any catalog control lacking existing sample data (so a faithful render would otherwise yield empty geometry), representative, font-safe sample data MUST be authored so the control renders faithful, control-specific geometry. Authoring sample data is the default path to faithfulness; FR-011's honest empty-state remains the backstop only for controls deliberately demonstrating an empty state, and `Unsupported` (FR-009) remains the fallback only when neither authored data nor an honest empty-state yields a recognizable depiction.

> Interacting / conflicting requirements: FR-001 (faithful depiction for all controls) vs. FR-009 (honest unsupported for controls that cannot be depicted). Resolution: faithfulness is the default and target for every catalog control; `Unsupported` is the explicit fallback only when a faithful render is not achievable, and choosing it is itself an honest, gate-visible outcome — never a silent low-fidelity image. The fidelity gate (FR-007) is what forces the choice: a control either passes fidelity or is declared unsupported.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Changes the contents of `FS.Skia.UI.Controls` (the control renderer / data extraction) and `FS.Skia.UI.SkiaViewer` (the scene painter, especially chart geometry). Adds a fidelity gate to `FS.Skia.UI.Build` (governance). Active control package path is `src/Controls/**`; the legacy `Charts` package is not the authoring path — chart geometry is rendered via the Scene painter from typed control data. Package versions of every packable project will bump on merge per project convention.
- **Public contract impact**: Likely escalates. A layout-aware/bounds-bearing Scene node or revised chart node is a **public Scene surface change** (public `.fsi`), and the renderer change alters `Scene.describe` output. A new build target (the fidelity gate) is a governance/contract surface. Treat this feature as escalated to the `maintainer-verify` path; confirm exact `.fsi` deltas during planning.
- **State workflow impact**: None. No stateful workflow, I/O, command, effect, subscription, or interpreter behavior changes — this is rendering and documentation-asset fidelity only.
- **Layout/rendering impact**: This is the core of the feature. Layout, charts, collections, rendering, screenshots, Skia, and visual output all change; screenshot/render baselines and `Scene.describe` snapshots will move and must be recaptured. Unsupported-environment diagnostics for the render-capable fidelity step must be classified (benign vs blocking host warnings).
- **Evidence obligations**: Real, viewed per-control PNG renders for every catalog control (or an honest unsupported marker); the fidelity gate's decoded-content report demonstrating it fails low-fidelity and passes faithful images; recaptured render/screenshot baselines; corrected readiness/catalog prose verified against decoded images. Failing-first: the fidelity gate must be demonstrated red on the pre-fix (079) previews before going green, wired durably as a **retained committed fixture set** (label-on-box fixtures asserted to fail, faithful fixtures asserted to pass) rather than a one-time manual run.
- **Unsupported scope**: Not building an interactive widget toolkit or a general-purpose GUI rendering of live controls; no GPU/Vulkan dependency for the gate; no new runtime control behavior; `custom-control` stays unsupported; no change to product/runtime control semantics; pixel-perfect visual styling parity with a target design system is out of scope (recognizable, control-specific depiction is the bar, not visual-design fidelity).
- **Build-target impact**: Adds a new render-capable fidelity gate target (decoded-content check) and wires it into the required evidence for preview-asset changes. The SkiaSharp-free catalog currency/byte-floor check (`ControlsCatalogDocsCheck` / `TrivialPreview`) is retained for GPU-free CI but is no longer treated as the fidelity guarantee. `EvidenceGraph` and `EvidenceAudit` participate; `GeneratedGuidanceCheck`/`TemplateCheck`/`GeneratedProductCheck` are exercised on the escalated path. Routing/`validation.contract.yml` updates if a new gate is registered.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of catalog controls either render a faithful, control-specific preview (control-specific geometry/chrome/content visibly present) or are honestly marked unsupported; 0 controls render as a label-on-a-box that the fidelity gate would otherwise pass.
- **SC-002**: A reviewer shown the regenerated previews for a one-per-family sample (chart, collection, value/selection, icon, image, layout) can correctly identify each control's family from the image alone.
- **SC-003**: The fidelity gate fails on 100% of the pre-fix (079) low-fidelity Tier-3 previews and passes on 100% of the regenerated faithful previews, demonstrated as a failing-first → passing transition and guarded durably by a **retained committed fixture set** (a small set of 079-style label-on-box previews plus their faithful counterparts) that the gate test asserts it fails and passes respectively.
- **SC-004**: 100% of per-control evidence and catalog claims are supported by content visibly present in the referenced image (zero unverifiable per-control visual claims remain).
- **SC-005**: The fidelity gate is a required part of the evidence for preview-asset changes, so a future low-fidelity-but-real preview is blocked by the build without relying on a reviewer manually decoding images.
- **SC-006**: No functional or runtime regression in product control behavior (this remains a rendering and documentation-asset change); existing non-preview tests continue to pass.

## Assumptions

- **Scope is the durable fix (report Option B + the §8 decoded-content gate), with honest evidence folded in.** Because regenerating faithful previews makes the catalog claims true, this feature also resolves the 079 honesty overclaim (report Option A) as part of its evidence correction, rather than leaving Option A as a separate prior step. If the maintainer wants the honesty relabel shipped independently and sooner, that can be sequenced within the plan; the spec's success conditions are unchanged either way.
- **"Faithful" means recognizable and control-specific, not pixel-perfect design fidelity.** The bar is that a reader can recognize the control and its rough appearance/state from the image; matching a specific design system's exact styling is out of scope.
- **The render-capable fidelity gate runs where SkiaSharp + native Skia are available** (the existing render-only evidence harness), not in the GPU-free currency gate. CI topology that already supports the render-only evidence path is assumed sufficient.
- **Sample data for previews** (chart series, collection items, selected options, glyphs) is authored to be representative and font-safe; existing catalog sample-data conventions are reused where present, and per FR-014 new representative sample data is authored for any control that lacks it so it can render faithful geometry rather than falling back to an empty-state or unsupported marker.
- **This feature follows the escalated `maintainer-verify` / serialized validation path** given the public Scene-surface and governance-gate impact.

## Dependencies

- The Scene primitives the painter already rasterizes correctly (polylines, filled rects, points/circles, arcs, text) — proven by prior renderer tests — are the building blocks for faithful geometry.
- The existing render-only evidence path (typed front door → `Control.render` → headless screenshot capture) for generating real PNGs.
- The catalog currency/generation pipeline (`CatalogDocsGen` / `ControlsCatalogDocsCheck`) for wiring the regenerated assets and the new gate.

## Key Entities

- **Control preview**: a committed PNG asset, one per catalog control, of documented dimensions, depicting that control; or an explicit unsupported marker in place of an image.
- **Content signature**: the **per-control** criteria the fidelity gate asserts a preview must satisfy — each catalog control declares its own signature (required primitive kinds and/or lit-pixel coverage in regions outside the title band), distinguishing a faithful depiction from a label-on-a-box. A control with no declared signature and no `Unsupported` status fails the gate (fail-closed, FR-013).
- **Fidelity fixture set**: a small retained, committed set of preview images — 079-style label-on-box renders paired with their faithful counterparts — used as durable gate-test inputs to assert the gate fails low-fidelity images and passes faithful ones (SC-003).
- **Fidelity gate**: the render-capable governance step that decodes each preview and validates its content signature, separate from the GPU-free byte-floor currency check.
- **Catalog/readiness evidence**: the prose claims about each control's preview, which must match decoded image content.
