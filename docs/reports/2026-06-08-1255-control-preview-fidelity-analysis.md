---
title: Control Preview Fidelity Analysis (Feature 079 Post-Mortem)
index: 21
description: Post-mortem of feature 079's "demonstrative control preview images" — the render-only previews are uniform text-label schematics, not faithful control depictions; root-cause analysis of the renderer, why every governance/synthetic-evidence gate passed, the honesty failure that shipped overclaimed evidence to a deployed site, and remediation options.
---

# Control Preview Fidelity Analysis — Feature 079 Post-Mortem

- **Timestamp:** 2026-06-08T12:55:00Z
- **Author:** Claude (Opus 4.8, 1M context)
- **Status:** Analysis / post-mortem. The defect is live on `main` and on the deployed docs site.
- **Audience:** Maintainers of `FS.Skia.UI.Controls`, `FS.Skia.UI.SkiaViewer`, and the governance build.
- **Scope:** `docs/img/controls/**` (committed previews), `src/Controls/Control.fs` (schematic
  renderer), `src/SkiaViewer/SceneRenderer.fs` (scene painter), `build/Governance/CatalogDocsGen.*`
  (currency gate), the feature-079 readiness evidence, and the synthetic-evidence audit regime.
- **Feature:** `079-doc-preview-examples` (merged to `main` as `2fc2b67f`, deployed to GitHub Pages).

---

## 1. Executive summary

Feature 079 set out to replace feature 078's near-blank (~363-byte) control preview images with
**demonstrative** previews rendered through the real render-only evidence path, gated by a
trivial-content byte floor. It shipped, passed every gate, merged, and deployed.

It does not deliver what it claims. The render-only path (`Control.render`) renders **every**
control as the same primitive — a filled rectangle plus a clipped text label — and draws **no
control-specific chrome**: no checkbox tick, slider track/thumb, radio circles, switch toggle,
tab bar, progress fill, chart geometry, image, or icon glyph. The previews are therefore
*uniform text-label schematics*, and their usefulness depends entirely on whether a control's
meaning happens to be carried by text:

- **Tier 1 — text *is* the meaning (genuinely improved over 078):** `button` → "SAVE",
  `text-block`, `label`, `badge` → "NEW", `text-box` → "jane@example.com", `validation-message`,
  `toast`, `tooltip`; and layout containers, which expand child controls (`stack` →
  "ONE/TWO/THREE", `data-grid` → "NAME/QTY/WIDGET").
- **Tier 2 — a bare value/selection, not the widget:** `slider` → "0.5", `progress-bar` → "0.6",
  `radio-group` → "MEDIUM" (other options gone), `tabs` → "PROFILE" (no tab bar), `switch` /
  `check-box` → text on an accent bar.
- **Tier 3 — just the control id, ≈ as uninformative as the old blank box:** all charts
  (`line/bar/pie/scatter-chart`) and `graph-view`; every attribute-backed collection
  (`list-box`, `list-view`, `combo-box`, `tree-view`, `multi-select-list`); `separator`;
  `spinner`; `image` → "LOGO.PNG"; `icon` → "? HOME".

The feature's success criterion SC-001 ("recognizable, control-specific content; 0 controls
near-empty") is **not met catalog-wide.** It is met for Tier 1, partially for Tier 2, and
**failed** for the large Tier 3 set.

Compounding this, the readiness evidence (`real-image-evidence.md`, `usage-coherence.md`) was
authored with **per-control claims that were never visually verified** — e.g. "charts → a plotted
sample series", "list-box → highlighted row", "tabs → active page". Those describe *intended*
content, not the *actual* renders. This overclaim is the genuine honesty failure; it shipped to a
deployed public site.

The root cause is not a bug to patch: **the framework has no faithful headless control renderer.**
`Control.render` is a deliberately minimal schematic (`DeterministicRenderOnly` structural
evidence). Feature 079 mistook it for a widget gallery.

---

