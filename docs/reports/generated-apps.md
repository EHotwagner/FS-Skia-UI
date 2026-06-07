---
title: Generated Apps
---

# Generated Apps

> **Canonical effects boundary:** the single source of truth for the two effect
> categories (application commands at the MVU edge vs viewer effects at the host
> boundary) and the `update`→host wiring is the page bundled into every generated
> project at `docs/effects-boundary.md` (authored from
> `template/base/docs/effects-boundary.md`). The effects notes below are aligned
> to it; from a generated project follow the bundled page without reading this
> framework report or framework source.

Generated graphical apps must validate user-reachable input and rendering
paths through public package surfaces. For feature `013-tetris-demo-integration`
the generated app guidance must name:

- Scene-returning function: `Product.Program.view`, with generated signatures
  describing it as returning `FS.Skia.UI.Scene.Scene`.
- Generated host value: `Product.Program.generatedHost`.
- App reducer: `Product.Program.update`.
- App-command boundary: pure reducers return app commands such as
  `DispatchHostCommand`; viewer effects such as `RenderScene` are produced by
  the host boundary and must not be appended to app command lists.
- Interactive run command: `dotnet run --project src/Product/Product.fsproj`.
- Persistent host default: `Viewer.runApp viewerOptions Product.Program.generatedHost`.
- Viewer-key driven start, options, primary interaction, pause/back, and
  restart/exit flows where those screens exist.
- Bounded real-viewer smoke command:
  `dotnet run --project src/Product/Product.fsproj -- --bounded-smoke <path>`
  and its unsupported-host behavior.
- Startup-focused diagnostics remain the default bounded smoke mode; generated
  apps may expose a separate frame-diagnostics smoke command that enables the
  frame category with an explicit sample limit.
- Deterministic scene-level visual evidence command:
  `dotnet run --project src/Product/Product.fsproj -- --scene-evidence <path>`.
- Screenshot evidence command:
  `dotnet run --project src/Product/Product.fsproj -- --screenshot-evidence <path>`.
  This is screenshot proof only when the report contains `status=ok`,
  `evidence-kind=screenshot`, dimensions, a screenshot artifact path, and
  live viewer-window capture after first-frame presentation.
  Unsupported hosts must report `status=unsupported`,
  `unsupported-host-reason`, and `fallback=deterministic-scene-evidence`
  without claiming screenshot proof. Deterministic scene output
  `deterministic-scene-evidence` must not claim screenshot proof.
- Layout readability evidence command:
  `dotnet run --project src/Product/Product.fsproj -- --layout-evidence <path> 1280 720`.
  Generated game products must also validate the documented constrained size
  640x480 and report named HUD/gameplay regions, HUD text bounds, active
  gameplay bounds, overlap status, proof level, and diagnostics.
- Local package restore setup, including feed path, package identities,
  versions, consumer package configuration, and restore command.

## Compact Consumer API Map

Generated demo authors should be able to find the public API shape before
coding:

- Package API reference: inspect the source-shaped package API reference
  generated from curated `.fsi` files. Do not use assembly reflection or
  repository source inspection as an authoring substitute.
- Keyboard keys: use `FS.Skia.UI.KeyboardInput.ViewerKey` cases
  `ArrowLeft`, `ArrowRight`, `ArrowUp`, `ArrowDown`, `Enter`, `Space`,
  `Escape`, `Backspace`, `Letter`, `Digit`, `Function`, and `Unknown`; normalize
  raw viewer events with `ViewerKeyboard.normalize`,
  `ViewerKeyboard.normalizeEvent`, and `ViewerKeyboard.toKeyId`.
- Host callbacks: a generated `Viewer.GeneratedAppHost` owns `Init`,
  `Update`, `View`, `OnTick`, `OnKey`, and `ShouldClose` while product reducers
  stay pure.
- Viewer effects: host boundaries emit `OpenWindow`, `ApplyWindowOptions`,
  `RenderScene`, `DispatchInput`, `CloseWindow`, `EmitDiagnostic`,
  `StartBoundedRun`, `CaptureScreenshot`, `CaptureImageEvidence`, `ReadPixels`,
  and write evidence effects.
