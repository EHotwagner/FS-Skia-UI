---
name: fsharp-graph-algorithms
description: Hand-roll DAG cycle detection, Kahn topo sort, and synthetic propagation in F#; property-test with FsCheck.
compatibility: F# governance library (build/Governance) under net10.0; build-tooling scope only.
metadata:
  author: fs-skia-ui
  source: docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md
---

# fsharp-graph-algorithms

Capability guidance for the evidence-graph algorithms ported from `compute-task-graph.py`. See the
capability report (`metadata.source`) §4.

## When to use

Computing the task DAG: cycle detection, topological ordering, synthetic-evidence propagation,
root-cause mapping, and phase-checkpoint implicit edges.

## Verdict: hand-roll as pure functions; do NOT pull a graph library

The three algorithms are small, standard, and central. Hand-rolling over a typed `Task`/`Dep`
model:

- **guarantees output parity** with the Python (you own every tie-break and ordering),
- is **pure and testable**,
- adds **zero runtime dependency**.

**QuikGraph rejected** — it would add a C# dependency for ~40 lines of standard code, and the
propagation rule is bespoke and sits outside it anyway. Keep it in mind only if graph work later
outgrows these primitives.

## Algorithms to port faithfully

- **Cycle detection** — 3-colour DFS (WHITE/GRAY/BLACK); a back-edge to a GRAY node is a cycle.
- **Topological sort** — Kahn (repeatedly emit zero-in-degree nodes). Preserve the Python's tie-break
  ordering so rendered output matches.
- **Synthetic propagation** (custom rule):
  `effective(T) = synthetic` if `declared(T)=synthetic`;
  else `auto-synthetic` if `declared(T)=done` AND any dep is `(auto-)synthetic` (except accepted
  `[SEH]`);
  else `declared(T)`. Maintain the upstream root-cause map per task.
- **Phase-checkpoint edges** — inject implicit deps from each Phase N+1 task to the last Phase N
  foundation task (pure list manipulation).

## Property tests (FsCheck v3 via Expecto.FsCheck — adopt)

Encode invariants and let FsCheck shrink counterexamples:

- propagation is **monotone**;
- a graph with **no synthetic roots has no auto-synthetic nodes**;
- topo order **respects every edge**;
- cycle detection finds a cycle **iff** Kahn cannot consume all nodes.

## Cautions

- Parity gate (Invariant 6) against the Stage-0 golden fixtures precedes deleting the Python.
- Accepted-`[SEH]` tasks are an explicit exception in the propagation rule — do not drop it.

Related: [[fsharp-parsing]] (produces the typed model), [[fsharp-code-generation]] (renders the
graph), [[fsharp-build-orchestration]] (test harness).
