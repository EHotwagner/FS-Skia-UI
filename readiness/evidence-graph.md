# EvidenceGraph — the task DAG policy

`./fake.sh build -t EvidenceGraph` computes **in-process** in compiled F#
(`FS.Skia.UI.Build.Evidence.Engine.runGraph`) — no Python or shell runner. It reads
`specs/<feature>/tasks.md` + `tasks.deps.yml` + `readiness/`, validates the graph, and renders
`readiness/task-graph.json` + `readiness/task-graph.md`.

## What it validates

- Every `Tnnn` in `tasks.md` has a matching key in `tasks.deps.yml`, and vice-versa.
- Every dependency reference resolves to a known `Tnnn`; no task depends on itself; the graph is
  acyclic.
- Every task carries object-form metadata with `deps` and `skillist`, and each `tasks.md` line
  mirrors the structured `skillist` exactly and in order.
- Declared skill ids resolve to exactly one readable local capability `SKILL.md`.

## What it computes

Effective status per task under the propagation rule: a `done` task with any `[S]`/`[S*]` dependency
is promoted to `auto-synthetic` (`[S*]`). Phase-checkpoint edges are auto-injected.

## On failure

Exits non-zero with the errors in `task-graph.md`'s verdict block; do not proceed to implement/audit
until the graph is clean. Per-feature verdicts live under `specs/<feature>/readiness/`.