- Adapter commands: generated app commands remain product-owned values such as
  `DispatchHostCommand`; Elmish adapter commands such as
  `DispatchViewer` bridge to viewer effects at the edge.
- Scene nodes: common scenes use `Scene.empty`, `Scene.group`,
  `Scene.rectangle`, `Scene.circle`, `Scene.text`, `Scene.textRun`,
  `Scene.line`, `Scene.path`, and shared `Scene.Point`, `Scene.Rect`, and
  `Scene.Color` records.
- Mixed Scene and Controls authoring: qualify collision-prone names such as
  `FS.Skia.UI.Scene.Rect`, `FS.Skia.UI.Scene.Paint`,
  `FS.Skia.UI.Scene.TextRun`, `FS.Skia.UI.Controls.TextBlock.create`,
  `FS.Skia.UI.Controls.TextBox.onChanged`, and
  `FS.Skia.UI.Controls.Stack.children`. Do not rely on namespace open order.
- Shared structurally-typed types (FR-008): when you need a bounds/geometry
  value near scene code, **reuse the shared `FS.Skia.UI.Scene.Rect` type**
  rather than defining a look-alike record with the same `X/Y/Width/Height`
  fields. A local same-shape record makes F# record-field inference ambiguous
  and can hijack which type a literal resolves to. Annotate the literal with the
  shared type (`let bounds : FS.Skia.UI.Scene.Rect = { X = 0.0; Y = 0.0; Width =
  240.0; Height = 80.0 }`) so resolution stays predictable. This pattern
  generalizes to any structurally-shared type, not just bounds.
- `ControlEventOrigin` carries `[<RequireQualifiedAccess>]` (spec 037): reference
  its cases qualified — `ControlEventOrigin.Text`, `ControlEventOrigin.Pointer`,
  … — so the `Text` case never shadows the unqualified scene `Text` constructor
  when both namespaces are opened.

Default text is intended to be readable in evidence screenshots on supported
Linux desktop hosts with common Latin fonts. Specify explicit fonts with
`TextRun.Font` when brand, typography, or exact font selection matters beyond
default readability. Use explicit fonts for brand or typography guarantees.

## Readiness Contract

For feature-scoped audits, the authoritative readiness directory is the active
feature path such as `specs/032-sokoban-feedback-followups/readiness/`, not a
repository-level output directory. Repository evidence directories like
`readiness/surface-baselines/` and generated consumer logs remain supporting
artifacts. Current generated-app follow-up readiness should prepare:

- `default-text-glyph-capture.md` with glyph coverage metrics, screenshot
  artifact path, font resolution, fallback-used, runtime limitations, and
  supported-host or unsupported-host classification.
- `interactive-window-close-evidence.md` with `mode=interactive-window`,
  first-frame, window-opened, close request source, input dispatch, clean exit
  path, elapsed time, aggregate hang diagnostics, and supported-host persistent
  launch evidence. This is the supported-host persistent launch evidence gate.
- `consumer-guidance-scan.md`, `readiness-contract-scan.md`, and
  `task-guidance-scan.md` with governance risk levels, aggregate hang
  diagnostics, runtime limitations, and scan results for required terms.

Follow-up classifications should distinguish framework behavior, generated-app
guidance, Spec Kit guidance, and consumer-author mistake so backlog ownership
stays clear.

Generated app tests should drive keyboard flows through normalized viewer key
events, not backend-specific raw string comparisons. Validation failures must
identify the app flow, input value, screen, rendering stage, diagnostic
category, package identity, or evidence path needed to act on the failure.

Bounded smoke, frame diagnostics, and scene evidence are explicit CI and
reviewer-diagnostic commands. They are not interactive readiness substitutes
for the default persistent graphical launch path, and they should stay behind
explicit command dispatch outside the default product launch branch.
The default command for a viewer-backed graphical profile must attempt
`Viewer.runApp viewerOptions Product.Program.generatedHost`; commands that
only print metadata, count controls, run bounded smoke, emit scene evidence, or
exit without a persistent launch attempt are diagnostic helpers only. Tests that
exercise the reducer should call `Product.Program.update`, not an unqualified
or framework-owned update helper.

