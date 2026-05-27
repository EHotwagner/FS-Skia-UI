# Implementation Plan: Asteroids Integration Feedback

**Branch**: `020-asteroids-integration-feedback` | **Date**: 2026-05-27 | **Spec**: `specs/020-asteroids-integration-feedback/spec.md`
**Input**: Feature specification from `/specs/020-asteroids-integration-feedback/spec.md`

## Summary

Add a public, framework-facing layout evidence contract and generated-game
validation guidance so HUD/status readability is separately proven from
deterministic render metadata. Generated game samples must reserve a HUD region,
keep gameplay entities in a gameplay region at default and constrained sizes,
fail validation on HUD/HUD or HUD/gameplay overlap, and label unsupported layout
inspection explicitly. Public docs and generated examples must consistently name
the consumer scene-returning value, generated host value, and app-qualified
update function. Readiness output must classify known benign desktop host
warnings without hiding launch, layout, rendering, or package failures.

This is a Tier 1 contracted framework/governance change. It affects public
Scene/Testing or validation signatures, generated template guidance, generated
product checks, readiness evidence, capability-skill metadata, and governance
tests. It does not rewrite game mechanics, introduce a new game engine, change
unrelated controls/chart/DataGrid behavior, or guarantee layout proof on hosts
that cannot expose the required facts beyond explicit unsupported diagnostics.

## Technical Context

**Language/Version**: F# on .NET `net10.0` for framework packages, generated
templates, governance tests, and FAKE targets  
**Primary Dependencies**: Existing FS.Skia.UI Scene, Layout, SkiaViewer,
Elmish, Testing packages; SkiaSharp 4 preview stack; Expecto; FAKE; Spec Kit
evidence scripts. No new runtime dependency is planned unless exact text bounds
or host warning classification proves impossible with existing APIs.  
**Testing**: Expecto semantic tests through `.fsi`, generated product tests,
FAKE targets (`Verify`, generated `Test`, `GeneratedProductCheck`,
`GeneratedGuidanceCheck`, `TemplateCheck`, `EvidenceGraph`, `EvidenceAudit`),
FSI transcripts, generated validation reports, and supported-host readiness
evidence.  
**Target Platform**: Windows and Linux generated graphical apps; unsupported
desktop/session limitations must be reported as unsupported layout/host facts,
not silently converted into proof.  
**Public Surface**: `src/Scene/Scene.fsi` and/or `src/Testing/Testing.fsi` may
change to expose scene layout evidence and generated validation contracts.
`template/capabilities.yml`, generated template files, public docs, surface
baselines, and capability skills must be reviewed.  
**Evidence Requirement**: Required real evidence paths are
`specs/020-asteroids-integration-feedback/readiness/hud-layout-readability.md`,
`public-contract-guidance.md`, `layout-evidence.md`,
`host-warning-classification.md`, `generated-validation.md`, and
`evidence-audit.md`.  
**Synthetic Evidence**: Synthetic or approximate text measurement is allowed
only when exact host/font metrics are unavailable, must be deterministic and
conservative, and must be disclosed as unsupported/approximate where used.
Synthetic overlap fixtures may prove validator failure behavior, but cannot
replace generated-product evidence that exercises the public contract.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Update generated template source/guidance and
  generated product tests when HUD/gameplay region construction, evidence
  commands, generated app signatures, or warning classifications change.
  Review `.template.config/template.json` if new skill, docs, validation, or
  evidence files are included in generated output.
- **Dependency impact**: No new dependency is planned. If exact text metrics
  require an added measurement package, update `Directory.Packages.props`,
  `template/base/Directory.Packages.props`, `docs/dependencies.md`, and
  `DependencyReport`.
- **Command-surface impact**: `Verify`, generated `Test`,
  `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateCheck`,
  `EvidenceGraph`, and `EvidenceAudit` must gain or verify coverage. `Dev`,
  `Ci`, `PackLocal`, `DependencyReport`, and `TemplateDrift` change only if
  aggregation or validation paths require it.
- **Generated project impact**: Generated game samples must reserve named HUD
  and gameplay regions, keep active gameplay entities in the gameplay region,
  expose layout evidence commands/reports, and document app-owned scene/host
  and update names without ambiguity.
- **Evidence paths**: Required readiness paths are:
  - `specs/020-asteroids-integration-feedback/readiness/hud-layout-readability.md`
  - `specs/020-asteroids-integration-feedback/readiness/public-contract-guidance.md`
  - `specs/020-asteroids-integration-feedback/readiness/layout-evidence.md`
  - `specs/020-asteroids-integration-feedback/readiness/host-warning-classification.md`
  - `specs/020-asteroids-integration-feedback/readiness/generated-validation.md`
  - `specs/020-asteroids-integration-feedback/readiness/evidence-audit.md`
- **`.fsi` / contract impact**: Public layout-evidence and generated validation
  contracts must start in `.fsi`, with surface baselines and docs updated before
  implementation. Public scene value, generated host, and app update naming
  guidance must be validated.
- **MVU/effect boundary**: Gameplay behavior is already product-owned MVU. This
  feature adds validation/evidence workflows. If warning classification or
  evidence generation becomes stateful/I/O-bearing, model it with explicit
  request/result records and keep file/host/process work at the interpreter
  edge.
- **Synthetic evidence**: PASS with restrictions. Approximate text bounds and
  negative overlap fixtures must be labeled synthetic/approximate where they
  stand in for unavailable host facts. Deterministic scene hashes remain valid
  render metadata, not layout-readability proof.