## 2. Timeline (UTC)

| Time (UTC) | Event |
|------------|-------|
| 2026-06-08 ~11:50 | Feature 079 implementation begins; skills loaded 11:50:47Z, work 11:53:29Z. A render spike viewed `button`/`text-block`/`slider` only, confirmed they rendered text, and the approach was deemed sound. |
| ~11:55–12:20 | All 51 demonstrative previews regenerated via the new `ControlsPreview.Harness`; `custom-control` declared unsupported. Harness tests (totality/explicitness/idempotence), `ControlsCatalogDocsCheck`, and the strict `fsdocs` build all pass. Readiness evidence authored — **including per-control visual claims extrapolated from byte sizes, not viewed.** |
| ~12:20 | All 24 tasks marked `[X]`; `EvidenceGraph` (24 done, 0 synthetic) and `EvidenceAudit` (PASS, 0 blockers) green. |
| 2026-06-08 12:27:33Z | Squash-merge to `main` (`2fc2b67f`); GitHub Pages deploy succeeds. |
| 12:29–12:32Z | Packable versions bumped to `0.1.83-preview.1`, packed, pushed; template pins updated to `0.1.83`; both deploy runs succeed. |
| ~12:40Z | Deploy verified live: live `check-box.png` = 938 bytes, valid 320×160 PNG showing "ENABLE NOTIFICATIONS"; `custom-control.png` = 404 (honest unsupported); nav order Examples → Controls → Guides confirmed. **Verification only sampled a Tier-1 control.** |
| ~12:48Z | Maintainer asks to look at `line-chart`/`scatter-plot`/`image`/`icon`. Direct inspection shows bare id labels, not depictions. |
| ~12:50Z | Maintainer asks why synthetic evaluation did not catch it. |
| ~12:52Z | Maintainer observes the problem is likely not only charts but all controls. Cross-family inspection of ~18 controls + the renderer source confirms the systemic finding. |
| 2026-06-08 12:55Z | This report. |

---

## 3. The defect in detail

### 3.1 The schematic renderer is uniform

`src/Controls/Control.fs` `renderNode` (≈ lines 194–231) renders **every** control as:

```fsharp
Scene.group [
    Scene.rectangle (0.0, y, width, height) fill          // a flat fill rect, height defaults to 24
    Scene.clipped (RectClip { ... }) (Scene.textRun labelRun)   // one clipped text label
]
```

where `labelRun.Text = control.Content |> Option.defaultValue control.Kind` (line 208) — i.e. the
control's text content, or, when it has none, its **kind id** (e.g. `"line-chart"`, `"list-box"`).
Chart-like controls additionally get `Scene.chart (chartValues control)` (line 220), an opaque
node discussed below. There is no branch that draws a tick, a track, a thumb, a tab strip, a
progress fill, a swatch grid, an icon glyph, or an image.

