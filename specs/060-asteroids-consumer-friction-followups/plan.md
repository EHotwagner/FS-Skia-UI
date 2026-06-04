# Implementation Plan: Asteroids-Demo Consumer Friction Follow-ups & Template-Update Skill Currency

**Branch**: `060-asteroids-consumer-friction-followups` | **Date**: 2026-06-03 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/060-asteroids-consumer-friction-followups/spec.md`

## Summary

Close the shippable subset of the nine Asteroids-demo consumer friction findings
(F1–F9) plus the `fs-skia-template-update` skill currency gap. The technical
approach is **consumer-contract hardening, not runtime change**: ship 059's
already-merged `resolveFeatureDir` to consumers (bump/pack/install template +
verification log); emit an authoritative `docs/api-surface/<Pkg>/<file>.fsi` tree
into generated projects (generated single-source from `template/capabilities.yml`
`contracts:`); add anti-drift governance checks so skill-claimed contract paths and
the template-update skill's package enumeration cannot drift from generated output /
the packable set; split generated `Tests.fs` into durable `GovernanceTests.fs` +
replaceable `BehaviorTests.fs`; and correct the keyboard skill, scene/keyboard
pitfall notes, and the HUD/gameplay-layout pattern in `fs-skia-layout-readability`.
F6/F7 land as authoring guidance, not new hard gates. See [research.md](./research.md)
for the approach decisions (D1–D10).

**Change classification: Tier 1** — no framework `.fsi` *signatures* change, but the
consumer contract (generated docs, generated test layout, capability/template-update
skill content) changes and new governance gates are added.

## Technical Context

**Language/Version**: F# / .NET (`net10.0`); governance engine `FS.Skia.UI.Build`
**Primary Dependencies**: existing repo packages only; no new dependencies. Generated
projects consume `FS.Skia.UI.*` `0.1.63-preview.1` (template pins already current).
**Testing**: Expecto + FAKE targets; generated-project instantiation
(`dotnet new fs-skia-ui`) restore/test; FSI not required (no new `.fsi` surface).
Generated-product evidence logs prove FR-001/FR-003 end-to-end.
**Target Platform**: Windows and Linux (template/governance; no GPU/runtime change).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Result: PASS.** No new public framework `.fsi` surface (Principle II/Tier-1 `.fsi`
clause N/A — signatures unchanged; emitted api-surface docs are copies of existing
`.fsi`). No stateful/I/O runtime workflow added (Principle IV N/A — the generated
evidence-runner feature resolution already exists and is pure file resolution). No
synthetic evidence planned (Principle V — real generated-project logs). Behavior-
changing governance checks ship with failing-first tests (Principle VI). Idiomatic
simplicity preserved (Principle III — generators/checks are plain F# over existing
governance modules).

### Repository Governance Decisions

- **Template ownership**: **Changes required.** New emitted tree
  `template/base/docs/api-surface/<Pkg>/<file>.fsi` (FR-003); split generated tests
  `template/base/tests/Product.Tests/{GovernanceTests.fs,BehaviorTests.fs}` (FR-005)
  with `Product.Tests.fsproj` compile-order update; corrected
  `template/product-skills/fs-skia-keyboard-input/SKILL.md` (FR-006) and pitfall notes
  in `template/product-skills/fs-skia-scene/SKILL.md` (FR-007). `.template.config/
  template.json` updates to include the new `docs/api-surface/**` content and the two
  test filenames (and exclude none that were previously single-`Tests.fs`). Template
  package version bumped + packed + installed (FR-002). All recorded in
  `template.json`; no deferral.
- **Dependency impact**: **No change.** No new `Directory.Packages.props` entries,
  no `docs/dependencies.md` / `DependencyReport` change — feature adds no packages.
  Generated-project pins already at `0.1.63-preview.1`.
- **Command-surface impact**: **Changes required** in `FS.Skia.UI.Build`
  (`build/Governance/**`): a generator for the api-surface tree (single-source from
  `capabilities.yml`, regenerated via `RefreshSurfaceBaselines`, currency-enforced like
  `SkillSyncCheck`/`TargetMetadataDrift`); a `SkillContractPathCheck` (FR-004); a
  `TemplateUpdateSkillPackageCheck` (FR-009/SC-006). `TemplateCheck` /
  `GeneratedProductCheck` extend to the emitted docs + split test files.
  `GeneratedGuidanceCheck`, `EvidenceGraph`, `EvidenceAudit` run unchanged.
  `Routing.fs` (and the generated `validation.contract.yml`) gain the new gates,
  routed for the consumer-contract/governance globs. **FAKE-backed targets share
  `.fake` state — run sequentially in deterministic order**; example:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t Verify`
  Authoritative per-change gate list comes from `./fake.sh build -t Route`.
- **Generated project impact**: **Changes required.** Generated projects gain a
  `docs/api-surface/` tree (default `app` profile emits the surface for its pinned
  packages); generated `tests/Product.Tests` carries two files instead of one; the
  bundled capability skills (keyboard/scene) and `fs-skia-layout-readability` carry
  corrected examples + pitfall/pattern notes. Generated `Dev`/evidence-runner behavior
  is unchanged (059 already shipped the resolver). Placeholder/excluded-history scans
  updated for the new filenames.
- **Evidence paths**: All under
  `specs/060-asteroids-consumer-friction-followups/readiness/`:
  - `target-metadata.md`, `agent-ready-verdict.md` (Route maintainer-verify required)
  - `skill-loading-evidence.md` (pre-task skill loads, ISO-8601 timestamps)
  - `aggregate-hang-diagnostics.md` (EvidenceAudit readiness vocabulary)
  - `generated-project/feature-resolution.log` — FR-001 proof: echoed
    `feature-directory=`/`tasks=` for a multi-task feature **and** the loud-failure
    output for a missing `SPECKIT_FEATURE_DIR` / empty `feature.json`
  - `generated-project/api-surface.log` — FR-003 proof: `docs/api-surface/<Pkg>/<Pkg>.fsi`
    present in a freshly generated project, contents matching the source `.fsi`
  - `generated-project/test-split.log` — FR-005 proof: governance file compiles/runs
    after the scaffold model is swapped, only behavior file rewritten
  - `template/template-pack.log`, `template/template-package-contents.md` (TemplatePack)
  - `skill-quality-check.md`, plus regenerated `.claude` tree (RefreshSurfaceBaselines)
- **`.fsi` / contract impact**: **No framework signature change.** Surface-area
  baselines under `readiness/surface-baselines/*.txt` unchanged. The feature *surfaces*
  existing `.fsi` into generated projects (copies, not edits) and adds checks that the
  surfaced copies match source. Capability/template-update skill content (consumer
  contract) changes; no compatibility/migration note needed for framework consumers.
- **MVU/effect boundary**: **N/A.** No new stateful or I/O-bearing workflow. The
  generated evidence-runner's feature resolution is existing pure path resolution
  (env → `feature.json` → loud fail); no `Model`/`Msg`/`Effect` is introduced.
- **Synthetic evidence**: **None planned.** All FR-001/FR-003/FR-005 evidence is real
  generated-project output. No mocks/fakes/placeholders; no `[S]` disclosure expected.
  If any verification step is environment-blocked, it is marked `[-]` skipped with
  written rationale (Principle VI), never `[X]` or synthetic.
- **Test evidence**: Failing-first governance tests in `FS.Skia.UI.Build` for the
  api-surface currency generator, `SkillContractPathCheck`, and
  `TemplateUpdateSkillPackageCheck` (each asserts the negative case fails before impl,
  passes after). Generated-product source-structure tests update to the two test
  filenames. Generated-project instantiation smoke tests via the template-update flow.
- **Observability**: New checks emit actionable diagnostics naming the offending skill
  path / missing emitted file / drifting package ID (e.g. `MISS api-surface:
  docs/api-surface/Foo/Foo.fsi claimed by fs-skia-foo`, `template-update skill lists
  phantom package FS.Skia.UI; missing FS.Skia.UI.Input`). Missing required readiness
  artifacts fail `Route --enforce` by class. Loud-failure messages for unresolved
  features already exist in `build.fsx` (verified, not changed).
- **Deferred scope**: F4's framework-source DU-case *rename* is deferred (pitfall note
  FR-007 is the committed remedy). F6/F7 (FR-010/FR-011) land as authoring guidance,
  **not** new hard merge gates, unless a low-cost executable check is found during
  Phase 1. No new game/demo, no runtime capability, no release/distribution change.

## Project Structure

```
specs/060-asteroids-consumer-friction-followups/
  spec.md
  plan.md            # this file
  research.md        # Phase 0 decisions D1–D10
  data-model.md      # Phase 1 entities
  quickstart.md      # Phase 1 verification walkthrough
  contracts/
    api-surface-emission.md          # FR-003 generated tree contract
    skill-contract-path-check.md     # FR-004 anti-drift check contract
    template-update-package-check.md # FR-009/SC-006 packable-set check contract
    generated-test-split.md          # FR-005 governance/behavior split contract
  readiness/         # evidence artifacts (created during implementation)

# Touched implementation paths (authored/generated during implement):
template/base/docs/api-surface/<Pkg>/<file>.fsi      # FR-003 (generated)
template/base/tests/Product.Tests/GovernanceTests.fs # FR-005
template/base/tests/Product.Tests/BehaviorTests.fs   # FR-005
template/base/tests/Product.Tests/Product.Tests.fsproj
template/product-skills/fs-skia-keyboard-input/SKILL.md  # FR-006
template/product-skills/fs-skia-scene/SKILL.md           # FR-007
.agents/skills/fs-skia-layout-readability/SKILL.md       # FR-008 (canonical)
.agents/skills/fs-skia-template-update/SKILL.md          # FR-009 (canonical)
.template.config/.../template.json                       # include new content
build/Governance/{ApiSurfaceGen,SkillContractPath,TemplateUpdatePackage}.fs(i)  # checks
build/Governance/Routing.fs                              # route new gates
.claude/skills/**                                        # regenerated from .agents
```

## Phase 1 Outputs

- `data-model.md` — governance entities (ApiSurfaceEntry, SkillContractClaim,
  PackableProject, GeneratedTestFile) and their validation rules.
- `contracts/*.md` — the four new/changed contracts listed above.
- `quickstart.md` — the end-to-end verification walkthrough producing the readiness
  logs.
- Agent context: `AGENTS.md` SPECKIT marker repointed to this plan.
