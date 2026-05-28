# Implementation Plan: Working Screenshot Taking

**Branch**: `026-working-screenshot-taking` | **Date**: 2026-05-28 | **Spec**: `specs/026-working-screenshot-taking/spec.md`
**Input**: Feature specification from `/specs/026-working-screenshot-taking/spec.md`

## Summary

Complete the existing screenshot evidence contract so supported viewer-backed
graphical apps can produce a real PNG screenshot artifact from rendered output.
The implementation will replace the current unsupported-only
`Viewer.captureScreenshotEvidence` path with a first-frame render/capture path
that validates a readable, non-blank image, writes a traceable evidence record,
and reports precise blocked stages for launch, render, readback, validation, or
file failures.

This is a Tier 1 contracted change. It affects `src/SkiaViewer` and
`src/Testing` public contracts, generated product evidence commands, template
guidance, governance/audit checks, surface baselines, FSI transcripts, and real
readiness evidence.

## Technical Context

**Language/Version**: F# on .NET `net10.0` for framework packages, generated
templates, governance tests, FAKE targets, and FSI transcripts.

**Primary Dependencies**: Existing FS.Skia.UI Scene, SkiaViewer, Testing,
Elmish, SkiaSharp `4.147.0-preview.3.1`, Silk.NET `2.23.0`, Expecto, FAKE,
and Spec Kit evidence scripts. No new dependency is planned for Phase 2; use
SkiaSharp image encoding and the viewer-owned rendered surface/pixel path first.
If native desktop capture proves necessary, it must be explicitly pinned in
`Directory.Packages.props`, documented in `docs/dependencies.md`, included in
the template package pins, and covered by `DependencyReport`.

