# Implementation Plan: Archive Readiness And API Docs

**Branch**: `036-archive-readiness-api-docs` | **Date**: 2026-05-30 | **Spec**: `specs/036-archive-readiness-api-docs/spec.md`
**Input**: Feature specification from `/specs/036-archive-readiness-api-docs/spec.md`

## Summary

Separate current readiness evidence from historical feature evidence without
destroying auditability, and decide whether the package API reference material
introduced by `035-api-discovery-names` should remain generated from curated
`.fsi` signatures or move to FSharp.Formatting/fsdocs. The implementation will
keep historical files in place by default, add archive indexes and markers,
teach active guidance and stale-reference scanners that archived evidence is
not current gate evidence, and produce a representative fsdocs comparison for
`Scene`, `Controls`, and one host/adapter package.

The plan keeps the current source-shaped `.fsi` reference workflow
authoritative for agents unless the spike proves fsdocs preserves every
authoring guarantee: F# spelling, union cases, record fields, parameter names,
XML documentation, package-adjacent discoverability, no repository source
inspection, and no reflection as the authoring strategy. fsdocs may become a
secondary or hybrid browsable documentation surface.

## Technical Context

**Language/Version**: F# on .NET `net10.0` for repository scripts, Expecto
governance tests, package/reference generation checks, and FAKE targets.
Documentation evaluation may use the `fsdocs` .NET tool and
FSharp.Formatting-generated output in an isolated spike.

**Primary Dependencies**: Existing FS.Skia.UI packages and repository tooling,
Expecto, FAKE, Spec Kit evidence scripts, the existing
`scripts/generate-package-api-reference.fsx` source-shaped `.fsi` generator,
and optional evaluation-only `fsdocs-tool`/FSharp.Formatting. No runtime
dependency is planned. If fsdocs becomes a committed build dependency, it must
be pinned in `Directory.Packages.props` or a checked-in tool manifest as
appropriate, documented in `docs/dependencies.md`, and covered by
`DependencyReport`.

**Testing**: Expecto governance tests for archive classification,
stale-reference scanning, generated guidance terms, package API reference
decision records, and fsdocs comparison output; optional local fsdocs spike
logs; FAKE targets run sequentially: `Dev`, `GeneratedGuidanceCheck`,
`TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`. Add `PackLocal`,
`PackageSurfaceCheck`, `FsiTranscripts`, `TemplateCheck`, or
`GeneratedProductCheck` only if the accepted API-reference decision changes
package contents, reference output paths, generated template contents, or
clean consumer validation obligations.

**Target Platform**: Windows and Linux repository maintainers and package
consumers. Validation is documentation, package-reference, and scanner focused;
runtime rendering, Vulkan, screenshots, desktop host behavior, and state
workflow semantics are out of scope.

**Public Surface**: Public `.fsi` signatures and surface baselines are expected
to remain unchanged. In-scope surfaces are `docs/generated-apps.md`,
`docs/template-profile.md`, `docs/dependencies.md` if a dependency is adopted,
`template/base/README.md`, `template/base/docs/product.md`, generated guidance
scanners, archive/stale-reference scripts or tests, and feature readiness
artifacts. The existing API-reference generator
`scripts/generate-package-api-reference.fsx` and
`tests/Package.Tests/PackageApiReferenceTests.fs` are review points.

**Evidence Requirement**: Required readiness paths are:

- `specs/036-archive-readiness-api-docs/readiness/archive-inventory.md`
- `specs/036-archive-readiness-api-docs/readiness/current-evidence-map.md`
- `specs/036-archive-readiness-api-docs/readiness/stale-reference-scan.md`
- `specs/036-archive-readiness-api-docs/readiness/api-reference-generator-evaluation.md`
- `specs/036-archive-readiness-api-docs/readiness/fsharp-formatting-spike.md`
- `specs/036-archive-readiness-api-docs/readiness/generated-guidance-check.md`
- `specs/036-archive-readiness-api-docs/readiness/evidence-graph.md`
- `specs/036-archive-readiness-api-docs/readiness/evidence-audit.md`

**Synthetic Evidence**: Synthetic positive archive or API-reference evidence is
not acceptable. Archive inventory rows must be derived from real repository
paths, stale-reference scans must inspect real active surfaces, and package API
comparison samples must use real generated output or a documented fsdocs
blocker. Synthetic malformed scanner fixtures are allowed only for
error-path tests with Principle V disclosure and `[SEH]` task labeling.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: PASS. Template guidance is in scope only to tell
  generated consumers that archived readiness material is not current evidence
  and to preserve source-shaped API reference guidance. Update
  `.template.config/template.json` only if new files are included in generated
  projects or template package contents.
- **Dependency impact**: PASS with a guarded optional dependency. fsdocs is
  allowed for evaluation as a local/tool-manifest spike. A committed fsdocs
  dependency requires pinning, `docs/dependencies.md`, generated-template
  governance review, and `DependencyReport`.
