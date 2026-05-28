# Runtime Limitations

Evidence:

- `specs/025-upgrade-skia-speckit/readiness/logs/skia-native-viewer-validation.md`
- `specs/025-upgrade-skia-speckit/readiness/logs/t037-skiaviewer-rerun.log`
- `specs/025-upgrade-skia-speckit/readiness/logs/t037-verify.log`

Runtime context: .NET 10 desktop, Vulkan, SkiaSharp preview, unsupported
macOS/mobile/browser, no software-renderer fallback.

Contract keywords: .NET 10 desktop; Vulkan; SkiaSharp preview; unsupported macOS/mobile/browser; no software-renderer fallback.

The SkiaSharp upgrade preserves the existing desktop host posture. No new
desktop OS support, browser/mobile path, or CPU/software renderer fallback is
introduced. Native/window failures remain observable compatibility evidence and
must preserve platform, command, failure reason, and blocking status.

During `Verify`, `tests/SkiaViewer.Tests` failed once with an
`UnsupportedEnvironment` window diagnostic after `frame 1 presented`; the
immediate focused rerun passed `48/48`. The broad rerun then completed all
targets through `EvidenceGraph`; final `EvidenceAudit` remains a separate
T039 gate.
