# Implementation Plan: Asteroids Feedback Skill Guidance

**Branch**: `034-asteroids-feedback-skills` | **Date**: 2026-05-30 | **Spec**: `specs/034-asteroids-feedback-skills/spec.md`
**Input**: Feature specification from `specs/034-asteroids-feedback-skills/spec.md`

## Summary

Improve generated FS.Skia.UI task guidance so Asteroids-style visual demo work starts with the right local skills, exposes audit-required readiness files before implementation, preserves visual-evidence honesty, and classifies feedback into framework, template/evidence workflow, documentation/discoverability, and consumer-authoring follow-up paths. The clarified scope also adds comprehensive XML documentation comments to every public `.fsi` compiled by packable framework packages, plus hard validation that generated and packed XML docs are present and non-empty. Runtime API shapes, package versions, rendering behavior, host resize APIs, and a new Asteroids demo remain deferred.

## Technical Context

**Language/Version**: Markdown/YAML for Spec Kit task guidance, contracts, readiness scaffolds, generated templates, and skill guidance; F# on .NET `net10.0` for public `.fsi` XML docs, governance tests, package validation, and FAKE-backed validation; Python 3 only for evidence graph or guidance scans if existing scripts require it.
**Primary Dependencies**: Existing local skill inventory (`.agents/skills/*/SKILL.md`, `src/*/skill/SKILL.md`, `template/fragments/*/skill/SKILL.md`), Spec Kit evidence extension scripts, generated task templates, Expecto, FAKE, MSBuild XML documentation generation already enabled by `Directory.Build.props`. No new dependency is planned.
**Testing**: Failing-first governance and generated-guidance tests for skill assignment patterns, multiple-skill guidance, readiness scaffold coverage, visual-evidence honesty, feedback classification, XML doc coverage for public `.fsi` surfaces, packed NuGet XML doc inclusion, and preservation of advisory-only validation behavior. FAKE-backed targets must run sequentially when more than one is needed.
**Target Platform**: Repository governance, generated FS.Skia.UI project task-authoring workflows, XML documentation generation, and package validation on Windows and Linux. Runtime rendering, screenshot capture internals, host resize APIs, release publishing, and a new Asteroids demo implementation are out of scope.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: In scope for `.specify/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/commands/speckit.tasks.md`, generated guidance checks, and generated-product task guidance copies. Review `.template.config/template.json` only if implementation introduces new generated files; otherwise preserve template identity and inclusion policy.
- **Dependency impact**: No dependency change is planned. `Directory.Packages.props`, `docs/dependencies.md`, generated dependency guidance, and `DependencyReport` are out of scope unless implementation unexpectedly requires a new parser or package.
- **Command-surface impact**: `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`, and package/XML documentation validation may need coverage. `Dev`, `Verify`, `Ci`, `PackLocal`, `DependencyReport`, and `TemplateDrift` should change only if touched artifacts require their normal validation. FAKE-backed commands must run sequentially and follow the repo order when more than one is needed.
- **Generated project impact**: Generated implementation task guidance, `tasks.deps.yml` skill metadata expectations, visible `[skillist: ...]` mirrors, readiness-file scaffolding, and generated validation wording are in scope. Default generated app runtime behavior, sample gameplay, persistent viewer implementation, and package consumers are out of scope.
- **Evidence paths**: Required readiness paths are `specs/034-asteroids-feedback-skills/readiness/skill-assignment-guidance.md`, `specs/034-asteroids-feedback-skills/readiness/readiness-scaffold-coverage.md`, `specs/034-asteroids-feedback-skills/readiness/visual-evidence-honesty.md`, `specs/034-asteroids-feedback-skills/readiness/feedback-classification.md`, `specs/034-asteroids-feedback-skills/readiness/generated-guidance-validation.md`, and `specs/034-asteroids-feedback-skills/readiness/xml-documentation-validation.md`. Implementation should also refresh graph and audit evidence under this feature readiness directory.
- **`.fsi` / contract impact**: Public F# runtime API shapes are not expected to change, but public XML documentation comments are in scope for every `.fsi` compiled by packable `src/*/*.fsproj`, including `src/Lib`, `src/Scene`, `src/SkiaViewer`, `src/Elmish`, `src/KeyboardInput`, `src/Layout`, `src/Controls`, `src/Controls.Elmish`, and `src/Testing`. Contracts are Markdown behavior contracts for task guidance, readiness scaffolding, visual evidence acceptance, feedback classification, advisory skill discovery, XML doc coverage, and packed XML doc inclusion. If implementation unexpectedly adds public F# symbols, work must return to `.fsi` first with semantic tests and surface baselines.
- **MVU/effect boundary**: Runtime MVU workflows are out of scope. The changed workflows are task generation, evidence production guidance, XML documentation validation, and package artifact inspection. Validator or scan logic should keep I/O at the script edge and make matching/classification rules deterministic and testable.
- **Synthetic evidence**: Synthetic fixtures are allowed for generated task-list edge cases, missing readiness scaffold examples, fallback image classifications, warning-classification examples, and malformed XML-doc validation examples because these validate guidance and error-path behavior. They must be named as guidance or validator fixtures and cannot replace real scans of repository guidance files, public `.fsi` files, generated XML docs, or packed NuGet artifacts.
- **Test evidence**: Add failing-first governance or generated-guidance tests for specialized skill assignments, multi-skill task metadata, audit readiness file enumeration, required readiness field cues, decodable-image proof wording, fallback/metadata/layout-only rejection wording, feedback owner classification, API/host friction guidance, XML doc comments on public `.fsi` members, generated XML doc files for packable packages, NuGet package XML doc inclusion, and preservation of valid advisory-only task lists.
- **Observability**: Generated guidance and readiness files must name the command or script path, skill ids and resolved `SKILL.md` paths, required readiness path, evidence artifact path, image dimensions/content classification, fallback or unsupported classification, owner category, XML doc package/member/file status, failure classification, and next action.
- **Deferred scope**: Runtime stroke rasterization, text rasterization, screenshot capture internals, host resize hooks, auto-close persistent launch APIs, release publishing, package version bumps, runtime API shape changes, and implementing a new Asteroids demo are deferred.

