(**
---
title: The design-token flow
category: Examples
categoryindex: 7
index: 2
---

# The design-token flow

This literate script is **evaluated at documentation-build time** (`fsdocs build
--eval`), so it is compiler-verified against the real `FS.Skia.UI.Controls`
design-token surface.

Design tokens have a **single source of truth**: the DTCG file
`src/Controls/design-tokens.tokens.json` (authored from the Penpot / DTCG design
source). From it, a generated F# module `DesignTokens` (with `Light` and `Dark`
sub-modules) is produced, and the `DesignTokenDrift` gate keeps the generated F#
in lock-step with the JSON. Themes are built from those typed primitives, and the
control suite reads the theme at render time. This script walks that flow from the
typed token values outward — no design tooling or GPU required.
*)

#I "../../src/Controls/bin/Debug/net10.0"
#r "FS.Skia.UI.Scene.dll"
#r "FS.Skia.UI.Layout.dll"
#r "FS.Skia.UI.Controls.dll"

open FS.Skia.UI.Controls

(**
## Typed token primitives

`DesignTokens.Light` / `DesignTokens.Dark` are compiler-checked values generated
from the DTCG source. Because they are real F# values, a typo or a removed token
is a *compile* error, and references to them are greppable.
*)

printfn "light foreground : %A" DesignTokens.Light.foreground
printfn "light background : %A" DesignTokens.Light.background
printfn "light accent     : %A" DesignTokens.Light.accent
printfn "dark  accent     : %A" DesignTokens.Dark.accent

(**
Some dark-theme tokens are **aliases** of their light counterparts in the DTCG
source (e.g. `dark.danger` references `light.danger`); the generator resolves the
alias so both evaluate to the same value:
*)

printfn "light danger     : %A" DesignTokens.Light.danger
printfn "dark  danger     : %A" DesignTokens.Dark.danger
printfn "danger aliased?  : %b" (DesignTokens.Light.danger = DesignTokens.Dark.danger)

(**
Non-colour primitives (sizing, density, contrast) flow through the same source:
*)

printfn "font size        : %.1f" DesignTokens.Light.fontSize
printfn "density          : %.1f" DesignTokens.Light.density
printfn "corner radius    : %.1f" DesignTokens.Light.cornerRadius
printfn "min contrast     : %.2f" DesignTokens.Light.contrastRequiredRatio

(**
## From tokens to a theme

`Theme.light` / `Theme.dark` are assembled from these primitives, and the
`Theme.with*` combinators let an app override individual tokens while keeping the
rest. Here we take the light theme and swap in the dark accent token — exactly how
a consumer applies a brand colour sourced from the design system:
*)

let brandTheme = Theme.light |> Theme.withAccent DesignTokens.Dark.accent

(**
The control suite reads the resolved `Theme` at render time, so every control
picks up the token values without per-control wiring. To change the palette,
edit the DTCG JSON and regenerate (`./fake.sh build -t RefreshSurfaceBaselines`);
`DesignTokenDrift` fails the build if the generated F# and the JSON disagree.

See the [design-token / Penpot deep dive](../controls-design/design-tokens-penpot.html),
the [typed front door](../controls-design/typed-front-door.html), and the
[API reference](../reference/index.html).
*)