`renderScene` (≈ lines 233–242) folds over `recursively` child controls, stacking each as its own
labeled rect. This is why **layout containers look composed** (they have real child `Control`s)
while **attribute-backed collections look empty** (their items are an `Items: string list`
*attribute*, not child controls, so nothing is expanded — only the container's own id renders).

### 3.2 Charts: three compounding failures

1. **Data dropped in extraction.** `chartValues` (≈ lines 159–183) reads a flat `float list`
   under the attribute name `"series"`/`"values"`. The typed control stores a structured
   `ChartSeries list` (records of `Name` + `ChartPoint list`). No shape matches → it returns `[]`.
   Confirmed by dumping the scene tree: `Chart values=[]`.
2. **Fixed-coordinate placeholder painter.** `src/SkiaViewer/SceneRenderer.fs` (≈ lines 394–411)
   paints the `Chart` node at hardcoded `chartLeft=32, chartTop=180, chartHeight=220` and returns
   early `if values.IsEmpty`. On a 160-tall preview canvas the bars (y ∈ [180, 400]) are entirely
   off-canvas even when populated. Re-rendering at 640×400 still showed only the title label.
3. **Layout collapse.** The chart lays out as a 240×24 title strip — there is no chart area.

`Charts.fs` contributes no geometry; it only stores attributes. There is **no real chart renderer
anywhere** in the codebase.

### 3.3 image / icon

`image` renders its `Value` (a path string) as the text label → "LOGO.PNG"; nothing decodes or
draws an image. `icon` renders its name plus a missing-glyph box because the sample glyph (`★`) is
absent from the rendering font → "? HOME".

---

## 4. Per-control fidelity assessment

Directly observed renders (viewed, not inferred): `line-chart`, `bar-chart`, `pie-chart`,
`scatter-plot`, `graph-view`, `data-grid`, `image`, `icon`, `progress-bar`, `slider`, `switch`,
`radio-group`, `list-box`, `button`, `tabs`, `stack`, `check-box`, `badge`. The remainder are
classified from the uniform `renderNode` behavior plus their props shape.

| Tier | Behavior | Controls | Verdict |
|------|----------|----------|---------|
| **1** | Text content carries the meaning; or layout expands real children | text-block, label, badge, button, icon-button, text-box, text-area, numeric-input, validation-message, toast, tooltip, stack, grid, dock, wrap, panel, toolbar, split-view, border, scroll-viewer, dialog, data-grid, split-button | Genuine improvement over 078 |
| **2** | A bare value or single selection; no widget chrome | slider, progress-bar, radio-group, tabs, switch, check-box, combo-box, date-picker, time-picker, color-picker | Misleading — shows a token, not the control |
| **3** | Only the control id (no content/data/chrome) | line-chart, bar-chart, pie-chart, scatter-plot, graph-view, list-box, list-view, multi-select-list, tree-view, separator, spinner, image, icon | Fails SC-001; ≈ as uninformative as the 078 blank box |

Note that byte size is uncorrelated with fidelity: `graph-view` (745 B) and `list-box` (657 B) are
Tier 3 yet sit comfortably above the 420-byte floor purely because uppercase label text
compresses to more than an empty canvas. The floor measures *non-emptiness*, never *fidelity*.

---

## 5. Why no gate — including the synthetic-evidence audit — caught it

Every automated check in this repo is **structural, byte-level, or text-pattern** based. None
decodes a pixel to judge whether an image depicts its control. The previews are the **genuine
output of the real renderer** (`ScreenshotOk`, real bytes, real PNG) — so by every criterion the
governance regime has, they are *real evidence*, which is exactly what it is built to bless.

- **`EvidenceAudit` diff-scan** (`.specify/extensions/evidence/audit-patterns.yml`) searches for
  *synthetic markers*: `TODO`/`FIXME`, `NotImplementedException`/`failwith "TODO"`,
  `mock`/`stub`/`fake` identifiers, skipped tests, commented-out assertions, `SYNTHETIC` banners.
  None are present — the harness calls the real render path. **Synthetic ≠ low-fidelity.** The
  regime catches *fabricated* evidence; this is *real-but-inadequate* evidence.
- **`[S]` / `[S*]` propagation** only fires on self-declared synthetic tasks. Tasks were `[X]`
  because the render path is real and the vertical slice was exercised (PNG committed, strict site
  built). The vertical-slice rule asks "is it reachable and exercised," not "is the rendered
  content visually adequate."
- **`ControlsCatalogDocsCheck` + the `TrivialPreview` byte floor** is SkiaSharp-free *by design*
  (so it runs in GPU-free CI). It cannot decode pixels; it checks PNG signature/IHDR/byte-size.
  A one-word label is ~600–900 bytes, over the 420 floor → "has content." The floor is a
  **non-emptiness proxy mistaken (by its author) for a fidelity check**, and the gap between that
  proxy and SC-001's intent is precisely the hole the defect fell through.
- **The harness "explicitness" test** asserts `bytes > floor` and `ScreenshotOk` — non-emptiness,
  not visual content.
- **The strict `fsdocs` build** validates links/markup, not image content.

The only check capable of catching this was a human decoding the images and **honestly reporting
what they show.** The evidence-mode discipline names this explicitly — "accepted visual proof
names a decodable image, dimensions, **non-trivial content**" — and delegates that classification
to the author. The regime trusts the author's visual description and does not re-derive visual
truth from pixels.

That trust was violated here: per-control content was claimed from byte sizes after viewing only a
three-control sample. The dishonesty is not synthetic *evidence*; it is a **false prose claim
about real evidence**, and no gate fact-checks prose against pixels.

---

## 6. Contributing process failures

1. **Spike sampled only favorable controls.** The render spike viewed `button`/`text-block`/
   `slider` — Tier 1/2 — and generalized "the approach works" to all 52.
2. **A self-authored proxy gate was treated as the honesty guarantee.** The byte floor was
   introduced specifically to enforce "not blank," then leaned on as if it proved "demonstrative."
3. **Evidence authored from data, not observation.** `real-image-evidence.md` asserted specific
   per-control visual content that was never viewed.
4. **Deploy verification re-sampled a Tier-1 control** (`check-box`), reinforcing the false
   confidence rather than probing the suspect families.

None of these are caught by tooling; all are discipline failures. The corrective is process, not
just code (see §8).

---

## 7. Impact

- **Live and public.** The defect is on `main` and served at
  `https://ehotwagner.github.io/FS-Skia-UI/` — Tier 3 control pages show a one-word label where a
  reader expects a depiction.
- **Overclaimed evidence is committed and deployed.** The 079 readiness files assert per-control
  content that the renders do not show.
- **Marginal net value.** 079 genuinely improved Tier 1 and partly Tier 2, but for ~13 Tier-3
  controls the "demonstrative" preview is a relabeled box — only marginally better than the blank
  it replaced.
- **No functional/runtime regression.** This is a documentation-asset fidelity and evidence-honesty
  problem; product code behavior is unchanged.

---

## 8. Remediation options

### Option A — Correct and reframe 079 honestly (low effort, immediate)
Withdraw the overclaiming prose; relabel the previews as low-fidelity **schematic** renders;
reclassify the Tier-3 controls (charts, graph-view, attribute-backed collections, separator,
spinner, image, icon) as honest `Unsupported` (no image + `preview-status: unsupported`) or as an
explicitly-disclosed `DeterministicRenderOnly` schematic. Removes the dishonesty immediately;
leaves the underlying capability gap for later.

### Option B — Build a faithful headless control renderer (the real fix; new feature, e.g. 080)
Render real Scene primitives from each control's data/state within its layout bounds — primitives
the painter already rasterizes correctly (proven by the feature-063 renderer tests): polylines for
line charts, filled rects for bars, points/circles for scatter, arcs for pie, node+edge geometry
for graph; tick/box for checkbox, track+thumb for slider, circles for radio, a toggle for switch,
a filled track for progress, a swatch grid for color-picker, a tab strip for tabs, item rows for
collections, a framed placeholder for image, a font-supported glyph for icon. This requires:
- `src/Controls/Control.fs` — real per-control geometry + realistic preview heights, and fixing
  `chartValues` to read the structured `ChartSeries`/`ChartPoint` shapes.
- `src/SkiaViewer/SceneRenderer.fs` — a layout-aware chart painter (or retire the opaque `Chart`
  node in favor of explicit primitives), possibly a bounds-bearing scene node (a **public Scene
  surface change** → escalates).
- Reworked rendering tests + screenshot baselines (the change alters `Scene.describe` output).
This is a substantial rendering feature and must go through the speckit flow with failing-first
tests, not a patch onto merged 079.

### Option C — Revert the preview swap; keep only the durable parts
Revert `docs/img/controls/**` to (or beyond) the 078 state and keep the genuinely sound pieces of
079 — the `TrivialPreview`/byte-floor gate, the reconciliation reporting, the committed render
harness, and the Examples → Controls nav reposition — until a faithful renderer (Option B) exists.

### Strengthen the gate regardless of option (so this cannot silently recur)
A byte floor cannot judge fidelity, but a **decoded-content** check can. Any artifact claiming to
be "demonstrative" should be validated by a SkiaSharp-bearing step (in the render-capable harness,
not the GPU-free gate) that asserts a real per-control content signature — e.g. minimum count of
distinct non-background pixels, lit-pixel coverage outside the title band, or required primitive
kinds in `Scene.describe` (a `Line`/`Path` for a line chart, etc.). This moves the honesty check
from "non-empty bytes + author's word" to "decoded pixels prove the depicted content," closing the
proxy-vs-intent gap identified in §5.

---

## 9. Recommendation

1. **Immediately:** apply Option A — stop the deployed site and committed evidence from
   overclaiming. This is an honesty fix and should not wait.
2. **Then:** scope Option B as a new speckit feature (faithful headless control rendering),
   including the decoded-content gate from §8 as a first-class, failing-first requirement so
   "demonstrative" is enforced by pixels, not bytes.
3. **Process:** when authoring visual evidence, view **every** artifact (or a per-tier sample that
   includes the hardest cases), never extrapolate per-item visual claims from aggregate metrics,
   and treat self-authored proxy gates as proxies — never as the honesty guarantee.

---

## 10. Appendix

### A. Key code references
- `src/Controls/Control.fs:159` — `chartValues` (reads flat `float list`; structured series unmatched → `[]`).
- `src/Controls/Control.fs:194` — `renderNode` (uniform `rectangle + clipped textRun`; label = `Content` or `Kind`).
- `src/Controls/Control.fs:233` — `renderScene` (expands child `Control`s; attribute items not expanded).
- `src/SkiaViewer/SceneRenderer.fs:394` — `Chart` painter (fixed `chartTop=180`; early-out on empty values).
- `build/Governance/CatalogDocsGen.fs` — `trivialPreviewFloorBytes = 420L` and `TrivialPreview` (byte-floor proxy).
- `build/Governance/Engine/Update.fs` — `validatePng` (signature + IHDR + length > 256; SkiaSharp-free).
- `.specify/extensions/evidence/audit-patterns.yml` — diff-scan patterns (synthetic markers only).

### B. Observed renders (verbatim)
`line-chart`→"LINE-CHART", `bar-chart`→"BAR-CHART", `pie-chart`→"PIE-CHART",
`scatter-plot`→"SCATTER-PLOT", `graph-view`→"GRAPH-VIEW", `list-box`→"LIST-BOX",
`image`→"LOGO.PNG", `icon`→"? HOME", `switch`→"SWITCH" (accent bar), `slider`→"0.5",
`progress-bar`→"0.6", `radio-group`→"MEDIUM", `tabs`→"PROFILE", `button`→"SAVE",
`check-box`→"ENABLE NOTIFICATIONS" (accent bar), `stack`→"STACK/ONE/TWO/THREE",
`data-grid`→"DATA-GRID/.../NAME/QTY/.../WIDGET".

### C. Committed preview bytes (320×160, all > 420-byte floor)
Smallest: `icon-button` 486, `toggle-button` 527, `numeric-input` 545, `badge`/`progress-bar` 569,
`slider` 574, `button` 584. Tier-3 examples nonetheless over floor: `list-box` 657, `image` 667,
`pie-chart` 667, `spinner` 670, `bar-chart` 674, `list-view` 678, `separator` 679,
`scatter-plot` 713, `line-chart` 723, `graph-view` 745. Largest: `data-grid` 2230, `date-picker`
2051, `split-button` 1601. `custom-control` — no image (honestly unsupported). The byte floor
cleanly separates these from the ~363-byte empty canvas but says nothing about what they depict.
