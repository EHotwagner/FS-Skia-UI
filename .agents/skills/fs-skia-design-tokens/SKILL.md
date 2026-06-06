---
name: fs-skia-design-tokens
description: Edit the FS.Skia.UI.Controls theme primitives through the DTCG single source and regenerate the typed F# token module. Use when changing a theme color/size/density/radius/contrast value, adding or aliasing a design token, understanding the DesignTokenDrift gate, or authoring against the typed DesignTokens.Light/Dark surface.
compatibility: F# governance generator (build/Governance/DesignTokenGen.fs) under net10.0 + the shipped FS.Skia.UI.Controls package; build-tooling + curated-surface scope.
metadata:
  author: fs-skia-ui
  source: specs/069-design-tokens-penpot/plan.md
---

# fs-skia-design-tokens

The 10 `Theme` primitives in `FS.Skia.UI.Controls` are **single-sourced** from a DTCG
(Design Tokens Community Group) JSON document and generated into a typed F# module. This
skill owns the tokens-first edit flow, the `DesignTokenDrift` currency gate, and authoring
against the generated `DesignTokens.Light/Dark` surface. It mirrors the feature-066 catalog
single-source pattern ([[fsharp-code-generation]]): one canonical source, a pure renderer, a
drift gate, and one regeneration entry point.

## Scope / when to use

- Changing a shipped theme value — a color, font family/size, density, corner radius, or the
  contrast-required ratio for `light` or `dark`.
- Adding a token or expressing one token as a DTCG **alias** of another (`"{light.danger}"`).
- Diagnosing a `DesignTokenDrift` failure (a hand-edited or stale generated module).
- Authoring product code against the typed token surface (`DesignTokens.Light.accent`).

**Not** for: live Penpot/MCP sync (deferred), migrating the remaining 41 controls, motion
tokens, runtime theme-switching UI, or changing the `Theme` *type* (that is a `Types.fsi`
contract change, not a token edit).

## Driven surface / contract

- **Single source (the one edit point)**: `src/Controls/design-tokens.tokens.json` — DTCG
  groups `light` and `dark`, each with the 10 primitives.
- **Generated module**: `src/Controls/DesignTokens.fs` — GENERATED, do not hand-edit. Curated
  signature `src/Controls/DesignTokens.fsi` (Principle II — the `.fsi` is the sole public
  declaration; the `.fs` carries no access modifiers).
- **Generator (`.fsi` contract)**: `build/Governance/DesignTokenGen.fsi` —
  `parse` / `renderValue` / `renderModule` / `splice` / `currency` / `isCurrent` /
  `currencyDrift`. Pure over in-memory text; the only filesystem read/write lives at the
  `Engine/Interpret.fs` edge (Principle IV).
- **Gate**: `DesignTokenDrift` fails when `DesignTokens.fs` is not a byte-identical
  regeneration of the DTCG source. It routes with the `controls-public-surface` rule.
- **Regeneration entry point**: `./fake.sh build -t RefreshSurfaceBaselines` runs
  `RegenerateDesignTokens` → `DesignTokenGen.splice`.

## Workflow

1. **Edit the DTCG source only.** Change the `$value` in
   `src/Controls/design-tokens.tokens.json`. Never edit `DesignTokens.fs` by hand.
2. **Regenerate**: `./fake.sh build -t RefreshSurfaceBaselines`. This rewrites
   `src/Controls/DesignTokens.fs` whole-file from the DTCG source.
3. **Verify currency**: `./fake.sh build -t DesignTokenDrift` (must PASS).
4. **Verify parity / no behavior change**:
   `dotnet test tests/Controls.Tests/Controls.Tests.fsproj --filter "Feature 069"`.
5. **Refresh surface baselines** if the public surface changed (new token name):
   regenerate the per-package + aggregate baselines and confirm the delta is additive-only.

A value-only edit changes the resolved `Theme.<field>` automatically — `Theme.fs` reads
`DesignTokens.Light/Dark.*`, so one DTCG edit propagates to both the token and the theme.

## DTCG → F# mapping (the rule to reproduce)

The renderer lowers each alias-resolved value to the exact F# literal today's `Theme.fs` used,
so a value-preserving migration has an empty diff:

- `color` `#rrggbbaa` → `Colors.rgba R G B A` (decimal bytes with `uy` suffix).
- `dimension` / `number` → `float` with a decimal point (`14.0`, `4.5`).
- `fontFamily` `null` → `None`; a concrete name → `Some "<name>"`.
- alias `"{group.token}"` → resolved deterministically to the target's concrete value, with
  cycle detection ([[fsharp-graph-algorithms]]).

```fsharp
// build/Governance/DesignTokenGen.fs — render a hex color to the F# literal
let private renderColorHex (raw: string) =
    let hex = raw.TrimStart('#')
    let byteAt i = System.Convert.ToByte(hex.Substring(i, 2), 16)
    sprintf "Colors.rgba %duy %duy %duy %duy" (byteAt 0) (byteAt 2) (byteAt 4) (byteAt 6)
```

## Runnable example — exercise the generator

```fsharp
#load "build/Governance/DesignTokenGen.fs"
open FS.Skia.UI.Build
open System.IO

let json = File.ReadAllText "src/Controls/design-tokens.tokens.json"
let facts = DesignTokenGen.parse json            // 20 alias-resolved facts (10 x light/dark)
let moduleText = DesignTokenGen.splice json       // the whole generated DesignTokens.fs text

// currency on the committed tree:
let fs = File.ReadAllText "src/Controls/DesignTokens.fs"
let current = DesignTokenGen.currency json fs
printfn "isCurrent = %b" (DesignTokenGen.isCurrent current)   // true on a clean tree
DesignTokenGen.currencyDrift current |> List.iter (printfn "%s")
```

## Cautions

- **Determinism.** Generation is byte-stable (fixed theme/token order, no clock/env), so the
  drift gate and golden comparisons are meaningful.
- **No partial emit.** A malformed document, an unresolvable/cyclic alias, or a missing
  required token raises a loud generation failure naming the offending token — never a partial
  module.
- **No new shipped dependency.** The parser lives in `FS.Skia.UI.Build`; `FS.Skia.UI.Controls`
  adds **no** package reference (no JSON dependency, no `Fable.Elmish`).
- **`Name` is not a token.** The theme variant label (`"light"`/`"dark"`) stays a code
  constant in `Theme.fs`.

## Persistent problems

When a problem outlasts reasonable in-repo attempts, extensive external research is
**mandatory** — consult **official online docs first** (the DTCG specification and the
.NET `System.Text.Json` docs), then community sources (forums, issue trackers, changelogs).
Record findings and resolving links in `specs/<feature>/feedback/` and, for durable lessons,
in this skill's **Sources** line. Offline, the mandate degrades to recording
"research blocked — <why>" rather than hard-failing the phase.

## Sources / links

- DTCG format specification: <https://tr.designtokens.org/format/>
- Penpot (DTCG interchange): <https://penpot.app/>
- `System.Text.Json`: <https://learn.microsoft.com/dotnet/api/system.text.json.jsondocument>
- Plan + data model: `specs/069-design-tokens-penpot/plan.md`,
  `specs/069-design-tokens-penpot/data-model.md`.

## Related

[[fsharp-code-generation]] (the single-source generation pattern + currency check),
[[fsharp-parsing]] (DTCG JSON parsing), [[fsharp-graph-algorithms]] (alias cycle detection),
[[fsharp-build-orchestration]] (the `DesignTokenDrift` / `RefreshSurfaceBaselines` targets).
