# Implementation Plan: Breakout Demo Feedback

**Branch**: `022-breakout-demo-feedback` | **Date**: 2026-05-27 | **Spec**: `specs/022-breakout-demo-feedback/spec.md`
**Input**: Feature specification from `/specs/022-breakout-demo-feedback/spec.md`

## Summary

Turn the BreakoutDemo1 implementation feedback into framework, generated-template,
and governance improvements for game-style generated apps. The change aligns
generated persistent-viewer guidance with the packaged public surface, adds
first-class filled circle and ellipse scene concepts, keeps deterministic render
proof distinct from live persistent-window and screenshot proof, documents the
pure app update versus viewer-effect boundary, and standardizes generated
evidence report conventions.

This is a Tier 1 contracted framework, generated-template, and governance
change. It affects public `.fsi` signatures, surface baselines, generated app
source/tests/docs, FAKE guidance checks, template/package validation, and
readiness audit discovery. The feature does not replace the persistent-launch
source of truth in `specs/021-persistent-launch-evidence`; it builds on that
contract and adds Breakout-derived shape, screenshot, guidance, and report
conventions.

## Technical Context

**Language/Version**: F# on .NET `net10.0` for framework packages, generated
templates, governance tests, and FAKE targets.

**Primary Dependencies**: Existing FS.Skia.UI Scene, SkiaViewer, Elmish,
Testing, template fragments, SkiaSharp 4 preview stack, Expecto, FAKE, and Spec
Kit evidence scripts. No new runtime dependency is planned. Screenshot support
must use existing viewer/platform capability where available or return an
explicit unsupported result.

**Testing**: Expecto semantic tests through `.fsi`, FSI transcripts, generated
product tests, deterministic scene evidence, generated guidance checks, surface
baseline checks, and FAKE targets (`Verify`, `TemplateCheck`,
`GeneratedGuidanceCheck`, `EvidenceGraph`, `EvidenceAudit`). Supported-host
screenshot checks run only when capture is exposed by the host; unsupported-host
checks must remain machine-readable and non-ambiguous.

**Target Platform**: Windows and Linux desktop sessions for live viewer and
screenshot evidence. Deterministic scene evidence must remain available without
live screenshot capability. Unsupported screenshot capture is acceptable only
when the report clearly states status, command, reason, and deterministic
fallback.

**Public Surface**: Review `src/Scene/Scene.fsi` for filled circle and filled
ellipse constructors, node shapes, evidence descriptions, and geometry helper
constructors if added. Review `src/SkiaViewer/SkiaViewer.fsi` for screenshot
evidence request/result contracts and generated viewer launch names. Review
`src/Testing/Testing.fsi` for generated evidence report, screenshot result, and
guidance validation helpers. Update surface baselines, docs, samples, and
generated contracts together with any `.fsi` change.

**Evidence Requirement**: Required real evidence paths are:

- `specs/022-breakout-demo-feedback/readiness/generated-viewer-guidance.md`
- `specs/022-breakout-demo-feedback/readiness/scene-shape-evidence.md`
- `specs/022-breakout-demo-feedback/readiness/screenshot-evidence.md`
- `specs/022-breakout-demo-feedback/readiness/effect-boundary-guidance.md`
- `specs/022-breakout-demo-feedback/readiness/evidence-report-conventions.md`

**Synthetic Evidence**: Synthetic screenshot success, synthetic public-surface
availability, and synthetic deterministic shape evidence are not acceptable for
passing readiness. Synthetic malformed report fixtures may be used only for
error-handling validation with Principle V disclosure; unsupported-host
screenshot results are real negative host facts, not synthetic success.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Generated template source, generated docs, generated
  tests, capability README fragments, and template inclusion policy must update
  when viewer launch names, evidence commands, report helpers, shape examples,
  or effect-boundary guidance change. Review `.template.config/template.json`
  if new generated files are included.
- **Dependency impact**: PASS with no planned new dependency. If screenshot
  capture requires a new package or platform helper, update
  `Directory.Packages.props`, template package pins, `docs/dependencies.md`,
  and `DependencyReport`.
- **Command-surface impact**: `Verify`, `TemplateCheck`,
  `GeneratedGuidanceCheck`, `EvidenceGraph`, and `EvidenceAudit` must validate
  guidance names, shape evidence, screenshot result wording, effect-boundary
  examples, and report conventions. `Dev`, `Ci`, `PackLocal`,
  `DependencyReport`, and `TemplateDrift` change only if aggregation,
  packaging, dependency, or drift coverage requires it.
- **Generated project impact**: Generated graphical apps must use one packaged
  persistent launch contract in source, tests, and docs; provide deterministic
  circle/ellipse examples; expose honest screenshot evidence behavior; keep
  pure update commands separate from viewer rendering effects; and reuse
  standard report and geometry conventions.
- **Evidence paths**: Required readiness paths are listed in Technical Context.
  Tasks must make these files discoverable before final audit.
- **`.fsi` / contract impact**: Public Scene, SkiaViewer, and Testing contracts
  start in `.fsi`, then semantic tests and FSI transcripts, then `.fs`
  implementation. Surface baselines and public docs update with signatures.
- **MVU/effect boundary**: PASS with constraints. Generated app reducers remain
  pure and emit app-level commands only. Viewer rendering/window/screenshot
  effects are produced and interpreted at the host boundary.
- **Synthetic evidence**: PASS with restrictions. Deterministic shape proof,
  package-surface proof, and supported screenshot proof require real evidence.
  Unsupported screenshot reports are accepted only as explicit negative facts
  with fallback guidance.
