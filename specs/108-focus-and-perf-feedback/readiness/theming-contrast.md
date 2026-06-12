# Live-theming + WCAG contrast proof (T034, SC-010, FR-017/018)

enforcing-test=tests/Controls.Tests/Feature108ThemingTests.fs

## resolve / toTheme (FR-017)

`Theming.resolve : ThemeMode -> Color -> RolePalette` seeds the neutral/structural roles from the
matching base `Theme` (Light/Dark) and places the supplied accent on the `Accent` and `FocusRing`
roles. `Theming.toTheme : RolePalette -> Theme` projects the palette's colours onto the framework
`Theme` (the "paint theme"). `Theming` lives in the child namespace `FS.Skia.UI.Controls.Theming` and
uses a Controls-local `ThemeMode = Light | Dark` (NOT `Color.Palettes.RampVariant`) so it adds no
Controls→Color package dependency (the plan's "no new dependency" constraint).

## Render-path vs reuse-key split (FR-018)

A consumer derives the paint theme on the render path — `Control.renderTree (Theming.toTheme palette)
size view` — while keeping a static `host.Theme` for the fragment-reuse key. This guarantees the
captured palette is EXACT for the frame, and a palette-only change (e.g. a new accent) never reuses a
stale fragment keyed by the old palette.

## WCAG contrast (SC-010) — reused, not re-implemented

`FS.Skia.UI.Color.Contrast.ratio` matches the WCAG relative-luminance reference:

- black on white → 21.0 : 1 (the WCAG maximum; asserted within 0.1).
- `Contrast.verdict Text 4.5 = Aa` (normal-text AA boundary, ≥ 4.5 : 1).
- `Contrast.verdict GraphicOrUi 3.0 = Aa` (large/graphic AA boundary, ≥ 3 : 1).
- `Contrast.verdict Text 2.0 = Fail`.

Contrast is the Color package's authority (`ContrastCheck` stays the sole contrast gate); Theming does
not re-implement it.
