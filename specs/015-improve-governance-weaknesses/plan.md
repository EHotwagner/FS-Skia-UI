# Implementation Plan: Improve Governance Weaknesses

**Branch**: `015-improve-governance-weaknesses` | **Date**: 2026-05-26 | **Spec**: `specs/015-improve-governance-weaknesses/spec.md`
**Input**: Feature specification from `specs/015-improve-governance-weaknesses/spec.md`

## Summary

Tighten the governance workflow added for task `skillist` handling by making implementation-time skill loading auditable, reporting skill-match confidence instead of regex certainty, mapping change risk to proportionate evidence, diagnosing aggregate build hangs without overstating product failures, and documenting current runtime platform limitations. The change is scoped to Spec Kit governance assets, evidence validation, build/readiness reporting, and roadmap documentation. It does not expand runtime platform support, change rendering behavior, change package identity, or alter public F# runtime APIs.

## Technical Context

**Language/Version**: F# / .NET 10 for build orchestration and governance checks; Python 3 stdlib for evidence graph validation; Markdown/YAML for Spec Kit guidance and readiness artifacts
**Primary Dependencies**: Existing FAKE build script, `.specify/extensions/evidence/scripts/python/compute-task-graph.py`, `.specify/extensions/evidence/scripts/bash/run-audit.sh`, Spec Kit task/implementation skills, local capability skill inventory; no new package dependency expected
**Testing**: Focused governance tests, parser/validation fixtures, evidence graph/audit runs, generated guidance checks, aggregate timeout diagnostics, and bounded focused reruns. Expected commands include `./fake.sh build -t EvidenceGraph`, `./fake.sh build -t EvidenceAudit`, `./fake.sh build -t GeneratedGuidanceCheck`, focused governance test filters, and `./fake.sh build -t Dev` only when the selected risk level requires broad validation.
**Target Platform**: Windows and Linux governance workflow behavior. Runtime platform support remains the current product scope: .NET 10 desktop with Vulkan/SkiaSharp preview constraints as documented, not expanded by this feature.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Required for governance guidance that users and generated products consume. Likely touch points are `.agents/skills/speckit-implement/SKILL.md`, `.agents/skills/speckit-tasks/SKILL.md`, `.agents/skills/speckit-evidence-graph/SKILL.md`, `.agents/skills/speckit-evidence-audit/SKILL.md`, `.specify/extensions/evidence/commands/*.md`, `.specify/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/commands/speckit.tasks.md`, `.specify/presets/fsharp-opinionated/commands/speckit.implement.md`, and generated guidance checks in `build.fsx`. Constitution text should change only where it must define the strengthened rule.
- **Dependency impact**: None expected. Do not change `Directory.Packages.props`, package identities, package contents, package versions, or dependency ownership reports except to document existing maturity risks.
- **Command-surface impact**: `EvidenceGraph`, `EvidenceAudit`, `GeneratedGuidanceCheck`, and `Dev` reporting may need updates. `Verify`, `Ci`, `TemplateCheck`, `DependencyReport`, `TemplateDrift`, and `PackLocal` should change only if existing wiring must consume the new verdict or evidence schema.
- **Generated project impact**: Generated governance guidance should inherit the stronger skill-loading evidence, skill-match confidence, and risk-level evidence rules where generated projects use the Spec Kit workflow. Runtime generated samples and product visuals remain unchanged.
- **Evidence paths**: Use `specs/015-improve-governance-weaknesses/readiness/skill-loading-evidence.md`, `specs/015-improve-governance-weaknesses/readiness/skill-detection-calibration.md`, `specs/015-improve-governance-weaknesses/readiness/governance-risk-levels.md`, `specs/015-improve-governance-weaknesses/readiness/aggregate-hang-diagnostics.md`, `specs/015-improve-governance-weaknesses/readiness/runtime-limitations.md`, `specs/015-improve-governance-weaknesses/readiness/evidence-graph.md`, and `specs/015-improve-governance-weaknesses/readiness/evidence-audit.md`.
- **`.fsi` / contract impact**: No `.fsi`, surface baseline, or public runtime API changes are expected. This is a Tier 1 governance contract change because task evidence and readiness verdict contracts change.
- **MVU/effect boundary**: Product MVU is out of scope. Governance workflow state is represented as static reports and validation records, not a new runtime stateful product surface. If build orchestration code gains new timeout-verdict state, keep it inside existing FAKE target interpretation.
- **Synthetic evidence**: Avoid synthetic task completion evidence. Fixture task lists and simulated hang logs are validation inputs and must be described as fixtures in readiness notes, not as proof of production runtime behavior.
- **Test evidence**: Add failing-first validation examples for missing or late skill-loading evidence, ambiguous skill matches, indirect ownership matches, false positives, valid empty skill lists, selected risk-level evidence paths, aggregate timeout verdicts, and focused-rerun separation.
- **Observability**: Diagnostics must name task id, declared skill id, resolved skill path, load timing problem, matched signals, confidence level, ambiguity, risk level, missing evidence path, aggregate stage, elapsed time, last observed command, focused rerun result, and verdict category as applicable.
- **Deferred scope**: No new capability skills, no broad build-system simplification, no runtime platform expansion, no software renderer, no dependency replacement, no macOS/mobile/browser support, and no guarantee of agent honesty beyond required recorded evidence.

