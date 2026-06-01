# Golden Evidence Fixtures — Stage 4 Parity Oracle

Byte-for-byte snapshots of the **original (Python) evidence engine's** output
over a frozen set of merged features, captured at the foundations-baseline SHA.
Stage 4 re-implemented the evidence engine in compiled F# and proved it
reproduces these files byte-for-byte (FR-002, FR-003, SC-002) **before** the
Python was deleted. As of feature 043 (T029) the Python/Bash engine is
**decommissioned**; these fixtures are now reproduced by the in-process compiled
engine `FS.Skia.UI.Build.Evidence.Engine` and asserted in
`EvidenceGoldenParityTests.fs`.

Pinned commit: `34faf1ed61ec0ec2a8a2a81168517cb5ccf499d1` (2026-05-31).

## Files per feature

| File | Reproduced by (current) | Originally captured from (historical, now removed) |
|---|---|---|
| `task-graph.json` | `Engine.runGraph` → `GraphArtifacts.TaskGraphJson` | the legacy graph-compute Python script |
| `task-graph.md` | `Engine.runGraph` → `GraphArtifacts.TaskGraphMd` | the legacy graph-compute Python script (same run) |
| `audit-counts.txt` | `Engine.runAudit` → `AuditArtifacts.AuditCounts` | graph-derived merge-gate counts (`accepted-seh-tasks`, `unaccepted-synthetic-tasks`, `auto-synthetic-tasks`, `late-seh-tasks`, `real-tasks`) |

The `task-graph.{json,md}` fixtures are identical to the committed
`specs/<F>/readiness/task-graph.{json,md}` at this SHA — re-running the engine
reproduces them byte-for-byte (verified via SHA-1; see "Reproducibility").

## Source features and coverage

| Role | Feature | Coverage it locks |
|---|---|---|
| current / most-recent completed | `038-authoring-guidance-consistency` | latest task/deps schema; all-real (38 done, all synthetic counts 0) |
| historical | `037-authoring-audit-robustness` | audit status-region scanner; skipped `[-]` tasks; all synthetic counts 0 |
| historical (substitute) | `036-archive-readiness-api-docs` | **accepted `[SEH]`** synthetic path — `accepted-seh-tasks=1` (T005) |

### Substitution note (spec Edge Cases; FR-003)

The plan originally named `017-synthetic-error-evidence` as the third source. At
the pinned SHA, `017` **does not produce a stable evidence output**: its graph
compute fails (`exit 3`, `verdict: error`) because its skilled tasks have no
committed `readiness/skill-loading-evidence.md`, so the engine reports validation
errors and the audit halts before a count block is produced. Per the spec's
substitution rule — *"if any selected feature does not produce a stable
(reproducible) evidence output at the pinned commit, substitute another merged
feature and record the substitution rather than committing an unstable fixture"*
— `017` is replaced by `036-archive-readiness-api-docs`, the merged feature that
both (a) passes graph compute deterministically and (b) carries an accepted
`[SEH]` synthetic task, preserving the synthetic-propagation coverage `017` was
chosen for. The substitution is also recorded in
[`docs/reports/_baselines/2026-05-31-foundations.md`](../../../../docs/reports/_baselines/2026-05-31-foundations.md).

> Coverage honesty: none of the three stable sources exercises `auto-synthetic`
> (`[S*]`) or `unaccepted-synthetic` counts (both are 0 across the set). The
> oracle locks the all-real baseline (038, 037) and the accepted-`[SEH]` path
> (036, `accepted-seh-tasks=1`). Exercising `[S*]`/unaccepted propagation in the
> oracle is a documented follow-up for a future stable synthetic-bearing feature.

## Capture procedure (reproducible, non-polluting)

The in-process engine reads the recorded feature as data
(`EvidenceInputs.RecordedFeature`); feeding the scanned feature's own name keeps
the `recorded-feature-vs-scanned` warning empty (matching the committed
`warnings: []`). The committed `task-graph.{json,md}` are the authoritative
engine output at this SHA.

`audit-counts.txt` holds the graph-derived merge-gate counts (effective-status
tally; accepted-`[SEH]` via the `seh.accepted` flag), emitted by
`Engine.runAudit` as `AuditArtifacts.AuditCounts`.

## Reproducibility (FR-003 / SC-002)

`EvidenceGoldenParityTests.fs` builds `EvidenceInputs` for each of the three
features, runs `Engine.runGraph` / `Engine.runAudit`, and asserts the produced
`task-graph.json`, `task-graph.md`, `audit-counts.txt`, and the five scan
outputs are **byte-for-byte identical** to the committed fixtures (DiffPlex
renders the first divergence on mismatch). The historical Python capture command
that originally produced these fixtures has been removed (043 / T029).

Any divergence at the pinned SHA triggers the substitution rule above.

This fixture set is the **Stage 4 parity oracle**.
