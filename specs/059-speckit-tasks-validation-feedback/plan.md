# Implementation Plan: Trustworthy `/speckit.tasks` Validation Experience

**Branch**: `059-speckit-tasks-validation-feedback` | **Date**: 2026-06-03 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/059-speckit-tasks-validation-feedback/spec.md`

## Summary

A field report (`docs/reports/2026-06-03-2128-speckit-tasks-governance-process-analysis.md`)
catalogued eight defects in the **task-validation experience** a generated
consumer project inherits. Three can yield a wrong result silently or a
guaranteed wasted iteration. This feature makes that experience honest and
self-consistent by fixing the **consumer-facing sources** in this repo so a
freshly generated project inherits correct behaviour:

1. **No false green (P1).** The template's `build.fsx` stops generating and
   defaulting to a runtime sample feature; it resolves the target feature from
   `.specify/feature.json` (the same source the framework engine
   `build/Governance/Engine/Model.fs` already uses), allows an env-var override,
   and fails loud when neither resolves. The runtime sample
   (`generated-evidence-workflow`) is removed (FR-002, FR-003, FR-004, FR-014).
2. **The documented command works and is non-contradictory (P2).** The bundled
   `speckit-tasks` skill's "Validation" section drops the non-existent
   `run-audit.sh` runner and defers to `speckit-evidence-graph`'s canonical
   `./fake.sh build -t EvidenceGraph`, documenting the override env var
   (FR-001, FR-005, FR-011).
3. **Deps file passes first try (P2).** The deps template and skill guidance
   document and exemplify the required `schema_version` + top-level `tasks:`
   wrapper (already present in the template but absent from the prose) and the
   new per-task `owns:` field; the parser emits a directive wrapper error
   (FR-006, FR-007).
4. **Hints resolve (P3).** Every skill id in the bundled hint tables is
   reconciled to a skill a consumer actually registers, and a governance check
   keeps them resolvable (FR-008). The unresolvable `fs-skia-layout` hint and the
   template example's `skillist: ["fs-skia-layout"]` are corrected.
5. **Honest, low-friction assessment (P3).** The title-trigger capability
   matcher in `build/Governance/Evidence/Audit.fs` is removed; evidence
   ownership moves to an explicit `owns:` field in `tasks.deps.yml`; the docs
   describe the trust-as-declared behaviour honestly; the overloaded
   `fs-skia-layout-evidence` skill is split into two registered skills
   (FR-009, FR-010, FR-012).

All edits land on canonical sources so generated `.claude` peers and generated
consumer projects inherit them (FR-013).

## Technical Context

**Language/Version**: F# / .NET `net10.0` (compiled governance front-end
`FS.Skia.UI.Build`); consumer template assets are YAML / Markdown / `build.fsx`.
**Primary Dependencies**: YamlDotNet (deps parsing), Expecto + FsCheck
(Governance.Tests), FAKE front-end. No new packages.
**Testing**: `tests/Governance.Tests` (Expecto) for engine behaviour
(`DepsParser`, `Audit`, `SkillRegistry`, routing/known-gates); FAKE governance
gates (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
`EvidenceGraph`, `EvidenceAudit`); a generated-consumer demonstration for SC-001.
**Target Platform**: Windows and Linux (consumer template + Linux dev host).

### Key facts established during research

- The template **is** the repo: `.template.config/template.json` copies
  `template/base/ → ./`, `.specify/ → .specify/` (excluding `feature.json`,
  `memory/constitution.md`, and `extensions/evidence/scripts/**`), and
  `.agents/skills/ →` both `.agents/skills/` and `.claude/skills/`, plus
  conditional `template/product-skills/*` per profile.
- The "sample feature" is **not** a committed directory; it is synthesised at
  run time by `ensureGeneratedEvidencePackage()` in `template/base/build.fsx`
  (~L64–141) and selected via `SPECKIT_FEATURE_DIR` / `GENERATED_EVIDENCE_FEATURE_DIR`
  with a `specs/generated-evidence-workflow` fallback. FR-014 removes this
  synthesiser + fallback.
- The framework engine already resolves correctly:
  `Engine/Model.fs activeFeatureId` reads `.specify/feature.json`'s
  `"feature_directory"` and **fails loud** with no fallback — the exact pattern
  FR-002/FR-003 want the template's `build.fsx` to adopt.
- `.specify/extensions/evidence/scripts/**` is excluded from the consumer, which
  is why the documented `run-audit.sh` does not exist there (FR-001).
- The title-trigger matcher is `capabilityTriggerGroups` /
  `expectedCapabilityMatches` in `build/Governance/Evidence/Audit.fs` (L38–95),
  consumed at L192–223 to emit the "high-confidence skill match omitted" blocking
  error and the `SkillAssessment` rows. `DepsParser.DepsEntry` (L7–15) has
  `Deps`, `Skillist`, `LegacyBareList` and **no** `owns` field yet.
- In the `app` profile a consumer registers all 25 canonical `.agents` skills
  plus `fs-skia-scene/-skiaviewer/-elmish/-keyboard-input/-ui-widgets`. There is
  **no** `fs-skia-layout` product-skill in any profile, so the "layout →
  `fs-skia-layout`" hint (and the deps template example) is unresolvable — only
  `fs-skia-layout-evidence` ships. This is the concrete FR-008 defect to fix.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change classification**: **Tier 1 (contracted change)**. No application
public `.fsi` changes, but the **consumer authoring contract** changes: the
`tasks.deps.yml` schema gains `owns:`, a skill is split (new registered ids),
the runtime sample is removed, and hint tables change. Surface/skill-registry
baselines and the deps-schema contract are affected.

### Repository Governance Decisions

- **Template ownership**: Changes touch `template/base/build.fsx`,
  `.specify/presets/.../tasks-deps-template.yml`, `.specify/templates/**`,
  the bundled `.agents/skills/{speckit-tasks,speckit-evidence-graph}` skills,
  and the `fs-skia-layout-evidence` split (new `.agents/skills/<new-skill>`
  dirs). `.template.config/template.json` **must** gain `sources` entries that
  copy the two split product/governance skills to `.agents/skills/` and
  `.claude/skills/` so consumers register them. No `sourceName`/symbol changes.
- **Dependency impact**: None. No `Directory.Packages.props`,
  `docs/dependencies.md`, or `DependencyReport` changes — no new packages and no
  template package-pin changes (routine merge bumps are out of scope per spec).
- **Command-surface impact**: No new FAKE targets unless the
  `fs-skia-layout-evidence` split introduces new skill gates — see below. If the
  split adds a registered skill that participates in any routing
  `RequiredGates`, then `build/Governance/Targets.fs` (union + `name` +
  registries), `build/Governance/Routing.fs`, and the
  `AgentValidation.knownGates` allowlist (L361–380) must all be updated in lock
  step, and `validation.contract.yml` regenerated via
  `RefreshSurfaceBaselines` (currency enforced by `TargetMetadataDrift`).
  **Decision**: the skill split adds **skill registry entries only**, not new
  build gates (skills are validated by the existing `SkillSyncCheck` /
  `SkillQualityCheck` gates that already match `.agents/skills/**`), so no
  `Targets`/`knownGates` change is expected — confirmed in Phase 0. FAKE-backed
  gates run sequentially in the deterministic order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
  The authoritative list comes from `./fake.sh build -t Route` for the real diff.
- **Generated project impact**: Material. The generated project no longer
  synthesises `specs/generated-evidence-workflow` (FR-014); its `build.fsx`
  evidence target resolves from `.specify/feature.json` and fails loud when
  unresolved (FR-002/FR-003); it inherits the split skills and corrected hint
  tables. `build/Governance/GeneratedProduct.fs` expectations that currently
  assert the generated project **ships** `specs/generated-evidence-workflow`
  (comment ~L970) MUST be updated to assert its **absence** and the new loud-fail
  behaviour, or `GeneratedProductCheck` will fail.
- **Evidence paths**: `readiness/task-graph.json` + `readiness/task-graph.md`
  (EvidenceGraph output for this feature, resolved from `.specify/feature.json`
  → `specs/059-speckit-tasks-validation-feedback`); `readiness/` audit artifacts
  named by `./fake.sh build -t Route --enforce`; a captured generated-consumer
  transcript demonstrating SC-001/SC-002 (correct feature dir + task count
  echoed, and loud fail when absent) stored under this feature's `readiness/`.
- **`.fsi` / contract impact**: `build/Governance/Evidence/DepsParser.fsi`
  (`DepsEntry` gains `Owns: string list option`) and
  `build/Governance/Evidence/Audit.fsi` (remove `expectedCapabilityMatches`;
  adjust `owns`-driven validation surface). The deps-file schema doc becomes a
  versioned consumer contract under `contracts/`. No application-library `.fsi`
  changes. Skill-registry / surface baselines regenerate via
  `RefreshSurfaceBaselines`.
- **MVU/effect boundary**: N/A — this is governance-tool and documentation work,
  not application runtime/state/I/O. The engine reads files at the `build.fsx`
  interpreter edge (already the established boundary); the change is pure parse +
  validate logic plus the template `build.fsx` resolver. No `Model`/`Msg`/
  `Cmd<Msg>` surface is introduced.
- **Synthetic evidence**: None planned. Real evidence is the corrected guidance
  plus a real generated-consumer validation run (real `dotnet new`, real
  `.specify/feature.json`, real engine). If a generated-consumer demonstration
  cannot run in-sandbox it is captured as a real transcript, not a mock; any
  unavoidable fixture is disclosed `[S]` per Principle V. No `[S]` expected.
- **Test evidence**: Failing-first Expecto tests in `tests/Governance.Tests`:
  (a) `DepsParser` parses `owns:` and rejects the bare-key shape with a directive
  wrapper message (FR-006/FR-007); (b) `Audit` no longer blocks on title triggers
  and derives ownership from `owns:` (FR-009/FR-010); (c) a hint-table resolution
  test asserts every hint id resolves to exactly one consumer-registered skill
  (FR-008); (d) skill-split registration + `SkillSyncCheck` currency
  (FR-012/FR-013). Plus the FAKE gate suite and the generated-consumer
  demonstration.
- **Observability**: The deps wrapper error must name the missing `tasks:`
  mapping directly (FR-007); the template `build.fsx` unresolved-feature failure
  must be non-zero with an actionable message naming `.specify/feature.json` and
  the override env var (FR-003); the evidence run must echo resolved feature dir
  + task count (FR-004). Silent fallback is forbidden (Principle VII).
- **Deferred scope**: No real per-task capability extractor (FR-009 requires
  honest docs only); no redesign of synthetic-propagation/audit semantics; no
  visual/release/distribution work; routine maintainer-merge version bumps are
  handled by the merge process, not here.

**Gate result**: PASS — no unjustified violations. Tier 1 obligations (`.fsi`
updates, baseline regeneration, test evidence, contract doc) are enumerated
above and scheduled in Phase 1.

## Project Structure

```
specs/059-speckit-tasks-validation-feedback/
├── spec.md
├── plan.md                # this file
├── research.md            # Phase 0 — resolved unknowns
├── data-model.md          # Phase 1 — DepsEntry/owns, skill-registry entities
├── quickstart.md          # Phase 1 — author validation walkthrough
├── contracts/
│   ├── tasks-deps-schema.md   # versioned deps-file contract (tasks: + owns:)
│   └── skill-hint-resolution.md # hint-id → consumer-registered-skill contract
├── checklists/
└── readiness/             # EvidenceGraph/Audit + consumer demo artifacts

Consumer-facing sources changed (canonical; peers regenerate):
  template/base/build.fsx                         # FR-002/003/004/014 resolver
  .specify/presets/fsharp-opinionated/templates/tasks-deps-template.yml  # FR-006/008
  .specify/templates/tasks-template.md            # FR-006 guidance (if needed)
  .agents/skills/speckit-tasks/SKILL.md           # FR-001/005/006/009/011
  .agents/skills/speckit-evidence-graph/SKILL.md  # FR-011 canonical command
  .agents/skills/fs-skia-layout-evidence/SKILL.md # FR-012 split
  .agents/skills/<new-evidence-mode-skill>/SKILL.md   # FR-012 new
  .agents/skills/<new-layout-readability-skill>/SKILL.md # FR-012 new (or rename)
  .template.config/template.json                  # FR-012/013 copy new skills
  template/capabilities.yml                       # FR-012 catalog (if referenced)
  .specify/templates/constitution-template.md (+ preset twin)  # FR-012 canonical capability-skill list (regenerates .specify/memory/constitution.md)
  template/base/README.md                         # FR-012 prose ref to split skill
  .specify/templates/tasks-template.md            # FR-012 prose ref to split skill
  .specify/presets/fsharp-opinionated/templates/tasks-template.md  # FR-012 prose ref
  .specify/presets/fsharp-opinionated/commands/speckit.tasks.md    # FR-012 prose ref

Compiled governance engine changed:
  build/Governance/Evidence/DepsParser.fs(+.fsi)  # FR-006/007/010 owns field
  build/Governance/Evidence/Audit.fs(+.fsi)       # FR-009/010 remove triggers
  build/Governance/GeneratedProduct.fs            # FR-014 update expectations
  build/Governance/Evidence/Render.fs(+.fsi)      # assessment render fallout
  build/Governance/GovernedBlocks.fs              # FR-012 re-point Verbatim splice refs (L158/170) + hardcoded skill list (L303-315)
  (possibly) build/Governance/SkillRegistry-adjacent hint validator # FR-008

Regenerated (not hand-edited):
  .claude/skills/**            via RefreshSurfaceBaselines (SkillSyncCheck)
  validation.contract.yml      via RefreshSurfaceBaselines (TargetMetadataDrift)
```

## Phase 0 — Research

See [research.md](./research.md). Open questions resolved there:

1. Exact `owns:` vocabulary and what each value gates (replacing the five
   `capabilityTriggerGroups`), and the downstream semantics that previously keyed
   off title triggers.
2. Whether the `fs-skia-layout-evidence` split is two **new** skills or a
   rename + one new skill, and the precise new registered ids.
3. Confirm the skill split needs **no** new build gates (only `SkillSyncCheck` /
   `SkillQualityCheck`), so `Targets`/`Routing`/`knownGates` stay untouched.
4. The authoritative consumer skill set per profile, to make the hint-resolution
   test (FR-008) and the corrected hints exact.
5. Whether the readiness-blocking scans (the persistent GUI-runtime,
   window-visibility, and audit-status families) are title-keyed and thus in
   FR-010 scope, or separate and out of scope.
6. The directive wrapper error wording for FR-007 and whether `LegacyBareList`
   already detects the bare-key case.

## Phase 1 — Design & Contracts

Outputs: [data-model.md](./data-model.md), [contracts/](./contracts/),
[quickstart.md](./quickstart.md), and the AGENTS.md plan-pointer update.

## Phase 2 — (planned, not executed here)

`/speckit.tasks` will break this into story-grouped tasks with `tasks.deps.yml`
(including the new `owns:` field exemplified on its own author-facing artifacts).
