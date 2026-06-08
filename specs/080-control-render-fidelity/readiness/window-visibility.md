# Window visibility (080) — N/A (headless evidence path)

This feature adds **no persistent graphical viewer** and **no default-executable
launch surface**. The `ControlsPreview.Harness` is a headless render-only
evidence path (`ViewerRenderTargetPng` screenshot capture), not an interactive
viewer, so the persistent-launch / interactive-visible-window task rule does not
apply (see `tasks.md` Vertical-slice note).

- **Authoritative command**: `dotnet run --project tests/ControlsPreview.Harness -- --render`
  (headless capture; no window presented).
- **Artifact path**: `docs/img/controls/*.png` (captured frames); no
  `interactive-visible-window.md` / `window-state-diagnostics.md` is required
  because no persistent window is launched.
- **Failure class**: a real `RenderingFailure`/`LaunchFailure` from the capture
  path is preserved (not downgraded); native-Skia-absent is a blocking host
  warning.
- **Next action**: none — headless evidence only; window-visibility proof is not
  in scope for this feature.

_Placeholder created in T002._
