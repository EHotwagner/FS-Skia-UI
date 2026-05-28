# Runtime Limitations

Recorded at: 2026-05-28T10:55:09+02:00

Known runtime limitation for this feature:

- Live viewer-window screenshot capture is not implemented/available for the
  current viewer host path. The T020 FSI attempt returned
  `status=ScreenshotUnsupported`, `capture-availability=CaptureUnavailable`,
  `capture-source=DeterministicSceneRender`, and `proves-screenshot=false`.
- The current desktop viewer stack targets .NET 10 desktop with Vulkan and
  SkiaSharp preview dependencies.
- Unsupported macOS/mobile/browser hosts are outside this validation scope.
- There is no software-renderer fallback for claiming live screenshot proof.

Accepted unsupported evidence:

- `readiness/logs/t020-live-screenshot-attempt.txt`
- `screenshot-capability-detail.md`
- `screenshot-success-artifact.md`

This limitation is intentionally not hidden by deterministic scene render or
pixel-readback fallback output. Fallback evidence remains diagnostic and must
not claim screenshot proof.
