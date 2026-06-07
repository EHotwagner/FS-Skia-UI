---
title: Evidence and Merge-Gate Audit
category: Design
categoryindex: 4
index: 22
description: The task-evidence model and the merge-gate audit — the five-state task legend, synthetic [S] evidence, computed [S*] propagation through the task DAG, the [SEH] error-handling exception, and the EvidenceGraph / EvidenceAudit gates with practitioner usage.
---

# Evidence and Merge-Gate Audit

A feature in this repository does not advance on assertion. Each task carries an
explicit completion state, and the two governance gates `EvidenceGraph` and
`EvidenceAudit` read those states out of `specs/<feature>/tasks.md`, validate the
task dependency graph, propagate synthetic-evidence taint through it, scan the
diff for risky patterns, and emit a machine-readable verdict. This page explains
the evidence model — the five-state legend, the synthetic `[S]` state, the
computed `[S*]` propagation, and the narrow `[SEH]` error-handling exception —
and then the two gates that consume it, including how to run them and how to
respond to a block. For *which* changes route these gates and at *which* tier,
see [routing and gates](./routing-and-gates.html); for how the generated
governance artifacts are kept current, see
[single-source generation](./single-source-generation.html); for where each
touchpoint lands in the speckit workflow, see
[speckit placement](./speckit-placement.html).

## The five-state task legend

Every task line in a feature's `tasks.md` opens with a status box. The legend is
fixed and is reproduced verbatim at the top of each task list:

| State | Meaning |
|---|---|
| `[ ]` | pending — not started |
| `[X]` | done with **real** evidence |
| `[S]` | done with **synthetic** evidence only (must be disclosed per Principle V) |
| `[F]` | failed |
| `[-]` | skipped, with written rationale |

These five states are the only values a human or agent *writes*. A real
`tasks.md` line looks like this (from the active fsdocs feature):

```text
- [X] T002 [skillist: fsdocs-setup] Pin `fsdocs-tool` in `.config/dotnet-tools.json`; ...
- [ ] T006 [P] [skillist: []] Add a `Governance.Tests` analysis-section check ...
```

The distinction between `[X]` and `[S]` is the load-bearing one. `[X]` claims
the task was proved by *real* evidence — a passing test, a generated artifact, an
actually-exercised path. `[S]` claims the task is functionally complete but its
proof is **synthetic**: a symbolic handle, a canned failure, a convenience mock,
a placeholder output, or any substitute that stands in for the real thing. A
`[S]` task is not a failure, but it is not merge-ready either: it is a disclosed
debt. The repository constitution's Principle V requires that every `[S]` be
disclosed in code, in tests, and in the **Synthetic-Evidence Inventory** table at
the bottom of `tasks.md`.

## The computed `[S*]` state

There is a sixth state, but it is never written by hand — it is **computed** by
the evidence gates and shown in `readiness/task-graph.md`:

> The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
> or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
> evidence audit.

`[S*]` is *propagated taint*. The intuition is that real evidence built on top of
synthetic evidence is not really real: if task `T040` is `[S]` (synthetic) and
task `T055` depends on it and was marked `[X]`, then `T055`'s "real" claim is
resting on a synthetic foundation, so the audit downgrades its **effective**
status to `[S*]`. The propagation rule is exactly:

```text
effective(T) =
    synthetic       if declared(T) == synthetic        (a written [S])
    auto-synthetic  if declared(T) == done AND any dep is (auto-)synthetic
    declared(T)     otherwise
```

A few consequences worth internalising:

- `[S*]` is transitive. Taint flows down the whole dependency chain, not just one
  hop, because an `[S*]` task is itself "(auto-)synthetic" for the purposes of its
  own dependents.
- `[S*]` clears **automatically** once its root-cause `[S]` upstreams are upgraded
  to `[X]`. You never edit an `[S*]`; you fix the `[S]` it descends from and
  re-run the gate. `readiness/task-graph.md` lists the root cause for each `[S*]`
  so you can find the upstream to fix.
- Phase-checkpoint edges are auto-injected: every task in phase *N+1* gets an
  implicit dependency on the last foundation task of phase *N*. These edges do not
  appear in `tasks.deps.yml` and need not be authored by hand, but they do carry
  propagation, so a synthetic foundation task taints the phases after it.

