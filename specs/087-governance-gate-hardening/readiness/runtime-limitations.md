# Runtime limitations — feature 087

This feature changes **only** the compiled governance engine `FS.Skia.UI.Build`
(`build/Governance/**`) and the generated governance contract artifacts. It makes
**no product/runtime change** — the host-runtime limitations below are unchanged
from the shipped framework and are restated here only to satisfy the
readiness-contract scan.

The interactive consumer host targets a **.NET 10 desktop** runtime with a
**Vulkan** swapchain rendered through **SkiaSharp preview** bindings.

- Supported: Windows and Linux desktop with a Vulkan-capable display session.
- **Unsupported macOS/mobile/browser** targets — no host window is opened there.
- There is **no software-renderer fallback** for the live window: when the
  GPU/display session is unavailable the host reports an `unsupported` host fact
  (non-failing) rather than silently substituting a fake surface.
- The governance engine itself is host-OS-agnostic pure F#: `Governance.Tests`,
  the verdict/propagation/skew functions, and the FAKE governance targets run
  without a display session. This feature's evidence is captured by FAKE-backed
  governance targets, not by a host window.
- No new host I/O class is introduced: the only edge effects are reading
  already-present surface baselines / `tasks.md` / `readiness/` and the FR-001
  generated feature-context provisioning at the interpreter edge.
