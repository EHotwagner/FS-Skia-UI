# Implementation Plan: Fix Refactor Process Reliability

**Branch**: `012-fix-refactor-process` | **Date**: 2026-05-17 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/012-fix-refactor-process/spec.md`

## Summary

Harden the Controls boundary refactor verification process so broad aggregate
runs can fail honestly when the runner is unhealthy, focused gates remain
directly actionable, governance scanners avoid known false positives, stale
boundary evidence is caught before final audit, and final readiness cannot
present degraded aggregate evidence as product proof.

The implementation changes the repository validation workflow rather than
Controls product behavior. The main surfaces are `build.fsx`, FAKE target
contracts, governance tests, scanner scripts, generated-product validation,
docs, and readiness reports under `specs/012-fix-refactor-process/readiness/`.
Controls public APIs, package ownership, runtime behavior, and generated
consumer semantics remain unchanged unless a process check needs more accurate
evidence about them.

## Technical Context

**Language/Version**: F# on .NET `net10.0`; SDK-style projects; existing FAKE
`build.fsx`; Bash and Windows command wrappers; generated products reference
packages rather than copying framework implementation source.
**Primary Dependencies**: Existing FSharp.Core, Expecto, FAKE tooling,
YamlDotNet where already used for capability/template metadata, and BCL APIs
such as `System.Diagnostics`, `System.IO`, and `System.Xml.Linq`. No new
third-party runtime dependency is planned. Structured scanner fixes should use
XML/project parsing, YAML parsing already present in repo tooling, or anchored
syntax scanning before adding any dependency.
**Storage**: Filesystem only: readiness reports, command logs, generated
product file lists, target-contract evidence, scanner diagnostics, and
process-health snapshots under `specs/012-fix-refactor-process/readiness/`.
**Testing**: Expecto governance tests, command-contract tests, scanner fixture
tests, FAKE target self-checks, focused target smoke evidence, generated
product validation evidence, evidence graph, and evidence audit.
**Target Platform**: Windows and Linux developer/CI environments that can
restore, build, test, pack, and instantiate generated products. Process-health
diagnostics must degrade explicitly when a signal is unsupported on a platform.
**Project Type**: Governed F# framework/template repository with multiple
packable libraries, examples, samples, tests, Spec Kit assets, local/package
agent skills, generated product validation, and repository-owned command
workflow.
**Performance Goals**: Broad preflight summary appears within 30 seconds.
Focused gates remain small enough to diagnose local failures independently.
Broad `Verify`/`Ci` fail fast before high-pressure aggregate work when default
or overridden runner-health thresholds are clearly insufficient.
**Constraints**: Do not change Controls product behavior, public Controls
APIs, package ownership, or generated consumer semantics except where evidence
accuracy directly requires validation/reporting updates. Public F# surfaces
remain governed by `.fsi` files and package surface baselines. Build workflow
state remains MVU-shaped through `BuildModel`, `BuildMsg`, `BuildEffect`,
pure `update`, and interpreter-side effects.
**Scale/Scope**: Tier 2 internal/process reliability work across build targets,
governance tests, scanner scripts, docs, generated-product validation, and
readiness evidence. The work may touch command-surface contracts and evidence
schemas, but it does not add a new public product package or change Controls
runtime contracts.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: PASS. This feature may update template validation,
  generated product inventories, generated guidance checks, and template drift
  evidence. It must not change generated product package ownership or selected
  Controls capability semantics except to make validation more accurate.
- **Dependency impact**: PASS. No new third-party dependency is planned.
  Dependency scanner fixes should use structured project XML parsing or
  anchored dependency syntax. If a new parser dependency becomes necessary, it
  must update `Directory.Packages.props`, `docs/dependencies.md`, and
  readiness dependency evidence before implementation proceeds.
- **Command-surface impact**: PASS. `Verify`, `Ci`, focused check targets,
  `DependencyReport`, `TemplateCheck`, `GeneratedProductCheck`,
  `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, and
  `EvidenceAudit` may change. `Dev` and `PackLocal` should change only if
  bootstrap validation or dependency reporting requires it.
- **Generated project impact**: PASS. Generated-product validation may become
  profile-aware and inventory source/test evidence more completely. Generated
  products must still consume public packages and must not copy framework
  implementation source.
- **Evidence paths**: PASS. Required readiness files are listed in Project
  Structure. Evidence must separate environment failures from product
  failures, preserve focused passing evidence, and block final readiness after
  a broad aggregate environment failure until a later healthy broad pass exists.
