# Design Tokens — DTCG → generated F# (feature 069)

## Feature classification (T002)

- **Tier**: Tier 1 (contracted change). Adds public API surface (the generated
  `DesignTokens` module under `FS.Skia.UI.Controls`) and a new build target +
  routing rule; additive-only, no behavior change.
- **Affected layer**: `FS.Skia.UI.Controls` (additive public surface — new
  `DesignTokens` module) + `FS.Skia.UI.Build` (the DTCG parser/generator
  `build/Governance/DesignTokenGen.fs`, plus the `DesignTokenDrift` gate and the
  `RegenerateDesignTokens` regeneration edge). No shipped-package dependency change.
- **Public-API impact**: additive `DesignTokens.Light.*` / `DesignTokens.Dark.*`
  only. The `Theme` type and the `Theme` module signatures are unchanged (SC-008).
- **MVU applicability**: **N/A** — pure build transform. The only effect is the
  build-side `RegenerateDesignTokens` interpreted at `Engine/Interpret.fs`,
  mirroring `RegenerateCatalog`; no product `Model`/`Msg`/`update` is added.

## The tokens-first flow

1. `src/Controls/design-tokens.tokens.json` (DTCG) is the single source of truth
   for the 10 `Theme` primitives in both `light` and `dark`.
2. `build/Governance/DesignTokenGen.fs` parses the DTCG document (in-process,
   `System.Text.Json`), resolves aliases deterministically, and renders the
   generated module text.
3. `RefreshSurfaceBaselines` runs `RegenerateDesignTokens`, writing
   `src/Controls/DesignTokens.fs` (whole-file) from the DTCG source.
4. `DesignTokens.fs` is curated behind `DesignTokens.fsi` (Principle II). `Theme.fs`
   re-expresses `light`/`dark` in terms of `DesignTokens.*` — value-identical.
5. `DesignTokenDrift` fails the build if `DesignTokens.fs` is not a current,
   byte-identical regeneration of the DTCG source.

## DTCG → F# mapping

See `data-model.md` §4 for the 20-cell mapping table (10 fields × 2 themes), the
parity oracle for SC-002.

## Evidence obligations

| Artifact | Authoritative command | Failure class |
| --- | --- | --- |
| `design-token-drift.md` | `./fake.sh build -t DesignTokenDrift` | stale-generated-design-tokens |
| `theme-token-parity.md` | `dotnet test tests/Controls.Tests` (parity tests) | theme-value-drift |
| `package-surface-expectations.md` | `./fake.sh build -t PackageSurfaceCheck` | non-additive-surface |
| `fsi/design-tokens-surface.txt` | `dotnet fsi` against the package surface | surface-unreachable |
| `logs/design-token-drift-fail.txt` | `./fake.sh build -t DesignTokenDrift` (hand-mutated) | stale-generated-design-tokens |

## Next action

Author the DTCG source (`src/Controls/design-tokens.tokens.json`), draft the
curated `DesignTokens.fsi` and the generator surface `DesignTokenGen.fsi`, then
implement `DesignTokenGen.fs` and wire `DesignTokenDrift`.
