# Contract: Guidance, Readiness, and Task Validator Follow-ups

## Consumer API Map

Generated consumer guidance must identify:

- keyboard key names or normalization entry points used by generated demos
- generated host responsibilities and callbacks
- viewer effects and how they differ from app commands
- adapter command categories used at the Controls/Elmish boundary
- common Scene nodes and construction helpers for HUD/gameplay scenes
- explicit-font guidance for brand or typography guarantees

## Readiness Contract Discovery

Guidance must name `specs/032-sokoban-feedback-followups/readiness/` as the authoritative feature readiness directory for this feature and distinguish it from repository-level evidence output directories.

The guidance must list these required files:

- `default-text-glyph-capture.md`
- `interactive-window-close-evidence.md`
- `consumer-guidance-scan.md`
- `readiness-contract-scan.md`
- `task-guidance-scan.md`

It must also name required terms for governance risk levels, aggregate hang diagnostics, runtime limitations, and supported-host persistent launch evidence when those evidence classes are required by an audit.

## Task Validator Pitfalls

Task-generation guidance must document:

- title wording examples that accidentally trigger required skills or graph validation behavior
- exact `tasks.deps.yml` shape and indentation requirements
- one structured dependency entry per `Tnnn`
- visible `skillist` mirrors on matching `tasks.md` task lines
- failure examples for dangling dependencies, missing task ids, and malformed lists

## Pass Conditions

- A generated-app feature author can find the API map and readiness files before implementation starts.
- Guidance scans find all five follow-up areas from the spec.
- Task authors can identify at least two known validator pitfalls before running `EvidenceGraph`.