**Gate result before Phase 0**: PASS. The plan names governance surfaces, skill inventory use, generated-task impact, public `.fsi` documentation scope, package XML validation, evidence paths, synthetic fixture boundaries, tests, diagnostics, and deferred runtime issues with no unresolved clarification.

## Project Structure

### Feature Artifacts

```text
specs/034-asteroids-feedback-skills/
|-- spec.md
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   |-- skill-assignment-guidance.md
|   |-- readiness-scaffold-coverage.md
|   |-- visual-evidence-honesty.md
|   |-- feedback-classification.md
|   |-- generated-guidance-validation.md
|   `-- xml-documentation-validation.md
`-- readiness/
    |-- skill-assignment-guidance.md
    |-- readiness-scaffold-coverage.md
    |-- visual-evidence-honesty.md
    |-- feedback-classification.md
    |-- generated-guidance-validation.md
    `-- xml-documentation-validation.md
```

### Source And Test Touch Points

```text
.specify/templates/tasks-template.md
.specify/presets/fsharp-opinionated/templates/tasks-template.md
.specify/presets/fsharp-opinionated/commands/speckit.tasks.md
.agents/skills/speckit-tasks/SKILL.md
.agents/skills/speckit-implement/SKILL.md
.agents/skills/speckit-evidence-graph/SKILL.md
.agents/skills/speckit-evidence-audit/SKILL.md
.agents/skills/fs-skia-layout-evidence/SKILL.md
src/*/skill/SKILL.md
template/fragments/*/skill/SKILL.md
src/**/*.fsi
src/**/*.fsproj
Directory.Build.props
template/base/build.fsx
template/base/tests/Product.Tests/Tests.fs
tests/Governance.Tests/SkillValidationTests.fs
tests/Governance.Tests/GovernanceEvidenceTests.fs
tests/Governance.Tests/GeneratedGuidanceTests.fs
tests/Governance.Tests/CommandContractTests.fs
tests/Governance.Tests/*Documentation*Tests.fs
```

## Phase 0 Research

Research is captured in `specs/034-asteroids-feedback-skills/research.md`.

## Phase 1 Design

Design entities are captured in `specs/034-asteroids-feedback-skills/data-model.md`.

Contracts are captured in:

- `specs/034-asteroids-feedback-skills/contracts/skill-assignment-guidance.md`
- `specs/034-asteroids-feedback-skills/contracts/readiness-scaffold-coverage.md`
- `specs/034-asteroids-feedback-skills/contracts/visual-evidence-honesty.md`
- `specs/034-asteroids-feedback-skills/contracts/feedback-classification.md`
- `specs/034-asteroids-feedback-skills/contracts/generated-guidance-validation.md`
- `specs/034-asteroids-feedback-skills/contracts/xml-documentation-validation.md`

Quickstart validation is captured in `specs/034-asteroids-feedback-skills/quickstart.md`.

## Constitution Check Post-Design

- **Spec -> FSI -> semantic tests -> implementation**: PASS. The design does not add runtime F# public API shape changes. Public `.fsi` documentation comments and Markdown contracts are the changed contract surfaces; governance/package tests cover them before implementation is complete.
- **Visibility lives in `.fsi`**: PASS. The feature uses `.fsi` as the source of public documentation obligations and avoids adding visibility modifiers to `.fs`.
- **Idiomatic simplicity**: PASS. Guidance and validation can use explicit rule tables, simple scans, XML parsing where needed, and existing tests without new abstractions or dependencies.
- **MVU/effect boundary**: PASS. Runtime MVU is out of scope. Task-generation, evidence-guidance scans, XML doc checks, and package inspection remain script-style workflows with deterministic classification logic.
- **Synthetic evidence disclosure**: PASS. Synthetic fixtures are limited to guidance/error-path examples and malformed documentation fixtures; real repository guidance scans, public `.fsi` scans, generated XML docs, and packed package validation remain required.
- **Test evidence mandatory**: PASS. Each story maps to failing-first governance tests, generated guidance scans, direct fixture checks, package artifact checks, or readiness evidence.
- **Observability and safe failure**: PASS. Contracts require actionable diagnostics for selected skills, missing readiness fields, unsupported visual proof, owner categories, undocumented public members, missing XML files, packed artifact omissions, and next actions.
