# Runtime Limitations

status=ok

Known runtime limitations:

- .NET 10 desktop is the supported generated runtime for this feature.
- Vulkan and SkiaSharp preview host behavior can vary by desktop session.
- unsupported macOS/mobile/browser targets remain out of scope for this
  generated viewer evidence path.
- There is no software-renderer fallback for claiming persistent desktop
  screenshot proof.
- The current SkiaViewer host does not expose screenshot capture, so screenshot
  evidence reports `status=unsupported`.
- Unsupported screenshot capture is a real host capability fact, not synthetic
  screenshot success.
- Generated screenshot reports must include `unsupported-host-reason` and
  `fallback=deterministic-scene-evidence` and must not claim a screenshot
  artifact.
- Persistent launch and deterministic render evidence remain separate evidence
  kinds.

Authoritative evidence:

- `screenshot-host-classification.md`
- `screenshot-evidence.md`
- `evidence-report-conventions.md`