- **Test evidence**: Add failing-first semantic and governance tests for region
  separation, constrained-size overlap detection, missing/unsupported layout
  facts, public naming guidance, task `skillist` metadata, and benign host
  warning classification.
- **Observability**: Reports must state HUD region, gameplay region, text
  bounds, overlap status, proof level, unsupported reasons, warning class,
  evidence path, and failure class.
- **Deferred scope**: No new game engine, no Asteroids mechanics rewrite, no
  unrelated chart/control/DataGrid work, no release automation, no marketplace
  distribution, and no unsupported-host proof beyond explicit diagnostics.

**Pre-design gate result**: PASS. The feature is Tier 1 and touches public
contracts, generated validation, evidence, and task skill governance, but the
plan includes `.fsi` review, failing-first tests, real readiness evidence,
synthetic/unsupported disclosure, and actionable diagnostics.

## Project Structure

```text
src/Scene/
  Scene.fsi                       # Public scene/layout evidence types if owned by Scene
  Scene.fs

src/Testing/
  Testing.fsi                     # Generated validation/layout evidence assertions if owned by Testing
  Testing.fs

template/
  capabilities.yml                # Must include fs-skia-layout-evidence skill inventory
  fragments/.../skill/SKILL.md    # Capability skill location if packaged in template fragments
  base/src/Product/Program.fs     # Generated HUD/gameplay regions and evidence commands
  base/docs/product.md            # Generated app guidance

docs/
  generated-apps.md               # Public scene/host/update naming and validation guidance
  evidence.md                     # Layout evidence vs deterministic render metadata
  testing.md                      # Generated validation/readiness contract

tests/
  Scene.Tests/                    # Semantic scene evidence tests if Scene owns contract
  Testing.Tests/                  # Validation helper tests if Testing owns contract
  Governance.Tests/               # Guidance, task skill, generated validation, audit checks
  Smoke.Tests/                    # Generated/sample game smoke coverage if needed

.agents/skills/
  fs-skia-layout-evidence/SKILL.md # Repo-local required layout/evidence capability skill

specs/020-asteroids-integration-feedback/
  plan.md
  research.md
  data-model.md
  quickstart.md
  contracts/
    generated-game-layout-contract.md
    layout-evidence-contract.md
    public-contract-guidance-contract.md
    host-warning-classification-contract.md
    task-skill-contract.md
  readiness/
```

## Phase 0: Research

Research is complete in `specs/020-asteroids-integration-feedback/research.md`.
Key decisions:

- Layout readability requires explicit HUD/gameplay regions plus text bounds;
  deterministic render hashes are insufficient proof.
- Bounds may be approximate only when deterministic, conservative, and labeled
  as approximate or unsupported in evidence.
- Generated game movement and wrap logic must use gameplay-region coordinates,
  not full-scene coordinates, once a HUD region is reserved.
- Public guidance uses app-owned names for generated signatures:
  `Product.Program.view` for `Scene`, `Product.Program.generatedHost` for the
  viewer host, and `Product.Program.update` when tests or signatures need the
  reducer.
- Benign host warnings are warning-class records that do not fail readiness
  when launch/layout/render/package checks are otherwise successful.
- Tasks touching HUD readability, layout evidence, public scene/host/update
  guidance, generated validation, or warning classification must declare
  `fs-skia-layout-evidence` in `tasks.deps.yml` and mirror it in `tasks.md`.

## Phase 1: Design and Contracts

Design artifacts produced:

- `specs/020-asteroids-integration-feedback/data-model.md`
- `specs/020-asteroids-integration-feedback/contracts/generated-game-layout-contract.md`
- `specs/020-asteroids-integration-feedback/contracts/layout-evidence-contract.md`
- `specs/020-asteroids-integration-feedback/contracts/public-contract-guidance-contract.md`
- `specs/020-asteroids-integration-feedback/contracts/host-warning-classification-contract.md`
- `specs/020-asteroids-integration-feedback/contracts/task-skill-contract.md`
- `specs/020-asteroids-integration-feedback/quickstart.md`

### Post-Design Constitution Check

- **Spec -> FSI -> tests -> implementation**: PASS. Public layout evidence and
  validation helpers start in `.fsi`, then semantic/generated/governance tests,
  then implementation.
- **Visibility in `.fsi`**: PASS. New public Scene or Testing symbols require
  matching `.fsi` entries and surface baseline updates.
- **Idiomatic simplicity**: PASS. Expected design uses records,
  discriminated unions, simple rectangle overlap checks, and explicit result
  values. No complex F# features are planned.
- **MVU/effect boundary**: PASS. The app update loop stays product-owned.
  Evidence/warning validation is modeled as request/result records; host/file
  effects stay outside pure classifiers.
- **Synthetic disclosure**: PASS with restrictions. Approximate text metrics
  and negative overlap fixtures must be disclosed and cannot be used to claim
  host-proven readability when required facts are unsupported.
- **Test evidence**: PASS. The quickstart names failing-first semantic,
  generated, guidance, graph, audit, and readiness evidence commands.
- **Observability and safe failure**: PASS. Contracts require explicit proof
  level, unsupported reasons, missing facts, overlap diagnostics, and warning
  classifications.

## Phase 2: Planning Boundary

Stop after design. Task generation should produce dependency-ordered tasks
with `skillist` metadata, required readiness files, failing-first tests, and
acceptance keywords before implementation begins. Every task touching generated
game HUD readability, layout evidence, scene/host/update guidance, generated
validation, or benign warning classification must list `fs-skia-layout-evidence`.
