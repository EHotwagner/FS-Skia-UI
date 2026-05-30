# Data Model: Archive Readiness And API Docs

## Readiness Artifact

Represents a file or directory that may be cited as evidence.

- `path`: repository-relative path.
- `featureId`: feature id when path is under `specs/<feature>/`.
- `artifactKind`: build log, test log, package surface, generated-product
  evidence, template evidence, audit report, visual evidence, diagnostic, docs
  page, or other.
- `evidenceRole`: current, archived, roadmap, deferred, supporting, or
  removable.
- `gate`: optional gate or command the artifact satisfies.
- `lastKnownStatus`: pass, fail, unsupported, informational, unknown.
- `preservationStatus`: retained in place, replaced by pointer, moved,
  removable, or deleted.

Validation rules:

- Archived retained artifacts must have `featureId`, `path`, `artifactKind`,
  `archivalMarker`, `rationale`, and `preservationStatus`.
- Current artifacts must name the gate or review purpose they satisfy.
- Removable artifacts must name why audit traceability is not required.

## Archive Decision

Records the maintainer decision for one artifact or artifact group.

- `artifactPath`: repository-relative path or glob.
- `classification`: current, archived, replaced, retained, roadmap,
  deferred, or removable.
- `archivalMarker`: stable text or metadata marker that tells reviewers the
  artifact is historical.
- `rationale`: reason for the classification.
- `owner`: docs, governance, package, template, generated-product, or feature.
- `replacementPath`: current authoritative path when one exists.
- `reviewDate`: date of classification.

Validation rules:

- `classification=archived` cannot satisfy current evidence gates.
- `classification=replaced` requires `replacementPath`.
- `classification=removable` requires an explicit audit-safety rationale.

## Current Evidence Map

Reviewer-facing map of authoritative evidence paths.

- `gate`: evidence gate or review area.
- `currentPath`: authoritative readiness path.
- `supportingPaths`: non-authoritative supporting files.
- `archivedPathPolicy`: how historical paths may be cited.
- `requiredFields`: report fields expected for the gate.
- `verificationCommand`: command that refreshes or checks the evidence.

Validation rules:

- Every required readiness path from the spec must appear.
- The map must state that historical readiness cannot satisfy current
  package, template, generated-product, or audit gates.

## Stale Reference

Scanner finding where an active surface cites archived material as current
evidence or uses obsolete guidance.

- `sourcePath`: active file containing the reference.
- `referencedPath`: cited historical or archived path.
- `scanArea`: active docs, template, generated guidance, build report, active
  feature, or historical informational.
- `severity`: blocking or informational.
- `reason`: why the reference is stale.
- `replacementPath`: current path or guidance when known.
- `line`: optional line number.

Validation rules:

- Findings from active docs, templates, generated guidance, build reports, and
  the active feature are blocking when they present archived evidence as
  current.
- Findings inside historical specs/readiness are informational unless cited by
  an active surface.

## API Reference Generator Decision

Decision record comparing the current `.fsi` workflow with fsdocs.

- `packageId`: package under comparison.
- `currentReferencePath`: current source-shaped output.
- `candidateGenerator`: current-fsi, fsdocs, hybrid, or blocked.
- `candidateOutputPath`: generated sample output or blocker report.
- `authoringFidelity`: pass, partial, fail, or blocked.
- `xmlDocPreservation`: pass, partial, fail, or blocked.
- `packageDiscoverability`: pass, partial, fail, or blocked.
- `dependencyImpact`: none, evaluation-only, committed-tool, or package.
- `decision`: authoritative, secondary, hybrid, rejected, or deferred.
- `nextAction`: specific follow-up.

Validation rules:

- Required package samples are `FS.Skia.UI.Scene`, `FS.Skia.UI.Controls`, and
  one host/adapter package such as `FS.Skia.UI.SkiaViewer` or
  `FS.Skia.UI.Controls.Elmish`.
- fsdocs cannot replace the current workflow unless all authoring fidelity
  checks pass without reflection or repository source inspection as the
  consumer authoring strategy.

## Reference Sample

Concrete output inspected for generator comparison.

- `packageId`: package represented.
- `generator`: current-fsi or fsdocs.
- `outputPath`: repository-relative output path.
- `sourceInputs`: `.fsi`, project, assembly, XML doc, or docs content inputs.
- `sampledSymbols`: required symbols checked.
- `missingSymbols`: symbols not found.
- `diagnostics`: warnings, blockers, or unsupported cases.

Validation rules:

- Samples must include Scene primitives, Paint helpers, Controls front doors,
  viewer or adapter records/cases, and mixed Scene/Controls qualification
  guidance where applicable.
- Missing output must be represented by a blocker with command, error, and
  next action.
