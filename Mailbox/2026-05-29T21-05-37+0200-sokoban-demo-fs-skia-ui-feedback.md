# Sokoban Demo FS.Skia.UI Feedback

Date: 2026-05-29T21:05:37+0200
Source app: `/home/developer/projects/SokobanDemo1`
Feature: `001-sokoban-demo` (Sokoban puzzle demo — grid rendering, deterministic
input, push mechanics, undo, level loading, win detection, restart, evidence mode)
Validation context: full Spec Kit workflow (`/speckit-tasks` → `/speckit-analyze`
→ `/speckit-implement`), `dotnet build`/`dotnet test` (48 Expecto tests),
`./fake.sh build -t Dev`/`Verify`, the authoritative feature evidence audit
(`verdict=PASS`), plus a live persistent-window launch on display `:1`.
Package pins: Scene 0.1.32-preview.1, SkiaViewer 0.1.33-preview.1, Elmish /
KeyboardInput / Layout / Controls / Controls.Elmish 0.1.31-preview.1; .NET 10.

## Summary

The Sokoban demo was implemented successfully against the current FS.Skia.UI
packages. The pure MVU core, scene rendering, layout-evidence proof,
deterministic JSON evidence run, and screenshot capture all work; the app
launches through the persistent interactive viewer path and renders a correct,
legible board; all 48 tests and the full governed `Verify` + evidence audit pass.

The integration surfaced **one clear framework rendering issue** and a set of
**documentation / generated-app guidance gaps** that cost real time. Notably,
after investigation, the most painful friction (producing supported-host
persistent-launch evidence) turned out **not** to be a missing framework
primitive — the viewer already exposes `CloseWindow` and `DispatchInput`
effects — but a gap in *guidance* and in the generated host wiring. That
distinction is the main actionable takeaway: the primitives exist; the
documented pattern for using them in CI/evidence does not.

This report is deliberately explicit about what is framework-attributable vs.
what was a consumer/tooling/author issue, so the framework backlog stays honest.

## Framework-attributable issues

### 1. Default `Text` rasterizes as solid blocks in the offscreen capture path

The most concrete framework issue. The HUD text rendered as solid filled
rectangles (not glyphs) in the screenshot captured via
`Viewer.captureScreenshotEvidence`.

I first assumed "headless host has no fonts." That is **false** on this host:

```text
$ fc-list | wc -l        # 48
/usr/share/fonts/Adwaita/AdwaitaSans-Regular.ttf: Adwaita Sans:style=Regular
/usr/share/fonts/OTF/AtkynsonMonoNerdFont-Bold.otf: AtkynsonMono Nerd Font ...
```

Real Latin text fonts are installed, yet the default `Text(point, string, color)`
node did not resolve any of them in the offscreen/screenshot render pipeline
(the board tiles — rectangles, circles, ellipses — rendered perfectly; only
text was affected). `FS.Skia.UI.Scene` exposes a `FontSpec` / `Font` on the
`Text` node, so a consumer *can* set one, but the **default** path produced
tofu/boxes.

Impact: any generated app that relies on the default `Text` constructor for its
HUD will produce screenshots whose HUD is illegible, even though
`--layout-evidence` reports `ReadableLayout` (layout proves *bounds*, not glyph
rasterization — so the gap passes the layout gate silently).

Suggested improvements:
- Wire fontconfig / system-font matching into the offscreen render path so the
  default `Text` uses an installed family.
- And/or embed a permissively-licensed fallback font (Noto/DejaVu subset) so
  default text is never tofu on a minimal host.
- Add a Scene/SkiaViewer capability test that asserts default `Text` produces
  non-blank glyph coverage in the screenshot path (the current layout-evidence
  proof would not catch this).

Caveat: confirmed only in the screenshot capture path; a fully interactive
desktop session may resolve fonts differently. Even so, deterministic default
text is something the framework should guarantee, since evidence/screenshot
capture is a first-class FS.Skia.UI workflow.

### 2. No documented `interactive-window` self-close path for CI evidence (guidance gap, primitives exist)

