# EvidenceAudit — the merge-gate policy

`./fake.sh build -t EvidenceAudit` produces a merge-readiness verdict, computed **in-process** in
compiled F# (`FS.Skia.UI.Build.Evidence.Engine.runAudit`). It combines two hard-blocking signals:

1. **Task graph** (via EvidenceGraph) — any `[S]` or propagated `[S*]` task counts against
   merge-readiness.
2. **Diff scan** — greps the unified `git diff <base>...HEAD` against the pattern library in
   `audit-patterns.yml`. Block-severity hits count against merge-readiness; advisory hits (e.g. the
   `SYNTHETIC:` disclosure banner) print but never block.

## Verdict / exit codes

- `0` — PASS: no synthetic tasks, no blocking diff-scan hits.
- `2` — NEEDS-EVIDENCE: at least one blocking signal (also the exit code under `--accept-synthetic`).
- `3` — graph compute failed (cycles / dangling refs); fix the graph first.

## Strictness

Configured **block on both**. The `--accept-synthetic "reason"` flag is the only documented escape
hatch; it is logged into `readiness/synthetic-evidence.json` and **never** silently changes the
verdict (Principle V). Machine-readable status is read **only** from fenced `audit-status` blocks —
prose and other fenced blocks are never read as status. Per-feature audit artifacts
(`diff-scan-hits.json`, `seh-audit-summary.json`, `task-graph.{json,md}`) live under
`specs/<feature>/readiness/`.
