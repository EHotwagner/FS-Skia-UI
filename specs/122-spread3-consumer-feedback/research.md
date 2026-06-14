# Research: Spread3 Consumer Feedback Remediation (Feature 122)

This phase executed the **dogfood-verify discipline** the spec mandates: every framework
claim in the Spread3 feedback was re-checked against the current tree (features 118–121).
The result substantially reshaped scope — two claims were already-shipped/over-broad, and
the headline blink defect has a different, concrete root cause than the consumer inferred.

## Verdict table

| Claim (from feedback) | Verdict | Disposition |
|---|---|---|
| Live window blinks black frames on Wayland `DirectToSwapchain` windowed-fullscreen | **CONFIRMED — root cause found** | BUILD (FR-001/002) |
| `ViewerOptions` has no startup-state knob → consumer cannot pick a normal window | **PARTIALLY REFUTED** | viewer layer already has it; real gap is controls/template threading (FR-005) |
| Generated `Program.fs` threads `--window-startup` only into the report, not the launch | **CONFIRMED** | BUILD (FR-005) |
| Need present-sync / buffer-count public knobs to escape the blink | **OVER-BROAD** | DEFER (FR-004) — FR-001 removes the need |
| `CustomControl` content is not painted by `renderTree`; catalog implies it is | **CONFIRMED** | BUILD doc/honesty (FR-007) |
| `renderTree`/CustomControl throws NRE under reflection | **CONFIRMED (in validate/create, on null Id/effects)** | BUILD null-guard (FR-006) |
| Live host vs screenshot path *diverge* on CustomControl | **REFUTED** | both use the same `paintNode` → both placeholder; no divergence |
| Readiness scanners require `key=value`, inconsistently documented | **CONFIRMED** | BUILD doc (FR-008) |
| `scaffold-map.md` lacks an additive-files note | **CONFIRMED** | BUILD doc (FR-009) |
| UI-widgets skill directory/`name:` mismatch traps `skillist` ids | **CONFIRMED in generated products** | BUILD doc (FR-010) |
| `fs-skia-viewer-host` documents only blur, not the black-frame symptom | **CONFIRMED** | BUILD doc (FR-011) |
| No-new-dependency property-test pattern note | n/a | BUILD optional skill note (FR-012) |

## Decision 1 — Black-frame root cause: idle-skip skips the buffer swap entirely (FR-001/002)

**Finding (src/SkiaViewer/Host/OpenGl.fs).** When `PresentMode = DirectToSwapchain` and the
scene is reference-/size-unchanged (`shouldPresent` is false), the host takes the `canIdleSkip`
branch (OpenGl.fs:496-510): it increments `skippedPresentCount`, zeroes timings, and **returns
early without calling `renderFrameDirect`** — so **`window.SwapBuffers()` (OpenGl.fs:439) is
never called** and the back buffer is never cleared/drawn (clear is coupled to the paint at
OpenGl.fs:433). The header comment (OpenGl.fs:489-492) states the optimization explicitly
"betts" on a **double-buffered** model: "the front buffer still holds the last presented frame."

**Why it blinks on Wayland windowed-fullscreen.** Wayland compositors commonly rotate **3+
swapchain buffers**. Skipping `SwapBuffers` on idle leaves the not-yet-displayed buffers
**undrawn**; when buffer rotation brings one to the display, it shows black. Offscreen/readback
evidence is unaffected because the `OffscreenReadback` path **always** renders + presents
(OpenGl.fs:455-483, "Readback present always renders") — which is exactly the
"offscreen correct, live blinks" asymmetry the consumer observed.

**Why the consumer's heartbeat test didn't disprove skip-paint.** The consumer forced a
per-frame scene delta and *still* blinked, concluding skip-paint was ruled out. But a per-frame
delta makes `shouldPresent` true → the host paints+swaps **one** buffer per frame; on a 3-buffer
swapchain a steadily-advancing single-buffer swap can still rotate an under-filled buffer into
view depending on cadence. The defect is the **buffer-fill invariant**, not the paint itself —
which is why neither the heartbeat nor `FrameRateCap` fixed it.

**Decision.** Replace the binary skip with a **bounded re-present** that keeps every swapchain
buffer populated, via a pure decision function (testable, deterministic):

```
[<RequireQualifiedAccess>]
type PresentAction = PaintAndPresent | RepresentLastGood | SkipPresent

// present iff shouldPresent; else re-present the last good frame until all buffers are
// known-filled (idleRepresentsRemaining > 0); else fully idle.
planPresent : prev:Scene option -> next:Scene -> sizeChanged:bool -> idleRepresentsRemaining:int -> PresentAction
```

Host wiring (internal):
- Keep `lastGoodFrame: SKImage option` = `surface.Snapshot()` taken after each real paint.
- On `PaintAndPresent`: paint + swap (as today); set `idleRepresentsRemaining := bufferFillDepth - 1`.
- On `RepresentLastGood`: **blit the cached `lastGoodFrame` to the canvas + Flush + SwapBuffers**
  (a single O(1) image blit — NO scene walk, NO measure/paint), then decrement the counter.
