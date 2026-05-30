---
name: speckit-tasks
description: Break down implementation plans into actionable task lists.
compatibility: Requires spec-kit project structure with .specify/ directory
metadata:
  author: github-spec-kit
  source: preset:fsharp-opinionated
---

# Speckit Tasks Skill

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
  are preferred over generic guidance when both match. Treat matching as a
  confidence review, not regex certainty: note matched signals, confidence,
  ambiguity, and reviewer disposition for medium, low, indirect, false-positive,
  or valid-empty cases.
- **Visible skill mirror.** Every task line in `tasks.md` MUST mirror the
  structured `skillist` value as `[skillist: ...]`. Use `[skillist: []]` for
  an empty list. Non-empty mirrors preserve the exact structured order, for
  example `[skillist: speckit-tasks, speckit-evidence-graph]`.
- **Task graph pitfall guidance.** Warn authors before graph validation about
  title trigger phrases and dependency formatting mistakes. Avoid wording such
  as "persistent GUI runtime" on a guidance-only task or "window visibility
  validation fixture" on a non-viewer task when those phrases would imply
  unrelated capability requirements. `tasks.deps.yml` MUST use one object-shaped
  entry per task id with indented `deps` and `skillist` fields; inline maps such
  as `T001: { deps: [], skillist: [] }`, duplicate keys, bare task lists,
  malformed indentation, dangling dependency ids, and mismatched visible
  `skillist` mirrors are invalid. The exact phrase
  "window visibility validation fixture" is a known title trigger phrase
  pitfall. Record window visibility validation fixture examples only when the
  task really owns viewer window evidence. The dependency file uses one key per
  task id. Preserve one key per task id in `tasks.deps.yml`; the required
  object shape uses indented `deps` and `skillist` fields.
- **Blocking trigger groups and readiness prefix.** `EvidenceGraph` enforces
  these Spec Kit title-trigger groups: graph validation (`task graph`,
  `evidence graph`, `readiness validation`, `tasks.deps.yml`,
  `structured task metadata`, `mirror mismatch`, `skillist field`,
  `skillist, list typing`, `obvious capability`,
  `multi-skill dependency order`, `migration blocker`,
  `validator diagnostics`, `EvidenceGraph`); evidence audit (`evidence audit`,
  `diff-scan`, `synthetic propagation`, `readiness-blocking`,
  `EvidenceAudit`); task generation (`/speckit.tasks`, `speckit.tasks`,
  `task-generation`, `task templates`, `tasks-template`, `tasks template`,
  `generated task guidance`, `post-generation skill evaluation`);
  implementation loading (`/speckit.implement`, `speckit.implement`,
  `implementation-loading`, `implementation skill`, `implementation command`,
  `load each`, `skill-load`, `before implementation`); and constitution
  (`constitution`, `constitutional`). Use the exact prefix
  `Complete readiness notes` for setup/readiness aggregation tasks that cite
  required filenames without requesting one of those workflows.
  Safe setup wording examples: `Complete readiness notes for skill-loading evidence
  workflow placeholder`, `Record required readiness filenames for later
  verification`, and `Create placeholder evidence files listed by the plan`.
- **Declared skill ids resolve from skill names.** The authoritative registry
  is `.agents/skills/*/SKILL.md`, `src/*/skill/SKILL.md`, and
  `template/fragments/*/skill/SKILL.md`. Declare each skill file's `name:`
  value in `skillist`; do not assume the directory name is accepted when it
  differs.
- **Advisory FS.Skia.UI capability hints.** These hints are non-blocking:
  rendering/scene -> `fs-skia-scene`; viewer/window host ->
  `fs-skia-skiaviewer`; Elmish workflow -> `fs-skia-elmish`;
  keyboard/input -> `fs-skia-keyboard-input`; layout -> `fs-skia-layout`;
  controls/forms/charts/graphs/DataGrid -> `fs-skia-ui-widgets`;
  generated game HUD readability, public-scene host update, host-warning
  classification, or evidence tasks -> `fs-skia-layout-evidence`.
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

## Validation

Immediately after writing both files, run:

```bash
.specify/extensions/evidence/scripts/bash/run-audit.sh specs/<FEATURE_ID> --graph-only
```

(or invoke `/speckit.evidence.graph` if the extension is installed).

This validates:
- Every Tnnn in `tasks.md` has a matching key in `tasks.deps.yml`.
- Every dep reference resolves to a known Tnnn.
- The graph is acyclic.
- Every task has structured `skillist` metadata and a matching `tasks.md`
  mirror.
- Declared skill ids resolve to exactly one readable skill file.
- High-confidence capability skill omissions, confidence, matched signals,
  reviewer disposition, non-minimal invalid skill sets, and invalid multi-skill
  prerequisite ordering are reported before implementation.

Report any failures to the user immediately; refuse to declare the tasks
phase complete until the DAG is clean.

## If the evidence extension is not installed

Fall back to emitting both files without running the validator. Warn the
user: *"The evidence extension is not installed, so the DAG cannot be
validated. Run `specify extension add evidence` to enable
`speckit.evidence.graph` and `speckit.evidence.audit`."*
