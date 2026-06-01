---
description: "Merge-gate audit: synthetic propagation + diff-scan. Hard-blocks on either."
---

# /speckit.evidence.audit

Produce a merge-readiness verdict for the current feature. Combines two
signals:

1. **Task graph** (via `speckit.evidence.graph`). Any `[S]` or `[S*]` task
   counts against merge-readiness.
2. **Diff scan** — greps `git diff <base>...HEAD` against the default
   pattern library in `audit-patterns.yml`. Block-severity hits count
   against merge-readiness. Advisory-severity hits print but do not block.

## How to invoke

```bash
./fake.sh build -t EvidenceAudit
```

The merge-gate audit computes in-process in compiled F#
(`FS.Skia.UI.Build.Evidence.Engine.runAudit`) — no Python or shell audit runner.
The feature-base ref is auto-detected (`main` / `master`); the diff-scan reads
the retained `audit-patterns.yml`. `--accept-synthetic` is logged but **never
changes the verdict** (Principle V).

## When it runs

- Automatically as the `after_implement` hook declared in the evidence
  extension's `extension.yml`.
- Manually any time the user wants a readiness snapshot.

## Exit codes

- `0` — PASS. No synthetic tasks, no blocking diff-scan hits.
- `2` — NEEDS-EVIDENCE. At least one blocking signal. (Still the exit code
  when `--accept-synthetic` is used.)
- `3` — graph compute failed (cycles, dangling refs). Fix the graph first.
- `4` — usage error.

## Strictness model

The audit is configured **block on both**: any remaining `[S]` or `[S*]`
AND any block-severity diff-scan hit are hard gates. The
`--accept-synthetic` flag is the only way past; it requires written
justification and is logged. Advisory-severity diff-scan hits are
informational only (the synthetic-banner pattern is intentionally
advisory — seeing `SYNTHETIC:` comments is proof that Principle V
disclosure is happening).

## When you see NEEDS-EVIDENCE

Walk the report top to bottom:

1. **Declared `[S]` tasks** — can any be upgraded to `[X]` by swapping in
   real evidence? If yes, update the task, fix the code, re-run. If no,
   confirm the Synthetic-Evidence Inventory row is current.
2. **Auto-propagated `[S*]` tasks** — these clear automatically once their
   root-cause `[S]` upstreams clear. Check the root-cause list in
   `readiness/task-graph.md`.
3. **Blocking diff-scan hits** — each hit names a file, line, pattern id,
   and reason. Either fix the code (preferred) or, if genuinely a false
   positive, extend the whitelist in `audit-patterns.yml` with a targeted
   `file_glob` or `line_regex`.
4. If merging now is unavoidable (staged rollout, upstream dependency not
   ready), use `--accept-synthetic "written reason"`. This is the
   documented escape hatch, not a bypass. The justification lives in
   `readiness/synthetic-evidence.json` and SHOULD be mirrored into the PR
   description.

## Output

- `specs/<FEATURE_ID>/readiness/task-graph.{json,md}` — refreshed by the
  graph compute step.
- `specs/<FEATURE_ID>/readiness/diff-scan-hits.json` — structured diff
  findings (blocking + advisory).
- `specs/<FEATURE_ID>/readiness/synthetic-evidence.json` — written only
  when `--accept-synthetic` is used.
