# Window visibility — applicability decision (feature 100, R5, T002/T003)

status=not-applicable
mode=render-only
window-obligation=none

## Visible decision (T003)

The persistent-launch / viewer-launch task-generation rule does **not** newly apply to R5. R5 wires
general navigation into the **existing** `runInteractiveApp` host loop and adds **no** default-
executable / persistent-launch entry point. Navigation is observed through the existing
`runInteractiveApp` seam; at-rest rendered output is unchanged; there is **no** new window-visibility /
screenshot obligation.

The full window-visibility evidence set records this not-applicable decision with honest values:

- [interactive-visible-window.md](./interactive-visible-window.md) — status=not-applicable, mode=render-only
- [close-reason-separation.md](./close-reason-separation.md) — no window close to classify
- [window-state-diagnostics.md](./window-state-diagnostics.md) — every diagnostic-class not-applicable
- [window-options.md](./window-options.md) — every option not-applicable
- [real-image-evidence.md](./real-image-evidence.md) — cross-references the responds-vs-renders capture
  as the rendered-output evidence captured through the deterministic `runInteractiveApp` seam
- [generated-validation.md](./generated-validation.md) — nothing ships into the template/generated products

The live desktop window that surfaces this behavior is `runInteractiveApp`, whose visibility was
established by 085/092/096 and is unchanged here.
