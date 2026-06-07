---
title: Design Tokens and Penpot
category: Design
categoryindex: 4
index: 21
description: The design-token flow from a Penpot/DTCG single source to the typed control surface — the DTCG token file, the generated typed F# DesignTokens module, the DesignTokenDrift gate, and how tokens reach typed controls and the Spec Kit process.
---

# Design Tokens and Penpot

The ten `Theme` primitives in `FS.Skia.UI.Controls` — five colors, a font family,
a font size, a density, a corner radius, and a contrast ratio, each for a `light`
and a `dark` variant — are **single-sourced** from one DTCG (Design Tokens Community
Group) JSON document and generated into a typed F# module. This is the tokens-first
integration delivered by feature 069: a designer can author the theme in Penpot
(whose native token format *is* DTCG) and export the JSON, or edit the checked-in
DTCG document directly; a pure generator lowers it to typed F#; and a drift gate
keeps the generated module byte-identical to its source. This page explains that
flow end to end — the source file, the generated `DesignTokens.Light`/`Dark`
surface, the `DesignTokenDrift` gate, how a token value reaches a typed control, how
to author tokens, and where in the Spec Kit process this workflow and custom
components belong.

Live Penpot/MCP synchronisation (driving the design tool from an agent at runtime)
is **deliberately out of scope** today; the integration that ships is the
standards-based DTCG-JSON-to-F# pipeline, which needs no live design-tool access and
runs entirely from committed files. See also the
[typed control front door](typed-front-door.html) that consumes this surface, the
[controls architecture](../architecture/controls.html) page for the theme/render
pipeline, the [single-source generation](../governance/single-source-generation.html)
governance pattern this mirrors, and the [API reference](../reference/index.html)
for the generated member signatures.

## The single source: a DTCG token document

The one edit point is `src/Controls/design-tokens.tokens.json`. It is a DTCG-format
document with two groups, `light` and `dark`, each carrying the same ten primitives.
Each token is a `$type` + `$value` pair, and DTCG **aliases** — `"{group.token}"` —
let one token reference another:

```json
{
  "light": {
    "foreground":            { "$type": "color",      "$value": "#1f2937ff" },
    "accent":                { "$type": "color",      "$value": "#2563ebff" },
    "fontFamily":            { "$type": "fontFamily", "$value": null },
    "fontSize":              { "$type": "dimension",  "$value": 14.0 },
    "density":               { "$type": "number",     "$value": 1.0 },
    "cornerRadius":          { "$type": "dimension",  "$value": 4.0 },
    "contrastRequiredRatio": { "$type": "number",     "$value": 4.5 }
  },
  "dark": {
    "danger":                { "$type": "color",      "$value": "{light.danger}" }
  }
}
```

The `dark.danger` token above is a live alias: it reuses `light.danger` rather than
repeating the hex, and the generator resolves it deterministically (with cycle
detection) at generation time.

## The generated typed module: `DesignTokens.Light` / `Dark`

The generator emits `src/Controls/DesignTokens.fs`, a whole-file generated module
banner-marked `GENERATED — do not edit`. Its curated signature
`src/Controls/DesignTokens.fsi` is the **sole public-surface declaration** (the
generated `.fs` carries no access modifiers, per Constitution Principle II) and
exposes two sub-modules of compiler-checked values:

```fsharp
module DesignTokens =
    module Light =
        val foreground : Color
        val accent : Color
        val fontFamily : string option
        val fontSize : float
        val density : float
        val cornerRadius : float
        val contrastRequiredRatio : float
        // ... background, danger, muted
    module Dark = // ... the same ten primitives
```

The generated implementation lowers each alias-resolved value to the exact F#
literal the theme used before single-sourcing, so the migration had an empty diff:

```fsharp
module DesignTokens =
    module Light =
        let foreground : Color = Colors.rgba 31uy 41uy 55uy 255uy
        let fontSize : float = 14.0
        let fontFamily : string option = None
    module Dark =
        let danger : Color = Colors.rgba 185uy 28uy 28uy 255uy   // resolved from {light.danger}
```

The lowering rule is fixed and reproducible: a `color` `#rrggbbaa` becomes
`Colors.rgba R G B A` with decimal `uy` bytes; a `dimension` or `number` becomes a
`float` with a decimal point; a `fontFamily` of `null` becomes `None` and a concrete
name becomes `Some "<name>"`; an alias resolves to its target's concrete value. Note
that `dark.danger`'s alias has resolved to the same bytes as `light.danger` in the
generated output.

## How tokens reach a typed control

`Theme.fs` reads every migrated field from the generated module — there are zero
inline color/size literals left in it:

```fsharp
let light : Theme =
    { Name = "light"                                   // variant label is NOT a token
      Foreground = DesignTokens.Light.foreground
      Accent = DesignTokens.Light.accent
      FontSize = DesignTokens.Light.fontSize
      CornerRadius = DesignTokens.Light.cornerRadius
      ContrastRequiredRatio = DesignTokens.Light.contrastRequiredRatio
      /* ... */ }
```

From there the value flows to the rendered control. A typed `view`
([the typed front door](typed-front-door.html)) lowers to a `Control<'msg>`;
`Control.render theme control` (reached via `Widget.render`) consumes the `Theme`,
so the resolved token value — `DesignTokens.Light.accent`, say — is what colours the
control on screen. The chain is:

```
design-tokens.tokens.json  →  DesignTokens.fs (generated)  →  Theme.light/dark
                           →  Control.render theme widget  →  rendered control
```

One DTCG edit therefore propagates to both the token value and the theme
automatically. The `Name` field (`"light"`/`"dark"`) is the one theme value that is
*not* a token — it labels the variant and stays a code constant.

