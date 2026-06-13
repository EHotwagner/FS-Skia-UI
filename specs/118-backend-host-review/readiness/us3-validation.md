# US3 validation — hosting modes and backend limits documented honestly (feature 118)

**Story:** the audit and hosting-mode tradeoff documentation exist and are honest about
evidence vs. live performance.

## Artifacts present

- [audit/present-path-audit.md](./audit/present-path-audit.md) — the present-path findings with
  concrete `Vulkan.fs` call sites: `renderSceneToPixels` readback (`:929` / `surface.ReadPixels`
  `:959`), per-frame `createStagingBuffer` (`:865`/`:971`) + per-frame `vkCreateCommandPool`
  (`:979`), the per-frame `vkQueueWaitIdle` full stall (`:1079`), `vkQueuePresentKHR` (`:1092`),
  the shared live/evidence readback routine, and the prior absence of any direct-to-swapchain
  path — plus the SkiaSharp #1502/#2191 binding gap that blocks the readback-free path.
- [audit/opengl-backend-resolution.md](./audit/opengl-backend-resolution.md) — the concrete
  resolution (OpenGL present backend), its consequences, and the recommended sequencing.
- [audit/hosting-mode-tradeoffs.md](./audit/hosting-mode-tradeoffs.md) — every host mode
  enumerated with tradeoffs.

## Host modes enumerated (FR-009)

`runInteractiveApp`, `runApp`/`runAppWithWindowBehavior`, `runInteractiveViewer`
(+`…WithWindowBehavior`), the bounded evidence runs `runBounded` / `runForFrames` /
`runUntilFirstFrame`, on-demand `captureScreenshotEvidence`, and headless
`ControlsElmish.Perf.runScript`. See hosting-mode-tradeoffs.md for each one's present path and
performance character.

## Readback / stall call sites recorded

- GPU→CPU readback: `Vulkan.fs:929` (`renderSceneToPixels`) / `:959` (`surface.ReadPixels`).
- Per-frame staging buffer + command pool: `Vulkan.fs:865`/`:971` / `:979`.
- Per-frame full-queue stall: `Vulkan.fs:1079` (`vkQueueWaitIdle`).
- Present: `Vulkan.fs:1092` (`vkQueuePresentKHR`).

## Evidence is not live performance proof (SC-008)

Stated explicitly in hosting-mode-tradeoffs.md: the deterministic `Perf.runScript` metrics are
counts/booleans from a headless, backend-less driver; the bounded runs prove window lifecycle,
not render throughput; on-demand capture proves pixels, not frame rate. The genuine live present
cost is a human/diagnostic signal (FR-011), surfaced via the FR-007 diagnostic, never a gate or
golden. No `FrameMetrics` field was added (FR-008).
