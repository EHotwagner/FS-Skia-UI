# Asteroids Demo FS.Skia.UI Implementation-Phase Feedback

Date: 2026-05-30T07:19:26+0200
Source app: `/home/developer/projects/AsteroidsDemo2`
Feature: `001-asteroids-demo` (2D Asteroids arcade demo — real-time MVU game loop,
continuous keyboard input, moving vector shapes, collision-heavy split/score/wave
state, deterministic evidence mode)
Validation context: `/speckit-implement` over the existing tasks (33 tasks),
`dotnet build`/`dotnet test` (42 Expecto tests), `./fake.sh build -t Dev`/`Test`/
`Verify`, the authoritative feature evidence gate (`EvidenceGraph` exit 0,
`EvidenceAudit verdict=PASS`), a deterministic `--evidence-run` (reproducible;
18000-step ≈ 5-min stability), and a **live persistent-window launch on display
`:1` (Wayland)**.
Package pins: Scene 0.1.33-preview.1, SkiaViewer 0.1.34-preview.1, Elmish /
KeyboardInput / Layout / Controls / Controls.Elmish 0.1.32-preview.1; .NET 10
(`net10.0`).

## Summary

The Asteroids demo was implemented successfully against the current FS.Skia.UI
packages. The pure MVU core (`Vec2` + seeded SplitMix64 `Rng`, ship dynamics,
firing/cooldown, bullet expiry/no-wrap, collision split + scoring, safe-center
respawn + invulnerability, wave progression, pause/restart), scene rendering,
HUD/gameplay layout-evidence proof, the opt-in deterministic `--evidence-run`
(reproducible, offline, ≥5-min stable), and the governed `Verify` + evidence
audit all pass; 42/42 Expecto tests are green.

The **persistent interactive launch works on this host** — a real, visible,
accessible Wayland window with verified input dispatch:

```text
status=ok mode=interactive-window window-opened=true window-visible=observed:true
accessible-window=true first-frame-presented=true input-dispatch="true"
renderer-mode=skia user-close-observed=true exit-path=true
diagnostic-class=environment-session-ready display-variable=WAYLAND_DISPLAY=wayland-0
```

The integration surfaced **one clear new framework rendering gap** (vector
`Line`/stroke content is invisible in the offscreen evidence renderer), a
**recurrence** of a previously-reported issue (default `Text` rasterizes as solid
blocks offscreen — see the Sokoban report), an **evidence-honesty gap** in
`--screenshot-evidence`, and several **host-contract / discoverability** frictions.
As with prior reports, framework-attributable items are separated from
consumer/tooling items so the backlog stays honest.

The two items I would actually file as framework bugs are **#1 (Line strokes not
rasterized offscreen)** and **#3 (`--screenshot-evidence` claims a proof it does
not produce)**. Together they mean a vector-style game's required evidence
screenshot (FR-025/SC-010: "shows the playfield, ship, ≥1 asteroid, HUD") cannot
actually be produced today, even though the live window renders correctly.

---

## Framework-attributable issues

### 1. (NEW) Vector `Line`/stroke nodes are not rasterized in the offscreen evidence renderer

The most concrete new finding. The Asteroids ship (triangle) and asteroids
(irregular polygons) are built from `SceneNode.Line` stroke segments — the
intended high-contrast vector look (FR-019). The only decodable image I could
produce, `--image-evidence` (`Viewer.runAppEvidence ... generatedHost` →
640×480 RGBA PNG, `image-decodable=True`), renders the HUD band correctly
(`Rectangle`, `Circle`, `FilledEllipse`, life dots) but the **entire gameplay
region below the HUD is empty** — no ship, no asteroids.

```text
--image-evidence report:
  path=...readiness/game-frame.png   image-decodable=True
  proves-scene-rendering=true        renderer-mode=skia   first-frame-presented=true
PNG: 640 x 480, 8-bit/color RGBA      (HUD shapes present; ship + asteroids absent)
```

