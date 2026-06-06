# Phase 1 Data Model: Design Tokens

## 1. The DTCG document (`src/Controls/design-tokens.tokens.json`)

DTCG ([Design Tokens Community Group](https://www.designtokens.org/)) format: a token is an object
with `$type` and `$value`; a group nests tokens; an alias is the string `"{group.token}"`. Two
theme value sets (`light`, `dark`) are expressed as sibling groups so both `Theme.light` and
`Theme.dark` are fully token-derived (spec Edge Case "Light vs. dark").

Illustrative shape (values reproduce today's `Theme.fs` literals exactly — behavior-preserving):

```jsonc
{
  "$description": "FS.Skia.UI.Controls theme primitives — single source of truth (feature 069).",
  "light": {
    "foreground":            { "$type": "color",     "$value": "#1f2937ff" },
    "background":            { "$type": "color",     "$value": "#f8fafcff" },
    "accent":                { "$type": "color",     "$value": "#2563ebff" },
    "danger":                { "$type": "color",     "$value": "#b91c1cff" },
    "muted":                 { "$type": "color",     "$value": "#64748bff" },
    "fontFamily":            { "$type": "fontFamily", "$value": null },
    "fontSize":              { "$type": "dimension", "$value": 14.0 },
    "density":               { "$type": "number",    "$value": 1.0 },
    "cornerRadius":          { "$type": "dimension", "$value": 4.0 },
    "contrastRequiredRatio": { "$type": "number",    "$value": 4.5 }
  },
  "dark": {
    "foreground":            { "$type": "color",     "$value": "#f1f5f9ff" },
    "background":            { "$type": "color",     "$value": "#111827ff" },
    "accent":                { "$type": "color",     "$value": "#60a5faff" },
    "danger":                { "$type": "color",     "$value": "{light.danger}" },
    "muted":                 { "$type": "color",     "$value": "#94a3b8ff" },
    "fontFamily":            { "$type": "fontFamily", "$value": null },
    "fontSize":              { "$type": "dimension", "$value": 14.0 },
    "density":               { "$type": "number",    "$value": 1.0 },
    "cornerRadius":          { "$type": "dimension", "$value": 4.0 },
    "contrastRequiredRatio": { "$type": "number",    "$value": 4.5 }
  }
}
```

> `dark` today is `{ light with Name="dark"; Foreground=...; Background=...; Accent=...; Muted=... }`
> — so `dark.danger`, `dark.fontSize`, `dark.density`, `dark.cornerRadius`,
> `dark.contrastRequiredRatio` equal `light`'s. The `dark.danger` alias `"{light.danger}"` above is
> the worked **alias** case (resolves to `#b91c1cff`); the remaining shared values MAY be aliases or
> repeated literals — the generator resolves both identically (D6). Final alias-vs-literal choice is
> a generation detail proven byte-identical by the parity table; it changes no observable value.

## 2. `DesignTokenFact` (build-side, `DesignTokenGen`)

The parsed, alias-resolved representation of one token in one theme — the unit the renderer and the
currency check operate on (mirrors `CatalogGen.TypedCatalogFact`).

| Field | Type | Meaning |
| --- | --- | --- |
| `Theme` | `string` | `"light"` or `"dark"` |
| `Name` | `string` | token name, e.g. `"foreground"` (maps to a `Theme` field — §4) |
| `Kind` | `TokenKind` | `Color` \| `Dimension` \| `Number` \| `FontFamily` |
| `Rendered` | `string` | the F# value literal this token lowers to (e.g. `Colors.rgba 31uy 41uy 55uy 255uy`, `14.0`, `None`) |

`TokenKind` is a closed DU. Resolution is total: every fact's `$value` is concrete (no unresolved
alias) before a fact is produced; a residual alias/cycle/missing token raises a generation failure
(D6) before any fact list is returned.

## 3. Token taxonomy (the 10 migrated primitives)

Exactly the 10 `Theme` fields (`Types.fsi:187-198`), each in both themes (10 × 2 = 20 facts). The
`Name` field of `Theme` (`"light"`/`"dark"`) is **not** a token — it labels the variant and stays a
code constant in `Theme.fs`.

| Class (plan §3.4 taxonomy) | `Theme` field | Token kind |
| --- | --- | --- |
| Color | `Foreground`, `Background`, `Accent`, `Danger`, `Muted` | `color` |
| Typography | `FontFamily` | `fontFamily` (→ `string option`) |
| Typography | `FontSize` | `dimension` (`float`) |
| Density | `Density` | `number` (`float`) |
| Shape | `CornerRadius` | `dimension` (`float`) |
| Accessibility | `ContrastRequiredRatio` | `number` (`float`) |

## 4. DTCG → `Theme` field mapping (deterministic, byte-identical — D5)

| `Theme` field | DTCG token | light value → rendered F# | dark value → rendered F# |
| --- | --- | --- | --- |
| `Foreground` | `<theme>.foreground` | `#1f2937ff` → `Colors.rgba 31uy 41uy 55uy 255uy` | `#f1f5f9ff` → `Colors.rgba 241uy 245uy 249uy 255uy` |
| `Background` | `<theme>.background` | `#f8fafcff` → `Colors.rgba 248uy 250uy 252uy 255uy` | `#111827ff` → `Colors.rgba 17uy 24uy 39uy 255uy` |
| `Accent` | `<theme>.accent` | `#2563ebff` → `Colors.rgba 37uy 99uy 235uy 255uy` | `#60a5faff` → `Colors.rgba 96uy 165uy 250uy 255uy` |
| `Danger` | `<theme>.danger` | `#b91c1cff` → `Colors.rgba 185uy 28uy 28uy 255uy` | (= light) `Colors.rgba 185uy 28uy 28uy 255uy` |
| `Muted` | `<theme>.muted` | `#64748bff` → `Colors.rgba 100uy 116uy 139uy 255uy` | `#94a3b8ff` → `Colors.rgba 148uy 163uy 184uy 255uy` |
| `FontFamily` | `<theme>.fontFamily` | `null` → `None` | `null` → `None` |
| `FontSize` | `<theme>.fontSize` | `14.0` → `14.0` | `14.0` → `14.0` |
| `Density` | `<theme>.density` | `1.0` → `1.0` | `1.0` → `1.0` |
| `CornerRadius` | `<theme>.cornerRadius` | `4.0` → `4.0` | `4.0` → `4.0` |
| `ContrastRequiredRatio` | `<theme>.contrastRequiredRatio` | `4.5` → `4.5` | `4.5` → `4.5` |

This 20-cell table **is** the parity oracle (SC-002): the test asserts each token-derived
`Theme.<theme>.<Field>` equals the pre-feature literal in `Theme.fs:7-26`. Source literals confirmed
against `src/Controls/Theme.fs` (read 2026-06-06).

## 5. `RegionStatus` / `TokenCurrency` (build-side currency)

Mirrors `CatalogGen.RegionStatus`/`CatalogCurrency`:

| Type | Shape | Meaning |
| --- | --- | --- |
| `RegionStatus` | `Current \| Stale \| Missing` | per-token status of the generated `.fs` vs. a fresh render |
| `TokenCurrency` | `{ Token: string; Theme: string; FilePath: string; Status: RegionStatus }` | one token's currency in the generated module |

`currency` returns one `TokenCurrency` per token; `isCurrent` is true iff all `Current`;
`currencyDrift` returns one actionable diagnostic per `Stale`/`Missing` token, each naming the
token, the theme, the generated file, and `./fake.sh build -t RefreshSurfaceBaselines` (FR-004).
A wholly-missing or unparseable generated file yields all-`Missing` (loud, never silent).

## 6. Generated module shape (`src/Controls/DesignTokens.fs`, behind curated `DesignTokens.fsi`)

```fsharp
// GENERATED — do not edit. Source of truth: src/Controls/design-tokens.tokens.json
// Regenerate via: ./fake.sh build -t RefreshSurfaceBaselines
namespace FS.Skia.UI.Controls

open FS.Skia.UI.Scene

module DesignTokens =
    module Light =
        let foreground : Color = Colors.rgba 31uy 41uy 55uy 255uy
        // … background, accent, danger, muted, fontFamily, fontSize, density,
        //    cornerRadius, contrastRequiredRatio
    module Dark =
        let foreground : Color = Colors.rgba 241uy 245uy 249uy 255uy
        // …
```

`Theme.fs` then reads (signature unchanged):

```fsharp
let light : Theme =
    { Name = "light"
      Foreground = DesignTokens.Light.foreground
      Background = DesignTokens.Light.background
      // … every migrated field sourced from DesignTokens.Light.* ; no inline literal …
      ContrastRequiredRatio = DesignTokens.Light.contrastRequiredRatio }
```

The exact `DesignTokens` surface (module/value names, types) is fixed in
`contracts/design-tokens.fsi`.
