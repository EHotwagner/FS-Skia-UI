# window-visibility — applicability (feature 116, T002)

status=not-applicable

Feature 116 opens no window and ships no new interactive/persistent graphical entry point. It is an
internal damage-set + bounded picture-cache + advisory-diagnostic + additive-metrics change. The full
window-visibility not-applicable set is authored as the gate enforces:

- [interactive-visible-window.md](./interactive-visible-window.md) — status=not-applicable
- [close-reason-separation.md](./close-reason-separation.md) — no window close to classify
- [window-state-diagnostics.md](./window-state-diagnostics.md) — every diagnostic-class not-applicable
- [window-options.md](./window-options.md) — every option not-applicable
- [real-image-evidence.md](./real-image-evidence.md) — no image produced
- [visual-evidence-honesty.md](./visual-evidence-honesty.md) — nothing rendered to a window

The existing `runInteractiveApp` window-launch contract is unchanged (no source edit to the launch
seam), so no new window visibility, accessibility, or first-frame-presented claim is made. The
interactive-UI run-and-use gate is N/A — the feature delivers an internal damage/picture-cache contract +
deterministic metrics observable via `ControlsElmish.Perf.runScript` plus an advisory diagnostic, not a
new interactive surface.