Filled shapes (`Rectangle`/`Circle`/`FilledEllipse`) and the HUD render; only the
`Line`-stroked entities are missing. The live Skia window almost certainly draws
them (renderer-mode=skia, see #3 for why I couldn't confirm pixels), so this is
specific to the offscreen/deterministic capture path used by
`Viewer.runAppEvidence` / `SceneEvidence.render`.

Impact: a vector-style demo cannot satisfy its own screenshot contract
(FR-025/SC-010 "screenshot shows ship + ≥1 asteroid") through any headless
evidence path, because the very nodes that draw the ship and asteroids are
dropped. The layout gate does not catch it — `--layout-evidence` reports
`ReadableLayout` from *bounds*, not from rasterized pixels.

Recommendation: rasterize `Line` (and `Path`/`Points`/stroke `Paint`) nodes in the
offscreen evidence renderer, or document explicitly that stroke-only content does
not appear in deterministic/offscreen evidence so consumers add a filled-shape
fallback for evidence.

### 2. (RECURRING) Default `Text` rasterizes as solid blocks in the offscreen capture path

Same symptom previously filed for Sokoban
(`2026-05-29T21-05-37+0200-sokoban-demo-fs-skia-ui-feedback.md`, issue #1): the
HUD `Text(point, string, color)` nodes (`SCORE`/`LIVES`/`WAVE`/`STATUS`) render as
solid filled rectangles rather than glyphs in the `--image-evidence` PNG. The
captured frame shows the text bounding boxes as light blocks. This reproduces on
AsteroidsDemo2 with the same package set, confirming it is not demo-specific.

Impact: HUD legibility cannot be demonstrated via offscreen screenshot even though
`--layout-evidence` passes; combined with #1, the offscreen evidence image conveys
almost none of the actual gameplay scene.

Recommendation: resolve a default font in the offscreen `Text` path (fonts are
installed on these hosts — see the Sokoban report's `fc-list` evidence), or have
the generated template set a `FontSpec` by default.

### 3. (NEW) `--screenshot-evidence` claims a screenshot proof it does not produce

`--screenshot-evidence` (`Viewer.captureScreenshotEvidence`,
`CaptureMode=ViewerRenderTargetPng`) reports a *live-window, proven* screenshot:

```text
status=ok  capture-source=LiveViewerWindow  capture-availability=CaptureAvailable
viewer-open-status=ViewerOpenConfirmed  first-frame-status=FirstFramePresentedStatus
pixel-content-validation=PixelContentNonBlank  proves-screenshot=True
image-width=640 image-height=480 frames-rendered=1
```

…but the file written at the artifact path (`screenshot-path`/`artifact-path`) is
the **ASCII key=value report itself**, not a PNG. There is no decodable image at
that path. So the command asserts `proves-screenshot=True` with
`pixel-content-validation=PixelContentNonBlank` while producing no image a
consumer can open.

This is the most concerning evidence-honesty gap: the evidence audit has explicit
anti-"metadata-only screenshot" checks, but they key off `real-image-evidence.md`
field values, not off the screenshot command writing its report to the same path
as the artifact. A reader trusting `proves-screenshot=True` would be misled.

Recommendation: write the PNG to a distinct artifact path and the report
alongside; only set `proves-screenshot=True` / `PixelContentNonBlank` after a
decodable image of the expected dimensions exists at the artifact path.

### 4. (NEW, latent) `--image-evidence` can substitute a 1×1 fallback PNG while still reporting `proves-scene-rendering=true`

The generated `imageEvidence` Ok-path does
`if not (isPngFile evidencePath) then writeFallbackPngEvidence evidencePath` (a
disclosed synthetic that writes a 1×1 base64 PNG) and then reports
`proves-scene-rendering=true`. On this run the renderer wrote a real 640×480 PNG,
so the fallback did not fire — but the code path means a host where
`runAppEvidence` returns Ok without writing a decodable frame would still claim
scene-rendering proof with a single pixel.

Recommendation: validate minimum dimensions / non-trivial content before claiming
`proves-scene-rendering`; treat a fallback substitution as `unsupported`, not
proof. (This is partly template-owned code, but the underlying need is a framework
contract for "did `runAppEvidence` actually emit a usable frame?".)

### 5. (NEW) The host contract cannot feed window size into the pure reducer (FR-016 resize)

`GeneratedAppHost<'model,'msg>` exposes `Tick : TimeSpan -> 'msg option` and
`MapKey : ViewerKey -> bool -> 'msg option`. Neither carries the current output
size, and there is no resize hook. The data-model wants the gameplay-region
`Bounds` recomputed each frame from host size, but there is no way to deliver that
size to pure `update` through the host. I had to fix a logical 640×480 bounds and
document the limitation honestly.

Recommendation: surface output size to the reducer — e.g. a `Resize of Size`
message, an `OnResize : Size -> 'msg option` hook, or include the size in `Tick`.
Without it, every consumer that claims live-resize support is either faking it or
working around the contract.

### 6. (ergonomics) No bounded / auto-close mode on the persistent `Viewer.runApp` path

The default `dotnet run` → `Viewer.runApp` correctly stays open until the window
is closed (FR-018). It did open and close cleanly here (`user-close-observed=true`,
exit 0), so this is **not** a capability gap. But for non-interactive smoke runs
it blocks until an external close, forcing reliance on the separate bounded
helpers (`--launch-evidence`, `--bounded-smoke`, `--evidence-run`). A first-class
`--frames N` / `--duration <t>` auto-close on the main launch path would let the
real persistent path itself be smoke-tested in CI.

### 7. (discoverability) Consumer packages ship no `.fsi` / ref-assembly / XML docs

To implement against the packages I had to reflect over the compiled DLLs to
recover the public surface — `SceneNode` cases (`Line`, `Circle`, `FilledEllipse`,
`Ellipse`, `Path`, `Text`, `TextRun`, `Group`…), the `Paint` module
(`stroke`/`fill`), `GeneratedAppHost` record shape, `ViewerKey` cases, and
`AdapterEffect`/`AdapterCommand`. The framework repo maintains
`readiness/surface-baselines/*.txt`, but those are not shipped to or referenced
from the consumer. A generated-consumer author has no in-package API reference.

Recommendation: ship the surface-baseline `.txt` (or `.fsi`/XML docs) inside the
NuGet packages, or include an `API.md` per capability in the generated template.

### 8. (ergonomics) Unqualified-name collisions and overlapping record fields

- `open FS.Skia.UI.Scene` followed by `open FS.Skia.UI.Controls(.Elmish)` shadows
  `SceneNode` constructors: `Line(p0,p1,paint)` resolved to `ControlEventOrigin`
  and `TextRun`/`Circle` patterns bound to the wrong type. Required deliberate
  open-ordering (Scene last for scene code; domain last in tests) in both
  `View.fs` and `Tests.fs`, and cost a couple of build cycles to diagnose.
- `Vec2`/`Point`/`Rect`/`Size` all expose `X`/`Y`(/`Width`/`Height`), producing
  pervasive `FS3566` "multiple type matches" warnings; every helper needed type
  annotations.

Recommendation: `[<RequireQualifiedAccess>]` on the colliding Controls unions (or
distinct case names), and a short guidance note on open-ordering + the
Scene-geometry vs app-`Vec2` field overlap.

---

## Process / Spec Kit issue (partly tooling)

### 9. The evidence-audit readiness-contract is hidden and under-scaffolded

`EvidenceAudit` hard-requires specific term strings and `key=value` fields across
a fixed set of readiness files. Several were **not enumerated** by the setup task
(T001) and were discovered only by failing the audit and reading
`.specify/extensions/evidence/scripts/bash/run-audit.sh`:

- term-checked: `governance-risk-levels.md` (`required evidence`, `broad
  validation`…), `aggregate-hang-diagnostics.md` (`verdict`, `stage`, `elapsed
  duration`, `last observed command`, `focused rerun`, `non-authoritative
  aggregate`), `runtime-limitations.md` (`.NET 10 desktop`, `Vulkan`, `SkiaSharp
  preview`, `unsupported macOS/mobile/browser`, `no software-renderer fallback`).
- window-visibility files (all required, none listed by T001):
  `interactive-visible-window.md`, `close-reason-separation.md`,
  `window-state-diagnostics.md`, `window-options.md`, `real-image-evidence.md`,
  plus `generated-validation.md` key fields (`exact-package-match`,
  `generated-tests-ran`, `authoritative`, `failure-class`).

Two audit cycles were spent reverse-engineering required terms/fields from the
script. This complements the task-generation analysis
(`2026-05-29T22-16-55+0200-asteroids-demo-speckit-task-generation-analysis.md`):
the gap is at implement-time, where the readiness contract the audit enforces is
not visible from the tasks or scaffolded by templates.

Recommendation: have `/speckit-tasks` (or a `readiness init`) scaffold every
audit-required readiness file with its required field skeleton, and/or publish the
contract `run-audit.sh` enforces as a checked-in doc.

---

## Consumer / author-attributable (kept honest)

- **I initially mis-assumed a headless host.** An earlier piped+`timeout` launch
  "hung," and I deferred the persistent-launch task (T016) as unsupported. The
  background launch then proved a real visible Wayland window with verified input
  dispatch. The deferral was an author misjudgment, not a framework limitation —
  though the inability to *confirm the rendered pixels* of that window is
  framework #3.
- **The ship/asteroids-as-`Line` design is a consumer choice.** A filled-triangle
  ship + filled-polygon asteroids would sidestep #1 for evidence — but #1 remains
  a genuine framework gap for the documented vector-rendering use case (FR-019
  explicitly wants vector outlines).
- The `FS3566` warnings (#8) are non-blocking and were left in place; benign and
  classified as such in readiness.

---

## What worked well

- Pure MVU boundary held cleanly; `update` stayed pure and all I/O lived at the
  `Viewer.*` edge. Real pure-`update` Expecto coverage was straightforward.
- The deterministic `--evidence-run` (pure seeded `Rng` + fixed 1/60 s timestep)
  is **bit-reproducible** across runs and stable over 18000 steps (≈5 min),
  offline (`network-access=none`, `downloaded-assets=none`).
- `--layout-evidence` HUD/gameplay separation + overlap-failure classification
  worked well at both default and constrained sizes.
- The governed gates are genuinely strict and caught my incomplete readiness docs
  before they could be over-claimed — the regime works.
- The persistent viewer path itself launched a correct, accessible, input-
  dispatching window on a real session.

## Prioritized recommendations

1. **(bug) Rasterize `Line`/`Path`/stroke nodes in the offscreen evidence
   renderer** (#1) — without it, vector games cannot produce a valid evidence
   screenshot.
2. **(bug) Fix `--screenshot-evidence` honesty** (#3) — write a real PNG to the
   artifact path; gate `proves-screenshot` on a decodable image.
3. **(bug, recurring) Resolve a default font for offscreen `Text`** (#2).
4. **(contract) Add a resize/size hook to `GeneratedAppHost`** (#5) so FR-016 is
   honestly satisfiable.
5. **(guardrail) Reject 1×1/empty fallback frames as proof** (#4).
6. **(DX) Ship API surface docs in consumer packages** (#7) and reduce
   unqualified-name collisions (#8).
7. **(tooling) Scaffold/publish the evidence-audit readiness contract** (#9).
