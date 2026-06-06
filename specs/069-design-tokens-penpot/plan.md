# Implementation Plan: Design Tokens + Penpot (DTCG → Generated F# + DesignTokenDrift)

**Branch**: `069-design-tokens-penpot` | **Date**: 2026-06-06 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/069-design-tokens-penpot/spec.md`

## Summary

Introduce a single-source design-token pipeline for the 10 `Theme` primitives, applying the
**exact** single-source-generation pattern feature `066` established for the controls catalog
(`build/Governance/CatalogGen.fs` → `ControlsCatalogGenerationCheck` → `RegenerateCatalog` in
`RefreshSurfaceBaselines`):

1. A canonical **DTCG-format JSON** document (`src/Controls/design-tokens.tokens.json`) becomes
   the single source of truth for the `light`/`dark` theme primitives. DTCG is the Penpot
   interchange format; live Penpot/MCP sync stays out of scope.
2. A **generated** F# token module ships in `FS.Skia.UI.Controls` — generated `DesignTokens.fs`
   behind a **curated** `DesignTokens.fsi` (Principle II). `Theme.light`/`Theme.dark` are
   re-expressed in terms of those generated tokens with **byte/value-identical** results
   (behavior-preserving).
3. A new **`DesignTokenDrift`** currency gate (mirroring `ControlsCatalogGenerationCheck`) fails
   the build when the generated F# is not a byte-identical regeneration of the DTCG source, and
   `RegenerateDesignTokens` is wired into `RefreshSurfaceBaselines` so the DTCG document is the
   one edit point.
4. A new **`fs-skia-design-tokens`** capability skill is authored in this branch (canonical
   `.agents/skills/`, `.claude` peer regenerated via `RefreshSurfaceBaselines`).

The generator/parser is compiled F# in `FS.Skia.UI.Build` (`build/Governance/DesignTokenGen.fs`),
as `CatalogGen` is — **no** new dependency enters the shipped `FS.Skia.UI.Controls` package. The
public-surface delta is additive-only (the new `DesignTokens` module surface); the `Theme` type
and module signatures are unchanged. Because it edits public `src/Controls/**/*.fsi` and adds
governance rules + a skill, `Route` escalates to the `controls-public-surface` gate set (now
including `DesignTokenDrift`) plus the governance/skill gates.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: Shipped package — **none new** (`FS.Skia.UI.Controls` references only
`Scene`, `Layout`, `KeyboardInput`; no `Fable.Elmish`, no JSON dependency). Build/governance —
`FS.Skia.UI.Build` parses the DTCG JSON in-process (existing `fsharp-parsing` capability; no new
governance dependency).
**Testing**: Expecto + FsCheck in the governance/controls test projects — failing-first contract
tests, per-field value-parity table, generation-determinism property, drift-detection and
malformed/cyclic-alias edge tests; FAKE-backed gates (`DesignTokenDrift`, the
`controls-public-surface` set) run sequentially.
**Target Platform**: Windows and Linux (build-time generation is pure text; no platform-specific
behavior).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Tier**: **Tier 1 (contracted change)** — adds public API surface (the generated `DesignTokens`
module under `FS.Skia.UI.Controls`) and a new build target/routing rule. Additive-only; no
behavior change. Requires the full artifact chain: spec, plan, `.fsi` updates, per-package surface
baseline regeneration, test evidence, documentation.

Principle alignment:
- **I (Spec→FSI→Tests→Impl)**: the `DesignTokens.fsi` token surface is sketched and FSI-exercised
  before the generated `.fs` body; `DesignTokenGen.fsi` is drafted before its `.fs`.
- **II (Visibility in `.fsi`)**: `DesignTokens.fsi` is **curated** and is the sole declaration of
  the generated module's public surface; the generated `.fs` carries no access modifiers.
- **III (Idiomatic simplicity)**: pure text generation + parsing; no SRTP/reflection/type
  providers/custom CE. No justification-requiring features anticipated.
- **IV (MVU boundary)**: **N/A — no stateful/I/O workflow.** Generation is a pure transform;
  the only I/O (read DTCG, write generated `.fs`) lives at the `Engine/Interpret.fs` edge, exactly
  as `regenerateCatalog` does. `update` purity is preserved because no new `update` is added.
- **V (Synthetic disclosure)**: the DTCG→`Color`/size lowering is **real** and value-parity-tested
  (US2). Target: **zero `[S]`**. The `[SEH]` narrow exception covers only malformed/cyclic-alias
  error-path tests whose real input is infeasible (those validate failure behavior).
- **VI (Test evidence)**: failing-first semantic tests precede implementation; parity table proves
  byte-identity; determinism is property-tested.
- **VII (Observability)**: `DesignTokenDrift` emits actionable diagnostics naming the stale/
  missing/invalid token, the generated file, and the regenerate command; malformed/cyclic DTCG
  fails loudly with the offending token, never a partial emit.

### Repository Governance Decisions

- **Template ownership**: **N/A for in-feature template edits — the template is value-driven and
  needs no change here.** The generated `DesignTokens` module ships inside the existing
  `FS.Skia.UI.Controls` package (no new packable project, no new `PackageId`), and the template
  pins `FS.Skia.UI.Controls` through the single `<FsSkiaUiVersion>` property. Pack/version-bump and
  template-pin refresh are **post-merge** concerns owned by `speckit-merge` and
  `fs-skia-template-update` (not this branch). `.template.config/template.json` is unchanged.
- **Dependency impact**: **No dependency change.** `FS.Skia.UI.Controls` adds **no** package
  reference (SC-007); `Directory.Packages.props`, `docs/dependencies.md`, and `DependencyReport`
  coverage are unchanged. The DTCG parser/generator lives in `FS.Skia.UI.Build`, which parses JSON
  in-process (no new governance package). A dependency-guard test asserts `Controls.fsproj` gains
  no reference (mirrors the `068` `Fable.Elmish`-free guard).
- **Command-surface impact**: **Yes — one new target + one wired step.** Add `DesignTokenDrift` to
  the `Target` enum / `allTargets` / name map / `directPrerequisites` / `failureOwner`
  (`build/Governance/Targets.fs` + `Targets.fsi`), mirroring `ControlsCatalogGenerationCheck`
  (`Targets.fsi:38`). Add `RegenerateDesignTokens` as a model effect
  (`Engine/Model.fs(i)` next to `RegenerateCatalog`) and splice it into `RefreshSurfaceBaselines`
  (`Engine/Update.fs:115` region). No semantic change to `Dev`, `Verify`, `Ci`, `PackLocal`,
  `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`,
  or `EvidenceAudit`. `validation.contract.yml` regenerates from `Routing.fs` (no hand-sync).
  FAKE-backed gates run **sequentially** in deterministic order; run only the gates `Route` prints.
- **Generated project impact**: **None to generated-project default contents.** No change to the
  minimal/default generated scaffold, selected Controls guidance, validation logs, placeholder or
  excluded-history scans, or generated `Dev` behavior. A generated product consuming
  `FS.Skia.UI.Controls` compiles unchanged and renders identically (US2). The new
  `fs-skia-design-tokens` skill is a maintainer/authoring capability skill, not generated-project
  content.
- **Evidence paths**: under `specs/069-design-tokens-penpot/readiness/`:
  - `design-tokens.md` — DTCG source design, token taxonomy, DTCG→F# mapping, tokens-first flow.
  - `design-token-drift.md` — `DesignTokenDrift` gate report (currency PASS + hand-edit/stale
    detection FAIL transcript).
  - `theme-token-parity.md` — the 10-field × 2-theme value-parity table (token-derived ≡
    pre-feature literal) and render-parity result.
  - `package-surface-expectations.md` — the additive `FS.Skia.UI.Controls` surface delta and
    regenerated per-package baseline rationale (`readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt`
    + `readiness/surface-baselines/FS.Skia.UI.Controls.txt`).
  - Generation/parsing FSI transcripts under `readiness/fsi/`; drift-fail transcript under
    `readiness/logs/`.
- **`.fsi` / contract impact**: **Yes, additive.** New curated `src/Controls/DesignTokens.fsi`
  declares the token surface; generated `src/Controls/DesignTokens.fs` implements it.
  `Types.fsi` (`Theme`), `Theme.fs` module signatures, and `Control.fsi` are **unchanged** in
  signature (SC-008). Both `FS.Skia.UI.Controls` per-package and aggregate surface baselines are
  regenerated; the delta is additive-only (new token names only). `DesignTokenGen.fsi` is
  build-tooling scope (not a tracked runtime surface baseline, same status as `CatalogGen.fsi`).
- **MVU/effect boundary**: **N/A — not a stateful/I/O workflow.** No `Model`/`Msg`/`update`
  product code is added. The only effect is the build-side `RegenerateDesignTokens` (write
  generated `.fs`) interpreted at `Engine/Interpret.fs`, mirroring `RegenerateCatalog`; the
  generation/currency functions in `DesignTokenGen` are pure.
- **Synthetic evidence**: target **zero `[S]`**. The DTCG→concrete-value lowering is real and
  value-parity-tested (US2 → FR-011). The only candidates for disclosure are the
  malformed-DTCG and cyclic-alias **error-path** tests; those are designed as `[SEH]`
  (`synthetic-error-handling-approved`) because their malformed input is infeasible to source
  for real and they validate explicit failure behavior — each carries the full Principle V
  disclosure and an Inventory row. No ordinary `[S]` mocks/fakes are planned.
- **Test evidence**: failing-first contract tests (assert `DesignTokens.fsi` declares the token
  surface; assert `DesignTokenGen` exists) committed red first. Governance tests in the build
  test project mirror `CatalogTests` Feature-066 block: byte-identity of rendered module vs.
  fixture, currency PASS on the committed tree, splice idempotency, drift FAIL on a hand-mutated
  generated file (names the token + regenerate command), Missing-region/whole-file reported loudly,
  determinism (regenerate twice ≡), alias resolution + cyclic/unresolvable failure. Controls tests
  add the 10×2 parity table and the dependency-guard. Render-parity re-renders the controls gallery.
- **Observability**: `DesignTokenDrift` writes a PASS/FAIL structured report to
  `readiness/design-token-drift.md`; FAIL `FailWith`s drift diagnostics naming the offending
  token(s), the generated file, and `./fake.sh build -t RefreshSurfaceBaselines`. Malformed/
  incomplete/cyclic DTCG fails generation loudly naming the offending token (never a partial emit).
  Missing generated file / missing DTCG source is reported as a loud failure class, not silent pass.
- **Deferred scope**: **Live Penpot/MCP integration** (inspect/draft/provenance, network sync,
  code↔design round-trip) is deferred to the later "Penpot MCP assist" roadmap item — DTCG is
  established here only as the interchange format. Migrating the remaining 41 controls (`070`),
  catalog expansion (`071+`), motion/animation tokens, runtime theme-switching UI, new
  color-science/contrast computation, and any shipped theme **value** change are all out of scope.

## Project Structure

```
specs/069-design-tokens-penpot/
├── spec.md
├── plan.md                      # this file
├── research.md                  # Phase 0 — decisions + rationale
├── data-model.md                # Phase 1 — DTCG model, token taxonomy, mapping
├── quickstart.md                # Phase 1 — edit-a-token → regenerate → gate walkthrough
├── contracts/
│   ├── design-tokens.fsi        # curated public token surface (sketch)
│   └── design-token-gen.fsi     # build-side generator surface (sketch)
├── checklists/                  # existing quality checklist
└── readiness/                   # evidence artifacts (produced during implement)