## The `[SEH]` synthetic-error-handling exception

There is one narrow, structured exception to the "synthetic evidence is debt"
rule. A task that exists specifically to test a **malformed-input or explicit
error path** is, by its nature, exercised with a deliberately bad input — and
that input is "synthetic" in the literal sense. Forcing such a task to find
"real" evidence is meaningless; the corrupt payload *is* the evidence.

These tasks may be annotated `[SEH]` and labelled
`synthetic-error-handling-approved`. The key constraints:

- It is still `[S]`. `[SEH]` is a *classification* of a synthetic task, not an
  escape from synthetic status. The audit counts it separately, but it remains
  synthetic.
- It may only be approved **during design, planning, clarification, or task
  generation** — not at implementation time. Implementation-time relabelling is
  rejected; a newly discovered error-path case must be sent back to task/design
  review.
- The Synthetic-Evidence Inventory row for an `[SEH]` task must carry the fields
  `Design source`, `Synthetic input class`, `Expected error behavior`, and
  `Acceptance status=accepted-seh`. Reviewers look for exactly these.

Eligible `[SEH]` cases are genuinely error-path: malformed parser input, corrupt
file content, invalid command arguments, protocol violations, missing required
data, hostile payloads, and forced error-result fixtures. Everything else stays
ordinary synthetic evidence — convenience mocks, incomplete integrations,
unavailable product capability, missing host support, placeholder outputs,
speed-only fixtures, and ordinary in-memory substitutes.

The audit reports `accepted-seh-tasks`, `unaccepted-synthetic-tasks`,
`auto-synthetic-tasks`, and `late-seh-tasks` as **separate** counts, so an
accepted `[SEH]` task is visible as a known, approved exception without ever being
silently treated as real `[X]` evidence.

## The `EvidenceGraph` gate

`EvidenceGraph` validates the structure of the task DAG and refreshes the
computed views. It runs the graph compute in-process in compiled F#
(`FS.Skia.UI.Build.Evidence.Engine.runGraph`) — there is no Python or shell audit
runner. The gate reads `tasks.md`, `tasks.deps.yml`, and `readiness/` at the
build interpreter edge and writes `readiness/task-graph.json` and
`readiness/task-graph.md`.

It validates **graph integrity**:

- every `Tnnn` in `tasks.md` has a matching key in `tasks.deps.yml`, and vice
  versa (no **dangling refs**, no **orphaned keys**);
- every dependency reference resolves to a known `Tnnn`;
- the graph is **acyclic** and no task depends on itself;
- there are no duplicate task ids;
- every task has object-form metadata with `deps` and `skillist`, and the
  `tasks.md` line mirrors the structured `skillist`;
- every declared skill id resolves to exactly one readable local skill.

It then **computes** effective status via the propagation rule above and renders
the `[S]`, `[S*]`, and accepted-`[SEH]` counts separately into the outputs:

- `readiness/task-graph.json` — structured state;
- `readiness/task-graph.md` — a mermaid diagram, an ASCII view, status counts,
  and the propagation report with root-cause annotations.

`EvidenceGraph` is validation and rendering only; it does **not** by itself block
a merge on synthetic evidence. Run it early and often — right after task
generation to confirm the initial DAG is well-formed, and after each status
change during implementation to refresh `[S*]` propagation cheaply.

### Running it

```bash
./fake.sh build -t EvidenceGraph
```

If it fails, it exits non-zero and writes the errors into the verdict block of
`task-graph.md`. Do not proceed until the graph is clean. The common structural
failures and their fixes:

- **Dangling ref** — `tasks.deps.yml` references a `Tnnn` that is not in
  `tasks.md`. Add the task line or remove the reference.
- **Orphaned key** — `tasks.deps.yml` has a key for a `Tnnn` that is not in
  `tasks.md`. Remove the key or add the task line.
- **Cycle** — a set of tasks transitively depend on each other; the message names
  the cycle path. Break it by removing one edge.
