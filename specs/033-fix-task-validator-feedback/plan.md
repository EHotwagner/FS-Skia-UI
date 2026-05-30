# Implementation Plan: Task Validator Feedback Follow-ups

**Branch**: `033-fix-task-validator-feedback` | **Date**: 2026-05-29 | **Spec**: `specs/033-fix-task-validator-feedback/spec.md`
**Input**: Feature specification from `specs/033-fix-task-validator-feedback/spec.md`

## Summary

Fix task graph validator feedback from the Asteroids demo task-generation analysis by making high-confidence skill matching token-aware, documenting the real trigger rules and readiness-notes escape hatch before validation, clarifying skill registry id resolution, adding advisory FS.Skia.UI capability guidance, and labeling graph-only validation output accurately. The implementation should stay inside Spec Kit governance assets, generated guidance, validator scripts, and governance tests; no runtime FS.Skia.UI API or package dependency change is planned.

## Technical Context

**Language/Version**: Python 3 for `.specify/extensions/evidence/scripts/python/compute-task-graph.py`; F# on .NET `net10.0` for governance tests and FAKE-backed validation; Markdown/YAML for Spec Kit guidance, templates, contracts, and readiness evidence.
**Primary Dependencies**: Existing Spec Kit evidence extension scripts, generated task templates, local skill registry (`.agents/skills/*/SKILL.md`, `src/*/skill/SKILL.md`, `template/fragments/*/skill/SKILL.md`), Expecto, FAKE. No new dependency is planned.
**Testing**: Failing-first governance tests for validator token boundaries, readiness-notes suppression, trigger guidance coverage, skill registry diagnostics, advisory capability guidance, and graph-only output labeling; generated guidance scans; targeted direct `compute-task-graph.py` fixture runs; sequential FAKE-backed targets when broad validation is needed.
**Target Platform**: Repository governance and generated project task-authoring workflows on Windows and Linux. Runtime rendering, viewer behavior, package runtime APIs, and generated demo implementation are out of scope.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: In scope for `.specify/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/templates/tasks-template.md`, generated guidance checks, and any generated-product guidance copies. Review `.template.config/template.json` only if new generated files are introduced; otherwise preserve template identity and inclusion policy.
- **Dependency impact**: No dependency change is planned. `Directory.Packages.props`, `docs/dependencies.md`, and `DependencyReport` are out of scope unless implementation unexpectedly requires a new parser or YAML dependency.
- **Command-surface impact**: `EvidenceGraph` behavior and graph-only output are in scope through the existing evidence script and generated build runner. `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, and `EvidenceAudit` may be used for validation. `Dev`, `Verify`, `Ci`, `PackLocal`, and `TemplateDrift` should change only if touched artifacts require normal validation. FAKE-backed commands must run sequentially, following the repo order when more than one is needed.
- **Generated project impact**: Generated task authoring guidance and generated evidence command labels are in scope. Default generated app behavior, persistent viewer launch, runtime samples, and package consumers are out of scope.
- **Evidence paths**: Required readiness paths are `specs/033-fix-task-validator-feedback/readiness/title-trigger-validation.md`, `specs/033-fix-task-validator-feedback/readiness/task-guidance-scan.md`, `specs/033-fix-task-validator-feedback/readiness/skill-registry-diagnostics.md`, `specs/033-fix-task-validator-feedback/readiness/advisory-capability-guidance.md`, and `specs/033-fix-task-validator-feedback/readiness/graph-only-output-label.md`. Implementation should also refresh task graph and evidence audit outputs under the feature readiness directory.
- **`.fsi` / contract impact**: No public F# runtime API change is planned. Contracts are Markdown behavior contracts for validator and guidance surfaces. If an implementation unexpectedly adds public F# symbols, work must return to `.fsi` first, with semantic tests and surface baseline updates.
- **MVU/effect boundary**: Runtime state workflows are out of scope. The validator is a script-style workflow: file reads, diagnostics, and artifact writes remain at the script edge; matching and id-resolution rules should be testable as deterministic logic.
- **Synthetic evidence**: Synthetic fixtures are allowed for malformed task metadata, title-trigger, and diagnostic test cases because these are validator error-path inputs. They must be named as validator fixtures and cannot replace real scans of the repository guidance files.
- **Test evidence**: Add failing-first governance or script tests for substring false positives, mandated readiness filename references, readiness-notes prefix suppression, trigger group documentation, directory/id mismatch diagnostics, advisory FS.Skia.UI guidance, graph-only label output, and preservation of existing graph protections.
- **Observability**: Validator diagnostics and readiness files must name the command or script path, mode, matched trigger group, accepted or missing skill id, registry path considered, failure classification, and next action. Graph-only output must state that it is graph validation, not full evidence audit.
- **Deferred scope**: Runtime package APIs, rendering behavior, new game features, release publishing, broad Spec Kit replacement, new package families, and generated demo implementation work are deferred.

**Gate result before Phase 0**: PASS. The plan names the affected governance surfaces, test evidence, command validation, synthetic fixture boundaries, diagnostics, and readiness obligations with no unresolved clarification.

## Project Structure

### Feature Artifacts

```text
specs/033-fix-task-validator-feedback/
|-- spec.md
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   |-- title-trigger-validation.md
|   |-- task-guidance-scan.md
|   |-- skill-registry-diagnostics.md
|   |-- advisory-capability-guidance.md
|   `-- graph-only-output-label.md
`-- readiness/
    |-- title-trigger-validation.md
    |-- task-guidance-scan.md
    |-- skill-registry-diagnostics.md
    |-- advisory-capability-guidance.md
    `-- graph-only-output-label.md
```