- **`.fsi` / contract impact**: PASS. No product `.fsi` or package public API
  change is expected. The contracts for this plan are command and evidence
  contracts, not new F# public product contracts. If implementation discovers a
  public API change is required, the feature must be reclassified and plan
  updated before that change is made.
- **MVU/effect boundary**: PASS. The existing build workflow is stateful and
  I/O-bearing; it must remain modeled through `BuildModel`, `BuildMsg`,
  `BuildEffect`, pure `update`, and interpreter-side process/filesystem
  effects. Process-health collection, bootstrap validation, and verdict
  writing must be represented as effects rather than executed inside `update`.
- **Synthetic evidence**: PASS. Synthetic fixtures may be used only for scanner
  rule tests and seeded stale-reference scenarios. They must be test fixtures
  rather than final readiness proof, and any task-level synthetic evidence must
  follow the constitution disclosure policy.
- **Test evidence**: PASS. Failing-first tests are required for process-health
  verdict classification, focused gate independence, bootstrap warning
  classification, dependency parsing false positives, generated product
  profile allowances, generated product inventory completeness, stale boundary
  scanning scope, and final readiness blocking after environment failure.
- **Observability**: PASS. All failures must name the affected target, rule,
  stage, health signal, override, generated profile, package reference, stale
  file, or readiness evidence path. Environment failures must state the
  recommended rerun environment.
- **Deferred scope**: PASS. Reworking the Controls boundary, restoring the
  legacy Charts package, adding release publishing automation, changing
  external CI providers, automatically migrating external applications, and
  broad runtime performance tuning remain out of scope.

### Constitution Gate Result

PASS. No unresolved clarification markers remain. The primary obligation
is honest validation: broad aggregates fail when evidence is non-authoritative,
focused gates remain actionable, and final readiness waits for a healthy broad
pass after any aggregate environment failure.

## Project Structure

### Documentation (this feature)

```text
specs/012-fix-refactor-process/
|-- spec.md
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   |-- process-health-and-verdicts.md
|   |-- focused-gate-contracts.md
|   |-- governance-scanner-contracts.md
|   |-- generated-product-validation.md
|   `-- readiness-evidence.md
|-- checklists/
|   `-- requirements.md
`-- readiness/
    |-- process-health.md
    |-- focused-gates.md
    |-- governance-scanners.md
    |-- stale-boundary-scan.md
    |-- generated-product-validation.md
    |-- bootstrap-runner.md
    |-- verification-verdicts.md
    |-- evidence-graph.md
    |-- evidence-audit.md
    `-- logs/
```

### Source Code (repository root)

```text
build.fsx
fake.sh
fake.cmd

scripts/
|-- dependency-report.fsx
`-- template-drift.fsx

docs/
|-- build.md
|-- dependencies.md
|-- evidence.md
|-- architecture.md
`-- controls-boundary-refactor-process-report.md

tests/
|-- Governance.Tests/
|   |-- CommandContractTests.fs
|   |-- DependencyGovernanceTests.fs
|   |-- GeneratedProjectValidationTests.fs
|   |-- TemplateDriftTests.fs
|   |-- ArtifactPathTests.fs
|   `-- TestSupport.fs
|-- Package.Tests/
`-- Smoke.Tests/

template/
|-- capabilities.yml
|-- fragments/
`-- base/

readiness/
`-- surface-baselines/
```

## Phase 0: Research

Research decisions are recorded in [research.md](./research.md). All planning
unknowns from the Technical Context are resolved.

## Phase 1: Design And Contracts

Design artifacts are recorded in:

- [data-model.md](./data-model.md)
- [contracts/process-health-and-verdicts.md](./contracts/process-health-and-verdicts.md)
- [contracts/focused-gate-contracts.md](./contracts/focused-gate-contracts.md)
- [contracts/governance-scanner-contracts.md](./contracts/governance-scanner-contracts.md)
- [contracts/generated-product-validation.md](./contracts/generated-product-validation.md)
- [contracts/readiness-evidence.md](./contracts/readiness-evidence.md)
- [quickstart.md](./quickstart.md)

### Post-Design Constitution Check

PASS. The design keeps process-health collection and verdict writing at the
workflow interpreter edge, keeps scanner fixtures separated from final
readiness proof, and does not introduce product public API changes. The
contracts are command/evidence contracts, so `.fsi` and package-surface changes
are not required unless implementation later discovers a product contract
change. Focused evidence remains first-class but cannot replace a required
healthy broad aggregate pass for final readiness.
