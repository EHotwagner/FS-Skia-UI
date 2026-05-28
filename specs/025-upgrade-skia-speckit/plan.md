# Implementation Plan: Upgrade SkiaSharp And Spec Kit

**Branch**: `025-upgrade-skia-speckit` | **Date**: 2026-05-28 | **Spec**: `specs/025-upgrade-skia-speckit/spec.md`
**Input**: Feature specification from `/specs/025-upgrade-skia-speckit/spec.md`

## Summary

Upgrade the repository's SkiaSharp package family and Spec Kit asset/version
metadata while preserving the compatibility package as a stable, evidence-owned
surface. The implementation must verify the latest compatible versions at the
time of edit, update central package pins and generated-template/governance
metadata consistently, refresh dependency and package-surface evidence, and
produce the compatibility consumer inventory and release posture required by
`docs/2026-05-27-2217-compatibility-package-analysis.md`.

This is a Tier 1 dependency/governance change. It introduces upgraded package
inputs and generated governance assets, but it does not authorize public API
removal, renderer redesign, new platform support, generated profile collapse,
or package publishing.

## Technical Context

**Language/Version**: F# on .NET `net10.0` for framework projects, generated
templates, governance tests, FAKE targets, and Spec Kit assets.

**Primary Dependencies**: Existing FS.Skia.UI Scene, SkiaViewer, Testing,
Elmish, template fragments, SkiaSharp 4 preview package family, Spec Kit
project assets, Expecto, FAKE, Silk.NET, Yoga.Net, and YamlDotNet. Planning
observed SkiaSharp `4.147.0-preview.3.1` and Spec Kit `0.8.15` as current
upstream candidates on 2026-05-28; implementation must re-check source-of-truth
before editing pins.

**Testing**: Expecto governance/package tests, package surface checks, FSI
transcripts only if public signatures change, generated product validation,
dependency reports, template checks, generated-guidance checks, template drift,
and FAKE targets (`DependencyReport`, `PackageSurfaceCheck`, `TemplateCheck`,
`GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`,
`Verify`, `Ci` as needed).

**Target Platform**: Supported Windows and Linux desktop hosts for viewer and
SkiaSharp native asset validation. Unsupported host behavior remains an
observable compatibility concern and must be recorded rather than hidden.

**Public Surface**: Default plan is no `.fsi` or public API changes, but this
Tier 1 feature still requires explicit public-surface evidence. Implementation
must run package-surface checks and record `readiness/package-surface-baseline.md`
as the `.fsi`/surface review result: unchanged, intentionally changed with the
`.fsi`-first path below, or blocked. If any compatibility-package public surface
change is discovered or required, update the `.fsi` first, add semantic/FSI
evidence, refresh surface baselines, and document migration impact before
implementation proceeds.

**Evidence Requirement**: Required real evidence paths are:

- `specs/025-upgrade-skia-speckit/readiness/version-selection.md`
- `specs/025-upgrade-skia-speckit/readiness/dependency-report.md`
- `specs/025-upgrade-skia-speckit/readiness/template-version-alignment.md`
- `specs/025-upgrade-skia-speckit/readiness/compatibility-consumer-inventory.md`
- `specs/025-upgrade-skia-speckit/readiness/compatibility-public-surface-map.md`
- `specs/025-upgrade-skia-speckit/readiness/compatibility-sample-migration.md`
- `specs/025-upgrade-skia-speckit/readiness/compatibility-release-policy.md`
- `specs/025-upgrade-skia-speckit/readiness/package-surface-baseline.md`
- `specs/025-upgrade-skia-speckit/readiness/evidence-audit.md`

**Synthetic Evidence**: Synthetic evidence is not acceptable for version
selection, dependency reports, package-surface status, generated template
alignment, or compatibility consumer inventory. Unsupported host facts are real
negative evidence only when they come from actual host/tool output and preserve
the failure reason.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Template package pins, generated Spec Kit assets,
  selected local skills, generated docs, generated guidance, validation logs,
  and `.template.config/template.json` must be reviewed. Update template
  metadata only when the upgraded Spec Kit assets or generated file set change.
- **Dependency impact**: PASS with required dependency governance.
  `Directory.Packages.props`, `template/base/Directory.Packages.props`,
  `.template.package/FS.Skia.UI.Template.fsproj`, `docs/dependencies.md`, and
  generated package guidance must be reviewed. `DependencyReport` evidence is
  mandatory. No new dependency identity is planned.