### Initial Gate Verdict

PASS. The plan keeps the work in governed workflow/reporting artifacts, identifies the changed evidence contracts, avoids public runtime/API/package changes, and declares focused evidence paths. No unresolved clarification remains.

## Project Structure

```text
specs/015-improve-governance-weaknesses/
|-- spec.md
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   |-- aggregate-timeout-verdict.md
|   |-- governance-risk-level.md
|   |-- skill-loading-evidence.md
|   `-- skill-match-assessment.md
`-- readiness/
    |-- skill-loading-evidence.md
    |-- skill-detection-calibration.md
    |-- governance-risk-levels.md
    |-- aggregate-hang-diagnostics.md
    |-- runtime-limitations.md
    |-- evidence-graph.md
    `-- evidence-audit.md

.agents/skills/speckit-implement/SKILL.md
.agents/skills/speckit-tasks/SKILL.md
.agents/skills/speckit-evidence-graph/SKILL.md
.agents/skills/speckit-evidence-audit/SKILL.md
.specify/extensions/evidence/scripts/python/compute-task-graph.py
.specify/extensions/evidence/scripts/bash/run-audit.sh
.specify/extensions/evidence/commands/speckit.evidence.graph.md
.specify/extensions/evidence/commands/speckit.evidence.audit.md
.specify/templates/tasks-template.md
.specify/presets/fsharp-opinionated/templates/tasks-template.md
.specify/presets/fsharp-opinionated/commands/speckit.tasks.md
.specify/presets/fsharp-opinionated/commands/speckit.implement.md
build.fsx
tests/Governance.Tests/
```

## Phase 0: Research

Research decisions are captured in `specs/015-improve-governance-weaknesses/research.md`.

Resolved decisions:

- Treat skill-loading evidence as a required per-task record with timing relative to task work, not as prose in a final summary.
- Keep applicable-skill detection heuristic, but downgrade it to confidence reporting with matched signals and reviewer disposition.
- Introduce explicit small/medium/broad governance risk levels to choose minimum evidence paths without weakening hard gates.
- Classify aggregate hangs as timeout/orchestration verdicts when focused product checks pass.
- Document runtime platform gaps as current limitations and roadmap boundaries, not defects fixed by this governance feature.

## Phase 1: Design & Contracts

Design artifacts:

- `specs/015-improve-governance-weaknesses/data-model.md`
- `specs/015-improve-governance-weaknesses/contracts/skill-loading-evidence.md`
- `specs/015-improve-governance-weaknesses/contracts/skill-match-assessment.md`
- `specs/015-improve-governance-weaknesses/contracts/governance-risk-level.md`
- `specs/015-improve-governance-weaknesses/contracts/aggregate-timeout-verdict.md`
- `specs/015-improve-governance-weaknesses/quickstart.md`

Agent context update:

- `AGENTS.md` points between the Spec Kit markers to `specs/015-improve-governance-weaknesses/plan.md`.

## Post-Design Constitution Check

PASS. The design defines concrete governance records and diagnostics while preserving existing runtime scope boundaries. It keeps `.fsi`, packages, renderer behavior, generated product visuals, and platform support unchanged, and it defines real readiness artifacts for the new evidence gates.