# Source — shipped package (FS.Skia.UI.Controls)
src/Controls/
├── design-tokens.tokens.json    # NEW — DTCG single source of truth (light + dark)
├── DesignTokens.fsi             # NEW — curated public token surface (Principle II)
├── DesignTokens.fs              # NEW — GENERATED token module (DesignTokenDrift-gated)
├── Types.fs / Types.fsi         # UNCHANGED signatures (Theme type)
├── Theme.fs                     # EDITED — light/dark re-expressed via DesignTokens (value-identical)
└── Controls.fsproj              # EDITED — <Compile> insert DesignTokens.fsi/.fs after Theme; no new dep

# Build / governance (FS.Skia.UI.Build) — no shipped-package impact
build/Governance/
├── DesignTokenGen.fsi / .fs     # NEW — parse DTCG → facts → render F# → currency/drift (mirrors CatalogGen)
├── Targets.fs / Targets.fsi     # EDITED — add DesignTokenDrift target (mirrors ControlsCatalogGenerationCheck:38)
├── Routing.fs                   # EDITED — add Targets.DesignTokenDrift to controls-public-surface gate list (~:138)
├── Engine/Model.fs / Model.fsi  # EDITED — add RegenerateDesignTokens effect (next to RegenerateCatalog)
├── Engine/Interpret.fs          # EDITED — dispatch RegenerateDesignTokens -> regenerateDesignTokens
├── Engine/Update.fs             # EDITED — RefreshSurfaceBaselines splice (~:115) + DesignTokenDrift arm (~:250)
└── Front/Governance.fs          # EDITED — regenerateDesignTokens function (mirrors regenerateCatalog:461)