- **Command-surface impact**: `DependencyReport`, `PackageSurfaceCheck`,
  `TemplateCheck`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`,
  and `EvidenceAudit` must be reviewed for expectation changes. `Dev`,
  `PackLocal`, `Verify`, and `Ci` should keep semantics unless version
  validation reveals a documented requirement.
- **Generated project impact**: Generated projects must receive aligned package
  pins and Spec Kit assets. Profiles must not accidentally add the broad
  `FS.Skia.UI` compatibility package where focused packages are intended.
- **Evidence paths**: Required readiness files are listed in Technical Context.
  Logs should live under `specs/025-upgrade-skia-speckit/readiness/logs/` when
  command output is captured.
- **`.fsi` / contract impact**: PASS with required Tier 1 surface evidence.
  The expected result is no public contract change, proven by package-surface
  review and `readiness/package-surface-baseline.md`. Any discovered `.fsi`
  change escalates to `.fsi` sketch, semantic tests, implementation, surface
  baseline, docs, and release notes.
- **MVU/effect boundary**: PASS. No new stateful product workflow is expected.
  Build/template/dependency evidence workflows are existing I/O-bearing FAKE and
  script boundaries; preserve report fields, log paths, and actionable failures.
- **Synthetic evidence**: PASS with restrictions. Do not use synthetic data for
  package version proof, dependency graph proof, template generated output, or
  compatibility consumer inventory.
- **Test evidence**: Add or update failing-first governance tests for SkiaSharp
  package-family alignment, Spec Kit metadata alignment, generated template
  package pins, compatibility-package consumer inventory coverage, and accidental
  broad package dependency detection.
- **Observability**: Reports must expose selected versions, source URLs or local
  source-of-truth paths, affected files, before/after dependency rows, package
  graph/cycle status, generated profile status, compatibility consumer counts,
  public-surface differences, and unsupported-host facts when applicable.
- **Deferred scope**: No package publishing, public API removal, renderer
  redesign, new desktop OS support, external consumer telemetry, or permanent
  deprecation decision beyond the release policy documented for this feature.

**Pre-design gate result**: PASS. The feature is Tier 1 because it updates
governed dependencies and generated Spec Kit assets. The plan preserves
compatibility package behavior by default, requires real dependency/template
evidence, and escalates any public surface change through the constitution's
`.fsi` and surface-baseline process.

## Project Structure

```text
Directory.Packages.props                 # Central SkiaSharp package-family pins
docs/
  dependencies.md                        # Dependency inventory and risk notes
  2026-05-27-2217-compatibility-package-analysis.md
  generated-apps.md / template-profile.md / speckit.md as needed

.specify/
  init-options.json
  extensions/*/extension.yml
  presets/*/preset.yml
  templates/*
  workflows/*

.agents/skills/
  speckit-* / fs-skia-* skills as generated-template inputs

template/
  base/Directory.Packages.props          # Generated package pins
  base/.specify/*                        # Generated Spec Kit assets
  fragments/*                            # Generated guidance/skills

.template.package/
  FS.Skia.UI.Template.fsproj             # Template package metadata

src/Lib/
  Library.fsi / Library.fs               # Compatibility package surface if any change is required

tests/
  Governance.Tests/
  Package.Tests/
  Smoke.Tests/                           # Host/native validation if needed

specs/025-upgrade-skia-speckit/
  plan.md
  research.md
  data-model.md
  quickstart.md
  contracts/
    version-upgrade-contract.md
    compatibility-package-contract.md
    template-alignment-contract.md
    readiness-evidence-contract.md
  readiness/
```

## Phase 0: Research

Research is complete in `specs/025-upgrade-skia-speckit/research.md`.
Key decisions:

- Treat SkiaSharp `4.147.0-preview.3.1` and Spec Kit `0.8.15` as planning
  candidates observed on 2026-05-28, but re-check official sources immediately
  before implementation.
- Keep all SkiaSharp package variants aligned as one package family unless a
  documented upstream compatibility reason requires otherwise.
- Update Spec Kit project assets as a governed asset set: core metadata,
  extensions, presets, templates, workflows, generated template copies, and
  selected local skills.
- Freeze `FS.Skia.UI` as a compatibility surface during this upgrade; inventory
  and classify before any facade/deprecation/public-surface action.
- Use existing FAKE report targets and add governance tests where coverage is
  missing rather than introducing a new reporting system.

## Phase 1: Design and Contracts

Design artifacts produced:

- `specs/025-upgrade-skia-speckit/research.md`
- `specs/025-upgrade-skia-speckit/data-model.md`
- `specs/025-upgrade-skia-speckit/contracts/version-upgrade-contract.md`
- `specs/025-upgrade-skia-speckit/contracts/compatibility-package-contract.md`
- `specs/025-upgrade-skia-speckit/contracts/template-alignment-contract.md`
- `specs/025-upgrade-skia-speckit/contracts/readiness-evidence-contract.md`
- `specs/025-upgrade-skia-speckit/quickstart.md`

### Post-Design Constitution Check

- **Spec -> FSI -> tests -> implementation**: PASS with explicit non-change
  evidence. No public API changes are planned, but package-surface review must
  prove that no `.fsi` update is required. If compatibility surface differences
  appear, implementation must pause and follow `.fsi`-first design.
- **Visibility in `.fsi`**: PASS. Planned package and governance edits do not
  require visibility changes.
- **Idiomatic simplicity**: PASS. Version alignment and inventory evidence are
  straightforward file/report updates plus governance tests.
- **MVU/effect boundary**: PASS. No new product workflow is introduced; existing
  build/template/report command boundaries remain the I/O edge.
- **Synthetic disclosure**: PASS. Required evidence is real repository scan,
  package metadata, generated output, or command output. Synthetic shortcuts are
  not accepted for readiness.
- **Test evidence**: PASS. Quickstart names failing-first governance/package
  tests and final FAKE gates.
- **Observability and safe failure**: PASS. Contracts require explicit selected
  versions, source paths/URLs, affected files, before/after reports, generated
  profile status, and failure classes.
- **Tier classification**: PASS. This remains Tier 1 due to dependency and
  generated-governance impact, even with no intended public API change.

## Phase 2: Planning Boundary

Stop here for `/speckit-plan`. Implementation tasks should be generated next
from this plan and must include skill metadata for dependency governance,
template updates, compatibility inventory, package surface checks, and evidence
audit.
