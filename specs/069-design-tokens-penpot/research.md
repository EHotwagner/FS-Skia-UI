# Phase 0 Research: Design Tokens + Penpot (DTCG → Generated F#)

All decisions below resolve the spec's design space against the **load-bearing precedent**:
feature `066`'s single-source catalog generation (`build/Governance/CatalogGen.fs`,
`ControlsCatalogGenerationCheck`, `RegenerateCatalog` in `RefreshSurfaceBaselines`). Where `066`
already made a choice, this feature copies it rather than re-deciding.

## D1 — Where the DTCG single source and the generated module live

**Decision**: The DTCG document is `src/Controls/design-tokens.tokens.json`. The generated module
is `src/Controls/DesignTokens.fs` behind a curated `src/Controls/DesignTokens.fsi`. The
parser/generator is `build/Governance/DesignTokenGen.fs(i)`.

**Rationale**: `066` keeps the fact source (`catalog.yml`) and the generated artifact
(`Catalog.fs`) **inside the shipped package** at `src/Controls/`, and the generator in
`build/Governance/` (`CatalogGen.fs:34-35`: `catalogYmlRel = "src/Controls/catalog.yml"`,
`catalogFsRel = "src/Controls/Catalog.fs"`). Co-locating tokens with the package they theme keeps
the `controls-public-surface` routing rule (`src/Controls/**`) matching both the source and the
generated module with no glob change. `.tokens.json` is the DTCG community convention, so a Penpot
export/import recognizes the file by extension.

**Alternatives considered**: a top-level `design/` or `tokens/` directory (rejected — would need a
new routing glob and split the surface from its package); putting `DesignTokenGen` in a new build
assembly (rejected — `FS.Skia.UI.Build` already hosts `CatalogGen`/`SkillTreeGen`, and FR-007
mandates the generator stay out of the shipped package, which `build/Governance` satisfies).

## D2 — The single source is an external JSON document, not a compiled F# value

**Decision**: Unlike `066` (whose source `catalogFacts` is a compiled F# value), the `069` source
is the checked-in DTCG **JSON file**. `DesignTokenGen` therefore **parses** the JSON (at the
`Engine/Interpret.fs` edge), resolves aliases, and renders F#. This composes the `fsharp-parsing`
capability with `fsharp-code-generation`, exactly as the spec assumes.

**Rationale**: DTCG must be a real interchange file Penpot can read/write (FR-001), so it cannot be
an F# literal. This is closer to `SkillTreeGen`, which reads canonical `.agents/skills/**` files at
the interpreter edge and renders the derived tree. The parse stays pure over in-memory text; the
file read/write is the only I/O and lives at the edge (Principle IV preserved).

**Alternatives considered**: emit the DTCG JSON *from* an F# fact table (like `066` emits
`catalog.yml`) — rejected because then the F# value, not the DTCG document, would be the true
source, inverting the spec's "DTCG document is the single source of truth" and breaking the Penpot
round-trip premise.

## D3 — Whole-file generation for `DesignTokens.fs` (not marked regions)

**Decision**: `DesignTokens.fs` is **fully generated** as one file with a `GENERATED — do not edit`
banner (mirroring the `SkillTreeGen` manifest header). Currency = compare the on-disk file text to
`render (parse dtcg)`. `DesignTokenGen.currencyDrift` still computes a **per-token** comparison so
a stale/divergent value names the specific token (FR-004), even though the unit of regeneration is
the whole file.

**Rationale**: `066` uses marked regions because `catalog.yml`/`Catalog.fs` interleave 41
hand-authored rows with 6 generated ones — bytes outside the markers must be preserved. The token
module has **no** hand-authored content to protect (the curated surface lives in the separate
`.fsi`), so whole-file generation is simpler and removes the partial-regeneration edge case by
construction. Per-token drift diagnostics keep the actionable-naming behavior `066` provides.

**Alternatives considered**: marked `# BEGIN GENERATED: token/<id>` regions inside a partly
hand-authored `DesignTokens.fs` (rejected — no hand-authored content justifies the marker
machinery; whole-file is the honest model). Generating into `Theme.fs` directly (rejected — would
mix generated and hand-authored bytes in the consumer-facing theme file and complicate review).

## D4 — `Theme.light`/`dark` re-expressed via generated tokens, values byte-identical