**Testing**: Expecto semantic tests through `.fsi`, FSI transcripts for public
SkiaViewer/Testing screenshot contracts, screenshot artifact validator tests,
generated product tests, template/guidance governance tests, FAKE target checks
(`Verify`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateCheck`,
`EvidenceGraph`, `EvidenceAudit`), and supported-host readiness evidence with a
real PNG artifact produced by working code.

**Target Platform**: Supported Windows and Linux desktop hosts for viewer-backed
screenshot evidence. Unsupported hosts remain valid negative evidence only when
reported as unsupported or failed with stage, classification, host facts, and no
successful screenshot claim.

**Public Surface**: Review and update `src/SkiaViewer/SkiaViewer.fsi` for any
additive screenshot capture mode, artifact validation, pixel validation, blocked
stage, or capture source fields. Review and update `src/Testing/Testing.fsi` for
validators that consume screenshot evidence records, detect blank artifacts, and
reject untraceable or synthetic screenshot claims. Update surface baselines for
`FS.Skia.UI.SkiaViewer` and `FS.Skia.UI.Testing`.

**Evidence Requirement**: Required readiness paths are:

- `specs/026-working-screenshot-taking/readiness/screenshot-capture-evidence.md`
- `specs/026-working-screenshot-taking/readiness/screenshot-artifacts.md`
- `specs/026-working-screenshot-taking/readiness/capture-failure-diagnostics.md`
- `specs/026-working-screenshot-taking/readiness/generated-guidance.md`
- `specs/026-working-screenshot-taking/readiness/package-surface-baseline.md`
- `specs/026-working-screenshot-taking/readiness/evidence-graph.md`
- `specs/026-working-screenshot-taking/readiness/evidence-audit.md`

**Synthetic Evidence**: Synthetic screenshot success is forbidden. Synthetic
malformed image/report fixtures may be used only for rejection and
error-handling tests with Principle V disclosure and `[SEH]` task labeling.
Unsupported-host records are real negative evidence only when produced by an
actual host attempt.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Generated graphical app templates and docs must expose
  `--screenshot-evidence` as a distinct evidence operation when the profile is
  screenshot-ready. Update `.template.config/template.json` only if files are
  added, removed, or renamed; otherwise update template source/docs/fragments and
  generated guidance tests.
- **Dependency impact**: PASS with no planned new package. Implementation must
  first use the current SkiaSharp/Silk.NET stack. Any added native capture
  package triggers `Directory.Packages.props`, template pins,
  `docs/dependencies.md`, and `DependencyReport`.
- **Command-surface impact**: `Verify`, `GeneratedProductCheck`,
  `GeneratedGuidanceCheck`, `TemplateCheck`, `EvidenceGraph`, and
  `EvidenceAudit` must verify screenshot-required artifacts. `Dev`, `Ci`,
  `PackLocal`, `DependencyReport`, and `TemplateDrift` change only if command
  aggregation, package pins, or template drift coverage require it.
- **Generated project impact**: Generated products keep normal launch behavior
  separate from screenshot evidence. Screenshot capture remains opt-in through a
  dedicated evidence command and must not run during the default interactive
  launch.
- **Evidence paths**: Required readiness files are listed in Technical Context.
  The PNG artifact path must be cited from both
  `screenshot-capture-evidence.md` and `screenshot-artifacts.md`.
- **`.fsi` / contract impact**: Any public contract change starts in `.fsi`,
  then failing semantic tests and FSI transcript, then `.fs` implementation,
  then surface baseline refresh. Contracts affected: SkiaViewer screenshot
  workflow/result and Testing screenshot evidence validator.
- **MVU/effect boundary**: PASS. Screenshot capture is I/O-bearing and must
  remain represented by `EvidenceWorkflowModel`, `EvidenceWorkflowMsg`,
  `EvidenceWorkflowEffect`, `initEvidenceWorkflow`, `updateEvidenceWorkflow`,
  and an interpreter at the viewer/generated command edge. Normal app update
  loops must not write files or close windows for evidence.
- **Synthetic evidence**: PASS with restrictions. Real screenshot success must
  include a working-code PNG and pixel validation. Synthetic rejection fixtures
  require `[SEH]` disclosure.
- **Test evidence**: Add failing-first semantic tests for real screenshot result
  shape, positive dimensions, non-blank pixel validation, blank/unreadable
  rejection, unsupported-host diagnostics, generated guidance, and evidence
  audit behavior.
- **Observability**: Reports must include status, command, app/sample identity,
  host facts, capture mode, artifact path, dimensions, pixel validation,
  blocked stage, classification, category, message, timestamp, and diagnostics.
- **Deferred scope**: No new game mechanics, renderer redesign, browser/mobile
  capture, broad new desktop platform support, package publishing, or
  replacement of persistent-launch/layout/scene evidence.

**Pre-design gate result**: PASS. The plan preserves `.fsi`-first public
contracts, MVU/effect separation, real evidence obligations, no synthetic
success path, and explicit unsupported/failure diagnostics.

## Project Structure

```text
src/SkiaViewer/
  SkiaViewer.fsi                  # Screenshot capture/result/workflow public contracts
  SkiaViewer.fs                   # Viewer capture interpreter and PNG/pixel validation

src/Testing/
  Testing.fsi                     # Screenshot report/artifact validation public contracts
  Testing.fs

template/
  base/src/Product/EvidenceCommands.fs
  base/src/Product/Program.fs     # Generated screenshot evidence command wiring
  base/docs/product.md            # Generated screenshot evidence guidance
  fragments/*/README.md           # Capability guidance if screenshot-ready profiles change

docs/
  evidence.md                     # Screenshot proof policy and audit expectations
  testing.md                      # Screenshot readiness command and validator guidance
  generated-apps.md               # Generated workflow guidance

tests/
  SkiaViewer.Tests/               # Capture workflow and supported-host smoke tests
  Testing.Tests/                  # Report/image validator semantic tests
  Governance.Tests/               # Guidance, template, and audit expectations

specs/026-working-screenshot-taking/
  plan.md
  research.md
  data-model.md
  quickstart.md
  contracts/
    screenshot-capture-contract.md
    screenshot-evidence-record-contract.md
    generated-guidance-contract.md
    evidence-audit-contract.md
  readiness/
```

## Phase 0: Research

Research is complete in `specs/026-working-screenshot-taking/research.md`.
Key decisions:

- Use a viewer-owned first-frame render-target/pixel-readback capture path and
  encode the result as PNG through SkiaSharp before considering platform
  screenshot APIs.
- Treat screenshot evidence as accepted only when capture source is live
  viewer/rendered output, the artifact is readable, dimensions are positive,
  pixel content is non-blank, and the report is traceable to command, host,
  sample, capture mode, and timestamp.
- Keep deterministic scene evidence, layout evidence, bounded launch evidence,
  and persistent launch evidence separate from screenshots.
- Model failures by blocked stage: desktop prerequisite, launch, first frame,
  render, capture/readback, validation, artifact write, or timeout.
- Require generated guidance and audit checks to reject metadata-only,
  placeholder, synthetic, blank, or untraceable screenshot claims.

## Phase 1: Design and Contracts

Design artifacts produced:

- `specs/026-working-screenshot-taking/research.md`
- `specs/026-working-screenshot-taking/data-model.md`
- `specs/026-working-screenshot-taking/contracts/screenshot-capture-contract.md`
- `specs/026-working-screenshot-taking/contracts/screenshot-evidence-record-contract.md`
- `specs/026-working-screenshot-taking/contracts/generated-guidance-contract.md`
- `specs/026-working-screenshot-taking/contracts/evidence-audit-contract.md`
- `specs/026-working-screenshot-taking/quickstart.md`

### Post-Design Constitution Check

- **Spec -> FSI -> tests -> implementation**: PASS. Public changes start in
  `.fsi`; quickstart requires failing-first semantic tests, FSI transcripts,
  implementation, and baseline refresh.
- **Visibility in `.fsi`**: PASS. No top-level visibility modifiers are planned
  in `.fs`; public exposure is controlled by SkiaViewer/Testing signatures.
- **Idiomatic simplicity**: PASS. The design uses records, discriminated
  unions, plain validators, and file/image checks. No complex F# feature is
  required.
- **MVU/effect boundary**: PASS. Screenshot capture and writes remain effects;
  `updateEvidenceWorkflow` stays pure and normal generated app behavior remains
  independent.
- **Synthetic disclosure**: PASS with restrictions. Screenshot success cannot be
  synthetic. Malformed fixtures are permitted only for rejection tests with
  `[SEH]`.
- **Test evidence**: PASS. The quickstart names semantic, generated,
  governance, package surface, and readiness checks.
- **Observability and safe failure**: PASS. Contracts require traceable fields,
  host facts, blocked stages, pixel validation results, and clear unsupported or
  failed outcomes.

## Phase 2: Planning Boundary

Stop after design. Task generation should produce dependency-ordered tasks with
`skillist` metadata, `.fsi` updates, failing-first tests, implementation,
surface baseline refresh, docs/template guidance, generated validation, real
readiness evidence, graph validation, and evidence audit.
