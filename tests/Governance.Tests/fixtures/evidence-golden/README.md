# Golden Evidence Fixtures — Stage 4 Parity Oracle

Byte-for-byte snapshots of the **current (Python) evidence engine's** output
over a frozen set of merged features, captured at the foundations-baseline SHA.
When Stage 4 re-implements the evidence engine in compiled F#, the
re-implementation MUST reproduce these files byte-for-byte before the Python is
deleted (FR-002, FR-003, SC-002). The engine is consumed **unchanged** (FR-011).

Pinned commit: `34faf1ed61ec0ec2a8a2a81168517cb5ccf499d1` (2026-05-31).

## Files per feature

| File | Producer | Command |
|---|---|---|
| `task-graph.json` | `compute-task-graph.py` | `python3 .specify/extensions/evidence/scripts/python/compute-task-graph.py specs/<F>` |
| `task-graph.md` | `compute-task-graph.py` (same run) | (written by the same command) |
| `audit-counts.txt` | graph-derived merge-gate counts | the four fields the audit prints (`accepted-seh-tasks`, `unaccepted-synthetic-tasks`, `auto-synthetic-tasks`, `late-seh-tasks`) plus `real-tasks`, computed from `task-graph.json` exactly as `run-audit.sh` computes them |

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

Run from the repo root. The engine reads the active feature from
`.specify/feature.json`; point it at each feature during capture so the
`recorded-feature-vs-scanned` warning stays empty (matching the committed
`warnings: []`). Restore historical features afterward so their trees stay
pristine — the committed `task-graph.{json,md}` are already the authoritative
engine output at this SHA:

```bash
cp .specify/feature.json /tmp/feature.bak
for F in 038-authoring-guidance-consistency \
         037-authoring-audit-robustness \
         036-archive-readiness-api-docs; do
  printf '{\n  "feature_directory": "specs/%s"\n}\n' "$F" > .specify/feature.json
  python3 .specify/extensions/evidence/scripts/python/compute-task-graph.py "specs/$F"
  cp "specs/$F/readiness/task-graph.json" "specs/$F/readiness/task-graph.md" \
     "tests/Governance.Tests/fixtures/evidence-golden/$F/"
  git checkout -- "specs/$F/readiness/task-graph.json" "specs/$F/readiness/task-graph.md"
  rm -f "specs/$F/readiness/skill-loading-evidence.template.md"
done
cp /tmp/feature.bak .specify/feature.json
```

`audit-counts.txt` holds the graph-derived merge-gate counts; regenerate them
from `task-graph.json` with the same logic `run-audit.sh` uses (effective-status
tally; accepted-`[SEH]` via the `seh.accepted` flag).

## Reproducibility (FR-003 / SC-002)

Verified at the pinned SHA: re-running `compute-task-graph.py` for each of the
three features regenerates `task-graph.json` and `task-graph.md`
**byte-for-byte identical** (SHA-1 match) to the committed fixtures. `diff`/`cmp`
are unavailable on this host; equality is checked via SHA-1:

```bash
python3 -c "import hashlib,sys;print(hashlib.sha1(open(sys.argv[1],'rb').read()).hexdigest())" <file>
```

Any divergence at the pinned SHA triggers the substitution rule above.

This fixture set is the **Stage 4 parity oracle**.