**Decision**: `Theme.fs` keeps its hand-authored `Theme` *records* but sources each migrated field
from the generated `DesignTokens` module (e.g. `Foreground = DesignTokens.Light.foreground`)
instead of an inline `Colors.rgba ...` literal. The `Name` field stays a code constant
(`"light"`/`"dark"`). Generated values are byte-identical to today's literals (verified by the
10×2 parity table, SC-002).

**Rationale**: FR-003 requires `Theme.light`/`dark` to be token-derived with no inline literal for
the migrated primitives, while the observable values stay identical. Keeping the record *shape* in
`Theme.fs` (only swapping literals for token references) is the minimal, reviewable change and
leaves `Theme.fs`'s module signature unchanged (additive surface only — D7). The current literals
(`Theme.fs:9-26`) map directly: `light.Foreground = Colors.rgba 31 41 55 255` ⇄ DTCG
`#1f2937ff` (0x1f=31, 0x29=41, 0x37=55), etc.

## D5 — DTCG → repo `Color`/`option`/number mapping (deterministic, byte-identical)

**Decision**: deterministic mapping rules, all pure:
- **color** token (`$type: "color"`), hex `#rrggbb` or `#rrggbbaa` → `Colors.rgba R G B A`
  where each pair is parsed as a `byte`; a 6-digit hex implies `A = 255uy`. (`Color` is
  `{ Red: byte; Green: byte; Blue: byte; Alpha: byte }`, `Scene.fsi:9`; `Colors.rgba`,
  `Scene.fsi:367`.)
- **dimension/number** token (`FontSize`, `Density`, `CornerRadius`, `ContrastRequiredRatio`) →
  an F# `float` literal rendered with an explicit decimal point (e.g. `14.0`) so it is
  byte-identical to the current literal.
- **fontFamily** token → `string option`. The current `Theme.FontFamily` is `None`; the DTCG
  token encodes the absence explicitly (a `null`/empty `$value`, or a documented sentinel) and the
  generator emits `None`; a concrete family emits `Some "<name>"`.

**Rationale**: byte-identity (FR-003, SC-002) requires the *rendered F#* and the *resolved value*
to match today exactly. Fixing the float rendering (always a decimal point) and the 6→8 digit
alpha rule makes the transform total and reversible. Mapping is unit-tested per field.

**Alternatives considered**: storing colors as decimal RGBA arrays in the DTCG (rejected — hex is
the DTCG color convention Penpot emits); emitting `Color` via a constructor other than
`Colors.rgba` (rejected — `Theme.fs` uses `Colors.rgba` today, so reusing it guarantees identity).

## D6 — DTCG alias/reference resolution; malformed/cyclic input fails loudly

**Decision**: `DesignTokenGen` resolves DTCG aliases (`"{group.token}"`) to concrete values during
parse, deterministically (e.g. a topological resolution; reuse the `fsharp-graph-algorithms`
cycle-detection pattern). An unresolvable reference, a cycle, a malformed document, or a token the
`Theme` mapping requires but is missing → **generation failure** naming the offending token, with
**no** F# emitted (never a partial module).

**Rationale**: directly required by the spec Edge Cases and FR-006. Failing before any write keeps
the generated file either fully current or visibly stale — never half-written. Cycle detection is a
solved pattern in `fsharp-graph-algorithms`; reusing it avoids bespoke logic.

**Synthetic note**: the malformed/cyclic-input tests use infeasible-to-source bad input and
validate an explicit error path → classified `[SEH]` (`synthetic-error-handling-approved`) per
Principle V, each with a full disclosure + Inventory row. The happy-path lowering is **real**
(no `[S]`).

## D7 — Curated `.fsi`, generated `.fs`; surface delta additive-only

**Decision**: `DesignTokens.fsi` is hand-authored (the sole public-surface declaration,
Principle II); `DesignTokens.fs` is generated and carries no access modifiers. Token **values**
changing never changes the `.fsi`; only adding/removing a token name does, and this feature only
**adds** the module. `Theme`'s type and `Theme` module signatures are untouched.

**Rationale**: Principle II forbids visibility in `.fs`; the generated body must therefore sit
behind a curated signature. This also makes `PackageSurfaceCheck`/`PerPackageSurfaceDiff` see an
**additive-only** delta (SC-008): the new `DesignTokens` surface, nothing removed or changed. Both
the per-package (`readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt`) and aggregate
(`readiness/surface-baselines/FS.Skia.UI.Controls.txt`) baselines are regenerated and reviewed.

## D8 — Routing: extend `controls-public-surface` rather than add a new rule

