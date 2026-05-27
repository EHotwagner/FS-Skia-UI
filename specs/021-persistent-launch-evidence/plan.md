# Implementation Plan: Persistent Launch Evidence

**Branch**: `021-persistent-launch-evidence` | **Date**: 2026-05-27 | **Spec**: `specs/021-persistent-launch-evidence/spec.md`
**Input**: Feature specification from `/specs/021-persistent-launch-evidence/spec.md`

## Summary

Add a framework-supported persistent-launch evidence path for generated
graphical apps. The evidence path must open the real interactive SkiaViewer
window on supported desktop hosts, prove that a first frame was presented,
record viewer-owned window and input facts, exercise a controlled evidence close
path, and write the machine-readable fields required by EvidenceAudit. The
normal generated app launch remains persistent and user-driven; evidence mode is
an explicit viewer/build-target operation.

This is a Tier 1 contracted framework, generated-template, and governance
change. It may affect `src/SkiaViewer/SkiaViewer.fsi`,
`src/Testing/Testing.fsi`, surface baselines, generated product guidance, FAKE
targets, template content, and readiness audit rules. Deterministic gameplay or
layout evidence remains separate from persistent-window evidence and must not be
presented as screenshot or visible-window proof.

## Technical Context

**Language/Version**: F# on .NET `net10.0` for framework packages, generated
templates, governance tests, and FAKE targets.

**Primary Dependencies**: Existing FS.Skia.UI SkiaViewer, Scene, KeyboardInput,
Testing, Elmish-style viewer workflow, SkiaSharp 4 preview stack, Expecto, FAKE,
and Spec Kit evidence scripts. No new runtime dependency is planned. External
window tools such as `wmctrl` or `xdotool` may remain optional diagnostics, but
accepted evidence must prefer viewer-native facts.

**Testing**: Expecto semantic tests through `.fsi`, FSI transcripts, generated
product tests, governance tests, generated readiness workflow checks, FAKE
targets (`Verify`, generated `Test`, `GeneratedProductCheck`,
`GeneratedGuidanceCheck`, `TemplateCheck`, `EvidenceGraph`, `EvidenceAudit`),
and supported-host persistent-launch readiness artifacts.

**Target Platform**: Windows and Linux graphical desktop sessions. Hosts without
desktop prerequisites must be classified as unsupported at the exact prerequisite
stage. Hosts with desktop prerequisites and a persistent process that remains
alive must not be labeled headless-only merely because external window
observation fails.

**Public Surface**: Review `src/SkiaViewer/SkiaViewer.fsi` for persistent-launch
request/result, window identity, first-frame, input-dispatch, controlled-close,
and artifact serialization contracts. Review `src/Testing/Testing.fsi` for
generated validation and host warning classification contracts. Update surface
baselines, docs, and samples together with any `.fsi` changes.

**Evidence Requirement**: Required real evidence paths are:

- `specs/021-persistent-launch-evidence/readiness/persistent-launch-evidence.md`
- `specs/021-persistent-launch-evidence/readiness/window-observation-diagnostics.md`
- `specs/021-persistent-launch-evidence/readiness/host-warning-classification.md`
- `specs/021-persistent-launch-evidence/readiness/generated-guidance.md`
- `specs/021-persistent-launch-evidence/readiness/evidence-audit.md`

**Synthetic Evidence**: Synthetic window facts are not acceptable for a passing
supported-host persistent-launch artifact. Synthetic malformed-output fixtures
may validate parser/classifier error handling only when disclosed under the
constitution. External observation failures may be recorded as real negative
facts, but they cannot override viewer-owned first-frame/window facts or user-
visible success into a headless-only classification.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Generated template source, generated docs, product
  tests, and template inclusion policy must update when evidence commands,
  generated host names, reducer qualification, readiness files, or launch
  scripts change. Review `.template.config/template.json` if new files are
  included in generated output.
- **Dependency impact**: PASS with no planned new dependency. If a platform
  helper package becomes necessary, update `Directory.Packages.props`,
  template package pins, `docs/dependencies.md`, and `DependencyReport`.
- **Command-surface impact**: `Verify`, generated `Test`,
  `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateCheck`,
  `EvidenceGraph`, and `EvidenceAudit` must verify the new persistent-launch
  evidence contract or generated guidance. `Dev`, `Ci`, `PackLocal`,
  `DependencyReport`, and `TemplateDrift` change only if aggregation,
  packaging, dependency, or drift coverage requires it.
- **Generated project impact**: Generated graphical apps must expose an
  explicit evidence-mode command or readiness target that writes a persistent-
  launch artifact while preserving normal interactive behavior. Generated tests
  and docs must qualify app-owned `Product.Program.view`,
  `Product.Program.generatedHost`, and `Product.Program.update` when framework
  capability namespaces are in scope.
- **Evidence paths**: Required readiness paths are listed in Technical Context.
  Implementation tasks must make these files discoverable before the final
  audit task.
- **`.fsi` / contract impact**: Public viewer/testing contracts start in
  `.fsi`, then semantic tests and FSI transcripts, then `.fs` implementation.
  Surface baselines and public docs update with the signatures.
- **MVU/effect boundary**: PASS with constraints. The persistent-launch
  workflow is I/O-bearing and must be modeled as explicit viewer request/result
  data and edge effects. Pure generated gameplay reducers remain unchanged and
  must not execute window, filesystem, process, or input side effects.
- **Synthetic evidence**: PASS with restrictions. A supported-host pass requires
  real launch, first-frame, controlled-close, and recorded input-dispatch facts.
  Synthetic parser fixtures must be labeled and cannot satisfy readiness.
