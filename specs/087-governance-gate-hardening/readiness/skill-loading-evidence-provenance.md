# Skill-loading-evidence provenance + at-implementation gap — feature 087 (T029)

FR-010 / SC-009: skill-loading-evidence rows carry a 9th `provenance` column
(`captured` | `asserted`), and a declared-but-unloaded skill is surfaced **at the
declaring task's implementation point**, not deferred to the `[X]` flip.

## 1. Provenance column (captured vs asserted)

The 9th column is single-sourced in `EvidenceFormatSchema.skillLoadingColumns`
(T006) and mirrored into `template/base/docs/evidence-formats.md`:

```
- columns (in order): TaskId | … | Exception | Provenance
- ordering: provenance ∈ { captured, asserted } (captured = observed during the
  run, recorded at the load action before code changes; asserted = hand-authored)
```

`Audit.validateSkillLoadingEvidence` validates the value when present
(`captured`|`asserted`); a legacy 8-column row is lenient (additive change — does
not break historical features). The 087 `skill-loading-evidence.md` uses BOTH
provenances honestly: rows for skills genuinely loaded before the work began are
`captured` (e.g. T004/T005/T006/T019/T021/T023/T024/T025/T027-codegen); rows whose
load timestamp is hand-authored/verified post-hoc are `asserted` (e.g.
T020/T022/T026/T027-parsing/T028/T029). This is the FR-010 honesty signal working
as designed.

## 2. Provenance validation — test-proven

tests/Governance.Tests "feature 087 US6 …" (532-test suite green):
  * a present provenance outside { captured, asserted } is rejected
    ("invalid provenance"); `captured` accepted; legacy 8-column row lenient.
  * the existing `loaded_at < work_started_at` rule and ISO-8601 are unchanged.

## 3. At-implementation gap report (real 087 tree)

`Audit.skillLoadingGapsAtImplementation merged.Tasks (Some skill-loading-evidence.md)`
reports declared-but-unloaded `(task, skill)` for EVERY task with a non-empty
declared skillist, REGARDLESS of `[X]`/`[S]` status — so the miss surfaces while
the declaring task is being implemented, not only at the `[X]` flip (which
`validateSkillLoadingEvidence` enforces only for Done/Synthetic).

Run over the real 087 tree at this point in implementation:

```
TOTAL-AT-IMPLEMENTATION-GAPS=23
  T007 -> fsharp-build-orchestration
  T008 -> fsharp-build-orchestration
  T008 -> fs-skia-template-update
  T009 -> fsharp-build-orchestration
  T009 -> fs-skia-template-update
  T010 -> fsharp-build-orchestration
  T011 -> fs-skia-template-update
  T011 -> fsharp-build-orchestration
  T012 -> fsharp-io-globbing
  T012 -> fsharp-build-orchestration
  T013 -> fsharp-io-globbing
  T013 -> fs-skia-template-update
  T014 -> fsharp-build-orchestration
  T015 -> fs-skia-template-update
  T016 -> fsharp-io-globbing
  T017 -> fsharp-io-globbing
  T018 -> fsharp-build-orchestration
  T030 -> fsharp-code-generation
  T031 -> fsharp-build-orchestration
  T032 -> fsharp-build-orchestration
  T032 -> fs-skia-template-update
  T033 -> speckit-evidence-graph
  T034 -> speckit-evidence-audit
```

The 23 gaps are exactly the not-yet-implemented tasks (US1/US2/US3 + Phase 9);
the completed tasks (T004-T006, T019-T029) carry rows and are correctly absent
from the gap list. As each remaining task is implemented and its skills loaded,
its rows are authored and the gap clears — the early-surfacing FR-010 demands.

failure-class: a declared-but-unloaded skill deferred to the `[X]` flip (silent
until late); now surfaced at implementation time, and an invalid/missing 9th
provenance value is rejected (FR-010, SC-009).
