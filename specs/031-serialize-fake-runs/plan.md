# Implementation Plan: Serialize FAKE Runs

**Branch**: `031-serialize-fake-runs` | **Date**: 2026-05-29 | **Spec**: `specs/031-serialize-fake-runs/spec.md`
**Input**: Feature specification from `specs/031-serialize-fake-runs/spec.md`

## Summary

Make repository and generated-product validation guidance explicitly serialize all FAKE-backed tests and FAKE targets because they share `.fake` state in this repository. The approach is to update agent-facing guidance, maintainer docs, generated template docs, and validation/evidence contracts so every workflow that asks for multiple FAKE-backed commands presents a deterministic order, records that order in readiness evidence, and tells contributors to rerun suspected race-affected commands sequentially before treating a failure as a product regression.

## Technical Context

**Language/Version**: F# on .NET `net10.0` for validation code; Markdown/YAML for repository, agent, and generated-template guidance.
**Primary Dependencies**: Existing Expecto tests, FAKE target graph, Spec Kit extension scripts, repository/template documentation, and generated product guidance. No new package or runtime dependency is planned.
**Testing**: Focused Expecto governance tests for guidance text and generated artifact content; FAKE targets run sequentially for evidence (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`, `Verify` as needed). Readiness evidence must record the exact FAKE-backed command order.
**Target Platform**: Windows and Linux command guidance. Runtime UI behavior, package identity, rendering, and public framework APIs are out of scope.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Template source docs and generated agent guidance are in scope when they mention `./fake.sh`, `fake.cmd`, generated `Dev`, `Test`, `Verify`, `EvidenceGraph`, or `EvidenceAudit`. Update `.template.config/template.json` only if new or moved template files must be included; otherwise preserve package/template identities.
- **Dependency impact**: No dependency changes. `Directory.Packages.props`, `docs/dependencies.md`, package references, and `DependencyReport` inputs should remain unchanged.
- **Command-surface impact**: Existing FAKE targets stay functionally sequential when invoked by a single FAKE run. Guidance and validation may change around `Dev`, `Verify`, `Ci`, `TemplateCheck`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit`, but the feature must not introduce concurrent FAKE target execution.
- **Generated project impact**: Generated README/product docs, local agent skills, and generated validation instructions must say FAKE-backed targets are serialized. Generated `Dev`, `Test`, `Verify`, `EvidenceGraph`, and `EvidenceAudit` behavior should not otherwise change.
- **Evidence paths**: Required readiness path is `specs/031-serialize-fake-runs/readiness/sequential-fake-validation.md`. Implementation should also refresh or produce `specs/031-serialize-fake-runs/readiness/guidance-scan.md`, `specs/031-serialize-fake-runs/readiness/fake-command-order.md`, `specs/031-serialize-fake-runs/readiness/evidence-graph.md`, and `specs/031-serialize-fake-runs/readiness/evidence-audit.md` when the related targets are run.
- **`.fsi` / contract impact**: No public F# signatures, surface baselines, public docs for framework APIs, or compatibility notes are expected. If implementation discovers reusable public F# code is necessary, stop and add `.fsi`, semantic tests, and baselines before implementation.
- **MVU/effect boundary**: No product state workflow changes. Validation guidance scanning may be represented as plain inputs, findings, and report outputs in build/test code; any filesystem/process effects remain at the existing build-script or test edge.
- **Synthetic evidence**: Successful readiness must be real guidance scans and real sequential command logs. Synthetic fixtures are allowed only for negative text-scanner cases and must be disclosed if represented as task evidence.
- **Test evidence**: Add failing-first guidance tests or generated-guidance checks proving updated instructions name FAKE-backed tests, FAKE targets, `.fake`, and sequential execution. Run all FAKE-backed validation commands for this feature one at a time and record the order.
- **Observability**: Failure triage guidance and readiness notes must identify command, start/end order, suspected concurrent FAKE context, `.fake` race classification, and next action: rerun the affected FAKE-backed commands sequentially before product debugging.
- **Deferred scope**: Broader build graph redesign, release packaging, package publishing, runtime UI/rendering, visual evidence, platform support expansion, and new non-FAKE parallelism automation are deferred.

**Gate result before Phase 0**: PASS. The feature is scoped to guidance, generated artifacts, tests, and readiness evidence; no unresolved clarification or constitution violation remains.

## Project Structure

### Feature Artifacts

```text
specs/031-serialize-fake-runs/
|-- spec.md
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   |-- guidance-contract.md
|   |-- readiness-evidence.md
|   `-- generated-product-guidance.md
`-- readiness/
    |-- sequential-fake-validation.md
    |-- guidance-scan.md
    |-- fake-command-order.md
    |-- evidence-graph.md
    `-- evidence-audit.md
```

### Source And Documentation Touch Points

```text
AGENTS.md
CLAUDE.md
README.md
docs/architecture.md
docs/build.md
docs/testing.md
docs/evidence.md
.agents/skills/*/SKILL.md
.claude/skills/*/SKILL.md
.claude/commands/*.md
.specify/templates/*.md
template/base/README.md
template/base/docs/product.md
template/base/.agents/skills/fs-skia-project/SKILL.md
template/base/.claude/skills/fs-skia-project/SKILL.md
template/profiles/*.yml
build.fsx
tests/Governance.Tests/
tests/Package.Tests/
```

## Phase 0 Research

Research is captured in `specs/031-serialize-fake-runs/research.md`.

## Phase 1 Design

Design entities are captured in `specs/031-serialize-fake-runs/data-model.md`.

Contracts are captured in:

- `specs/031-serialize-fake-runs/contracts/guidance-contract.md`
- `specs/031-serialize-fake-runs/contracts/readiness-evidence.md`
- `specs/031-serialize-fake-runs/contracts/generated-product-guidance.md`

Quickstart validation is captured in `specs/031-serialize-fake-runs/quickstart.md`.

## Constitution Check Post-Design

- **Spec -> FSI -> semantic tests -> implementation**: PASS. No public F# API is planned; the contracts require stopping for `.fsi`, semantic tests, and baselines if implementation creates public F# surface.
- **Visibility lives in `.fsi`**: PASS. Planned work is guidance, generated artifacts, and validation checks, not public module surface.
- **Idiomatic simplicity**: PASS. Use existing Markdown/YAML guidance and existing Expecto/build validation patterns; no complex language features are planned.
- **MVU/effect boundary**: PASS. No product I/O workflow changes. Guidance validation is modeled as files in, findings/reports out, with filesystem/process effects at existing validation edges.
- **Synthetic evidence disclosure**: PASS. Successful evidence must be real guidance scans and real sequential command logs; synthetic negative fixtures cannot satisfy readiness.
- **Test evidence mandatory**: PASS. The plan requires failing-first governance/generated-guidance checks and sequential FAKE-backed validation evidence.
- **Observability and safe failure**: PASS. Contracts require race-like failure notes to name command order, concurrent FAKE suspicion, `.fake` risk, and the sequential rerun action.
