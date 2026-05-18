# Implementation Plan: Task Skilllist Governance

**Branch**: `014-task-skilllist-governance` | **Date**: 2026-05-18 | **Spec**: `specs/014-task-skilllist-governance/spec.md`
**Input**: Feature specification from `specs/014-task-skilllist-governance/spec.md`

## Summary

Add a mandatory post-task-generation skill evaluation gate. Every task must carry a structured `skillist` field and a matching visible `tasks.md` mirror, readiness validation must reject missing or inconsistent values, and implementation must load each declared skill before starting the task. This is a Spec Kit governance and workflow change: update the constitution, task-generation skill/template guidance, implementation skill guidance, evidence validation contracts, and readiness evidence. No runtime package API or visual behavior changes are expected.

## Technical Context

**Language/Version**: F# / .NET 10 for repository checks and scripts; Markdown/YAML/Python for Spec Kit governance artifacts  
**Primary Dependencies**: Existing Spec Kit skills/templates, `.specify/extensions/evidence/scripts/python/compute-task-graph.py`, `.specify/extensions/evidence/scripts/bash/run-audit.sh`; no new package dependency expected  
**Testing**: Governance checks, evidence graph/audit checks, focused parser/validation fixtures, `./fake.sh build -t EvidenceGraph`, `./fake.sh build -t EvidenceAudit`, `./fake.sh build -t GeneratedGuidanceCheck`, and `./fake.sh build -t Dev` as applicable  
**Target Platform**: Windows and Linux unless narrowed by existing script behavior  

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Required. Update `.specify/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/commands/speckit.tasks.md`, `.agents/skills/speckit-tasks/SKILL.md`, `.agents/skills/speckit-implement/SKILL.md`, `.specify/templates/constitution-template.md`, and `.specify/memory/constitution.md`. Update `.specify/integrations/codex.manifest.json` if generated skill hashes are part of the governed integration state.
- **Dependency impact**: None expected. No changes to `Directory.Packages.props`, package versions, or `docs/dependencies.md`.
- **Command-surface impact**: `EvidenceGraph`, `EvidenceAudit`, `GeneratedGuidanceCheck`, and possibly `TemplateDrift` may need changes because the task schema and generated guidance change. `Dev` and `Verify` consume those targets transitively. `PackLocal`, `TemplateCheck`, and `DependencyReport` should remain unchanged unless existing governance wiring requires them.
- **Generated project impact**: Generated product Spec Kit skills and templates must include the `skillist` requirement. Generated products must validate copied local skills and block implementation when a declared task skill is missing, unreadable, or ambiguous.
- **Evidence paths**: Use `specs/014-task-skilllist-governance/readiness/logs/skillist-validation.txt`, `specs/014-task-skilllist-governance/readiness/logs/evidence-graph.txt`, `specs/014-task-skilllist-governance/readiness/logs/evidence-audit.txt`, `specs/014-task-skilllist-governance/readiness/logs/generated-guidance-check.txt`, `specs/014-task-skilllist-governance/readiness/task-skilllist-fixtures.md`, and final readiness notes.
- **`.fsi` / contract impact**: No `.fsi` or public runtime API changes expected. This is a Tier 1 governance contract change because task metadata schema and workflow contracts change.
- **MVU/effect boundary**: Not applicable. The feature changes static workflow guidance and validation, not a stateful runtime or I/O workflow exposed through library APIs. Script I/O remains at existing command boundaries.
- **Synthetic evidence**: Avoid planned synthetic evidence. Fixture task lists for validation are test inputs, not substitutes for production behavior, and should be labeled as validation fixtures in readiness notes.
- **Test evidence**: Add failing-first validation fixtures for missing `skillist`, mismatched mirror values, omitted obvious capability skills, missing skills, unreadable skills, and valid loading records. Add or update generated-guidance tests so templates and skills contain the mandatory rule.
- **Observability**: Diagnostics must identify the task id, field problem, unresolved skill id, mirror mismatch, or omitted obviously applicable skill. Reports should state whether validation blocked readiness or implementation.
- **Deferred scope**: No creation of new capability skills, no renderer/UI behavior, no package distribution changes, no automated migration of external repositories. Existing generated task lists in this repository may be migrated or regenerated only where needed for validation.

### Initial Gate Verdict

PASS. The plan names the changed governance contracts, keeps runtime/API scope bounded, avoids unsupported visual/package changes, and defines evidence paths for the new task readiness and implementation-loading gates.

## Project Structure

```text
specs/014-task-skilllist-governance/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
└── contracts/
    └── task-skilllist-contract.md

.specify/memory/constitution.md
.specify/templates/constitution-template.md
.specify/templates/tasks-template.md
.specify/presets/fsharp-opinionated/templates/tasks-template.md
.specify/presets/fsharp-opinionated/commands/speckit.tasks.md
.specify/presets/fsharp-opinionated/commands/speckit.implement.md
.agents/skills/speckit-tasks/SKILL.md
.agents/skills/speckit-implement/SKILL.md
.specify/extensions/evidence/scripts/python/compute-task-graph.py
```

## Phase 0: Research

Research decisions are captured in `specs/014-task-skilllist-governance/research.md`.

Resolved decisions:

- Treat `skillist` as a required structured field in `tasks.deps.yml`, not only a Markdown convention.
- Mirror `skillist` on each `tasks.md` task line for reviewer visibility.
- Extend evidence graph/readiness validation because it already parses task ids, task status, dependencies, and report output.
- Make implementation skill loading a per-task precondition in `speckit-implement`.
- Update constitution and constitution template in the same change, because this feature adds a mandatory gate.

## Phase 1: Design & Contracts

Design artifacts:

- `specs/014-task-skilllist-governance/data-model.md`
- `specs/014-task-skilllist-governance/contracts/task-skilllist-contract.md`
- `specs/014-task-skilllist-governance/quickstart.md`

Agent context update:

- `AGENTS.md` must point between the Spec Kit markers to `specs/014-task-skilllist-governance/plan.md`.

## Post-Design Constitution Check

PASS. The design keeps the change in governed task/workflow artifacts, makes the new gate enforceable through structured metadata and evidence validation, documents implementation-time loading as a stop condition, and does not introduce runtime APIs, package dependency changes, MVU obligations, visual output, or synthetic-evidence reliance.
