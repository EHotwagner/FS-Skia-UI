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
./fake.sh build -t EvidenceGraph
```

The graph computes **in-process** in compiled F#
(`FS.Skia.UI.Build.Evidence.Engine.runGraph`); there is no Python or shell audit
runner. The gate reads `tasks.md` / `tasks.deps.yml` / `readiness/` at
the `build.fsx` interpreter edge and writes `readiness/task-graph.json` +
`readiness/task-graph.md`.

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

## Authoritative status region (spec 037, US2)

Machine-readable status values are read **only** from a fenced code block whose
info string is exactly `audit-status`. Prose, markdown bullets, and any other
fenced block are never read as status, so a blocker term inside explanatory text
or a negation cannot raise a false block (FR-004, FR-005).

Deterministic resolution rule:

1. **First region wins** — the first `audit-status` region that declares a key
   provides its authoritative value.
2. **Duplicate key within the region is a parse error** — never silent
   last-wins.
3. **Prose never wins** — a key in prose/bullets/other blocks is ignored.
4. **Malformed entry** (missing `=`, empty key) is a parse error — never
   silently treated as passing or failing.

Blocking is structured, not substring (FR-006): the audit blocks on explicit
violating values (`exact-package-match` not in {true,yes},
`package-resolution=nu1603`, `taskbar-only=true`, or `taskbar-entry=true` with
`window-visible=false`) — never on substring presence of `taskbar-only` /
`mismatch` / `nu1603` in text. Scanner:
`.specify/extensions/evidence/scripts/python/audit-status-scan.py`.

## Sequential FAKE Commands

FAKE-backed commands (`./fake.sh`, `fake.cmd`, or `dotnet fake`) share
repository `.fake` state and are not safe to run concurrently. Non-FAKE graph
file reads may run in parallel when they do not invoke FAKE or depend on
`.fake`, but multiple FAKE-backed targets must run sequentially:

1. `./fake.sh build -t EvidenceGraph`
2. `./fake.sh build -t EvidenceAudit`