# Tests
tests/                           # governance-gen tests (mirror CatalogTests 066 block) + Controls parity/guard tests

# Skill (canonical; .claude peer regenerated)
.agents/skills/fs-skia-design-tokens/SKILL.md   # NEW — DTCG→F# flow, DesignTokenDrift, tokens-first authoring

# Generated/derived (regenerated by RefreshSurfaceBaselines — never hand-edited)
validation.contract.yml                          # regenerated from Routing.fs
.claude/skills/fs-skia-design-tokens/**          # regenerated from .agents peer
readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt   # regenerated additive
readiness/surface-baselines/FS.Skia.UI.Controls.txt         # regenerated additive
```

## Phase 0 / Phase 1 outputs

- **research.md** — resolves the structural decisions (DTCG document shape, whole-file vs.
  marked-region generation, alias resolution, color/option mapping, routing extension vs. new rule,
  target naming) with rationale and alternatives.
- **data-model.md** — the DTCG token model, the `DesignTokenFact` shape, the token taxonomy, and
  the deterministic DTCG→`Theme`-field mapping table (all 10 fields × 2 themes).
- **contracts/** — sketched `DesignTokens.fsi` (curated public surface) and `DesignTokenGen.fsi`
  (build-side generator surface) for FSI validation before implementation.
- **quickstart.md** — the maintainer walkthrough: edit one DTCG value → `RefreshSurfaceBaselines`
  → generated module + `Theme` field update from one edit → `DesignTokenDrift` passes.
- **AGENTS.md** — `<!-- SPECKIT START -->` reference updated to this plan.
```