### Source And Test Touch Points

```text
.specify/extensions/evidence/scripts/python/compute-task-graph.py
.specify/templates/tasks-template.md
.specify/presets/fsharp-opinionated/templates/tasks-template.md
.specify/presets/fsharp-opinionated/commands/speckit.tasks.md
.agents/skills/speckit-tasks/SKILL.md
.agents/skills/speckit-evidence-graph/SKILL.md
template/base/build.fsx
template/base/tests/Product.Tests/Tests.fs
tests/Governance.Tests/SkillValidationTests.fs
tests/Governance.Tests/GovernanceEvidenceTests.fs
tests/Governance.Tests/CommandContractTests.fs
tests/Governance.Tests/GeneratedGuidanceTests.fs
build.fsx
```

## Phase 0 Research

Research is captured in `specs/033-fix-task-validator-feedback/research.md`.

## Phase 1 Design

Design entities are captured in `specs/033-fix-task-validator-feedback/data-model.md`.

Contracts are captured in:

- `specs/033-fix-task-validator-feedback/contracts/title-trigger-validation.md`
- `specs/033-fix-task-validator-feedback/contracts/task-guidance-scan.md`
- `specs/033-fix-task-validator-feedback/contracts/skill-registry-diagnostics.md`
- `specs/033-fix-task-validator-feedback/contracts/advisory-capability-guidance.md`
- `specs/033-fix-task-validator-feedback/contracts/graph-only-output-label.md`

Quickstart validation is captured in `specs/033-fix-task-validator-feedback/quickstart.md`.

## Constitution Check Post-Design

- **Spec -> FSI -> semantic tests -> implementation**: PASS. The design does not add runtime F# public API. Markdown contracts and validator behavior tests cover the changed public governance surface.
- **Visibility lives in `.fsi`**: PASS. No `.fsi` changes are planned; any discovered F# public surface change must return to `.fsi` first.
- **Idiomatic simplicity**: PASS. Token-aware matching can be implemented with simple Python helpers and explicit rule tables; no new complex F# features or dependencies are needed.
- **MVU/effect boundary**: PASS. Runtime MVU is out of scope; validator I/O remains at the script edge and matching/id-resolution rules remain deterministic.
- **Synthetic evidence disclosure**: PASS. Synthetic task-list fixtures are limited to validator behavior and error-path tests; real guidance scans remain required.
- **Test evidence mandatory**: PASS. Each story maps to failing-first governance tests, direct script fixtures, or generated guidance scans with required readiness outputs.
- **Observability and safe failure**: PASS. Contracts require actionable diagnostics for trigger matches, accepted ids, registry mismatches, and graph-only mode labeling.