- **Test evidence**: Add failing-first semantic tests for artifact contract
  fields, blocked-stage classification, first-frame/control-close transitions,
  generated guidance names, benign warning classification, and audit discovery
  of required readiness files.
- **Observability**: Artifacts and diagnostics must state mode, command, window
  facts, first-frame status, input-dispatch status, exit path, blocked stage,
  classification, category, message, host facts, and whether each diagnostic is
  a generic probe, synthetic fixture, or real launch attempt.
- **Deferred scope**: No new game mechanics, no reducer rewrite, no release
  automation, no guarantee of automated visibility proof on hosts that cannot
  expose required facts, and no unrelated Controls/chart/graph/DataGrid work.

**Pre-design gate result**: PASS. The feature is Tier 1 and stateful/I/O-
bearing, but the plan preserves `.fsi`-first design, MVU/effect separation,
real evidence requirements, synthetic restrictions, and actionable diagnostics.

## Project Structure

```text
src/SkiaViewer/
  SkiaViewer.fsi                  # Persistent-launch request/result and viewer evidence contracts
  SkiaViewer.fs                   # Viewer interpreter and artifact serialization

src/Testing/
  Testing.fsi                     # Generated validation and warning classification contracts
  Testing.fs

template/
  capabilities.yml                # Capability skill inventory and generated guidance metadata
  base/src/Product/Program.fs     # Generated evidence command/host names
  base/tests/Product.Tests/       # Generated validation of launch guidance and names
  base/docs/product.md            # Generated readiness instructions

docs/
  generated-apps.md               # App-owned naming and readiness guidance
  evidence.md                     # Persistent launch vs layout/render metadata
  testing.md                      # Generated validation/evidence contract

tests/
  SkiaViewer.Tests/               # Viewer MVU and launch evidence tests
  Testing.Tests/                  # Artifact/warning validation helpers
  Governance.Tests/               # Generated guidance, audit, and task metadata checks

.agents/skills/
  fs-skia-layout-evidence/SKILL.md # Required for warning classification and generated guidance tasks

specs/021-persistent-launch-evidence/
  plan.md
  research.md
  data-model.md
  quickstart.md
  contracts/
    persistent-launch-evidence-contract.md
    window-observation-diagnostics-contract.md
    host-warning-classification-contract.md
    generated-guidance-contract.md
    evidence-audit-contract.md
  readiness/
```

## Phase 0: Research

Research is complete in `specs/021-persistent-launch-evidence/research.md`.
Key decisions:

- Accepted supported-host evidence must be viewer-owned and machine-readable,
  with external title/window tools treated as supplemental diagnostics only.
- Evidence mode is separate from default interactive launch so normal apps stay
  persistent until user close.
- Blocked-stage classification must distinguish prerequisites, process launch,
  window creation, first-frame/render, observation/capture, input verification,
  and controlled close.
- Known GTK/module warning noise is benign only when launch, first frame, and
  exit facts pass; concrete launch/render/layout/package failures remain fatal.
- Generated tests and guidance must use app-qualified names:
  `Product.Program.view`, `Product.Program.generatedHost`, and
  `Product.Program.update`.
- Tasks touching generated guidance, warning classification, or readiness
  evidence must declare `fs-skia-layout-evidence`.

## Phase 1: Design and Contracts

Design artifacts produced:

- `specs/021-persistent-launch-evidence/research.md`
- `specs/021-persistent-launch-evidence/data-model.md`
- `specs/021-persistent-launch-evidence/contracts/persistent-launch-evidence-contract.md`
- `specs/021-persistent-launch-evidence/contracts/window-observation-diagnostics-contract.md`
- `specs/021-persistent-launch-evidence/contracts/host-warning-classification-contract.md`
- `specs/021-persistent-launch-evidence/contracts/generated-guidance-contract.md`
- `specs/021-persistent-launch-evidence/contracts/evidence-audit-contract.md`
- `specs/021-persistent-launch-evidence/quickstart.md`

### Post-Design Constitution Check

- **Spec -> FSI -> tests -> implementation**: PASS. Public viewer/testing
  contracts start in `.fsi`; semantic, generated, and governance tests precede
  implementation.
- **Visibility in `.fsi`**: PASS. New public SkiaViewer or Testing symbols
  require matching `.fsi` entries and surface baseline updates.
- **Idiomatic simplicity**: PASS. Planned contracts use records,
  discriminated unions, explicit result values, and straightforward artifact
  serialization. No complex F# feature is required.
- **MVU/effect boundary**: PASS. Window opening, first-frame observation, input
  dispatch, filesystem writes, and controlled close are explicit viewer effects
  interpreted at the edge. Product reducers remain pure.
- **Synthetic disclosure**: PASS with restrictions. Synthetic error fixtures
  cannot satisfy supported-host readiness; real launch evidence is mandatory for
  pass artifacts.
- **Test evidence**: PASS. The quickstart names failing-first semantic,
  generated, guidance, graph, audit, and supported-host readiness commands.
- **Observability and safe failure**: PASS. Contracts require exact fields,
  blocked-stage classification, diagnostic source, benign-warning separation,
  and missing-fact messages.

## Phase 2: Planning Boundary

Stop after design. Task generation should produce dependency-ordered tasks with
`skillist` metadata, required readiness files, failing-first tests, `.fsi`
updates, surface baseline updates, and acceptance keywords before
implementation begins. Every task touching generated guidance, warning
classification, or readiness host warning evidence must list
`fs-skia-layout-evidence`.
