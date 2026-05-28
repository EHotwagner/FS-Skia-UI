# Implementation Plan: Racer Feedback Follow-Ups

**Branch**: `024-racer-feedback-followups` | **Date**: 2026-05-28 | **Spec**: `specs/024-racer-feedback-followups/spec.md`
**Input**: Feature specification from `/specs/024-racer-feedback-followups/spec.md`

## Summary

Turn the top-down racer consumer feedback into focused framework, generated
guidance, and readiness evidence improvements. Generated samples and docs must
avoid app-domain geometry names that collide with scene/layout primitives, live
screenshot evidence must produce real PNG proof on supported Windows and Linux
desktop hosts when capture is available, unsupported capture must expose
separate launch/open and capture-capability facts, known GTK module warnings
must be classified as benign host warnings when first-frame launch succeeds,
and Linux detached GUI guidance must recommend a detached-session pattern that
preserves logs and detaches standard input.

This is a Tier 1 contracted framework/governance change. It affects generated
template guidance, readiness diagnostics, screenshot evidence result wording or
public contracts, governance tests, and real readiness evidence. It does not
change generated game mechanics, renderer architecture, broad platform support,
state workflow semantics, or release automation.

## Technical Context

**Language/Version**: F# on .NET `net10.0` for framework packages, generated
templates, governance tests, and FAKE targets.

**Primary Dependencies**: Existing FS.Skia.UI Scene, SkiaViewer, Testing,
Elmish, template fragments, SkiaSharp 4 preview stack, Expecto, FAKE, and Spec
Kit evidence scripts. No new runtime dependency is planned. If live screenshot
capture requires an additional platform package, it must be explicitly pinned
and covered by dependency governance before implementation.

**Testing**: Expecto semantic tests through `.fsi`, FSI transcripts where
public contracts change, generated product tests, screenshot success and
unsupported-result checks, generated guidance checks, host-warning classifier
tests, template validation, and FAKE targets (`Verify`, `TemplateCheck`,
`GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`).
Supported-host screenshot success requires real window capture evidence on at
least one supported Windows or Linux desktop host.

**Target Platform**: Supported Windows and Linux desktop hosts for live viewer
and live screenshot evidence. Unsupported launch or capture capability remains
valid only when reported as an explicit host fact and not relabeled as
screenshot proof.

**Public Surface**: Review `src/SkiaViewer/SkiaViewer.fsi` for additive
screenshot capability detail and live-window capture-source fields. Review
`src/Testing/Testing.fsi` for screenshot result validators, guidance validators,
host warning classification helpers, or report schemas if owned by Testing.
Review generated template source/docs and surface baselines with every public
contract change.

**Evidence Requirement**: Required real evidence paths are:

- `specs/024-racer-feedback-followups/readiness/baseline-status.md`
- `specs/024-racer-feedback-followups/readiness/generated-guidance-validation.md`
- `specs/024-racer-feedback-followups/readiness/screenshot-capability-detail.md`
- `specs/024-racer-feedback-followups/readiness/screenshot-success-artifact.md`
- `specs/024-racer-feedback-followups/readiness/host-warning-classification.md`
- `specs/024-racer-feedback-followups/readiness/detached-launch-guidance.md`

**Synthetic Evidence**: Synthetic screenshot success is not acceptable.
Deterministic scene rendering remains real deterministic render evidence, but
cannot be claimed as live screenshot proof. Synthetic malformed report fixtures
may be used only for error-handling validation with Principle V disclosure.
Unsupported screenshot results are real negative host facts when they come from
an actual host attempt and preserve launch/capture capability detail.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Update generated template source, docs, capability
  README fragments, generated tests, and `.template.config/template.json` only
  if new or renamed generated files are included. Guidance must consistently
  avoid `Rect`, `Point`, and `Size` as app-domain geometry examples when
  scene/layout primitives are in scope.
- **Dependency impact**: PASS with no planned new dependency. If screenshot
  capture needs an added package or native helper, update
  `Directory.Packages.props`, template package pins, `docs/dependencies.md`,
  and `DependencyReport`, and document the owner and platform reason.
