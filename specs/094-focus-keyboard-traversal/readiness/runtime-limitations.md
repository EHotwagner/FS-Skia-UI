# Runtime limitations & unsupported-scope handling

## Unsupported-scope / failure handling (feature 094)

- A delivered key that matches no focused control and no traversal candidate resolves to
  `Focus.route` → `Fallthrough`, a **defined no-op** that falls through to `host.MapKey` — never a
  silent drop and never an exception.
- A focused control removed between frames reuses E2 stale-target recovery
  (`ControlRuntimeMsg.RecoverStaleTarget` / the `StaleTarget` effect / diagnostic); `Focus.traverse`
  recovers a stale current id to the first stop (Next) / last stop (Previous), or `None` for an empty
  order — total, never throws.
- No new accessibility primitive is introduced — the focus model reads `AccessibilityMetadata` only.

## Platform runtime envelope (inherited, unchanged by this feature)

- Targets **.NET 10 desktop** (Windows and Linux). The pure `Focus` reducers and the offscreen
  `routeFocusedKey` route-probe are deterministic and require no window.
- The interactive host renders through **Vulkan** via the **SkiaSharp preview** backend; there is
  **no software-renderer fallback**.
- **unsupported macOS/mobile/browser**: those targets are out of scope for the live host (the
  logic proofs remain valid everywhere, being window-free).
