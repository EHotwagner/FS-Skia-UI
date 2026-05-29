# Implementation Plan: Agent Validation Framework

**Branch**: `028-agent-validation-framework` | **Date**: 2026-05-28 | **Spec**: `specs/028-agent-validation-framework/spec.md`
**Input**: Feature specification from `/specs/028-agent-validation-framework/spec.md`

## Summary

Add a first-class agent validation framework for FS.Skia.UI. The implementation will introduce a machine-readable validation contract that maps changed paths and feature concerns to the smallest authoritative gates, add an `AgentReady` validation path that prefers active feature metadata and falls back to git merge-base diff, emit one consolidated verdict for agent/reviewer handoff, separate generated app evidence policy from normal launch, add typed front doors and schema diagnostics for every existing standard controls module, and migrate in-scope validation targets to native FAKE target registration while preserving testable planning metadata and stable command names.

This is a Tier 1 contracted framework/governance change. It affects public controls contracts, build target contracts, generated template behavior, command metadata, validation evidence, docs, and readiness obligations. It does not publish packages, remove compatibility APIs, replace the renderer, add new platform support, or change normal product MVU semantics.

## Technical Context

**Language/Version**: F# on .NET `net10.0` for framework packages, build targets, governance tests, generated product tests, and generated app code. Existing Spec Kit evidence extensions remain Bash/Python script assets.

**Primary Dependencies**: Existing FAKE build stack, Expecto, FS.Skia.UI Controls/Testing/generated template assets, Spec Kit evidence scripts, and local NuGet package validation. No new external package dependency is planned.

**Testing**: Expecto governance and semantic tests, FSI transcripts for public controls surface additions, package surface baseline checks, generated product tests, template validation, target metadata drift tests, validation contract routing tests, `AgentReady`, `EvidenceGraph`, `EvidenceAudit`, `Verify`, and `Ci` aggregation as applicable.

**Target Platform**: Windows and Linux development hosts. Desktop/screenshot support is not expanded by this feature; unsupported host outcomes must be classified separately from product failures.

**Public Surface**: Additive `.fsi` changes are expected in `src/Controls/*` for typed control kinds, typed event kinds, typed/common attribute values, standard control schema access, and custom extension escape hatches. Public build command names remain stable while target registration migrates. Generated template command behavior and docs are public generated-consumer contracts.

**Evidence Requirement**: Required real evidence paths are:

- `specs/028-agent-validation-framework/readiness/validation-contract.md`
- `specs/028-agent-validation-framework/readiness/agent-ready-verdict.md`
- `specs/028-agent-validation-framework/readiness/target-metadata.md`
- `specs/028-agent-validation-framework/readiness/evidence-policy-separation.md`
- `specs/028-agent-validation-framework/readiness/typed-controls-front-door.md`
- `specs/028-agent-validation-framework/readiness/environment-failure-classification.md`
- `specs/028-agent-validation-framework/readiness/evidence-graph.md`
- `specs/028-agent-validation-framework/readiness/evidence-audit.md`

**Synthetic Evidence**: Synthetic malformed contract, metadata, graph, and verdict fixtures may be used only for rejection/error-handling tests with `[SEH]` task labeling and disclosure. Successful routing, metadata drift, generated launch separation, typed controls, and agent verdict proof must come from real repository files, real generated template artifacts, real command outputs, or real compile-time/semantic checks.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Generated project changes are expected in `template/base/build.fsx`, `template/base/src/Product/Program.fs`, `template/base/src/Product/EvidenceCommands.fs`, possibly a new generated product evidence adapter module, generated tests under `template/base/tests/Product.Tests`, profile/fragments guidance, and `.template.config/template.json` only if generated file membership changes.
- **Dependency impact**: PASS. No new dependency is planned. If implementation adds a package, update `Directory.Packages.props`, generated package pins, `docs/dependencies.md`, and `DependencyReport`.
- **Command-surface impact**: `build.fsx` must add `AgentReady` or an equivalent stable command, validation contract loading, target metadata discovery, metadata drift validation, native FAKE target registration for in-scope validation targets, and compatibility for existing command names. `GeneratedGuidanceCheck`, `TemplateCheck`, `PackageSurfaceCheck`, `FsiTranscripts`, `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`, `Verify`, and `Ci` are in scope. `Dev`, `PackLocal`, `DependencyReport`, and `TemplateDrift` change only for direct metadata, package-content, or drift-validation requirements.
- **Generated project impact**: Normal launch remains persistent and interactive. Evidence commands become explicit policy workflows that write governed reports only when invoked. Generated controls guidance must use typed standard front doors by default and reserve custom paths for deliberate extension scenarios.
- **Evidence paths**: Required readiness files are listed in Technical Context and must be discoverable by tasks, validation contract outputs, target metadata, evidence graph, and evidence audit.
- **`.fsi` / contract impact**: Public controls changes must follow `.fsi` first, then FSI/semantic tests, then `.fs` implementation, surface baseline updates, docs, and generated guidance. Build/validation contracts are captured under `contracts/`.
- **MVU/effect boundary**: Agent validation and generated evidence workflows are I/O-bearing. The design must expose a pure selection/verdict model (`ValidationSelectionModel`, messages, effects, `init`, `update`) and keep git diff, metadata reads, process execution, report writes, and generated product execution at the interpreter edge. Normal product app MVU remains separate.
- **Synthetic evidence**: PASS with restrictions. Negative fixtures for malformed contracts/metadata/verdicts are allowed only as `[SEH]`; final agent-ready proof cannot be based on canned success reports.
- **Test evidence**: Add failing-first tests for changed-path routing, degraded fallback, consolidated verdict fields, failure classification, target metadata drift, native target/discovery parity, generated launch separation, typed standard controls rejection, custom extension distinction, and schema-backed diagnostics.
- **Observability**: Reports must include selected rule ids, authority level, changed-path source, required/completed/missing gates, missing artifacts, failure owner, environment/prerequisite classification, next command, stale prerequisite remediation, and diagnostics.
- **Deferred scope**: New game mechanics, renderer redesign, package publishing, new platform support, browser/mobile screenshot capture, screenshot contract replacement, command-name removal, and public compatibility API removal are out of scope.

