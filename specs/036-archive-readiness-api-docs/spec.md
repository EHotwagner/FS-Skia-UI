# Feature Specification: Archive Readiness And API Docs

**Feature Branch**: `036-archive-readiness-api-docs`  
**Created**: 2026-05-30  
**Status**: Draft  
**Input**: User description: "archive historical readiness material and other no longer relevant material. also check if reference api material would not be better created by https://fsprojects.github.io/FSharp.Formatting/"

## Clarifications

### Session 2026-05-30

- Q: Which archival strategy should be authoritative for historical readiness material? → A: Keep historical files in place but add archive indexes/markers and update active guidance/scanners to ignore them as current evidence.
- Q: What scope should make stale readiness references blocking? → A: Blocking scan covers active docs, templates, generated guidance, build reports, and the active feature; historical specs are informational only.
- Q: What package scope is required for the FSharp.Formatting/fsdocs comparison? → A: Compare representative packages: Scene, Controls, and one host/adapter package such as SkiaViewer or Controls.Elmish.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Find Current Evidence Without Historical Noise (Priority: P1)

A maintainer reviewing the repository needs to distinguish current authoritative
readiness evidence from historical feature evidence. They can open the
readiness and specs areas and quickly identify which material is current, which
material is archived, and which material must not be used to satisfy current
gates.

**Independent Test**: A reviewer is given a list of readiness files from active,
stable, and historical locations and can classify each file as current,
archived, or roadmap/deferred using repository guidance alone.
Blocking stale-reference failures apply to active docs, templates, generated
guidance, build reports, and the active feature; historical specs and
readiness folders are reported for information without becoming blocking
unless they are cited as current evidence by an active surface.

### User Story 2 - Preserve Auditability While Archiving (Priority: P1)

A contributor cleaning stale readiness files needs to reduce clutter without
destroying traceability. They can move or mark historical material in a way that
keeps links, feature provenance, synthetic-evidence disclosures, and prior
merge evidence available for audit. Historical readiness files remain in place
by default; archival is expressed through indexes, markers, and active
guidance/scanners rather than broad file movement.

**Independent Test**: After archival, historical evidence remains discoverable
by feature id and purpose, while current validation commands and docs no longer
present archived material as pass/fail evidence for the active repository.

### User Story 3 - Decide Whether API Reference Generation Should Use FSharp.Formatting (Priority: P2)

A package maintainer needs to know whether package API reference material is
better produced by the existing source-shaped reference workflow or by
FSharp.Formatting/fsdocs. They can review a side-by-side decision record that
compares source-shaped F# authoring fidelity, generated output quality,
package-consumer discoverability, maintenance cost, dependency impact, and
governance fit.

**Independent Test**: Given the decision record and sample output, a reviewer
can verify that the current source-shaped `.fsi` generator remains the
authoritative agent reference, and that any FSharp.Formatting/fsdocs output is
accepted only as a secondary or hybrid documentation surface when it does not
weaken agent authoring guarantees. The comparison uses representative package
coverage: `Scene`, `Controls`, and one host/adapter package such as
`SkiaViewer` or `Controls.Elmish`.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The repository MUST provide a current-vs-archived evidence map
  that identifies authoritative readiness locations, historical readiness
  locations, roadmap/deferred material, and material safe to remove from active
  review paths.
- **FR-002**: Historical readiness material MUST remain traceable by feature id,
  purpose, original path, archival marker, and archival rationale while staying
  in its original location by default.
- **FR-003**: Current docs and guidance MUST state that archived readiness
  material is not acceptable as evidence for current package, template,
  generated-product, or audit gates.
- **FR-004**: Archival MUST preserve synthetic-evidence disclosures,
  unsupported-host classifications, historical command results, and feature
  provenance when those artifacts are kept.
- **FR-005**: Obsolete or duplicate material MUST either be marked archived with
  a reason, replaced by a pointer to the current authoritative location, or
  explicitly retained with an owner and review rationale. File movement is
  reserved for clearly obsolete generated outputs whose paths are not needed
  for historical audit.
- **FR-006**: The repository MUST keep curated source-shaped `.fsi` reference
  generation as the authoritative package API reference for agents unless a
  later feature explicitly proves a replacement can preserve every agent
  authoring guarantee.
- **FR-007**: The API-reference evaluation MUST compare at least: F# authoring
  spelling fidelity, record-field and union-case visibility, parameter names,
  XML documentation preservation, package-adjacent discoverability, Markdown or
  HTML output suitability, dependency and build impact, and compatibility with
  generated product guidance.
- **FR-008**: Any proposed API-reference generator change MUST prove that clean
  package consumers can still discover Scene primitives, Paint helpers, viewer
  records, keyboard cases, Controls front doors, and mixed Scene/Controls
  qualification guidance without repository source inspection or reflection as
  the authoring strategy.
- **FR-009**: Validation MUST include a stale-material scan that fails when
  archived readiness paths are referenced as current evidence by active docs,
  templates, generated guidance, build reports, or the active feature.
  Historical specs and readiness folders MUST be reported informationally
  unless they are cited as current evidence by an active surface.
- **FR-010**: Validation MUST include a reference-material comparison report
  with sample output from the current workflow and, where feasible, sample
  output from FSharp.Formatting/fsdocs or a documented blocker explaining why
  direct generation could not be fairly evaluated. The report MUST classify
  FSharp.Formatting/fsdocs as secondary/hybrid unless it matches the current
  agent-facing `.fsi` contract. The local sample comparison MUST cover
  `Scene`, `Controls`, and one host/adapter package such as `SkiaViewer` or
  `Controls.Elmish`; remaining packages may be covered by decision rationale
  unless the sample exposes a package-specific risk.