## The `DesignTokenDrift` gate

The single source stays authoritative because of a currency gate. `DesignTokenDrift`
fails whenever `DesignTokens.fs` is not a byte-identical regeneration of the DTCG
source — that is, whenever the generated module is hand-edited or left stale. The
generator (`build/Governance/DesignTokenGen.fsi`) is pure over in-memory text — its
`parse`/`renderValue`/`renderModule`/`splice`/`currency`/`isCurrent`/`currencyDrift`
functions do no I/O — and generation is byte-stable (fixed theme/token order, no
clock or environment input), so the gate and golden comparisons are meaningful.
There is no partial emit: a malformed document, an unresolvable or cyclic alias, or
a missing required token raises a loud generation failure that names the offending
token, never a half-written module. This mirrors the feature-066 catalog
single-source pattern and the wider
[single-source generation](../governance/single-source-generation.html) discipline:
one canonical source, a pure renderer, a drift gate, one regeneration entry point.

The gate routes with the `controls-public-surface` rule
(`build/Governance/Routing.fs`), so a change under `src/Controls/**` lists
`DesignTokenDrift` among its gates and `Route --enforce` blocks a stale generated
module.

## How to author a token

The flow is tokens-first and deliberately small:

1. **Edit the DTCG source only** — change a `$value` (or add an alias) in
   `src/Controls/design-tokens.tokens.json`. Never hand-edit `DesignTokens.fs`.
   When the value originates in Penpot, export the DTCG JSON and reconcile it into
   this file.
2. **Regenerate** with `./fake.sh build -t RefreshSurfaceBaselines`, which runs
   `RegenerateDesignTokens` → `DesignTokenGen.splice` and rewrites
   `DesignTokens.fs` whole-file.
3. **Verify currency** with `./fake.sh build -t DesignTokenDrift` (must pass).
4. **Verify no behaviour change** by running the Controls tests; a value-only edit
   changes the resolved `Theme.<field>` automatically because `Theme.fs` reads the
   generated values.
5. **Refresh surface baselines** if a *new* token name was added (a public-surface
   delta), and confirm the delta is additive-only.

Changing the `Theme` *type* (adding or removing a primitive) is a `Types.fsi`
contract change, not a token edit, and follows the public-surface rules instead.

## Where this sits in the Spec Kit process

The design-token workflow and custom FS Skia UI components are **created and
consumed at distinct Spec Kit phases**:

- **Tokens originate at the design source** (Penpot, or the checked-in DTCG file)
  and are best reconciled during **`speckit-specify`/`speckit-clarify`**, where the
  design system and theme intent are pinned into the spec. The Penpot MCP, where
  used at all, is assistive spec-drafting input here — never an authoritative
  generator.
- **Custom components and the typed control surface are created during
  `speckit-implement`**, driven by the `fs-skia-typed-controls` skill for control
  authoring and the `fs-skia-design-tokens` skill for the token edit. The DTCG edit,
  regeneration, and `DesignTokenDrift` verification all happen in this phase.
- **Currency and additivity are checked at the gating phases** — `Route` selects the
  `controls-public-surface` gate set, and `speckit-analyze`/the evidence audit
  confirm the generated module is current and the public-surface delta is
  additive-only before merge.

For the broader spec → clarify → plan → tasks → implement → analyze flow and how
custom components thread through it, see the
[Spec Kit process](../speckit/process.html) page.

## Analysis

### Implementation strengths

- The generator (`build/Governance/DesignTokenGen.fs`) is pure over in-memory text
  with all filesystem access pushed to the build edge, and emits byte-stable output
  with a fixed token order, which makes the `DesignTokenDrift` gate and golden
  comparisons genuinely meaningful rather than flaky.
- Generation is all-or-nothing: a malformed document, a cyclic or unresolvable
  alias, or a missing required token raises a loud failure that names the offending
  token instead of writing a partial module, so a broken source can never produce a
  silently-wrong theme.

### Implementation weaknesses

- The token set is fixed at exactly the ten existing `Theme` primitives across two
  variants — there is no spacing scale, no typography composite, no shadow or
  motion token — so the generator proves the pattern but covers only a thin slice of
  what a DTCG design system can express.
- The DTCG-to-F# mapping is hand-written to reproduce today's literals exactly
  (hex → `Colors.rgba … uy`, dimension → `float`), which is precise but brittle: a
  new DTCG `$type` the renderer does not yet handle is a generator change, not a
  data-only edit, so designers cannot extend the vocabulary without a code change.

### Design pros

- Choosing DTCG JSON as the single source makes the design system standards-based
  and version-controllable, and keeps the integration free of the live Penpot
  plugin/MCP token-API gaps — the pipeline runs from committed files and needs no
  design-tool access in CI.
- One edit point propagates to both the token value and the derived `Theme`
  automatically (`Theme.fs` reads `DesignTokens.Light`/`Dark`), and DTCG aliases let
  shared values like `dark.danger = {light.danger}` be expressed once, so the design
  intent is DRY and the generated F# stays in lock-step.

### Design cons

- Deferring live Penpot/MCP sync means the design-to-code link is a manual
  export/reconcile step today: the JSON is the source of truth, but nothing
  enforces that it actually matches the current Penpot file, so the two can drift
  silently between exports.
- Single-sourcing adds a generated artifact, a drift gate, and a regeneration step
  to every theme change, which is heavier than editing a constant directly; for the
  ten primitives shipped, the governance machinery is arguably out of proportion to
  the surface it guards — the value is in the *pattern* it establishes for future
  growth, not in the current token count.
