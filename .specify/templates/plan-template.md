# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]
**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

## Summary

[Primary requirement and technical approach.]

## Technical Context

**Language/Version**: F# / .NET
**Primary Dependencies**: [packages or N/A]
**Testing**: Expecto, FAKE targets, FSI, generated product evidence as needed
**Target Platform**: Windows and Linux unless narrowed by the spec

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

<!-- GeneratedGuidanceCheck pass criteria (machine-enforced): EVERY decision area
     below MUST be filled in. The build FAILS if any area is left empty, kept as a
     boilerplate placeholder, or carries a `NEEDS CLARIFICATION` / `TODO` marker.
     A genuine "N/A" is allowed ONLY with a one-line rationale (e.g.
     "N/A — no dependency change"); a bare "N/A" still fails. Replace each bullet's
     guidance text with the actual decision before committing. -->

- **Template ownership**: Decide whether source, docs, samples, tests, Spec Kit
  assets, package policy, or command-surface changes must update
  `.template.config/template.json` or a deferral.
- **Dependency impact**: Decide whether `Directory.Packages.props`,
  `docs/dependencies.md`, generated template inclusion, and
  `DependencyReport` coverage are required.
- **Command-surface impact**: Decide whether `build.fsx`, wrappers, `Dev`,
  `Verify`, `Ci`, `TemplateCheck`, `DependencyReport`,
  `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, or
  `EvidenceAudit` must change. FAKE-backed commands (`./fake.sh`, `fake.cmd`,
  or `dotnet fake`) share `.fake` state and are not safe to run concurrently;
  require sequential execution and deterministic order when multiple
  FAKE-backed targets or tests are needed, while preserving safe non-FAKE
  parallelism when checks do not invoke FAKE or depend on `.fake`.
  Example order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t Verify`
- **Generated project impact**: Decide whether default/minimal generated
  contents, selected Controls guidance, local skills, validation logs,
  placeholder scans, excluded-history scans, or generated `Dev` behavior must
  change.
- **Evidence paths**: Identify exact readiness paths for logs, FSI transcripts,
  packed-library tests, generated project output, dependency reports, guidance
  reports, drift reports, screenshots, and audit output.
- **`.fsi` / contract impact**: Decide whether signatures, public docs, surface
  baselines, sample contracts, or compatibility notes change.

<!-- Generated from .specify/memory/constitution.md by `./fake.sh build -t RefreshSurfaceBaselines`; do not hand-edit between the markers. -->
<!-- BEGIN GENERATED: constitution/fsi-visibility -->
**II. Visibility Lives in `.fsi`, Not in `.fs`** — Every public F# module MUST have a corresponding `.fsi` signature file.
<!-- END GENERATED: constitution/fsi-visibility -->
- **MVU/effect boundary**: For stateful or I/O-bearing work, identify `Model`,
  `Msg`, `Effect` or `Cmd<Msg>`, `init`, pure `update`, emitted effect
  assertions, and real interpreter evidence.
- **Synthetic evidence**: Identify mocks, fakes, placeholders, canned
  responses, or in-memory substitutes and plan `[S]` disclosure when present.
- **Test evidence**: Define failing-first semantic tests, governance tests,
  packed-library or host smoke tests, and target-level evidence.
- **Observability**: Define actionable diagnostics, log paths, report fields,
  missing artifact-class failures, and unsupported environment messages.
- **Deferred scope**: Separate current obligations from deferred visual
  evidence, release validation, external repository split, distribution
  automation, or broader roadmap work.

## Project Structure

[Replace with the real paths for this feature.]