- On `SkipPresent`: do nothing (steady-state idle — byte-identical to feature 120/121).

`bufferFillDepth` = internal constant (default **3**, covering typical triple-buffering; a 4th
re-present is cheap insurance — plan picks 3 and documents). Not public (FR-004 deferred).

**FR-002 reconciliation.** The idle CPU win (no scene-walk/draw) is preserved: `RepresentLastGood`
does only a cached-image blit, and once buffers are filled the path is `SkipPresent` = full idle,
byte-identical to 120/121. The offscreen/readback path is **untouched** → screenshot goldens stay
byte-identical.

**Alternatives considered.** (a) Always present every frame (drop idle-skip) — regresses
120/121 idle savings; rejected. (b) Re-issue `SwapBuffers` without redrawing — back buffer is
stale on triple-buffer; unreliable; rejected. (c) Expose buffer-count/present-sync knobs and let
the consumer escape — pushes a framework bug onto consumers; deferred (FR-004).

**Evidence honesty.** The Wayland windowed-fullscreen *visual* blink cannot be reproduced in this
headless/Mesa environment (no Wayland windowed-fullscreen compositor in CI). The real, citable
evidence is: (1) the pure `planPresent` golden showing the
`PaintAndPresent → RepresentLastGood×(n-1) → SkipPresent…` sequence; (2) a host-level test that a
static scene yields a populated buffer on every present (no undrawn buffer) via the present-action
log; (3) offscreen byte-identical goldens. The end-to-end Wayland observation is a disclosed
manual/`[-]` item with rationale — NOT an `[S]` synthetic pass.

## Decision 2 — Window-behavior threading for the CONTROLS host (FR-003/FR-005)

**Finding.** The viewer layer **already** models window behavior:
`ViewerWindowBehaviorRequest { ResizePolicy; MaximizePolicy; StartupState; StartupPosition;
BackendPreference }` with `StartupState` cases incl. `Normal` and `WindowedFullscreen`
(SkiaViewer.fsi:60-94), and launch entry points `Viewer.runAppWithWindowBehavior` /
`runInteractiveViewerWithWindowBehavior` (SkiaViewer.fsi:632/638). The **GAME** profile already
threads it (Program.fs:161-165 → `runAppWithWindowBehavior` when a window flag is supplied).

**The real gap.** `ControlsElmish.runInteractiveApp` takes only `options + host`
(ControlsElmish.fsi:503-504) and internally calls `Viewer.runInteractiveViewer options viewerHost`
(ControlsElmish.fs:1245) — there is **no window-behavior overload**. So the **CONTROLS** profile
(Program.fs:156) launches with the default windowed-fullscreen and the parsed
`windowBehaviorRequest` is consumed only by `manualWindowOptionResults` (Program.fs:145, the
report). `--window-startup normal` is therefore **inert** for controls apps — exactly as reported.

**Decision.**
1. Add `ControlsElmish.runInteractiveAppWithWindowBehavior : options -> behavior -> host -> Result`
   (mirrors `runInteractiveApp`, delegates to `Viewer.runInteractiveViewerWithWindowBehavior`).
   FR-003's startup-state selection is satisfied through this + the existing viewer surface.
2. Update template `Program.fs` (app profile, Program.fs:156) to mirror the game branch:
   call `runInteractiveAppWithWindowBehavior viewerOptions windowBehaviorRequest interactiveHost`
   when `windowFlagSupplied args`, else `runInteractiveApp` (byte-identical default).

**Alternatives.** Widen `runInteractiveApp` in place — breaks the public signature for every
consumer; rejected in favor of an additive overload (matches `runApp`/`runAppWithWindowBehavior`).

## Decision 3 — CustomControl honesty + NRE guard (FR-006/FR-007)

**Finding.** `renderTree`'s leaf painter (Control.fs:1342-1364) paints a rectangle + label for
any kind not in `richFamilies` (Control.fs:348-364); **`custom-control` is not in that set**, so it
renders a placeholder labelled with its `Content` (or the kind string `custom-control`). The
`CustomControlDefinition` `Render`/`Draw`/`Layout` fields (CustomControl.fsi:8-19) are **phantom —
never invoked**. `RetainedRender` uses the **same** `paintNode` (RetainedRender.fs:7), so live and
screenshot paths agree (no divergence). NRE risk is in `CustomControl.validate`/`create`
(CustomControl.fs:21/26/31) where `definition.Id.Trim()` / `effect.Trim()` / `Accessibility.defaultFor …
definition.Id` dereference a null `Id`/effect. The catalog (Catalog.fs:218) advertises
"Product-owned wrapper for custom Skia content," implying rasterization.

**Decision (doc/honesty + guard — the low-risk, surface-stable option; behavioral painting deferred).**
- **FR-006**: in `CustomControl.validate`/`create`, replace `x.Trim() = ""` with
  `String.IsNullOrWhiteSpace x` and null-guard the effect strings and the `Accessibility.defaultFor`
  Id argument, so null content never throws (returns a validation diagnostic instead). Add a test.