- **FR-011**: The feature MUST update reviewer-facing guidance so future
  contributors know where to place historical evidence, how to cite archived
  material, and which current evidence paths satisfy repository gates.
- **FR-012**: The feature MUST avoid changing runtime behavior, rendering,
  state workflows, or public API contracts unless the API-reference evaluation
  proves that a documentation-only change cannot meet package consumer needs.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package contents may change only if the accepted
  API-reference decision moves package-adjacent reference material into package
  artifacts or changes how package references are produced. Package identities
  and publishing remain out of scope. Generated package consumers may need
  guidance updates if reference paths or formats change.
- **Public contract impact**: Public `.fsi` signatures and surface baselines are
  expected to remain unchanged. Documented public APIs, generated reference
  material, README guidance, sample discovery guidance, and historical
  readiness pointers are in scope.
- **State workflow impact**: Stateful workflow, I/O commands, effects,
  subscriptions, interpreter behavior, and product MVU semantics are out of
  scope. File movement, archival scans, and documentation-generation evaluation
  are repository maintenance activities only.
- **Layout/rendering impact**: Runtime layout, charts, DataGrid behavior,
  rendering, screenshots, Vulkan, Skia output, visual evidence capture, and
  unsupported environment diagnostics are out of scope except where historical
  evidence classification must preserve prior diagnostics honestly.
- **Evidence obligations**: Required real evidence paths include
  `specs/036-archive-readiness-api-docs/readiness/archive-inventory.md`,
  `specs/036-archive-readiness-api-docs/readiness/current-evidence-map.md`,
  `specs/036-archive-readiness-api-docs/readiness/stale-reference-scan.md`,
  `specs/036-archive-readiness-api-docs/readiness/api-reference-generator-evaluation.md`,
  `specs/036-archive-readiness-api-docs/readiness/fsharp-formatting-spike.md`,
  `specs/036-archive-readiness-api-docs/readiness/generated-guidance-check.md`,
  `specs/036-archive-readiness-api-docs/readiness/evidence-graph.md`, and
  `specs/036-archive-readiness-api-docs/readiness/evidence-audit.md`.
- **Unsupported scope**: Release publishing, external documentation hosting,
  broad package redesign, runtime rendering fixes, new visual demos, and
  deletion of historical audit material without traceability are out of scope.
- **Build-target impact**: `GeneratedGuidanceCheck`, `TemplateDrift`,
  `EvidenceGraph`, and `EvidenceAudit` are expected to be relevant.
  `PackLocal`, `PackageSurfaceCheck`, `FsiTranscripts`, `TemplateCheck`,
  `GeneratedProductCheck`, `DependencyReport`, `Verify`, and `Ci` should change
  only if the accepted API-reference decision changes package contents,
  generated product guidance, dependencies, or reference output paths.

## Key Entities

- **Readiness Artifact**: A file or directory containing build, test, template,
  package, generated-product, audit, visual, or diagnostic evidence.
- **Archive Decision**: A record identifying whether an artifact is current,
  archived, replaced, retained, or removable, including rationale and owner.
- **Current Evidence Map**: Reviewer-facing index of authoritative evidence
  paths and the gates they satisfy.
- **Stale Reference**: Any active doc, spec, task, build report, or generated
  guidance entry that presents historical material as current evidence.
  Historical specs and readiness folders are informational scan inputs unless
  referenced from an active surface.
- **API Reference Generator Decision**: The recorded decision that the current
  source-shaped `.fsi` workflow remains authoritative for agents, with
  FSharp.Formatting/fsdocs considered only for secondary or hybrid output.
- **Reference Sample**: A representative package API output used to compare
  authoring fidelity, documentation quality, and package-consumer usefulness.
  Required representative samples are `Scene`, `Controls`, and one
  host/adapter package.

## Assumptions

- Historical feature readiness material has audit value and should usually be
  archived or clearly marked rather than deleted.
- Current authoritative evidence should remain under stable readiness paths or
  the active feature readiness directory named by repository guidance.
- FSharp.Formatting/fsdocs is a candidate for secondary documentation because
  it is designed for F# documentation and API reference generation, but the
  current workflow remains authoritative for agents because it intentionally
  preserves source-shaped F# signatures for package consumers.
- A hybrid outcome is acceptable only if it keeps source-shaped authoring
  references authoritative while using generated documentation output for
  broader browsability or LLM-oriented companion files.

## Success Criteria *(mandatory)*

- **SC-001**: A reviewer can classify at least 95% of sampled readiness paths as
  current, archived, retained, or removable within 10 minutes using the new
  evidence map and archive inventory.
- **SC-002**: Active repository docs, templates, generated guidance, build
  reports, and the active feature contain zero blocking references that cite
  archived readiness material as current pass/fail evidence.
- **SC-003**: 100% of archived retained artifacts have feature id, original
  path, archival marker, rationale, and preservation status.
- **SC-004**: The API-reference decision record covers all required comparison
  dimensions, keeps the current `.fsi` generator authoritative for agents, and
  includes representative FSharp.Formatting/fsdocs output or a documented
  evaluation blocker for `Scene`, `Controls`, and one host/adapter package.
- **SC-005**: Clean package-consumer discovery guarantees from the prior API
  discovery feature remain satisfied after any archival or reference-generator
  decision.
- **SC-006**: EvidenceGraph and EvidenceAudit pass with no unaccepted synthetic
  evidence and no blocking diff-scan hits caused by stale readiness references.
