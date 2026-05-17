# Setup Evidence Notes

## Scope Alignment

`docs/controls-boundary-refactor-process-report.md`, `spec.md`, `plan.md`,
`data-model.md`, `quickstart.md`, and the contracts all describe the same
follow-up scope: hardening validation and readiness evidence for the Controls
boundary refactor process. The scope is process reliability, scanner accuracy,
target contracts, and readiness honesty. It is not a Controls runtime behavior
change.

## Classification And Constraints

- Tier: Tier 2 internal/process reliability work.
- Product API: no product `.fsi` or public Controls API change is expected.
- Package ownership: no package identity, package content ownership, or active
  Controls ownership change is expected.
- Build workflow: stateful validation work remains modeled through the
  `BuildModel`, `BuildMsg`, `BuildEffect`, pure `update`, and interpreter
  boundary in `build.fsx`.

## `.fsi` Applicability

No product `.fsi` signature file change is part of this feature. The changed
contract is the repository command workflow and readiness evidence contract in
`build.fsx`, governance tests, scanner scripts, docs, and readiness reports.
If a later implementation step discovers that a product public API or `.fsi`
signature must change, implementation must pause and update the feature plan
before making that change.

## Synthetic-Evidence Policy

Scanner fixtures and seeded stale-reference scenarios may use synthetic
fixtures when they are disclosed in tests and evidence. Final readiness
evidence is expected to be real: focused FAKE targets, broad aggregate verdicts,
governance tests, scanner runs, generated product validation, evidence graph,
and evidence audit.

No task is marked `[S]` during setup because these notes are direct evidence
from the feature artifacts and repository documentation, not substitute product
or process execution.

## Prerequisite Artifacts

The setup scaffold exists under `specs/012-fix-refactor-process/readiness/`.
No missing prerequisite artifacts were found for setup. The feature has its
specification, plan, data model, quickstart, contracts, task list, dependency
graph, readiness scaffold, setup notes, and evidence graph output. Foundation
implementation may begin after T005 is marked complete and the evidence graph
refreshes successfully.
