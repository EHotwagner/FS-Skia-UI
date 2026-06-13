# Unsupported scope & deferral (feature 118)

Feature 118 is the performance roadmap's **Phase 9 (Backend and Host Mode Review)** — the final
rung. Explicitly OUT of this rung:

- **Render-thread / compositor split** (FR-010) — not attempted.
- **Layer / scene-submission diffing, scene-graph caching, GPU / layer caches** (FR-010) — none
  created.
- **Any timing-based pass/fail gate** (FR-011) — the present-mode signal is a human/diagnostic
  `ViewerDiagnosticEvent`, never a gate; deterministic gating stays on counts and booleans.
- **A `FrameMetrics` present field** (FR-008) — `FrameMetrics` is headless (no backend); a
  backend present field would be permanently zero/absent.

## Blocked-by-dependency (not deferred by choice)

The readback-free `DirectToSwapchain` present path (FR-002 / SC-002) is **blocked upstream** by
SkiaSharp's managed-binding Vulkan gap (`SKSurface.Create` cannot wrap a swapchain image —
mono/SkiaSharp #1502; image-layout interop unbound — #2191). It is **not** achievable on any
SkiaSharp version, including the newest preview. The implementation seam ships and degrades
safely to `OffscreenReadback` (FR-005). The readback-free goal is recorded as
blocked-by-dependency and its concrete resolution — an **OpenGL present backend** — is written
up in `audit/opengl-backend-resolution.md` as the next roadmap phase (its own spec/plan,
constitution amendment, and dependency change; out of scope for Phase 9).

## Applicability of cross-cutting principles

- **Principle IV (Elmish/MVU)** — N/A as a state change: `PresentMode` is configuration carried
  in `ViewerModel.Options`; no new `Msg`/`Effect`, `Viewer.update` unchanged. The present
  switch + safe fallback live in the backend interpreter edge (`Vulkan.fs`), the correct home
  for I/O; `update` stays pure.
- **Interactive-UI run-and-use gate** — APPLICABLE and satisfied: the live windowed viewer was
  launched on a real Vulkan backend in both present modes (`smoke/direct-mode-smoke.md`,
  `default-byte-identity.md`); the production render path (`renderFrame`) was exercised.

## Failure diagnostics

A missing required evidence artifact fails `Route --enforce` (names artifact + tier). A race-like
or unknown-concurrent-FAKE failure is rerun sequentially before any product-debugging
classification (shared `.fake` state).