- **Command-surface impact**: `GeneratedGuidanceCheck`, `TemplateDrift`,
  `TemplateCheck`, `EvidenceGraph`, and `EvidenceAudit` may need expectation
  updates. `Dev`, `Verify`, `Ci`, `PackLocal`, and `DependencyReport` must keep
  existing semantics unless dependency governance is triggered.
- **Generated project impact**: Generated game/sample guidance must use
  domain-specific geometry names such as `WorldRect`, `WorldPoint`, and
  `TrackBounds`; preserve existing interactive, bounded first-frame,
  deterministic render, screenshot success, and unsupported screenshot paths;
  and document reliable Linux detached GUI launch with log capture and
  detached standard input.
- **Evidence paths**: Required readiness files are listed in Technical Context
  and must be discoverable by task and audit artifacts.
- **`.fsi` / contract impact**: Any additive screenshot result, capability
  detail, host warning classification, or validation helper starts in `.fsi`,
  then semantic tests and FSI transcripts, then `.fs` implementation. Surface
  baselines and docs update with public signatures.
- **MVU/effect boundary**: Generated app state workflows remain unchanged.
  Screenshot capture and host warning collection are I/O-bearing evidence
  workflows; model capability details as explicit request/result records and
  keep window/process/filesystem work at the interpreter edge. Use a local
  evidence workflow boundary with:
  - `EvidenceWorkflowModel` for request status, viewer-open status,
    first-frame status, capture availability, warning observations, and output
    path.
  - `EvidenceWorkflowMsg` for launch started/completed, first frame observed,
    capture succeeded/unsupported/failed, warnings classified, and report
    written.
  - `EvidenceWorkflowEffect` for viewer launch, screenshot capture,
    process-output collection, file writes, and generated guidance validation.
  - `init` to build the initial model from the evidence request and emit
    startup effects.
  - `update` as the pure message/model transition that emits effects.
  - an interpreter boundary in SkiaViewer/generated evidence commands that
    executes window/process/filesystem effects and converts results back to
    messages.
- **Synthetic evidence**: PASS with restrictions. Screenshot success,
  generated guidance proof, and benign warning classification acceptance require
  real files or real captured process output. Deterministic render evidence is
  valid only as fallback/diagnostic evidence.
- **Test evidence**: Add failing-first semantic/governance tests for naming
  guidance, screenshot success fields, unsupported capability detail, live
  capture-source wording, benign GTK warning classification, and detached
  launch guidance.
- **Observability**: Reports must expose status, evidence-kind, capture source,
  PNG path, dimensions, viewer-open status, capture availability, unsupported
  reason, fallback, warning class, preserved warning text, command, log path,
  and failure class where applicable.
- **Deferred scope**: No new desktop OS support beyond Windows/Linux, no
  renderer replacement, no generated game redesign, no package release process
  redesign, no simple terminal detachment guarantee, and no screenshot success
  claim when live capture is unavailable.

**Pre-design gate result**: PASS. The feature is Tier 1 and includes
stateful/I/O-bearing evidence behavior, but the plan preserves `.fsi`-first
public contracts, MVU/effect separation, real screenshot evidence requirements,
explicit unsupported-host reporting, and actionable diagnostics.

## Project Structure

```text
src/SkiaViewer/
  SkiaViewer.fsi                  # Screenshot evidence and capability-detail public contracts if owned here
  SkiaViewer.fs

src/Testing/
  Testing.fsi                     # Evidence report, guidance, warning, and screenshot validators if owned here
  Testing.fs

template/
  base/src/Product/Program.fs     # Generated evidence commands and naming examples
  base/docs/product.md            # Generated readiness, screenshot, and detached launch guidance
  fragments/*/README.md           # Capability-specific guidance updates
  capabilities.yml                # Skill inventory review if generated guidance task metadata changes

docs/
  generated-apps.md               # Geometry naming and detached GUI launch guidance
  evidence.md                     # Screenshot proof vs deterministic render fallback and capability detail
  testing.md                      # Generated validation and readiness report conventions

tests/
  SkiaViewer.Tests/               # Screenshot result/capability-detail semantic tests
  Testing.Tests/                  # Report validator and warning-classifier tests
  Governance.Tests/               # Generated guidance, template, and audit checks
  Smoke.Tests/                    # Supported-host screenshot smoke evidence when available

specs/024-racer-feedback-followups/
  plan.md
  research.md
  data-model.md
  quickstart.md
  contracts/
    generated-guidance-contract.md
    screenshot-evidence-contract.md
    host-warning-classification-contract.md
    detached-launch-guidance-contract.md
    readiness-evidence-contract.md
  readiness/
```