**Decision**: add `Targets.DesignTokenDrift` to the existing `controls-public-surface` rule's gate
list (`Routing.fs:130-147`), exactly as `066` added `Targets.ControlsCatalogGenerationCheck` there
(`Routing.fs:138`). No new rule, no glob change — the DTCG document and generated module both live
under `src/Controls/**` which the rule already matches.

**Rationale**: FR-009 explicitly permits "a new routing rule **or** extension of
`controls-public-surface`". The `066` precedent is the extension form, and it is the minimal change
that single-sources cleanly: `validation.contract.yml` regenerates from `Routing.fs`
(`Update.fs:101`, `ContractView.render Routing.rules ...`) with no hand-sync (SC-009).

**Alternatives considered**: a standalone `design-tokens` rule with its own glob (rejected — the
paths are identical to `controls-public-surface`, so a second rule would double-match and add
contract noise for no benefit).

## D9 — Target name `DesignTokenDrift`; wiring mirrors `ControlsCatalogGenerationCheck`

**Decision**: the new target is named **`DesignTokenDrift`** (the spec's name in FR-004/SC-005/
SC-010). It is added to the `Target` enum, `allTargets`, the name map, `directPrerequisites`
(`[]`), and `failureOwner` (`"product"`) in `Targets.fs`/`Targets.fsi`, mirroring
`ControlsCatalogGenerationCheck` (`Targets.fsi:38`). `RegenerateDesignTokens` is added as a model
effect (`Engine/Model.fs(i)` next to `RegenerateCatalog`, `Model.fsi:110`), dispatched in
`Engine/Interpret.fs:60` to a new `regenerateDesignTokens` (`Front/Governance.fs`, mirroring
`regenerateCatalog:461`), and spliced into `RefreshSurfaceBaselines` right after `RegenerateCatalog`
(`Update.fs:115`). The `DesignTokenDrift` target arm mirrors the
`ControlsCatalogGenerationCheck` arm (`Update.fs:250`): read DTCG + generated `.fs` at the edge,
`DesignTokenGen.currency`, `currencyDrift`, write the PASS/FAIL report, `FailWith` on drift.

**Rationale**: matching the proven `066` shape makes the target a mechanical copy, keeps a mistyped
gate a compile error (the selector is compiled F#), and keeps `TargetMetadataDrift`/
`validation.contract.yml` currency automatic.

## D10 — The `fs-skia-design-tokens` skill lands in this branch

**Decision**: author canonical `.agents/skills/fs-skia-design-tokens/SKILL.md` (frontmatter
`name: fs-skia-design-tokens`, a one-line `description`, `compatibility`, `metadata.source`),
teaching the DTCG→generated-F# flow, the `DesignTokenDrift` gate, and the tokens-first authoring
flow. Regenerate the `.claude` peer via `RefreshSurfaceBaselines` (never hand-edit `.claude`).

**Rationale**: FR-010 + plan §16.4 ("each new skill should land in the same feature branch that
first needs it"). `SkillRegistry` discovers skills dynamically from frontmatter `name:` (no
hardcoded list); `SkillSyncCheck` enforces `.agents`→`.claude` currency and `SkillQualityCheck`
enforces frontmatter quality. The skill edit routes to the focused-authority skill gate set, which
`Route` will print alongside `controls-public-surface`.

## Resolved unknowns

| Spec unknown | Resolution |
| --- | --- |
| DTCG document location/shape | `src/Controls/design-tokens.tokens.json`, light + dark groups (D1, data-model §1) |
| Source is JSON file vs F# value | JSON file, parsed at edge (D2) |
| Generated-module file strategy | whole-file generation + banner; per-token drift naming (D3) |
| How `Theme` re-expresses tokens | record shape kept; literals → `DesignTokens.*` references (D4) |
| Color/number/option mapping | hex→`Colors.rgba`; float w/ decimal; `None`/`Some` (D5) |
| Alias/cyclic/malformed handling | deterministic resolve; loud fail, no partial emit; `[SEH]` tests (D6) |
| Surface placement / additivity | curated `.fsi` + generated `.fs`; additive-only baseline delta (D7) |
| Routing rule vs extension | extend `controls-public-surface` (D8) |
| Target name + wiring | `DesignTokenDrift`, mirrors `ControlsCatalogGenerationCheck` (D9) |
| Skill ownership/sync | `.agents` canonical, `.claude` regenerated; SkillSync/Quality gated (D10) |

No `NEEDS CLARIFICATION` remain.
