# window-visibility — applicability (feature 115, T003)

status=not-applicable

Feature 115 opens no window and ships no new interactive/persistent graphical entry point. It is a
dependency-version + governance-asset maintenance change. The full window-visibility not-applicable set is
authored as the gate enforces:

- [interactive-visible-window.md](./interactive-visible-window.md) — status=not-applicable
- [close-reason-separation.md](./close-reason-separation.md) — no window close to classify
- [window-state-diagnostics.md](./window-state-diagnostics.md) — every diagnostic-class not-applicable
- [window-options.md](./window-options.md) — every option not-applicable
- [real-image-evidence.md](./real-image-evidence.md) — no image produced

The existing `runInteractiveApp` window-launch contract is unchanged (no source edit), so no new window
visibility, accessibility, or first-frame-presented claim is made by this feature.
