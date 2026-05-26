# Implementation Plan: Synthetic Error Evidence

**Branch**: `017-synthetic-error-evidence` | **Date**: 2026-05-26 | **Spec**: `specs/017-synthetic-error-evidence/spec.md`
**Input**: Feature specification from `/specs/017-synthetic-error-evidence/spec.md`

## Summary

Add a narrow, design-approved synthetic error-handling classification for Spec Kit tasks. Malformed-input and explicit error-path tasks may be tagged `[SEH]`, labeled `synthetic-error-handling-approved`, completed as `[S]`, and still allow `EvidenceAudit` to return PASS when every synthetic task is valid `[SEH]`. The classification must be made during design, planning, clarification, or task generation; implementation-time relabeling remains a readiness failure.

This is a Tier 1 governance-contract change. It changes the Spec Kit task contract, synthetic evidence policy, evidence graph/audit interpretation, generated task guidance, implementation guidance, documentation, fixtures, and readiness artifacts. It does not change runtime packages, public `.fsi` surfaces, rendering behavior, or generated product package contents.

## Technical Context

**Language/Version**: F# on .NET `net10.0` for governance tests and FAKE targets; Markdown/YAML/JSON for Spec Kit artifacts  
**Primary Dependencies**: Existing FAKE, Expecto, Spec Kit shell scripts, evidence extension scripts; no new package dependency planned  
**Testing**: Expecto governance tests, evidence extension fixture runs, `GeneratedGuidanceCheck`, `EvidenceGraph`, `EvidenceAudit`, focused docs/template checks  
**Target Platform**: Repository governance workflow on Windows and Linux shell-compatible hosts; no product runtime platform change  
**Public Surface**: No `.fsi` package surface change. Public governance surface changes include `.specify/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/commands/speckit.tasks.md`, `.specify/presets/fsharp-opinionated/commands/speckit.implement.md`, `.specify/memory/constitution.md`, generated guidance checks, and evidence audit outputs.  
**Evidence Requirement**: Completion requires real command evidence for guidance updates, valid/invalid `[SEH]` fixtures, graph rendering, audit PASS for approved `[SEH]`, audit rejection for late or non-eligible cases, and documentation/readiness review.  
**Synthetic Evidence**: The feature uses synthetic malformed-input fixtures by design. Those fixture tasks must be tagged `[SEH]`, remain `[S]`, disclose the synthetic input class, and prove that approved `[SEH]` can pass audit while ordinary `[S]` remains blocking.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Update `.specify/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/commands/speckit.tasks.md`, `.specify/presets/fsharp-opinionated/commands/speckit.implement.md`, and documentation that explains synthetic evidence. Generated product templates are not expected to change.
- **Dependency impact**: No dependency changes are planned. `Directory.Packages.props`, generated package inclusion, package versions, and `docs/dependencies.md` should remain unchanged unless an existing governance test unexpectedly couples to them. `DependencyReport` is not a required evidence path.
- **Command-surface impact**: `EvidenceGraph`, `EvidenceAudit`, and `GeneratedGuidanceCheck` must change or gain coverage. `Dev`, `Verify`, and `Ci` may change only if they aggregate these targets or need expected-output updates.
- **Generated project impact**: No default/minimal generated product contents change. Generated Spec Kit task guidance changes so future task lists can emit `[SEH]`, `synthetic-error-handling-approved`, and required inventory fields before implementation.
- **Evidence paths**: Required readiness paths are:
  - `specs/017-synthetic-error-evidence/readiness/seh-classification-rules.md`
  - `specs/017-synthetic-error-evidence/readiness/task-generation-seh.md`
  - `specs/017-synthetic-error-evidence/readiness/audit-accepted-seh.md`
  - `specs/017-synthetic-error-evidence/readiness/audit-rejects-late-seh.md`
  - `specs/017-synthetic-error-evidence/readiness/non-eligible-synthetic-cases.md`
  - `specs/017-synthetic-error-evidence/readiness/generated-guidance-check.md`
  - `specs/017-synthetic-error-evidence/readiness/evidence-graph.md`
  - `specs/017-synthetic-error-evidence/readiness/evidence-audit.md`