## Phase 0: Research

Research is complete in `specs/024-racer-feedback-followups/research.md`.
Key decisions:

- Generated guidance must reserve generic geometry names for scene/layout
  primitives and recommend app-domain names such as `WorldRect`, `WorldPoint`,
  `TrackBounds`, `CarPose`, and `CheckpointBounds`.
- Screenshot evidence success means live viewer-window capture after
  first-frame presentation with a PNG artifact, positive dimensions, and
  `evidence-kind=screenshot`; deterministic render evidence remains separate
  fallback/diagnostic evidence.
- Unsupported screenshot evidence must preserve separate facts for viewer open
  status and capture availability whenever they can be determined.
- Known GTK module warnings for `colorreload-gtk-module` and
  `window-decorations-gtk-module` are benign host warnings only when
  first-frame launch evidence succeeds and no unrelated failing warning/error is
  present.
- Linux background GUI guidance should prefer a `setsid ... > log 2>&1 <
  /dev/null &` style detached-session pattern over simple terminal detachment.

## Phase 1: Design and Contracts

Design artifacts produced:

- `specs/024-racer-feedback-followups/research.md`
- `specs/024-racer-feedback-followups/data-model.md`
- `specs/024-racer-feedback-followups/contracts/generated-guidance-contract.md`
- `specs/024-racer-feedback-followups/contracts/screenshot-evidence-contract.md`
- `specs/024-racer-feedback-followups/contracts/host-warning-classification-contract.md`
- `specs/024-racer-feedback-followups/contracts/detached-launch-guidance-contract.md`
- `specs/024-racer-feedback-followups/contracts/readiness-evidence-contract.md`
- `specs/024-racer-feedback-followups/quickstart.md`

### Post-Design Constitution Check

- **Spec -> FSI -> tests -> implementation**: PASS. Additive public
  SkiaViewer/Testing contracts start in `.fsi`; failing-first semantic,
  generated, and governance tests precede implementation.
- **Visibility in `.fsi`**: PASS. New public symbols require matching `.fsi`
  entries and surface baseline updates.
- **Idiomatic simplicity**: PASS. Planned contracts use records,
  discriminated unions, string-preserving diagnostics, and straightforward
  validators. No complex F# feature is required.
- **MVU/effect boundary**: PASS. Generated app reducers remain unchanged;
  screenshot/window/process/file work is represented through the local
  `EvidenceWorkflowModel` / `EvidenceWorkflowMsg` / `EvidenceWorkflowEffect`
  boundary, with pure `init`/`update` transitions and host interpretation at
  the SkiaViewer/generated evidence command edge.
- **Synthetic disclosure**: PASS with restrictions. Synthetic malformed-result
  fixtures may validate rejection only. Screenshot success and warning
  acceptance require real artifacts/output.
- **Test evidence**: PASS. The quickstart names failing-first semantic,
  generated, guidance, template, graph, audit, and readiness evidence commands.
- **Observability and safe failure**: PASS. Contracts require stable report
  fields, preserved warning text, explicit unsupported reasons, separated
  viewer-open/capture facts, PNG path/dimensions, and no ambiguous screenshot
  claims.

## Phase 2: Planning Boundary

Stop after design. Task generation should produce dependency-ordered tasks with
`skillist` metadata, required readiness files, failing-first tests, `.fsi`
updates, surface baseline updates, template/guidance changes, and acceptance
keywords before implementation begins. Tasks touching generated guidance,
evidence wording, screenshot capture, host warning classification, or detached
launch documentation must include `fs-skia-layout-evidence`; tasks touching
SkiaViewer or Testing public contracts must include the matching capability
skills from the repository skill inventory when available.