**Pre-design gate result**: PASS. The plan preserves `.fsi`-first public contracts, MVU/effect separation for command routing, no-new-dependency intent, stable command names, real evidence requirements, and explicit unsupported/degraded classification.

## Project Structure

```text
build.fsx
  # Native FAKE target registration, AgentReady, metadata discovery, verdict output

validation.contract.yml
  # Machine-readable changed-path and feature-concern validation routing

src/Controls/
  Types.fsi/.fs                 # Typed control/event/attribute value primitives
  Control.fsi/.fs               # Typed standard creation/lowering compatibility
  Attributes.fsi/.fs            # Typed event/attribute front doors and custom escape hatch
  Charts.fsi/.fs                # Typed chart data attributes
  DataGrid.fsi/.fs              # Typed grid data attributes
  Catalog.fsi/.fs               # Schema-backed known control registry
  Diagnostics.fsi/.fs           # Schema-backed missing/unsupported diagnostics

template/base/
  build.fsx
  src/Product/Program.fs        # Normal launch and thin dispatch only
  src/Product/EvidenceCommands.fs
  tests/Product.Tests/Tests.fs

docs/
  build.md
  controls.md
  evidence.md
  generated-apps.md
  testing.md

tests/
  Controls.Tests/
  Governance.Tests/
  Package.Tests/

specs/028-agent-validation-framework/
  plan.md
  research.md
  data-model.md
  quickstart.md
  contracts/
    validation-contract.md
    agent-verdict-contract.md
    target-metadata-contract.md
    typed-controls-contract.md
    generated-evidence-policy-contract.md
  readiness/
```

Typed controls coverage includes all existing standard Controls modules. Modules
not receiving dedicated typed front-door files, such as `Accessibility`,
`Collections`, `ControlRuntime`, `CustomControl`, `RichText`, `TextInput`, and
`Theme`, are covered through shared schema, catalog, and diagnostics work unless
implementation discovers a module-specific typed API is required.

## Phase 0: Research

Research is complete in `specs/028-agent-validation-framework/research.md`.
Key decisions:

- The validation contract should live as repository-owned YAML and be validated by governance tests against runnable targets, target metadata, docs, and representative changed-path scenarios.
- `AgentReady` should select focused gates from active feature metadata first, fall back to git merge-base diff, and degrade explicitly to a broad fallback when changed-path context is unavailable.
- The consolidated verdict should be both machine-readable JSON and reviewer-readable Markdown, with the JSON treated as the authoritative compact handoff.
- Generated app evidence policy should move out of normal app launch; product-owned facts remain in product modules while policy orchestration/report wording lives in explicit evidence commands and build targets.
- Typed controls should be additive and compatibility-preserving: known control kinds/events/attributes receive typed front doors, while custom usage remains possible through visibly named custom APIs.
- Native FAKE target registration should replace the custom runner for runnable validation targets, while pure metadata records preserve testability and drift checks.

## Phase 1: Design and Contracts

Design artifacts produced:

- `specs/028-agent-validation-framework/research.md`
- `specs/028-agent-validation-framework/data-model.md`
- `specs/028-agent-validation-framework/contracts/validation-contract.md`
- `specs/028-agent-validation-framework/contracts/agent-verdict-contract.md`
- `specs/028-agent-validation-framework/contracts/target-metadata-contract.md`
- `specs/028-agent-validation-framework/contracts/typed-controls-contract.md`
- `specs/028-agent-validation-framework/contracts/generated-evidence-policy-contract.md`
- `specs/028-agent-validation-framework/quickstart.md`

### Post-Design Constitution Check

- **Spec -> FSI -> tests -> implementation**: PASS. Public controls additions are planned as `.fsi`-first work with FSI transcripts, semantic tests, and surface baseline updates. Build/validation behavior is captured in contracts.
- **Visibility in `.fsi`**: PASS. Public controls symbols must be present in `.fsi`; implementation files must avoid top-level visibility modifiers.
- **Idiomatic simplicity**: PASS. The planned model uses records, discriminated unions, simple YAML/JSON contracts, and existing FAKE/Expecto infrastructure. No SRTP, reflection, type providers, custom operators, or complex computation expressions are required.
- **MVU/effect boundary**: PASS. Agent validation routing and generated evidence orchestration are explicitly modeled as pure selection/verdict decisions plus edge effects.
- **Synthetic disclosure**: PASS with restrictions. Synthetic fixtures are limited to malformed/error-path rejection and require `[SEH]` handling; authoritative success evidence must be real.
- **Test evidence**: PASS. Quickstart names failing-first tests, focused target runs, generated product checks, package surface checks, evidence graph, and audit outputs.
- **Observability and safe failure**: PASS. Contracts require explicit degraded, unsupported, environment, stale prerequisite, product, template, governance, and missing-evidence classifications.

## Phase 2: Planning Boundary

Stop after design. Task generation should produce dependency-ordered tasks with `skillist` metadata, `.fsi`-first typed controls work, failing-first routing/verdict/metadata tests, native target migration, generated evidence policy separation, docs/guidance updates, readiness evidence files, evidence graph validation, and final evidence audit.
