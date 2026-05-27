# Task Generation Evidence

## Assumptions

- The feature remains broad Tier 1 because public runtime contracts, generated product behavior, readiness artifacts, and audit gates all change.
- Phase checkpoint edges are implicit and are not duplicated in `tasks.deps.yml`.
- User-story tasks tagged `[US*]` require vertical-slice evidence through a public entry point before they can be marked `[X]`.
- MVU-bearing tasks must expose owned state, messages, effects, pure `update`, and interpreter evidence.

## Skill Confidence Review

- High confidence: `fs-skia-skiaviewer` for viewer host lifecycle, close reasons, diagnostics, options, image evidence, and generated viewer startup.
- High confidence: `fs-skia-elmish` for pure lifecycle `Model`/`Msg`/`Effect` and interpreter boundaries.
- High confidence: `fs-skia-testing` for generated product validation helpers, template tests, FAKE target wiring, and generated test execution.
- High confidence: `fs-skia-scene` for scene rendering and pixel/image evidence.
- High confidence: `speckit-evidence-graph` and `speckit-evidence-audit` for DAG and readiness gates.
- Medium confidence accepted: `fs-skia-keyboard-input` only where input-device observation or dispatch evidence is materially touched.
- Valid-empty `skillist: []`: readiness-writing, inventory, broad documentation, and aggregate validation tasks with no single capability owner.

## Valid-Empty Dispositions

Tasks with `[skillist: []]` are accepted when the work is cross-cutting documentation, readiness recording, inventory, or aggregate command capture. If implementation discovers a narrower owner, the task metadata must be updated before the task starts.

## SEH Approval Rationale

T014 is the only design-approved synthetic error-handling task. It covers malformed readiness rows, invalid evidence command arguments, corrupt image metadata records, missing generated-validation fields, and hostile artifact paths. These are validation/error-path fixtures and do not replace supported-host visible-window evidence, generated validation evidence, or real image artifacts.

## Graph Validation Expectations

Every task must exist in both `tasks.md` and `tasks.deps.yml`, every dependency must resolve, the graph must remain acyclic, structured `skillist` metadata must mirror visible task lines, and declared skill ids must resolve to exactly one readable skill file. The graph command must be rerun after every status change.

## Risk-Level Evidence Rules

Small validation is acceptable only for isolated documentation or fixture edits. Medium validation is required for a single implementation area or public contract change. Broad validation is required before feature completion because this feature changes runtime behavior, generated templates, validation targets, and evidence/audit governance.
