# Quickstart: editing a theme primitive via the DTCG single source

This is the maintainer flow the feature delivers (US1) and the `fs-skia-design-tokens` skill
teaches. It demonstrates that the DTCG document is the **one** edit point and that
`DesignTokenDrift` keeps the generated F# in lock-step.

## The single edit point

To change any of the 10 theme primitives (`Foreground`, `Background`, `Accent`, `Danger`, `Muted`,
`FontFamily`, `FontSize`, `Density`, `CornerRadius`, `ContrastRequiredRatio`) for `light` or `dark`,
edit **only** `src/Controls/design-tokens.tokens.json`. Never hand-edit `src/Controls/DesignTokens.fs`
(it is generated) or re-inline a literal in `src/Controls/Theme.fs`.

## Walkthrough — change the light accent color

1. **Edit the DTCG source**:
   ```jsonc
   // src/Controls/design-tokens.tokens.json
   "light": { "accent": { "$type": "color", "$value": "#1d4ed8ff" } }   // was #2563ebff
   ```

2. **Regenerate from the one command** (the same single regenerate entry point as the catalog):
   ```bash
   ./fake.sh build -t RefreshSurfaceBaselines
   ```
   This runs `RegenerateDesignTokens`, which rewrites `src/Controls/DesignTokens.fs` from the DTCG
   document (`DesignTokens.Light.accent` becomes `Colors.rgba 29uy 78uy 216uy 255uy`). `Theme.light.Accent`
   now resolves to the new value with **no** edit to `Theme.fs` (it references `DesignTokens.Light.accent`).

3. **Verify the gate is green**:
   ```bash
   ./fake.sh build -t DesignTokenDrift
   ```
   PASS: the generated module is a current, byte-identical regeneration of the DTCG source.

## What the gate catches (US1 / SC-005)

- **Stale generation** — edit the DTCG value but skip `RefreshSurfaceBaselines`:
  `DesignTokenDrift` **FAILS**, naming the stale token (e.g. `light.accent`), the generated file,
  and `./fake.sh build -t RefreshSurfaceBaselines`.
- **Hand-edit of generated F#** — edit `DesignTokens.fs` directly: same FAIL (the generated file is
  not the source of truth).
- **Malformed / cyclic / missing DTCG** — generation fails loudly naming the offending token; **no**
  partial module is emitted.

## Behavior-preservation check (US2 / SC-002, SC-003)

For the shipped feature, the DTCG document reproduces today's exact values, so:

```bash
./fake.sh build -t Route        # prints the escalated controls-public-surface set incl. DesignTokenDrift
# then run ONLY the gates Route prints, FAKE-backed gates sequentially
```

- The 10-field × 2-theme parity table asserts every `Theme.light/dark` field equals its
  pre-feature literal (token-derived ≡ old value).
- Re-rendering the controls gallery produces identical node/visual output.

## Determinism (SC-006)

```bash
./fake.sh build -t RefreshSurfaceBaselines   # run twice
git diff --exit-code src/Controls/DesignTokens.fs   # no diff — generation is pure
```

## Authoring directly against a token (US3)

```fsharp
open FS.Skia.UI.Controls
// reference a token instead of copying a literal — greppable, stays in sync with the DTCG source
let myAccent = DesignTokens.Light.accent
```