- **Test evidence**: Add failing-first semantic tests for circle/ellipse public
  surface and evidence, generated viewer launch name drift, screenshot report
  success/unsupported fields, effect-boundary guidance, and report helper
  ordering/exit behavior.
- **Observability**: Screenshot and report artifacts must include actionable
  status, command, output path where applicable, dimensions when captured,
  unsupported-host reason, fallback, and diagnostics. Guidance checks must fail
  on stale public names or screenshot claims without screenshot facts.
- **Deferred scope**: No new game mechanics, no Breakout demo rebuild, no
  guarantee of screenshot capture on hosts that cannot expose it, no unrelated
  Controls/chart/graph/DataGrid work, no release automation rewrite, and no
  redefinition of persistent-launch evidence covered by feature 021.

**Pre-design gate result**: PASS. The feature is Tier 1 and includes
stateful/I/O-bearing evidence paths, but the plan preserves `.fsi`-first API
design, MVU/effect separation, real evidence requirements, and explicit
unsupported-host behavior.

## Project Structure

```text
src/Scene/
  Scene.fsi                       # Circle/ellipse scene and evidence contracts
  Scene.fs

src/SkiaViewer/
  SkiaViewer.fsi                  # Packaged viewer launch and screenshot evidence contracts
  SkiaViewer.fs

src/Testing/
  Testing.fsi                     # Generated report/guidance/screenshot validation contracts
  Testing.fs

template/
  base/src/Product/Program.fs     # Generated launch/evidence/report examples
  base/tests/Product.Tests/       # Generated contract and guidance tests
  base/docs/product.md            # Generated readiness and effect-boundary guidance
  fragments/*/README.md           # Capability-specific guidance updates

tests/
  Scene.Tests/                    # Shape public-surface and deterministic evidence tests
  SkiaViewer.Tests/               # Screenshot result and viewer launch contract tests
  Testing.Tests/                  # Report and generated guidance validation tests
  Governance.Tests/               # Template/guidance/audit discovery checks

specs/022-breakout-demo-feedback/
  plan.md
  research.md
  data-model.md
  quickstart.md
  contracts/
    generated-viewer-contract.md
    scene-shape-primitives-contract.md
    screenshot-evidence-contract.md
    effect-boundary-contract.md
    evidence-report-contract.md
  readiness/
```

## Phase 0: Research

Research is complete in `specs/022-breakout-demo-feedback/research.md`. Key
decisions:

- Generated guidance must select one currently packaged viewer launch contract
  and verify that source, tests, docs, package surface, and readiness wording
  agree.
- Filled circle and ellipse scene concepts should be first-class public Scene
  primitives, with painted variants optional only if they follow the same public
  model and deterministic evidence expectations.
- Screenshot evidence must either produce bounded machine-readable screenshot
  facts or an explicit unsupported result; deterministic pixel/readback proof
  remains a separate fallback, not screenshot proof.
- Generated examples must show a pure app update returning app commands and a
  host boundary that turns model state into viewer render/window effects.
- Generated apps should reuse Scene geometry types and standard key-value
  report helpers to avoid duplicate local shape records and inconsistent
  evidence output.

## Phase 1: Design and Contracts

Design artifacts produced:

- `specs/022-breakout-demo-feedback/research.md`
- `specs/022-breakout-demo-feedback/data-model.md`
- `specs/022-breakout-demo-feedback/contracts/generated-viewer-contract.md`
- `specs/022-breakout-demo-feedback/contracts/scene-shape-primitives-contract.md`
- `specs/022-breakout-demo-feedback/contracts/screenshot-evidence-contract.md`
- `specs/022-breakout-demo-feedback/contracts/effect-boundary-contract.md`
- `specs/022-breakout-demo-feedback/contracts/evidence-report-contract.md`
- `specs/022-breakout-demo-feedback/quickstart.md`

### Post-Design Constitution Check

- **Spec -> FSI -> tests -> implementation**: PASS. Public Scene,
  SkiaViewer, and Testing contracts start in `.fsi`; semantic, generated, and
  governance tests precede implementation.
- **Visibility in `.fsi`**: PASS. New public symbols require matching `.fsi`
  entries and surface baseline updates.
- **Idiomatic simplicity**: PASS. Planned contracts use records,
  discriminated unions, straightforward helpers, and explicit result values.
  No complex F# feature is required.
- **MVU/effect boundary**: PASS. Reducer examples keep app commands pure while
  viewer rendering/window/screenshot work remains host-interpreted.
- **Synthetic disclosure**: PASS with restrictions. Synthetic malformed-report
  tests may validate rejection only; supported screenshot or shape proof cannot
  be synthetic.
- **Test evidence**: PASS. The quickstart names failing-first semantic,
  generated, guidance, graph, audit, and supported/unsupported screenshot
  commands.
- **Observability and safe failure**: PASS. Contracts require stable report
  fields, explicit unsupported reasons, fallback fields, stale-name failures,
  and no ambiguous screenshot claims.

## Phase 2: Planning Boundary

Stop after design. Task generation should produce dependency-ordered tasks with
`skillist` metadata, required readiness files, failing-first tests, `.fsi`
updates, surface baseline updates, template changes, and acceptance keywords
before implementation begins. Tasks touching generated game HUD/readability,
scene layout evidence, public scene/host/update guidance, or benign host
warning classification must include `fs-skia-layout-evidence`; tasks touching
Scene, SkiaViewer, Elmish, or Testing public contracts must include the matching
capability skills from the constitution inventory.
