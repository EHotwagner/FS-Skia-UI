# Implementation Plan: Single-Source Generation of Duplicated Governance Artifacts (Stage 2.2–2.5)

**Branch**: `044-foundations-single-source-generation` | **Date**: 2026-06-01 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/044-foundations-single-source-generation/spec.md`

## Summary

Close the three remaining "two copies + a drift-check" duplication classes by replacing each
drift-check with a **generation-currency** check, mirroring the feature-042 precedent
(`validation.contract.yml` is *generated* from `Routing.fs`; a hand-edit fails with a
"regenerate from source" diagnostic):

1. **Skill trees (US1)** — `.agents/skills/` becomes the single canonical source; `.claude/skills/`
   is **generated** from it by enumeration, covering **all 25** skills (today `SkillSyncCheck`
   guards only 6, leaving 19 unguarded). `SkillSyncCheck` is reframed from a byte-identity *peer*
   check into a *currency* check; the redundant `SkillExamplesCheck` is **retired**.
2. **Skillist (US2)** — `tasks.deps.yml` `skillist:` is canonical; the `tasks.md`
   `[skillist: …]` annotation is the **derived view**. The feature-043 evidence-audit comparison
   (`Audit.fs`, `mirror <> sk`) is reframed from a peer drift-check into an active-feature-scoped
   currency check with an actionable "regenerate" diagnostic. No historical re-derivation.
3. **Constitution (US3)** — `.specify/memory/constitution.md` is the single source; templates carry
   generated principle-summary fragments spliced between explicit `BEGIN GENERATED`/`END GENERATED`
   markers (clarification 2026-06-01); a currency check fails on stale regions; genuine hand-written
   guidance outside the markers is preserved.

**Technical approach**: follow the established single-source mechanics exactly — pure
generation/comparison functions in the compiled `FS.Skia.UI.Build` governance library (each with a
curated `.fsi`), all filesystem I/O kept at the `build.fsx` interpreter edge (Principle IV). The
**single regeneration entry point is the existing `RefreshSurfaceBaselines` target** (which already
emits the generated `validation.contract.yml`); generated/derived artifacts stay **committed**, and
**currency is enforced at gate time** by reusing existing gates rather than proliferating targets:
- skills → `SkillSyncCheck` (reframed to currency);
- constitution fragments → folded into `TargetMetadataDrift` alongside the existing contract-currency
  check (same precedent, same home);
- skillist → reframed inside the active-feature evidence audit (`Audit.fs`).

This touches `.specify/**`, the skill trees, and governance paths, so `Route` **escalates** it to the
full serialized gate set. The runtime (`Scene → SkiaViewer → Elmish`) and every public product `.fsi`
are untouched.

## Technical Context

**Language/Version**: F# / .NET `net10.0` (inherits `Directory.Build.props`: `TreatWarningsAsErrors`,
`FS0078`-as-error, Central Package Management).
**Primary Dependencies**: none new. `YamlDotNet 17.1.0` (already central — reused for the
`tasks.deps.yml` skillist read via the existing `Evidence.DepsParser`); `Fake.Core.Target 6.1.4`
(build-tooling only). Explicitly **no** `FSharp.Compiler.*`, **no** new bespoke parser, **no**
symlink/POSIX-only tooling, **no** shelling to `diff`/`cmp`/`sha256sum` (in-process comparison only).
**Testing**: Expecto unit tests in `tests/Governance.Tests`, asserting **typed** results
(generation output, currency `string option` diagnostics, marker-splice preservation); existing
golden-diff harness (DiffPlex) where byte-level proof is wanted. FAKE-target evidence from the
serialized six-target escalated run.
**Target Platform**: Windows and Linux (governance text artifacts only; copy-generation is
byte-identical on every supported platform by construction — no platform-specific runtime).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-checked after Phase 1 design — both pass.*

This is a **Tier 1 (contracted change)** for the **governance-library surface only**: it adds new
public modules to `FS.Skia.UI.Build` (each with a curated `.fsi` per Principle II) and retires the
`SkillExamples` module. It introduces **no** product `.fsi` change, **no** product surface-baseline
diff, **no** new dependency, and **no** runtime change (Invariant 2). As a `.specify/**` + skill-tree
+ governance-path change, `Route` **escalates** it to the full serialized gate set.

### Repository Governance Decisions

- **Template ownership** — **Changes required, governance-only.** `.specify/templates/plan-template.md`
  and `.specify/templates/tasks-template.md` gain `BEGIN GENERATED`/`END GENERATED` marker regions
  carrying constitution-derived principle fragments (US3). No `.template.config/template.json`
  change, no generated-product (`template/base/**`) content change, no sample/command-surface change.
- **Dependency impact** — **No new package.** No `Directory.Packages.props` change, no new
  `PackageVersion`. `docs/reports/dependencies.md` unchanged (no new dependency identity).
  `DependencyReport` unaffected.
- **Command-surface impact** — `SkillSyncCheck` is **reframed** (byte-identity peer check →
  generation-currency check, all 25 skills); `SkillExamplesCheck` is **retired** (DU case, gate,
  `runSkillExamplesGate`, `SkillExamples.fs/.fsi`, and its tests removed). `RefreshSurfaceBaselines`
  gains skill-tree + constitution-fragment regeneration effects. `TargetMetadataDrift` gains the
  constitution-fragment currency check. `EvidenceGraph`/`EvidenceAudit` skillist handling reframed
  to currency. `validation.contract.yml` regenerates from `Routing.fs` (target-set change →
  `TargetMetadataDrift`/`ContractView` must stay coherent after the `SkillExamplesCheck` removal).
  FAKE-backed commands run **sequentially** in the canonical serialized order (Invariant 5); safe
  non-FAKE reads/tests may parallelize.
- **Generated project impact** — **none.** No default/minimal generated contents, selected Controls
  guidance, validation logs, or generated `Dev` behavior change. The skill trees and templates are
  repo-author governance assets, not generated-product content.
- **Evidence paths** (all under `specs/044-foundations-single-source-generation/readiness/`):
  - `logs/serialized-gates.md` — the escalated six-target run log (`Dev`, `GeneratedGuidanceCheck`,
    `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`).
  - `currency/skills-edit-without-regen.md` — SC-001/SC-008: edit a canonical skill (one of the 19
    previously-unguarded slugs) without regenerating → `SkillSyncCheck` fails naming
    `RefreshSurfaceBaselines`; regenerate → bit-identical across all 25 → passes.
  - `currency/new-skill-zero-allowlist.md` — SC-002: add a skill directory, regenerate, derived
    tree gains it with zero allowlist edits.
  - `currency/skillist-edit-without-regen.md` — SC-003/SC-008: edit canonical deps skillist →
    audit currency flags stale; regenerate → green; edit derived annotation alone → flagged stale.
  - `currency/constitution-edit-without-regen.md` — SC-005/SC-008: change a principle → fragment
    currency flags stale; regenerate → templates reflect change; hand-written prose preserved.
  - `currency/skillist-no-historical-regression.md` — SC-004: audit across existing feature
    directories yields zero new failures (active-feature scope proof).
  - `logs/byte-identity-25.md` — SC-001: in-process proof all 25 derived `SKILL.md` are
    byte-identical to canonical.
  - `logs/provenance-headers.md` — SC-006: every generated artifact carries provenance (per-file
    for templates/manifest; tree-level manifest for the byte-identical skill tree — see research R5).
  - `logs/duplication-delta.md` — SC-007: eliminated-line delta vs the Stage-0 baseline
    (`docs/reports/_baselines/2026-05-31-foundations.md`).
  - `logs/runtime-untouched.md` — SC-009: `git diff --stat` over product `src/**` = 0.
  - `logs/no-fcs-grep.txt`, `logs/no-shell-diff-grep.txt` — no `FSharp.Compiler.*`, no
    `diff`/`cmp`/`sha256sum`/symlink shelling in the generation path.
  - `unit-tests.md` — typed Governance.Tests results for the new generation/currency modules.
- **`.fsi` / contract impact** — new curated `.fsi` for each new governance module (`SkillTreeGen`,
  `SkillistView`, `ConstitutionFragments`); `SkillExamples.fsi` deleted. These are **build-tooling**
  surface, not product public contract. `PackageSurfaceCheck`/`FsiTranscripts` show **no product
  baseline diff** (Invariant 1).
- **MVU/effect boundary** — the only interpreter touched is `build.fsx`'s `BuildEffect` interpreter.
  Generation `WriteFile` effects fold into the `RefreshSurfaceBaselines` arm next to the existing
  `validation.contract.yml` write; currency checks call the pure library functions from the
  `SkillSyncCheck` / `TargetMetadataDrift` / evidence-audit arms. `update` stays pure (effect *data*
  only); all file reads/writes live in `interpret`. Pure-transition tests assert generation output
  and currency diagnostics on in-memory inputs (no repo-tree touch); interpreter behavior is
  exercised by the FAKE-target evidence.
- **Synthetic evidence** — **none planned.** All evidence is real (live gate runs showing
  edit-without-regenerate fails and post-regenerate passes, in-process byte-identity proof, typed
  test runs, grep proofs). The feature's own audit must return `verdict=PASS` with 0
  `[S]`/`[S*]`/late-seh/diff-scan.
- **Test evidence** — failing-first: each new currency function has a test that fails on a stale
  fixture and passes on a current one; the marker-splice has a test proving out-of-marker prose is
  byte-preserved; the skill generator has an enumeration test proving a synthetic 26th skill is
  covered with no allowlist edit. Re-point/retire the `SkillExamplesCheck` tests.
- **Observability** — every currency failure emits an **actionable** diagnostic naming the exact
  regeneration command (`./fake.sh build -t RefreshSurfaceBaselines`), distinct from a bare "A and B
  differ" (FR-012). Missing/empty/malformed canonical input fails the generator loudly rather than
  emitting a partial derived artifact (spec Edge Case; Principle VII).
- **Deferred scope** — Stage 5 (MEL-engine relocation / `build.fsx` retirement), Stage 6 (content
  trimming, contract `schema_version`, evidence-bloat hygiene), Stage 7. Symlink-based sharing is
  out. No further Python/Bash porting. No product/runtime/packaging/public-`.fsi` change.

**Gate result: PASS** (pre-design and post-design). No principle violation requires justification;
the only Tier-1 surface (new governance `.fsi` modules) is curated per Principle II. One spec
tension (FR-003 byte-identity vs FR-011 per-file provenance header) is resolved in research **R5**
(tree-level provenance manifest) without weakening either requirement.

## Project Structure

New/changed paths (repo-relative):

```
build/Governance/
  FS.Skia.UI.Build.fsproj            # +SkillTreeGen/SkillistView/ConstitutionFragments compile items; -SkillExamples
  SkillTreeGen.fsi   / .fs           # US1: pure enumerate-canonical → derived-tree plan + currency (FR-001..FR-004)
  SkillistView.fsi   / .fs           # US2: pure render [skillist: …] annotation from deps; currency (FR-005..FR-007)
  ConstitutionFragments.fsi / .fs    # US3: pure extract principle fragments + marker-splice + currency (FR-008..FR-010)
  SkillSync.fsi      / .fs           # reframed: currency diagnostic delegates to SkillTreeGen (FR-004)
  SkillExamples.fsi  / .fs           # DELETED (retired, FR-004)
  Targets.fsi        / .fs           # -SkillExamplesCheck DU case + name/prereq/dispatch references
build.fsx                            # RefreshSurfaceBaselines: +skill-tree/+constitution WriteFile effects;
                                     # SkillSyncCheck arm → currency; TargetMetadataDrift arm → +constitution currency;
                                     # -runSkillExamplesGate / -SkillExamplesGate effect
.claude/skills/                      # GENERATED (committed, bit-identical to .agents/skills); +tree-level provenance manifest
.claude/skills/<MANIFEST>            # provenance: source + regeneration command (R5)
.specify/templates/plan-template.md  # +BEGIN/END GENERATED constitution-fragment region(s)
.specify/templates/tasks-template.md # +BEGIN/END GENERATED constitution-fragment region(s)
validation.contract.yml             # regenerated (target-set changed by SkillExamplesCheck removal)
tests/Governance.Tests/
  SkillTreeGenTests.fs               # enumeration coverage (all 25 + a synthetic 26th), currency, byte-identity
  SkillistViewTests.fs               # render annotation, currency, prose-undisturbed
  ConstitutionFragmentsTests.fs      # fragment extraction, marker-splice preservation, currency
  (SkillExamplesTests removed)       # retired with the module
AGENTS.md                            # plan pointer updated to this plan
```

Design artifacts for this feature: [research.md](./research.md), [data-model.md](./data-model.md),
[contracts/](./contracts/), [quickstart.md](./quickstart.md).

## Complexity Tracking

No constitution deviations requiring justification. No custom operators, SRTP, reflection,
non-trivial computation expressions, type providers, or non-simple active patterns are introduced;
the generation/comparison logic is plain F# functions over lists and strings (Principle III).
```
