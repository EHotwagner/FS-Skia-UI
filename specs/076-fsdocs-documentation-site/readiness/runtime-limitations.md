# Runtime limitations & evidence-mode disposition (feature 076)

This feature ships **documentation + doc-build configuration only**; it introduces
no runtime surface. The runtime-limitation envelope below is restated so embedded
visual evidence (FR-015) is honest about where rendering is and is not supported.

## Supported runtime envelope

- Target: **.NET 10 desktop** only (the repo's `net10.0` desktop host).
- Rendering: **Vulkan** via **SkiaSharp preview** bindings.
- **unsupported macOS/mobile/browser** — these platforms are out of the supported
  envelope; documentation must not present them as supported render targets.
- There is **no software-renderer fallback**: where Vulkan is unavailable the host
  degrades benignly (render-only evidence mode) rather than fabricating frames.

## Evidence-mode disposition for embedded visuals (FR-015 / T026)

The two required literate examples (`docs/examples/typed-control-mvu.fsx`,
`docs/examples/design-token-flow.fsx`) exercise **GPU-free** model/props/lowering
paths and embed **no** screenshots. No fabricated visuals are used anywhere in the
site. If a future page embeds a screenshot it must follow evidence-mode rules:
render-only, no fabrication, benign degradation where rendering is unsupported.

- **Disposition**: no rendered/screenshot visuals are embedded in this feature's
  docs → T026 is recorded `[-]` (no visuals embedded) with this rationale.
