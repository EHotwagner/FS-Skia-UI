# Window visibility (085)

Index for the window-visibility evidence class authored as `key=value` blocks
(FR-015) — never markdown tables for the parsed tokens. The class members:

- `interactive-visible-window.md` — status/mode/window-visible/accessible-window/
  first-frame-presented/self-closed-for-evidence.
- `close-reason-separation.md` — close-reason/user-close-observed/
  evidence-close-observed (evidence-close never reported as user-close).
- `window-state-diagnostics.md` — diagnostic-class ∈ {environment-session,
  window-visibility, app-lifecycle, product-defect} + native-handle/visible/
  focusable/renderable-surface/input-devices.
- `window-options.md` — option=resize/maximize/startup-state/startup-position/
  backend, each with status/observed.
- `real-image-evidence.md` — evidence-kind/status/artifact-decodable/
  proves-scene-rendering/proves-desktop-visibility.
- `generated-validation.md` — exact-package-match/generated-tests-ran/
  authoritative/failure-class.

**Authoritative command**: T018 launches the durable `Viewer.runInteractiveApp`
host from the default executable path on `DISPLAY=:1` with a self-closing evidence
host. **Failure class**: taskbar-only / process-only substitution claimed as a
visible window is blocking. **Next action**: T018/T019/T038 finalize the deferred
tokens to observed values.