- **Command-surface impact**: PASS. Expected gates are `Dev`,
  `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, and
  `EvidenceAudit`. Additional package/template gates are conditional on a
  decision to alter package contents or generated reference paths. FAKE-backed
  commands must run sequentially because `.fake` state is shared.
- **Generated project impact**: PASS. Generated-product docs may change to
  point at the current evidence map and to reject archived readiness as current
  gate evidence. Runtime product code is out of scope.
- **Evidence paths**: PASS. Required readiness paths are listed in Technical
  Context and become the active feature evidence set.
- **`.fsi` / contract impact**: PASS. No public `.fsi` change is planned.
  Any later public API-reference contract change must start with `.fsi`,
  failing-first tests, implementation, and baseline refresh.
- **MVU/effect boundary**: PASS. The feature is repository maintenance and
  documentation generation evaluation, not runtime stateful workflow. File I/O
  remains in scripts/tests/FAKE edge commands.
- **Synthetic evidence**: PASS with restrictions. Positive evidence must come
  from real repository paths and real generated/spike output. Synthetic
  malformed scanner fixtures require `[SEH]` disclosure.
- **Test evidence**: PASS. Add failing-first tests for archive inventory
  fields, current evidence map terms, active-surface stale-reference blocking,
  generated guidance updates, and API-reference generator decision coverage.
- **Observability**: PASS. Reports must include artifact path, feature id,
  classification, archival marker, rationale, owner, replacement/current path,
  scan area, blocking status, package id, generator, sample output path, and
  next action.
- **Deferred scope**: PASS. No release publishing, external documentation
  hosting, broad package redesign, runtime rendering fixes, visual demos, or
  deletion of historical audit material without traceability.

**Pre-design gate result**: PASS. The plan preserves `.fsi` ownership, avoids
runtime workflow changes, keeps real evidence requirements, and treats fsdocs
as a governed documentation-evaluation path rather than an unreviewed
replacement.

## Project Structure

```text
scripts/
  generate-package-api-reference.fsx     # Existing source-shaped reference generator
  archive-readiness-inventory.fsx        # New or extended archive inventory helper if needed
  stale-readiness-reference-scan.fsx      # New or extended active-surface scanner if needed
  template-drift.fsx                     # Existing drift gate; update only if template rules change

docs/
  generated-apps.md                      # Current evidence/API reference guidance
  template-profile.md                    # Generated-product evidence path rules
  dependencies.md                        # Update only if fsdocs is adopted as committed dependency

template/
  base/README.md                         # Generated consumer guidance
  base/docs/product.md                   # Product authoring and evidence guidance

tests/
  Governance.Tests/                      # Archive/stale-reference/guidance scanner tests
  Package.Tests/                         # API reference generator and comparison checks

specs/035-api-discovery-names/
  readiness/package/api-reference/       # Current source-shaped reference material
  readiness/package-surfaces/            # Historical/current supporting package evidence

specs/036-archive-readiness-api-docs/
  plan.md
  research.md
  data-model.md
  quickstart.md
  contracts/
    archive-classification-contract.md
    stale-reference-scan-contract.md
    api-reference-generator-decision-contract.md
    generated-guidance-contract.md
  readiness/
```

## Phase 0: Research

Research is complete in `specs/036-archive-readiness-api-docs/research.md`.
Key decisions:

- Keep historical readiness files in place by default and express archival
  through indexes, markers, and active-surface scanner behavior.
- Treat active docs, templates, generated guidance, build reports, and the
  active feature as blocking stale-reference scan surfaces. Historical specs
  are informational unless cited as current evidence by an active surface.
- Keep `scripts/generate-package-api-reference.fsx` authoritative for agent
  package API authoring because it emits curated `.fsi` source-shaped
  signatures and explicitly avoids reflection/repository-source authoring
  fallback.
- Evaluate FSharp.Formatting/fsdocs as a secondary or hybrid documentation
  surface. fsdocs builds API docs from project assemblies/XML docs, can
  generate HTML and LLM-oriented files, and is useful for browsability, but it
  must prove the source-shaped agent contract before replacing the curated
  reference.
- Compare representative packages: `FS.Skia.UI.Scene`,
  `FS.Skia.UI.Controls`, and `FS.Skia.UI.SkiaViewer` unless implementation
  discovers a better host/adapter sample such as `FS.Skia.UI.Controls.Elmish`.

## Phase 1: Design and Contracts

Design artifacts produced:

- `specs/036-archive-readiness-api-docs/research.md`
- `specs/036-archive-readiness-api-docs/data-model.md`
- `specs/036-archive-readiness-api-docs/contracts/archive-classification-contract.md`
- `specs/036-archive-readiness-api-docs/contracts/stale-reference-scan-contract.md`
- `specs/036-archive-readiness-api-docs/contracts/api-reference-generator-decision-contract.md`
- `specs/036-archive-readiness-api-docs/contracts/generated-guidance-contract.md`
- `specs/036-archive-readiness-api-docs/quickstart.md`

### Post-Design Constitution Check

- **Spec -> FSI -> tests -> implementation**: PASS. No public `.fsi` change is
  planned. Any future package contract change must follow the full `.fsi`
  chain.
- **Visibility in `.fsi`**: PASS. The feature does not change `.fs` public
  visibility or public module surfaces.
- **Idiomatic simplicity**: PASS. Use plain file-system scanners, Markdown
  reports, and deterministic path inventories. fsdocs is isolated to a spike
  unless accepted through dependency governance.
- **MVU/effect boundary**: PASS. No runtime state workflow is changed. Script
  and scanner I/O remains at repository tooling edges.
- **Synthetic disclosure**: PASS with restrictions. Positive evidence is real
  repository/generator evidence; malformed scanner fixtures are `[SEH]` only.
- **Test evidence**: PASS. Quickstart names failing-first governance/package
  tests and sequential FAKE verification.
- **Observability and safe failure**: PASS. Contracts require actionable
  diagnostics for path classification, active-surface stale references,
  generator coverage gaps, fsdocs blockers, and next actions.

## Phase 2: Planning Boundary

Stop after design. Task generation should produce dependency-ordered tasks for
archive inventory, current evidence map, stale-reference scanner tests and
implementation, generated guidance updates, fsdocs/reference comparison,
readiness reports, and sequential FAKE-backed verification.
