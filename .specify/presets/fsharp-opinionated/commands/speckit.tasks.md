---
description: "Emit tasks.md and tasks.deps.yml in lockstep, then validate the DAG."
---

# /speckit.tasks

Generate the feature's task breakdown from its spec and plan. This preset
requires TWO files, both in `specs/[FEATURE_ID]/`:

1. **`tasks.md`** — the human checklist. Status legend:
   - `[ ]` pending / `[X]` done (real evidence) / `[S]` synthetic-only /
     `[F]` failed / `[-]` skipped.
   - Never write `[S*]` yourself. That marker is computed by the evidence
     audit from the DAG; writing it by hand will confuse the script.
   - `[SEH]` is an annotation, not a completion status. Use it only for
     design-approved synthetic error-handling tasks that will remain `[S]`
     when completed with malformed-input or explicit error-path synthetic
     evidence.

2. **`tasks.deps.yml`** — the dependency topology and task skill metadata.
   Sibling to `tasks.md`. Every Tnnn id in `tasks.md` MUST appear as a key
   here with both `deps` and `skillist` fields. `skillist` is an ordered list
   of applicable capability skill identifiers, or `[]` when no capability
   skill applies.

Use the preset's `tasks-template.md` and `tasks.deps-template.yml` as
starting points; replace the example task bodies with real work items
derived from the spec and plan.

## Discipline

- **Lockstep emission.** You MUST write both files in the same turn. Never
  emit `tasks.md` without `tasks.deps.yml`, and vice versa. If you only
  have partial information, write placeholder object metadata (`deps: []`,
  `skillist: []`) and note which ones need review in your summary.
- **Compulsory skill evaluation.** Immediately after drafting tasks, evaluate
  every task against available capability skills: `.agents/skills/*/SKILL.md`,
  `src/*/skill/SKILL.md`, template capability skill paths, and active
  generated product skill destinations when applicable. Choose the minimal
  ordered set that would materially help implementation. Capability skills
  are preferred over generic guidance when both match. The validator does not
  re-derive skills from titles: it trusts each declared `skillist` as-written
  and only adds a structured requirement where a task declares gated evidence
  via its `owns:` field. Task titles are free-form and never scanned.
- **Visible skill mirror.** Every task line in `tasks.md` MUST mirror the
  structured `skillist` value as `[skillist: ...]`. Use `[skillist: []]` for
  an empty list. Non-empty mirrors preserve the exact structured order, for
  example `[skillist: speckit-tasks, speckit-evidence-graph]`.
- **Task graph pitfall guidance.** Task titles are free-form — `EvidenceGraph`
  never scans them for capability phrases. `tasks.deps.yml` MUST use one key per
  task id under the top-level `tasks:` wrapper; bare `Tnnn:` keys with no
  wrapper are rejected with one directive error.
- **Structured evidence ownership (`owns:`).** Whether a task owns gated
  graph/audit (or other) evidence is declared only by its `owns:` field, drawn
  from the closed vocabulary `graph-validation` (implies
  `speckit-evidence-graph`), `evidence-audit` (implies `speckit-evidence-audit`),
  `task-generation` (implies `speckit-tasks`), `implementation-loading` (implies
  `speckit-implement`), and `constitution` (implies `speckit-constitution`).
  Each declared `owns:` value requires its implied skill in that task's
  `skillist`; an unknown value is a directive error. Most tasks own nothing.
  Migrate older files that relied on the removed title-trigger matcher by adding
  `owns: [<value>]` to the owning task.
- **Declared skill ids resolve from skill names.** The authoritative registry
  is `.agents/skills/*/SKILL.md`, `src/*/skill/SKILL.md`, and
  `template/fragments/*/skill/SKILL.md`. Declare each skill file's `name:`
  value in `skillist`; do not assume the directory name is accepted when it
  differs.
- **Advisory FS.Skia.UI capability hints.** These hints are non-blocking, and
  every id resolves to a skill a generated consumer registers:
  rendering/scene -> `fs-skia-scene`; viewer/window host ->
  `fs-skia-skiaviewer`; Elmish workflow -> `fs-skia-elmish`;
  keyboard/input -> `fs-skia-keyboard-input`; layout readability ->
  `fs-skia-layout-readability`;
  controls/forms/charts/graphs/DataGrid -> `fs-skia-ui-widgets`;
  generated game HUD readability and public-scene host update ->
  `fs-skia-layout-readability`; deterministic evidence mode and host-warning
  classification -> `fs-skia-evidence-mode`.