- **Duplicate task id** — the same `Tnnn` appears twice. Renumber one.
- **Missing or invalid `skillist`** — regenerate the task list so every task has
  structured `skillist` metadata and a matching visible mirror.
- **Unresolved skill** — fix the skill id, restore the missing `SKILL.md`, or
  disambiguate duplicated skill ids.

## The `EvidenceAudit` gate

`EvidenceAudit` is the **merge-gate** verdict. Like the graph step it runs
in-process (`FS.Skia.UI.Build.Evidence.Engine.runAudit`). It combines two
independent signals and **hard-blocks on either** — it is configured "block on
both":

1. **Synthetic propagation.** It re-runs the graph compute, then counts any
   remaining `[S]` or `[S*]` task against merge-readiness. A feature with any live
   synthetic taint is not merge-ready.
2. **Diff scan.** It greps the unified `git diff <base>...HEAD` (the feature base
   `main`/`master` is auto-detected) against the pattern library in
   `audit-patterns.yml`. **Block-severity** hits count against merge-readiness;
   **advisory-severity** hits print but do not block. (The `SYNTHETIC:`-banner
   pattern is intentionally advisory — seeing those disclosure comments is proof
   that Principle V disclosure is happening, not a violation.)

The gate writes the audit artifacts: `readiness/diff-scan-hits.json` (structured
blocking + advisory findings), the SEH summary, and the scan hit files. It reads
machine-readable status only from a fenced code block whose info string is
exactly `audit-status`; prose, bullets, and other fenced blocks are never read as
status, so a blocker term appearing inside explanatory text cannot raise a false
block. Within that region the first declaration of a key wins, a duplicate key is
a parse error (never silent last-wins), and a malformed entry is a parse error
(never silently passing).

Crucially, an `--accept-synthetic` flag exists but **never changes the verdict**
(Principle V). It is logged with its written justification into
`readiness/synthetic-evidence.json`, but the exit code stays the same. It is a
disclosure mechanism, not a bypass.

### Running it and reading the verdict

```bash
./fake.sh build -t EvidenceAudit
```

Exit codes:

- `0` — **PASS**: no synthetic tasks, no blocking diff-scan hits.
- `2` — **NEEDS-EVIDENCE**: at least one blocking signal (this is still the exit
  code when `--accept-synthetic` is used).
- `3` — graph compute failed (cycles, dangling refs); fix the graph first.
- `4` — usage error.

When you see **NEEDS-EVIDENCE**, walk the report top to bottom:

1. **Declared `[S]` tasks** — can any be upgraded to `[X]` by swapping in real
   evidence? If yes, fix the code, update the task, and re-run. If genuinely an
   approved error-path case, confirm it is annotated `[SEH]` with a complete,
   `accepted-seh` inventory row.
2. **Auto-propagated `[S*]` tasks** — do not touch them; they clear once their
   root-cause `[S]` upstreams clear. The root-cause list is in
   `readiness/task-graph.md`.
3. **Blocking diff-scan hits** — each names a file, line, pattern id, and reason.
   Fix the code (preferred), or if it is genuinely a false positive, extend
   `audit-patterns.yml` with a targeted `file_glob` or `line_regex` whitelist.
4. If merging now is genuinely unavoidable, `--accept-synthetic "written reason"`
   records the justification — it does not clear the block, and the reason should
   be mirrored into the PR description.

## Sequencing

These are FAKE-backed targets. `./fake.sh`, `fake.cmd`, and `dotnet fake` share
repository `.fake` state and are **not** safe to run concurrently. When both are
needed, run them in the fixed order:

1. `./fake.sh build -t EvidenceGraph`
2. `./fake.sh build -t EvidenceAudit`

Non-FAKE file reads may run in parallel, but the two gates themselves must run
sequentially. `EvidenceAudit` re-runs the graph compute internally, so a clean
`EvidenceGraph` first is both a faster iteration loop and a precondition: if the
graph cannot compute (exit `3`), there is nothing for the audit to score.

---

See also: [governance index](./index.html) ·
[routing and gates](./routing-and-gates.html) ·
[single-source generation](./single-source-generation.html) ·
[speckit placement](./speckit-placement.html) ·
[API reference](../reference/index.html).
