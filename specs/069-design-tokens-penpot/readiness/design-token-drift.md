# DesignTokenDrift gate report (feature 069)

- **Authoritative command**: `./fake.sh build -t DesignTokenDrift`
- **Generated artifact under test**: `src/Controls/DesignTokens.fs`
- **Single source**: `src/Controls/design-tokens.tokens.json`
- **Regenerate**: `./fake.sh build -t RefreshSurfaceBaselines`
- **Failure class**: `stale-generated-design-tokens`

## Currency PASS (committed tree)

`./fake.sh build -t DesignTokenDrift` → `Status: Ok`. Report
(`readiness/design-token-generation.md`):

```
# Design Token Generation

PASS: the 20 generated tokens in src/Controls/DesignTokens.fs are a current, byte-identical regeneration of the DTCG source src/Controls/design-tokens.tokens.json.

- generated-tokens: 20 (10 primitives x light/dark)
- generated-file: src/Controls/DesignTokens.fs
- single-source: src/Controls/design-tokens.tokens.json
- regenerate: ./fake.sh build -t RefreshSurfaceBaselines
- failure-class: stale-generated-design-tokens
```

## Drift FAIL (hand-mutated generated file)

Hand-editing `light.foreground` in the GENERATED `src/Controls/DesignTokens.fs` and
re-running the gate fails loudly, naming the divergent token, theme, and the
regenerate command (full transcript: `logs/design-token-drift-fail.txt`):

```
Status: Failure
src/Controls/DesignTokens.fs is stale — its generated token 'foreground' (light theme)
no longer matches the DTCG source src/Controls/design-tokens.tokens.json.
Regenerate via ./fake.sh build -t RefreshSurfaceBaselines.
```

Restoring the file (or running `RefreshSurfaceBaselines`) returns the gate to PASS.

## SC-004 — single-edit value propagation

Full transcript: `logs/sc004-value-edit-propagation.txt`. Editing **one** DTCG value
(`light.accent` `#2563ebff` → `#123456ff`) and regenerating propagated the change to
**both** the generated `DesignTokens.Light.accent` **and** the resolved
`Theme.light.Accent` — with **no** manual edit to the generated module:

```
DesignTokens.Light.accent = { Red = 18uy; Green = 52uy; Blue = 86uy; Alpha = 255uy }
Theme.light.Accent        = { Red = 18uy; Green = 52uy; Blue = 86uy; Alpha = 255uy }
propagated (token = theme field): true
```

The edit was reverted and the tree confirmed clean (`isCurrent = true`, empty
`git diff`). The regeneration edge exercised (`DesignTokenGen.splice`) is the exact
pure function `RegenerateDesignTokens` runs inside `RefreshSurfaceBaselines` (wired
in `Engine/Update.fs` and verified by the PASS above).