- **Phase-checkpoint edges are implicit.** The graph compute script
  auto-injects an edge from every task in Phase N+1 to the last foundation
  task of Phase N. You do NOT repeat those edges in the yml — write only
  non-phase cross-edges (e.g., `T015: [T011, T013]` says US1 impl depends
  on US1 tests and US1 fixtures).
- **Story grouping.** Tasks belong to a phase (Phase 1..N) and optionally a
  user story (`[US1]`, `[US2]`, ...). Keep phases sequential; stories
  within a phase may run in parallel.
- **Tier annotation.** Mark each task `[T1]` or `[T2]` if the phase
  classification differs from the spec's overall tier. Omit when it
  matches.
- **Parallel-safe marker.** `[P]` means "no deps inside this phase" — the
  script can verify it; you SHOULD emit it as a hint.
- **Elmish/MVU applicability.** For any stateful or I/O-bearing story, emit
  explicit tasks for the `.fsi` contract (`Model`, `Msg`, `Effect` or
  `Cmd<Msg>`, `init`, `update`, interpreter boundary), pure transition tests,
  emitted-effect assertions, and real interpreter evidence where safe. For a
  simple pure feature, state that Principle IV is not applicable in the
  evidence-obligations task.
- **Synthetic-evidence inventory.** Include the empty Synthetic-Evidence
  Inventory table from the template. It will grow as `/speckit.implement`
  adds `[S]` tasks.
- **Synthetic error-handling classification.** Assign `[SEH]` plus
  `synthetic-error-handling-approved` only during design, planning,
  clarification, or task generation. Each approved row must include the
  design source, rationale, synthetic input class, expected error behavior,
  and acceptance status. Eligible examples: malformed parser input, corrupt
  file content, invalid command arguments, protocol violations, missing
  required data, hostile payloads, and forced error-result fixtures.
  Non-eligible examples: convenience mocks, incomplete integrations,
  unavailable product capability, missing host support, placeholder outputs,
  speed-only fixtures, and ordinary in-memory substitutes. implementation-time relabeling
  is forbidden; newly discovered cases return to design/task work.
- **Risk-level evidence.** Generated tasks should name small, medium, and broad
  governance risk levels, focused validation for the chosen level, when broad
  validation is required, and how non-authoritative aggregate results are
  recorded.
- **Visual demo skill assignment.** Assign scene rendering -> fs-skia-scene,
  screenshot capture -> fs-skia-skiaviewer, layout readability ->
  fs-skia-layout-readability, persistent viewer launch -> fs-skia-skiaviewer,
  deterministic evidence mode -> fs-skia-evidence-mode,
  generated-package validation -> fs-skia-template-update, graph validation ->
  speckit-evidence-graph, audit validation -> speckit-evidence-audit. Preserve
  implementation-before-evidence, graph-before-audit, and
  debug-before-broad-rerun ordering; the visible mirror
  `[skillist: speckit-tasks, fs-skia-layout-readability]` illustrates exact
  structured order.
- **Visual demo readiness scaffolds.** Enumerate
  `readiness/visual-evidence-honesty.md`, `readiness/window-visibility.md`,
  `readiness/governance-risk-levels.md`,
  `readiness/aggregate-hang-diagnostics.md`,
  `readiness/runtime-limitations.md`,
  `readiness/generated-guidance-validation.md`, and
  `readiness/real-image-evidence.md` before audit. Each readiness file names
  the authoritative command, artifact path, failure class, and next action.

## Validation

Immediately after writing both files, validate the task graph with the canonical
in-process target (the same entry point the `speckit-evidence-graph` skill
documents — there is **no** `run-audit.sh` or other shell/python runner):

```bash
./fake.sh build -t EvidenceGraph
```

The target resolves the feature from `.specify/feature.json`. To validate a
different feature, override with the `SPECKIT_FEATURE_DIR` environment variable:

```bash
SPECKIT_FEATURE_DIR="specs/<FEATURE_ID>" ./fake.sh build -t EvidenceGraph
```

Confirm the echoed `feature-directory=…` and `tasks=<n>` match your feature
before trusting the verdict; if neither `.specify/feature.json` nor an override
resolves a feature, the target fails loudly and never falls back to a sample.

This validates:
- Every Tnnn in `tasks.md` has a matching key in `tasks.deps.yml`, and vice
  versa.
- Every dep reference resolves to a known Tnnn; the graph is acyclic.
- Every task has structured `skillist` metadata and a matching `tasks.md`
  mirror.
- Declared skill ids resolve to exactly one readable skill file.
- Each declared `owns:` value is in the closed vocabulary and carries its
  implied skill in the `skillist`.

Report any failures to the user immediately; refuse to declare the tasks
phase complete until the DAG is clean. For the full merge-gate audit, run
`./fake.sh build -t EvidenceAudit`.