The Spec Kit evidence audit's persistent-launch gate requires a record with
`status=ok mode=interactive-window window-opened=true exit-path=true ...`. The
default `Viewer.runApp` is correctly persistent (FR-024: stays open until the
user closes it), so in a non-interactive CI/agent harness it never returns and
never prints that status line. The only self-terminating path
(`Viewer.runBounded` / screenshot) reports `mode=persistent-evidence`, which the
audit explicitly rejects as "bounded-only substitution."

This produced a real dead end during implementation. **However**, inspecting the
`ViewerEffect` surface shows the primitives already exist:

```text
ViewerEffect+CloseWindow
ViewerEffect+DispatchInput
ViewerEffect+OpenWindow / RenderScene / CaptureScreenshot / ApplyWindowOptions ...
```

So FS.Skia.UI is **not** missing the capability. What is missing:
- A documented, generated-app-ready pattern for "run the real interactive-window
  host, then `DispatchInput` a close key (or emit `CloseWindow`) after first
  frame, and capture the `interactive-window` exit record" — i.e. a CI-friendly
  way to *prove* the persistent path without a human.
- Optionally, a `Viewer.runApp` overload taking an optional frame/time budget or
  `CancellationToken` that performs a genuine interactive-window run and returns
  the same `interactive-window` outcome on auto-close (distinct from the
  `persistent-evidence` bounded path the audit rejects).

Generated-app impact: the default generated host (`interpretAtHostBoundary`)
only ever emits `RenderScene`. It never translates the model's `CloseRequested`
into a `CloseWindow` effect, so the generated app records a close request that
nothing acts on — the window can't be closed from the reducer even though the
effect exists. The template's generated host should wire `CloseWindow` on a
close-confirmed model state by default; that both satisfies FR-024-style
"close on confirm" and yields a clean interactive exit for evidence.

## Documentation / skill gaps (highest time cost)

These are where better skills/docs would most have helped. Ordered by time lost.

### 3. No consumer-facing API map; had to reflect over the DLLs

The capability skills (`fs-skia-elmish`, `fs-skia-scene`, `fs-skia-skiaviewer`)
give accurate *philosophy* ("keep `Model`/`Msg`/`update` pure; I/O at the edge")
but point at framework-repo `.fsi` files (`src/Elmish/Elmish.fsi`) that do not
exist in a consumer project. To get the concrete shapes I needed, I reflected
over the compiled assemblies:

- `ViewerKey` cases (`Letter of char` + nullary `ArrowUp`/`Backspace`/`Escape`…)
- the host record shape `{ Init; Update; View; MapKey; Tick; Diagnostics }`
- the `ViewerEffect` case set
- `AdapterCommand<'msg>` being a list

Suggested improvement: ship a consumer-facing API cheatsheet (in the package
XML docs, or as a generated-app skill) covering the `ViewerKey` DU, the host
record, the `ViewerEffect` set, and the `Scene` node constructors. This would
remove an entire discovery phase for every generated-app author.

### 4. The readiness/evidence contract is only discoverable by failing the audit

The exact required readiness files and their **mandatory terms** were learned by
failing `run-audit.sh` and reading the script:

- `governance-risk-levels.md` must contain `small`, `medium`, `broad`,
  `required evidence`, `broad validation`.
- `aggregate-hang-diagnostics.md` must contain `verdict`, `stage`,
  `elapsed duration`, `last observed command`, `focused rerun`,
  `non-authoritative aggregate`.
- `runtime-limitations.md` must contain `.NET 10 desktop`, `Vulkan`,
  `SkiaSharp preview`, `unsupported macOS/mobile/browser`,
  `no software-renderer fallback`.
- `supported-host-persistent-launch.txt` must contain a full key=value record
  (`status`, `mode`, `command`, `window-opened`, `input-dispatch`, `exit-path`,
  `blocked-stage`, `classification`, `category`, `message`).

I also initially created these under the repo-root `readiness/` before learning
the audit reads `specs/<feature>/readiness/`. That **two-readiness-directory
distinction** (repo-root for evidence outputs vs. feature dir for audit-scanned
contract files) is undocumented and cost a full fail/fix cycle.