- **`.fsi` / contract impact**: No `.fsi`, package API, sample contract, or surface baseline change is planned. The contract change is to task metadata, inventory rows, and evidence audit report semantics.
- **MVU/effect boundary**: Product MVU does not apply. Governance workflow state does apply: task classification state, provenance timing, audit verdict state, and fixture acceptance/rejection states must be modeled in tests and reports.
- **Synthetic evidence**: PASS with deliberate exception. The feature is about synthetic malformed-input/error-path evidence. `[SEH]` tasks remain `[S]`, use loud disclosure, and may make `EvidenceAudit` PASS only when every synthetic task is valid design-approved `[SEH]`. Ordinary synthetic evidence, late classification, and convenience fixtures remain blocking.
- **Test evidence**: Add failing-first governance tests for eligible `[SEH]`, non-eligible synthetic cases, late reclassification, inventory requirements, graph/report counts, audit PASS for all-approved `[SEH]`, and audit rejection when any unapproved `[S]` or `[S*]` remains.
- **Observability**: Evidence outputs must report accepted synthetic error-handling count, unaccepted synthetic count, late classification failures, missing rationale fields, label/tag mismatches, design-phase source, and final verdict.
- **Deferred scope**: This feature does not approve arbitrary synthetic evidence, remove `[S]`/`[S*]`, change runtime packages, alter generated app behavior, expand platform support, or replace the broader `--accept-synthetic` human override.

**Pre-design gate result**: PASS. The feature intentionally modifies Principle V behavior for a narrow class. The plan treats this as a governance-contract change and requires updates to constitution text, guidance, audit checks, and readiness fixtures before implementation.

## Project Structure

```text
.specify/
  memory/constitution.md                         # Principle V narrow [SEH] exception
  templates/tasks-template.md                    # Canonical task status and [SEH] guidance
  presets/fsharp-opinionated/
    templates/tasks-template.md                  # Preset task template mirror
    commands/speckit.tasks.md                    # Task generation command guidance
    commands/speckit.implement.md                # Implementation-time prohibition
  extensions/evidence/                           # Evidence graph/audit scripts and fixture handling

tests/Governance.Tests/
  *.fs                                           # Guidance, graph, and audit behavior coverage

docs/
  evidence.md                                    # Synthetic evidence and [SEH] policy
  speckit.md                                     # Workflow guidance if needed

specs/017-synthetic-error-evidence/
  plan.md
  research.md
  data-model.md
  quickstart.md
  contracts/
    synthetic-error-evidence-contract.md
    evidence-audit-contract.md
  readiness/
```

## Phase 0: Research

Research is complete in `specs/017-synthetic-error-evidence/research.md`. Key decisions:

- Use `[SEH]` as a task annotation plus `synthetic-error-handling-approved` as the inventory/task metadata label.
- Preserve `[S]` as the task completion status for approved synthetic error-handling evidence.
- Make audit PASS possible only when every synthetic task is valid design-approved `[SEH]`.
- Track design-phase provenance explicitly so implementation-time reclassification can be rejected.
- Keep ordinary synthetic evidence, convenience fixtures, unsupported-host substitutes, and missing real product evidence outside the `[SEH]` exception.

## Phase 1: Design and Contracts

Design artifacts produced:

- `specs/017-synthetic-error-evidence/data-model.md`
- `specs/017-synthetic-error-evidence/contracts/synthetic-error-evidence-contract.md`
- `specs/017-synthetic-error-evidence/contracts/evidence-audit-contract.md`
- `specs/017-synthetic-error-evidence/quickstart.md`

### Post-Design Constitution Check

- **Spec -> FSI -> tests -> implementation**: PASS with scope note. No `.fsi` package surface is changed; failing-first governance tests and fixture contracts are required before implementation.
- **Visibility in `.fsi`**: PASS. No public F# module surface is planned. If implementation creates new public F# modules, planning must be updated with `.fsi` and baseline tasks.
- **Idiomatic simplicity**: PASS. Expected changes are parsers, records, discriminated report states, and explicit validation functions. No complex F# feature is required.
- **MVU/effect boundary**: PASS by non-applicability. This is not a product stateful workflow. Audit state transitions are documented in the data model and should be tested as pure validation/report transitions.
- **Synthetic disclosure**: PASS with required constitution update. `[SEH]` is a narrow formal exception that keeps `[S]` disclosure while changing audit verdict semantics for approved error-handling evidence.
- **Test evidence**: PASS. The plan requires failing-first governance tests and fixture evidence for accepted, rejected, and late-classified cases.
- **Observability and safe failure**: PASS. Missing rationale, missing label, late classification, and non-eligible synthetic categories must produce actionable audit diagnostics.

## Phase 2: Planning Boundary

Stop after design. Task generation should convert this plan into dependency-ordered tasks with `skillist` metadata. At minimum, implementation tasks that touch governance scripts, generated guidance, or evidence audit behavior must include the relevant Spec Kit and evidence skills when available; repository capability skills under `src/*/skill/SKILL.md` are not expected to apply unless implementation unexpectedly touches product packages.
