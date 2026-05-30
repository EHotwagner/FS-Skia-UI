# US1 Evidence — Fail-Loud Active-Feature Resolution

Covers FR-001, FR-002, FR-003, SC-001, SC-002 and the spec Edge Cases for an
empty/unparseable task file and a recorded-vs-scanned mismatch.

## Authoritative resolution

The active feature is resolved from `.specify/feature.json`'s
`feature_directory` entry, consistently across the three resolution surfaces:

| Surface | Behavior |
|---|---|
| `build.fsx` `activeFeatureId` | Reads `feature_directory`; **no** hardcoded placeholder fallback. Missing/unreadable/empty → hard `failwithf`. |
| `compute-task-graph.py` | Echoes `resolved-feature` + `real-task-count`; surfaces a recorded-vs-scanned mismatch; blocks on an empty/unparseable task file. |
| `common.sh` `get_feature_paths` | Order: `SPECIFY_FEATURE_DIRECTORY` → `feature.json` → branch prefix; an unresolved real feature is terminal-fail, never a stub fallback. |

## Real resolution (correct path) — SC-001 / FR-003

From `readiness/logs/evidence-graph.txt` (real `EvidenceGraph` run on this feature):

```
  resolved-feature: 037-authoring-audit-robustness
  real-task-count: 30
graph validation PASS
```

The audit reports the feature's **real** task count (30), not a 1-task stub.

## Unresolved feature hard-fails — SC-002 / FR-002

`readiness/logs/unresolved-feature.txt` — `build.fsx` with `feature.json` removed
(controlled, auto-restored) fails at load before any target runs:

```
Cannot resolve the active feature: the file does not exist. Expected an
authoritative "feature_directory" entry in .../.specify/feature.json. The
evidence graph/audit refuses to fall back to a placeholder feature (FR-001, FR-002).
```

`readiness/logs/unresolved-common-sh.txt` — `common.sh get_feature_paths` with no
resolvable feature (`exit=1`):

```
ERROR: No real active feature resolved (order: SPECIFY_FEATURE_DIRECTORY ->
.specify/feature.json -> branch prefix). Refusing to fall back to a placeholder.
```

## Empty/unparseable task file is blocking — Edge Case / FR-003

`readiness/logs/empty-task-file.txt` — a resolved feature whose `tasks.md` declares
no tasks is a blocking failure (`exit=2`), never a silent stub count:

```
VALIDATION FAILED
  resolved feature '051-empty-feature' has an empty or unparseable task file
  (...); refusing to report a stub task count
```

## Recorded-vs-scanned mismatch is surfaced — US1 scenario 3

When `feature.json` records one feature but a different directory is scanned,
`compute-task-graph.py` emits a prominent warning naming both:

```
recorded-feature-vs-scanned mismatch: .specify/feature.json records
'<recorded>' but the audit scanned '<scanned>'
```

## Tests

`tests/Governance.Tests/FeatureResolutionRobustnessTests.fs` exercises every path
above against the real scripts (`compute-task-graph.py`, `common.sh`) and asserts
the `build.fsx` placeholder fallback is removed. All 6 pass.
