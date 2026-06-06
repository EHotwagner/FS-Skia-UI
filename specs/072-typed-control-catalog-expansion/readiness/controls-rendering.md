# Controls Rendering & Accessibility Evidence — Expansion (072)

Deterministic render-only evidence for the five new controls (FR-009, SC-005).
Render is **real** through the existing `Control.render` / `Scene.renderReadbackEvidence`
IR path — no `[S]`, no renderer change.

## Render coverage (≥2 viewports, stable node counts)

`tests/Controls.Tests/RenderingTests.fs` → "Feature 072 new-control rendering
(SC-005)" renders each control at **320×240** and **1024×768**:

- Asserts `rendered.Diagnostics` is empty at both viewports.
- Asserts `evidence.DeterministicHash` is non-empty (render-only, byte-identical
  on re-capture).
- Asserts `NodeCount` is **stable across viewports** (node count is independent of
  viewport; the overlay popup is always present so open/closed states keep the
  count stable).

| control | lowered root | representative node count |
|---------|--------------|---------------------------|
| toggle-button | `button` | 1 |
| split-button | `toolbar` | 5 (toolbar + primary + trigger + overlay + menu) |
| date-picker | `stack` | 5 + days-in-month day buttons |
| time-picker | `stack` | 4 (stack + hour + ":" + minute) |
| color-picker | `wrap` | 1 + one cell per swatch |

## Accessibility coverage

`tests/Controls.Tests/AccessibilityTests.fs` → "Feature 072 new-control
accessibility (FR-009, SC-005)" asserts each lowered root carries:

- its catalog role (`Button` / `Menu` / `TextBox` / `TextBox` / `List`),
- a focusable trigger with `Enter`/`Space` activation keys,
- non-empty navigation keys (arrow keys for the calendar / menu / swatch grid),
- **no** accessibility diagnostics from `Accessibility.validate`.

## Honesty classification

Render evidence is deterministic render-only output (hashes), not screenshots, so
there is no benign/blocking host-warning classification to record for this
feature. No GPU window is required.
