---
name: speckit-evidence-graph
description: Validate and render the task DAG; compute synthetic propagation.
compatibility: Requires spec-kit project structure with .specify/ directory
metadata:
  author: github-spec-kit
  source: evidence:commands/speckit.evidence.graph.md
---

# /speckit.evidence.graph

Parse `specs/<feature>/tasks.md` and `specs/<feature>/tasks.deps.yml`,
validate the graph (acyclic, no dangling refs, every id present in both
files), validate task `skillist` metadata and mirrors, and render
`readiness/task-graph.json` + `readiness/task-graph.md`.

## How to invoke

```bash
.specify/extensions/evidence/scripts/python/compute-task-graph.py specs/<FEATURE_ID>
```

or via the audit runner in graph-only mode:

```bash
.specify/extensions/evidence/scripts/bash/run-audit.sh specs/<FEATURE_ID> --graph-only
```

## When to run

- Right after `/speckit.tasks` — confirms the initial DAG is well-formed
  before implementation begins.
- After every status change during `/speckit.implement` — refreshes `[S*]`
  propagation cheaply.
- Automatically as the `before_implement` hook (declared in the evidence
  extension's `extension.yml`) — refuses to start implement on a broken
  graph.

## What it validates

- Every `Tnnn` in `tasks.md` has a matching key in `tasks.deps.yml`.
- Every `Tnnn` in `tasks.deps.yml` has a matching task line in `tasks.md`.
- Every dep reference resolves to a known `Tnnn`.
- The graph is acyclic.
- No task depends on itself.
- Every task has object-form metadata with `deps` and `skillist`.
- Every `tasks.md` task line mirrors the structured `skillist`.
- Declared skill ids resolve to exactly one readable local capability skill.
- Obvious capability omissions and invalid multi-skill prerequisite ordering
  are readiness failures.

## What it computes

Effective status per task, under the propagation rule:

```
effective(T) =
    synthetic      if declared(T) == synthetic
    auto-synthetic if declared(T) == done AND any dep is (auto-)synthetic
    declared(T)    otherwise
```

Phase-checkpoint edges are auto-injected (every task in Phase N+1 gets an
implicit edge to the last foundation task of Phase N). These do not appear
in `tasks.deps.yml` and do not need to be written by hand.

## On failure

The script exits non-zero and writes the errors into `task-graph.md`'s
verdict block. Do not proceed with `/speckit.implement` until the graph
is clean.

Common failure modes and their fixes:

- **Dangling ref** — `tasks.deps.yml` references `Tnnn` that isn't in
  `tasks.md`. Add the task line or remove the ref.
- **Orphaned key** — `tasks.deps.yml` has a key for `Tnnn` that isn't in
  `tasks.md`. Remove the key or add the task line.
- **Cycle** — a set of tasks transitively depend on each other. The error
  message names the cycle path. Break it by removing one edge.
- **Duplicate task id** — the same `Tnnn` appears twice in `tasks.md`.
  Renumber one.
- **Missing or invalid `skillist`** — migrate or regenerate the task list so
  every task has structured `skillist` metadata and a matching visible mirror.
- **Unresolved skill** — fix the skill id, restore the missing `SKILL.md`, or
  resolve ambiguous duplicated skill ids before implementation.

## Output

- `specs/<FEATURE_ID>/readiness/task-graph.json` — structured state.
- `specs/<FEATURE_ID>/readiness/task-graph.md` — mermaid diagram, ASCII
  view, status counts, propagation report with root-cause annotations.

Commit both files alongside the feature's other artifacts.

## Sequential FAKE Commands

FAKE-backed commands (`./fake.sh`, `fake.cmd`, or `dotnet fake`) share
repository `.fake` state and are not safe to run concurrently. Non-FAKE graph
file reads may run in parallel when they do not invoke FAKE or depend on
`.fake`, but multiple FAKE-backed targets must run sequentially:

1. `./fake.sh build -t EvidenceGraph`
2. `./fake.sh build -t EvidenceAudit`