- **FR-007**: rewrite the Catalog.fs:218 purpose to the honest statement — `custom-control` is a
  product-owned *wrapper*; `renderTree`/preview paints a **labeled placeholder**, and geometry that
  must appear in the rasterized/screenshot path should be built from primitive controls
  (`Border`+`TextBlock`+`Stack`). Mirror the note into the `fs-skia-ui-widgets` skill. Regenerate
  `docs/controls-catalog.md` (CatalogDocsGen) since the description string changes.

**Why not paint the content.** Invoking `Render`/`Draw` in `renderTree` is a real behavioral
change to the render path with its own coordinate/clip/evidence implications and a wider blast
radius; the consumer's own resolution was to use primitive controls. The doc-fix removes the trap
at a fraction of the risk; behavioral painting is left as a future option (recorded).

## Decision 4 — Governance/doc/skill papercuts (FR-008/009/010/011/012)

- **FR-008 (CONFIRMED).** `build/Governance/Evidence/Scans.fs` parses `interactive-visible-window.md`,
  `window-state-diagnostics.md`, and `generated-validation.md` with `parseKeyValues` (kv regex,
  Scans.fs:14) → their tokens must be literal `key=value`; readiness-contract files use substring
  `Contains` (Scans.fs:105-106) → prose ok. `evidence-formats.md` shows the `=value` shape only for
  `window-state-diagnostics` (diagnostic-class=…), not for `interactive-visible-window` /
  `generated-validation`. **Fix:** update `template/base/docs/evidence-formats.md` to render every
  required token for those two files in explicit `token=value` form (status=…, mode=…,
  window-visible=…, exact-package-match=…, generated-tests-ran=…, authoritative=…, failure-class=…).
  Doc-only; no scanner change (so historical evidence keeps passing).
- **FR-009 (CONFIRMED).** `template/base/tests/Product.Tests/GovernanceTests.fs:41-68` checks the six
  files' presence + relative `IndexOf` order only. **Fix:** add a one-line note to
  `template/base/docs/scaffold-map.md` that new source files may be added as long as the six scanned
  files keep their relative compile order.
- **FR-010 (CONFIRMED in generated products).** The template SOURCE has matching
  `name: fs-skia-ui-widgets` (== directory), but generation **substitutes the project name into the
  `name:` line** (Spread3 → `name: spread3-widgets`, description "Generated spread3 guidance…")
  while leaving the directory `fs-skia-ui-widgets` and **not** substituting the `tasks-template.md`
  advisory (which hardcodes `fs-skia-ui-widgets` at line 178). That asymmetry is the trap. **Fix
  (doc-only, safe):** update the `.specify/templates/tasks-template.md` advisory so the widgets hint
  explicitly flags that its directory is `fs-skia-ui-widgets` but the resolved `name:` in a generated
  product is the project-prefixed form (e.g. `<project>-widgets`) — reinforcing the existing
  line-171 "declare `name:` not directory" rule with the exact skill that gets substituted. (Deeper
  option — stop substituting the skill `name:` in `build/Governance/GeneratedProduct.fs` so name==dir
  in generated products — is recorded but deferred as higher-risk to the generation contract.)
- **FR-011 (CONFIRMED).** `.agents/skills/fs-skia-viewer-host/SKILL.md:77-84` documents only the
  windowed-fullscreen **blur** caveat. **Fix:** add an interleaved-black-frame section (Wayland
  `DirectToSwapchain`) noting the framework now keeps all swapchain buffers populated (FR-001) and
  that `--window-startup normal` now actually applies to controls apps (FR-005); explicitly mark the
  former "size-aware view" advice as a **blur** fix, not a blink fix (and warn that filling the
  extent with a full grid is an O(cells) ANR trap). Regenerate the `.claude` mirror
  (RefreshSurfaceBaselines / SkillSyncCheck).
- **FR-012 (optional).** Add a short note to the `fs-skia-ui-widgets` (or a testing) skill on the
  no-new-dependency property-test pattern (deterministic generative loops through the real engine,
  disclosed in the test header). Cheap; folded into the FR-007 skill edit.

## Out of scope (recorded)

- **FR-004** present-sync (vsync/FIFO) / buffer-count **public** knobs — deferred; FR-001 removes the
  consumer-visible need. An internal `bufferFillDepth` constant covers the fix.
- The generalizable formula-engine / primitive-grid recipes from the feedback — candidate
  `FS.Skia.UI.SkillSupport` triage, tracked separately.
- Behavioral `CustomControl` painting in `renderTree` — recorded future option.

## Net build scope

FR-001, FR-002, FR-005 (incl. FR-003 via existing surface), FR-006, FR-007, FR-008, FR-009,
FR-010 (doc), FR-011, FR-012. Dropped/deferred: FR-004 (deferred), the CustomControl divergence
claim (refuted). One additive `.fsi` change (`runInteractiveAppWithWindowBehavior`); the present
path and CustomControl guard are internal; the rest are template/governance/skill docs.
