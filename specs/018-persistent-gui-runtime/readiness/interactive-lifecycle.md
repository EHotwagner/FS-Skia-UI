# Interactive Lifecycle Evidence

Task: T023

## Outcome Contract

Expected default interactive launch fields:

- `mode=interactive-window`
- `self-closed-for-evidence=false`
- `first-frame-presented=true` only after a real frame callback
- `user-close-observed=true` only after user or host close
- `input-dispatch=true|false|not-applicable` separated from evidence launch
- `exit-path=true` only for an explicit close path

The first frame is a lifecycle milestone, not completion. The interactive window
must stay open until user/host close; bounded first-frame, frame-count, scene
metadata, screenshot, and pixel-readback commands are evidence helpers only.

## Evidence Commands

- `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj -m:1`
  - Log: `specs/018-persistent-gui-runtime/readiness/logs/t019-skiaviewer-tests.txt`
  - Result: PASS, 25 tests.
- `dotnet pack src/SkiaViewer/SkiaViewer.fsproj -c Release -o /tmp/fs-skia-ui-t019-pack`
  - Log: `specs/018-persistent-gui-runtime/readiness/logs/t019-pack-skia-viewer.txt`
  - Result: PASS, package created.
- `dotnet fsi /tmp/fs-skia-ui-t019.fsx`
  - Log: `specs/018-persistent-gui-runtime/readiness/logs/t019-fsi-packed-lifecycle.txt`
  - Result: `running=true firstFrameState=FirstFramePresented closeOnFrame=false closed=true closeEffects=true`.
- `dotnet test template/base/tests/Product.Tests/Product.Tests.fsproj -m:1 --filter "generated default game"`
  - Log: `specs/018-persistent-gui-runtime/readiness/logs/t021-generated-default-game-tests.txt`
  - Result: PASS for board/grid, side panel, keyboard input, tick progression, and evidence flags.
- `dotnet test tests/Testing.Tests/Testing.Tests.fsproj -m:1`
  - Log: `specs/018-persistent-gui-runtime/readiness/logs/t022-testing-validation-helpers.txt`
  - Result: PASS for generated default launch validation helper.

## Host Notes

No fake window loop was used for T019-T023. The packed FSI evidence exercises
the public lifecycle/update path and confirms first-frame does not emit
`CloseWindow`; native window user-close evidence still depends on a supported
desktop session.

The default generated executable smoke log
`specs/018-persistent-gui-runtime/readiness/logs/t021-default-executable-smoke.txt`
is intentionally not treated as final lifecycle proof because the template
currently resolves older `0.1.16-preview.1` packages and emits `NU1603` drift.
Package-resolution enforcement is covered by US4 tasks.

## Explicit Close Criteria

Interactive completion requires one of:

- a native user close callback;
- a generated host effect that emits `CloseWindow`;
- a host process termination path documented as host close.

Evidence-mode completion is separate and must report `mode=persistent-evidence`
with `self-closed-for-evidence=true`.