For Linux desktop review sessions where the generated viewer should outlive the
shell, launch it in a detached session while preserving diagnostics:

```bash
setsid dotnet run --project src/Product/Product.fsproj > readiness/logs/generated-viewer.log 2>&1 < /dev/null &
```

Keep the `readiness/logs/generated-viewer.log` path with the review notes so
stderr, stdout, and startup diagnostics remain inspectable after the shell
exits.

Generated evidence reports must keep deterministic render proof, persistent
launch proof, and screenshot proof as separate evidence kinds. A deterministic
scene hash or pixel fallback can support diagnosis, but it must not be relabeled
as persistent-window or screenshot evidence.
Generated layout, image, screenshot, and pixel-readback evidence commands should
share stable report conventions without forcing the default app profile to
reference the Testing package: `status`, `command`, `output`, stable key
ordering, normalized `ok`/`unsupported`/`failed` status vocabulary, skipped
gates, next command, and unsupported-host `unsupported-host-reason` plus
`fallback=deterministic-scene-evidence`. Evidence reports may only claim the
authority of the gates they actually complete.
Generated app message examples must qualify app-owned messages such as
`Product.Program.Msg.CloseRequested`; `CloseRequested` is an app-owned message.
When generated code stores a domain vector, use an explicit conversion helper
such as `toScenePoint` before passing the value as a `Scene.Point`.
Generated evidence can record semantic scene facts for lander, terrain, landing
pad, and HUD metrics, but deterministic-scene-evidence does not prove semantic
object presence in a live screenshot. Pixel-readback fallback evidence must
include `fallback-reason` and `proves-screenshot=false` unless live
viewer-window screenshot capture succeeded.
Generated gameplay examples should reuse shared Scene geometry for layout,
containment, collision, and rendering evidence when the Scene shape model
already fits; do not introduce local duplicate bounds records for the same
entities.
When an app needs domain-owned geometry aliases or records, use names that
describe the product space, such as `WorldRect`, `WorldPoint`, `TrackBounds`,
`CarPose`, or `CheckpointBounds`. Reserve generic `Rect`, `Point`, and `Size`
for shared Scene/layout primitives so generated code does not need
ambiguity-driven type annotations.

Generated smoke reports should include a diagnostic mode and captured
diagnostic categories so startup failures can be read without repeated
frame-loop noise. Frame-loop messages should appear only in an explicit
frame-focused run.

Generated consumers must use local NuGet packages produced by `PackLocal` for
package validation. They must not copy repository implementation source to
stand in for package consumption.

Bounded-only graphical apps need an explicit migration decision before they can
claim readiness: adopt the persistent generated host, declare headless or
non-interactive scope, or document a missing persistent viewer capability as a
blocking product/package gap.

Repository validation for generated app flows is produced by `TemplateCheck`,
`GeneratedGuidanceCheck`, `TemplateDrift`, `PackLocal`, and
`GeneratedProductCheck`. The active feature stores the generated consumer
summary at `readiness/generated-product-validation.md` and detailed logs under
`readiness/generated-consumer-validation/`.
Generated app and template command names are part of the native FAKE target
registry. `TargetMetadataDrift` must stay green after generated workflow
changes so generated guidance, docs, target metadata, and
`validation.contract.yml` continue to name runnable commands.

## Archive And API Reference Guidance

For current governance work, current feature readiness paths are authoritative for current gates. historical feature readiness is audit context only unless a
current evidence map explicitly marks it as supporting evidence. Archived material must not be cited as current package, template, generated-product, or audit pass/fail evidence.

The source-shaped `.fsi` package API reference remains authoritative for agent
authoring. FSharp.Formatting/fsdocs output is secondary or hybrid unless the
active generator decision record marks it authoritative. Package consumers must not use assembly reflection or repository source inspection as an authoring substitute.
