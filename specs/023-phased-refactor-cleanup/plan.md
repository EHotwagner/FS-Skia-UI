# Implementation Plan: Phased Refactor Cleanup

**Branch**: `023-phased-refactor-cleanup` | **Date**: 2026-05-27 | **Spec**: `specs/023-phased-refactor-cleanup/spec.md`
**Input**: Feature specification from `/specs/023-phased-refactor-cleanup/spec.md`

## Summary

Perform a Tier 2 behavior-preserving cleanup of localized refactoring hotspots
identified in `docs/2026-05-27-2204-refactoring-analysis.md`. The work keeps
public package signatures, package IDs, generated command names, evidence report
fields, status vocabulary, FAKE target names, generated profile names, exit-code
meanings, and readiness artifact paths stable while consolidating generated
evidence/report behavior and splitting oversized responsibility hubs in phases.

The first delivery slice starts with generated product evidence/report cleanup,
then generated source file splitting, then build governance decomposition, and
finally SkiaViewer internals. Compatibility package restructuring is deferred to
a separate design feature.

## Technical Context

**Language/Version**: F# on .NET `net10.0` for framework packages, generated
templates, governance tests, and FAKE targets.

**Primary Dependencies**: Existing FS.Skia.UI packages, generated product
template, Expecto, FAKE, FSharp.Formatting documentation inputs, and Spec Kit
evidence scripts. No new runtime, test, template, or build dependency is
planned for the first cleanup pass.

**Testing**: Expecto package and governance tests, generated product validation,
FAKE targets, template drift/guidance checks, surface baseline checks, and
phase-specific readiness files. Required checks include `TemplateCheck`,
`GeneratedGuidanceCheck`, `TemplateDrift`, package surface checks, focused
package tests, `EvidenceGraph`, and `EvidenceAudit`; `Dev`, `Verify`, `Ci`,
`PackLocal`, and `DependencyReport` must remain user-facing stable.

**Target Platform**: Windows and Linux for existing viewer/runtime behavior.
Template and governance checks must remain runnable in the repository's normal
development environment. Unsupported screenshot hosts must continue to report
explicit unsupported evidence rather than screenshot success.

**Public Surface**: No public `.fsi` signature, documented public API, sample
contract, surface baseline, package ID, generated profile name, command name, or
FAKE target name is expected to change. Any cleanup that requires a public
contract change must stop and move to a separate Tier 1 feature.

**Evidence Requirement**: Required real evidence paths are:

- `specs/023-phased-refactor-cleanup/readiness/baseline-status.md`
- `specs/023-phased-refactor-cleanup/readiness/generated-evidence-cleanup.md`
- `specs/023-phased-refactor-cleanup/readiness/template-split-validation.md`
- `specs/023-phased-refactor-cleanup/readiness/build-governance-decomposition.md`
- `specs/023-phased-refactor-cleanup/readiness/viewer-internal-boundary.md`

**Synthetic Evidence**: No synthetic evidence is planned. Pre-existing failures
must be recorded in the baseline readiness file before each phase begins.
Unsupported-host viewer or screenshot evidence is a real negative host fact
only when it preserves the existing explicit unsupported classification.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Generated template source and `template/base/src/Product/Product.fsproj`
  compile order will change during the source split. Review
  `.template.config/template.json` only if template inclusion or file packaging
  changes. Generated docs/tests update only to preserve the same public command
  and report contracts after file movement.
- **Dependency impact**: PASS with no planned dependency changes. Do not edit
  `Directory.Packages.props`, template package pins, `docs/dependencies.md`, or
  dependency report expectations unless decomposition reveals an existing
  dependency-governance defect.
- **Command-surface impact**: `build.fsx` remains the stable FAKE command
  surface. `TemplateCheck`, `GeneratedGuidanceCheck`, `TemplateDrift`, package
  surface checks, focused tests, `EvidenceGraph`, and `EvidenceAudit` validate
  preservation. `Dev`, `Verify`, `Ci`, `PackLocal`, and `DependencyReport`
  remain available under the same names and semantics.
- **Generated project impact**: Generated profile outputs remain buildable and
  testable. `Program.fs` is reduced to launch and command dispatch after the
  split; product model, rendering description, layout evidence, evidence
  commands, and window options move to responsibility-specific generated files.
- **Evidence paths**: Required readiness paths are listed in Technical Context.
  Every phase must write baseline and final check results to the relevant file.
- **`.fsi` / contract impact**: PASS as Tier 2. Public signatures and baselines
  remain untouched. If a public API change appears necessary, stop and create a
  separate Tier 1 plan with `.fsi`-first design.
- **MVU/effect boundary**: PASS with preservation constraints. Product state
  workflows, commands, effects, subscriptions, and host interpretation semantics
  remain behaviorally unchanged; only ownership boundaries move.
- **Synthetic evidence**: PASS. No synthetic proof is needed. Existing real
  negative unsupported-host classifications remain valid if explicitly reported.
