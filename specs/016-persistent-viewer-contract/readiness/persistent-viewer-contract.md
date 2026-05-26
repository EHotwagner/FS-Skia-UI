# Persistent Viewer Contract Readiness

## T002 Scope Record

Risk level: broad Tier 1.

Package/API scope:

- `FS.Skia.UI.SkiaViewer` gains a public persistent viewer contract, generated app host contract, runtime capability diagnostics, and launch outcome records.
- `src/SkiaViewer/SkiaViewer.fsi` and `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt` are authoritative public-surface gates.
- No new runtime package dependency is planned; dependency governance still runs because generated consumers and package behavior change.

Template/generated product scope:

- Generated viewer-backed graphical apps must use a persistent host by default.
- Bounded smoke, first-frame, frame-count, and scene metadata paths remain explicit evidence helpers only.
- Default paths that only print metadata, count controls, run bounded smoke, or exit without a persistent launch attempt are readiness failures.

Governance scope:

- `GeneratedGuidanceCheck`, `GeneratedProductCheck`, `TemplateCheck`, `EvidenceGraph`, and `EvidenceAudit` must distinguish persistent graphical launch evidence from bounded and unsupported-host diagnostics.
- `Dev`, `Verify`, and `Ci` may aggregate these checks, but aggregate results are non-authoritative when focused gates disagree.

Unsupported scope:

- This feature does not add new platform support, mobile/browser support, macOS support, release distribution automation, game mechanics, or visual redesign.
- Unsupported hosts must report diagnostics, but completion still requires one supported-host persistent launch artifact.

## T005 Public Surface Draft

Drafted `src/SkiaViewer/SkiaViewer.fsi` with:

- `ViewerRuntimeCapability`
- `ViewerLaunchOutcome`
- `GeneratedAppHost<'model,'msg>` ahead of `Viewer.runApp`
- `Viewer.runtimeCapability`
- `Viewer.run`
- `Viewer.runApp`

Verification:

- `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` passed after the `.fsi` and implementation shell were aligned.

## T006 Semantic And FSI Surface Tests

Added SkiaViewer semantic coverage for:

- `Viewer.run` launch outcome fields or unsupported-host diagnostics.
- `Viewer.runtimeCapability` persistent window, bounded smoke, keyboard, renderer mode, unsupported reasons, and missing package capability shape.
- Persistent API validation remaining separate from bounded helper validation.

Verification:

- `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` passed with 17 tests.

## T007 MVU Host Boundary Tests

Added generated app host coverage for:

- `GeneratedAppHost.dispatchKey` routing through public key normalization and host `Update`.
- Emitted `ViewerEffect` assertions for render refresh.
- Pure `Tick` mapping.
- `Viewer.runApp` persistent outcome or unsupported-host diagnostic through the public host boundary.

Verification:

- `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` passed with 17 tests.

## T019/T020 Persistent Window Lifecycle Update

Replaced the deterministic persistent launch success constructor in
`src/SkiaViewer/SkiaViewer.fs` with a Silk.NET `WindowOptions.DefaultVulkan`
window lifecycle helper.

Current behavior:

- `Viewer.run` validates options, checks runtime capability, opens a real
  Silk.NET persistent window on supported hosts, hands off the scene through
  the viewer update path during the first render callback, and closes
  intentionally after first frame presentation.
- `Viewer.runApp` now routes through the same persistent window lifecycle
  after `host.Init()` and `host.View model`.
- `Viewer.runApp` attaches Silk.NET keyboard input when available, maps key
  down/up events through `host.MapKey`, applies pure `host.Update`
  transitions, refreshes `host.View`, interprets `RenderScene` and
  `CloseWindow` effects at the viewer edge, and dispatches `host.Tick`
  from the window update callback.
- Unsupported/headless hosts still return `UnsupportedEnvironment` diagnostics
  before claiming persistent readiness.

Remaining evidence gap:

- This repository run did not capture a supported desktop window artifact.
  T024 remains required for `status=ok`, `mode=persistent-window`,
  `window-opened=true`, declared input dispatch, and intentional exit evidence.

Verification:

- `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --no-restore`
  passed with 17 tests on 2026-05-26.
- `./fake.sh build -t PackLocal` passed on 2026-05-26.
- `./fake.sh build -t GeneratedProductCheck` passed on 2026-05-26 after
  repacking the local feed; generated consumer validation is currently
  categorized as `UnsupportedHost`, with semantic tests, bounded smoke, and
  headless scene evidence passing.

## T012 Documentation Stubs

Added initial documentation anchors in:

- `docs/build.md`
- `docs/evidence.md`
- `docs/generated-apps.md`
- `docs/v3Design.md`

Verification:

- `rg -n "persistent-window|Viewer.runApp viewerOptions generatedHost|supported-host persistent|bounded smoke" docs/build.md docs/evidence.md docs/generated-apps.md docs/v3Design.md template/fragments/skiaviewer/README.md` found the new persistent and bounded-evidence separation text.
