# Theme ↔ token value parity (feature 069)

- **Authoritative command**: `dotnet test tests/Controls.Tests/Controls.Tests.fsproj --filter "Feature 069"`
  (parity + render-parity tests in `tests/Controls.Tests/DesignTokenParityTests.fs`)
- **Failure class**: `theme-value-drift`
- **Result**: PASS — 7/7 (value parity + render parity + dependency guard + typed-surface reference).

## 20-cell value-parity table (SC-002)

Each token-derived `Theme.<theme>.<Field>` equals its pre-feature literal
(data-model §4). `DesignTokens.<Theme>.<token>` resolves byte-identically.

| Theme field | Token | light value | dark value |
| --- | --- | --- | --- |
| `Foreground` | `foreground` | `Colors.rgba 31uy 41uy 55uy 255uy` | `Colors.rgba 241uy 245uy 249uy 255uy` |
| `Background` | `background` | `Colors.rgba 248uy 250uy 252uy 255uy` | `Colors.rgba 17uy 24uy 39uy 255uy` |
| `Accent` | `accent` | `Colors.rgba 37uy 99uy 235uy 255uy` | `Colors.rgba 96uy 165uy 250uy 255uy` |
| `Danger` | `danger` | `Colors.rgba 185uy 28uy 28uy 255uy` | `Colors.rgba 185uy 28uy 28uy 255uy` (alias `{light.danger}`) |
| `Muted` | `muted` | `Colors.rgba 100uy 116uy 139uy 255uy` | `Colors.rgba 148uy 163uy 184uy 255uy` |
| `FontFamily` | `fontFamily` | `None` | `None` |
| `FontSize` | `fontSize` | `14.0` | `14.0` |
| `Density` | `density` | `1.0` | `1.0` |
| `CornerRadius` | `cornerRadius` | `4.0` | `4.0` |
| `ContrastRequiredRatio` | `contrastRequiredRatio` | `4.5` | `4.5` |

Full-record equality is also asserted: `Theme.light = frozenLight` and
`Theme.dark = frozenDark` (the frozen pre-feature literal records).

## Render parity (SC-003)

The controls gallery (`Stack` of `TextBlock`/`Button`/`CheckBox`/`ProgressBar`) is
rendered through `Control.render` against the token-derived `Theme.light`/`Theme.dark`
and against the frozen pre-feature themes. The deterministic readback hash
(`Scene.renderReadbackEvidence`) is **identical** for both, with zero render
diagnostics — node/visual output is unchanged.

## FSI surface evidence

`readiness/fsi/design-tokens-surface.txt` shows
`DesignTokens.Light.foreground = Theme.light.Foreground : true` and
`DesignTokens.Dark.danger = Theme.dark.Danger : true` against the compiled package.