Suggested improvement: a `readiness-contract` reference (files + required terms +
which directory) surfaced by `speckit-tasks`, so Phase-1 setup creates them
correctly instead of after a failed gate. (This is Spec Kit tooling rather than
FS.Skia.UI proper, but it ships alongside the generated app.)

### 5. `speckit-tasks` validator gotchas (trial-and-error)

Learned by failing the graph validator during `/speckit-tasks`:

- Task **titles** are regex-scanned for trigger phrases. The phrase
  "before implementation" and the substring "skill-**load**" (e.g. in a filename
  like `skill-loading-evidence-workflow.md`) force unwanted required-skill
  matches; "EvidenceGraph"/"EvidenceAudit" in a title auto-require their skills.
- `tasks.deps.yml` must have a top-level `tasks:` key with 2-space task keys and
  4-space fields; a bare `T001:` at column 0 silently parses as "no key" and
  every task reports as missing.

Two bullets in the `speckit-tasks` skill would have saved ~3 fail/fix cycles.

### 6. No warning about the Text/font gotcha

One line in `fs-skia-scene` — "set an explicit `FontSpec`; the default `Text`
may not resolve a system font in the headless capture path" — would have
pre-empted issue #1 entirely.

## What worked well

- `Scene` primitives (`Rectangle`, `Line`, `Circle`, `FilledEllipse`, `Text`,
  `Group`) composed cleanly for the board + HUD.
- `ViewerKeyboard.normalizeEvent` and the `ViewerKey` DU made deterministic,
  auto-repeat-suppressing input mapping straightforward.
- The screenshot-evidence contract worked end-to-end on this host:
  `status=ok`, `viewer-open-status=ViewerOpenConfirmed`,
  `capture-source=live-viewer-window`, `proves-screenshot=true`, real non-blank
  640x480 RGBA PNG of the actual scene.
- The layout-evidence types (`LayoutEvidenceReport`, region/overlap model,
  `ReadableLayout`/`DeterministicRenderOnly`) mapped naturally onto a Sokoban
  HUD/board split.
- The pure-core/effect-edge boundary held with zero friction: `init`/`update`
  stayed pure; all file/window/process I/O lived at the `EvidenceCommands.fs` /
  `Program.fs` edge; `interpretAtHostBoundary` kept app commands and viewer
  effects cleanly separated.
- Deterministic evidence run produced byte-for-byte identical JSON across runs
  (verified by matching SHA-256), driven purely by the reducer.

## Honest caveats (not framework-fixable)

- A persistent GUI genuinely cannot self-close in a headless harness without
  being told to; that tension is inherent, mitigated by #2's suggestions.
- The remaining friction (an interpolated-string-with-nested-quotes compile
  error; an `open`-shadowing issue where `FS.Skia.UI.SkiaViewer` shadowed the
  app's `Move`/`update`/`Model`) were author mistakes, not framework defects —
  though #3 (a consumer API map) would have made the shadowing easier to avoid.

## Prioritized recommendations for FS.Skia.UI

1. **Default text rendering** — resolve installed system fonts (or embed a
   fallback) in the offscreen/screenshot path so default `Text` is never tofu;
   add a capability test for non-blank glyph coverage. (Issue #1)
2. **Generated host close wiring + CI launch pattern** — wire `CloseWindow` on a
   close-confirmed model state in the generated host, and document a
   `DispatchInput`/`CloseWindow` recipe (or a budgeted `runApp` overload) for
   producing `mode=interactive-window` exit evidence without a human. (Issue #2)
3. **Consumer API cheatsheet** — `ViewerKey`, host record, `ViewerEffect` set,
   `Scene` constructors — in package XML docs or a generated-app skill. (Issue #3)
4. **Readiness-contract reference** surfaced before the audit, including the
   repo-root vs. feature-dir distinction. (Issue #4)
5. **`speckit-tasks` validator notes** on title regex triggers and
   `tasks.deps.yml` indentation. (Issue #5)
