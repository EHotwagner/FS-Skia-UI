# Public Surface & Doc Delta: Feature 122

## A. Additive public `.fsi` (Tier 1, additive — no break)

### `src/Controls.Elmish/ControlsElmish.fsi`
```fsharp
val runInteractiveAppWithWindowBehavior:
    options: ViewerOptions ->
    behavior: ViewerWindowBehaviorRequest ->
    host: InteractiveAppHost<'model, 'msg> ->
        Result<ViewerLaunchOutcome, ViewerRunFailure>
```
- XML `///` doc required (XML-doc gate). One new top-level surface member → surface baseline moves;
  `RefreshSurfaceBaselines` regenerates per-package + top-level baselines.

### `src/SkiaViewer/Host/OpenGl.fsi` (test seam, mirrors `shouldPresent`)
```fsharp
[<RequireQualifiedAccess>]
type PresentAction =
    | PaintAndPresent
    | RepresentLastGood
    | SkipPresent

module GlHost =
    val planPresent:
        prev: FS.Skia.UI.Scene.Scene option -> next: FS.Skia.UI.Scene.Scene ->
        sizeChanged: bool -> idleRepresentsRemaining: int -> PresentAction
```
- `[<RequireQualifiedAccess>]` attr precedes `///` doc precedes `type` (XML-doc gate ordering).
- `PresentAction` declared in BOTH `OpenGl.fs` and `OpenGl.fsi`.

## B. Internal-only (no `.fsi` delta)

- `src/Controls/CustomControl.fs` — null guards in `validate`/`create` (FR-006).
- `src/SkiaViewer/Host/OpenGl.fs` — `lastGoodFrame`/`idleRepresentsRemaining`/`representedCount`/
  `bufferFillDepth` host state + the `RepresentLastGood` blit-and-swap branch (FR-001/002).

## C. Catalog / generated docs (FR-007)

### `src/Controls/Catalog.fs:218`
- **Before**: `"Product-owned wrapper for custom Skia content."`
- **After (honest)**: e.g. `"Product-owned wrapper; renderTree/preview paints a labeled placeholder
  (custom Render/Draw is not rasterized) — build must-show geometry from primitive controls
  (Border/TextBlock/Stack)."`
- Regenerate `docs/controls-catalog.md` (CatalogDocsGen); update any test asserting the old string.

## D. Template & governance docs (FR-008/009)

- `template/base/docs/evidence-formats.md` — render the required tokens for `interactive-visible-window.md`
  (`status=…  mode=…  window-visible=…  accessible-window=…  first-frame-presented=…
  self-closed-for-evidence=…`) and `generated-validation.md` (`exact-package-match=…
  generated-tests-ran=…  authoritative=…  failure-class=…`) in explicit `key=value` form, with a note
  that these files are parsed as key/value (unlike the prose-token readiness-contract files).
- `template/base/docs/scaffold-map.md` — one-line note: new source files may be added provided the six
  scanned files (`Model.fs → View.fs → LayoutEvidence.fs → WindowOptions.fs → EvidenceCommands.fs →
  Program.fs`) keep their relative compile order.

## E. Spec Kit advisory (FR-010)

- `.specify/templates/tasks-template.md` — the controls/widgets hint flags that the directory is
  `fs-skia-ui-widgets` but its resolved `name:` in a **generated** product is the project-prefixed
  form (e.g. `<project>-widgets`); use the resolved `name:` in `skillist` ids (reinforces the
  existing line-171 rule with the one skill that gets substituted at generation).

## F. Skills (FR-007/011/012) — `.agents` source, `.claude` regenerated

- `.agents/skills/fs-skia-viewer-host/SKILL.md` — add an interleaved-black-frame section (Wayland
  `DirectToSwapchain` windowed-fullscreen): framework now keeps all swapchain buffers populated
  (FR-001); `--window-startup normal` now applies to controls apps (FR-005); the prior "size-aware
  view" advice is a **blur** fix only and filling the extent with a full grid is an O(cells) ANR trap.
- `.agents/skills/fs-skia-ui-widgets/SKILL.md` (+ `template/product-skills/fs-skia-ui-widgets/SKILL.md`)
  — CustomControl placeholder note (FR-007) + optional no-new-dependency property-test pattern note
  (FR-012).
- Regenerate `.claude/**` mirrors via `./fake.sh build -t RefreshSurfaceBaselines`; `SkillSyncCheck`
  enforces currency.

## Gate impact

`Route` escalates to maintainer-verify. Moving parts: package-surface baselines (A), XML-doc gate (A),
CatalogDocsGen (C), `TemplateCheck`/`GeneratedProductCheck` (D + Program.fs), `SkillSyncCheck` (F),
`GeneratedGuidanceCheck` (plan Constitution block), `EvidenceGraph`/`EvidenceAudit` (0 synthetic).