- **Test evidence**: Run the smallest relevant checks before each phase to
  record pre-existing failures, then run phase-specific checks after changes.
  Acceptance requires evidence that commands, reports, generated outputs, and
  viewer behavior remain stable.
- **Observability**: Report fields, readiness paths, failure messages,
  unsupported-host diagnostics, and missing-artifact classifications remain
  stable and actionable.
- **Deferred scope**: Compatibility package restructuring, public API removal,
  package strategy migration, UI model redesign, runtime replacement, broad
  release automation rewrite, shared utility package creation, and generated
  profile collapse are out of scope.

**Pre-design gate result**: PASS. The feature is Tier 2, has no planned public
contract or dependency change, preserves MVU/effect boundaries, records real
phase evidence, and defers compatibility/package strategy decisions.

## Project Structure

```text
template/base/src/Product/
  Product.fsproj                  # F# compile order for generated split files
  Model.fs                        # Generated product model and update state
  View.fs                         # Rendering description/view construction
  LayoutEvidence.fs               # Layout/readability evidence helpers
  EvidenceCommands.fs             # CLI evidence command implementations
  WindowOptions.fs                # Viewer/window behavior options
  Program.fs                      # Entrypoint and command dispatch only

scripts/build/                    # Proposed loaded script modules for later phases
  Paths.fsx
  Process.fsx
  Reports.fsx
  TemplateValidation.fsx
  GeneratedScanning.fsx
  PackageResolution.fsx
  ProcessHealth.fsx

src/SkiaViewer/
  SkiaViewer.fsi                  # Only public facade signature; unchanged
  SkiaViewer.fs                   # Public facade implementation remains signed
  HostCapability.fs               # Implementation detail; no new public signed module
  WindowBehavior.fs               # Implementation detail; no new public signed module
  VisualEvidence.fs               # Implementation detail; no new public signed module
  SceneConversion.fs              # Implementation detail; no new public signed module

tests/
  Governance.Tests/
  Testing.Tests/
  SkiaViewer.Tests/

specs/023-phased-refactor-cleanup/
  plan.md
  research.md
  data-model.md
  quickstart.md
  contracts/
    behavior-preservation-contract.md
    generated-template-contract.md
    build-governance-contract.md
    viewer-internal-boundary-contract.md
  readiness/
```

## Phase 0: Research

Research is complete in `specs/023-phased-refactor-cleanup/research.md`. Key
decisions:

- Preserve all public command, package, generated, report, readiness, and
  surface contracts during the cleanup.
- Classify duplication before consolidation so intentional generated/template
  or package-boundary copies are not removed incorrectly.
- Split generated product source only after evidence/report behavior is first
  centralized in the current generated shape.
- Decompose `build.fsx` into loaded scripts while keeping target registration
  and dependency wiring in the entrypoint.
- Split SkiaViewer internals behind the unchanged `.fsi` public facade.
- Defer compatibility package strategy to a later Tier 1 design decision.

## Phase 1: Design and Contracts

Design artifacts produced:

- `specs/023-phased-refactor-cleanup/research.md`
- `specs/023-phased-refactor-cleanup/data-model.md`
- `specs/023-phased-refactor-cleanup/contracts/behavior-preservation-contract.md`
- `specs/023-phased-refactor-cleanup/contracts/generated-template-contract.md`
- `specs/023-phased-refactor-cleanup/contracts/build-governance-contract.md`
- `specs/023-phased-refactor-cleanup/contracts/viewer-internal-boundary-contract.md`
- `specs/023-phased-refactor-cleanup/quickstart.md`

### Post-Design Constitution Check

- **Spec -> FSI -> tests -> implementation**: PASS as Tier 2. No public `.fsi`
  changes are planned; if one becomes necessary, this feature stops and the
  change moves to Tier 1.
- **Visibility in `.fsi`**: PASS. New internal modules stay private by omission
  from public `.fsi` surfaces.
- **Idiomatic simplicity**: PASS. The design prefers file/module extraction and
  local helpers over new abstractions or shared packages.
- **MVU/effect boundary**: PASS. Product and viewer state/effect behavior stays
  unchanged; the cleanup only separates ownership.
- **Synthetic disclosure**: PASS. No synthetic evidence is planned or needed.
- **Test evidence**: PASS. Quickstart names baseline and phase-specific checks
  and required readiness files.
- **Observability and safe failure**: PASS. Existing report fields, failure
  messages, unsupported classifications, and readiness paths are preserved.

## Phase 2: Planning Boundary

Stop after design. Task generation should produce dependency-ordered,
phase-grouped tasks with `skillist` metadata, required readiness files,
pre-phase baseline checks, implementation tasks, post-phase verification, and
explicit acceptance criteria for unchanged public behavior. Tasks touching
generated game/layout evidence must include `fs-skia-layout-evidence`; tasks
touching generated template packaging should include `fs-skia-template-update`;
tasks touching SkiaViewer internals should include `fs-skia-skiaviewer` when
that capability skill is present in the repo skill inventory.
