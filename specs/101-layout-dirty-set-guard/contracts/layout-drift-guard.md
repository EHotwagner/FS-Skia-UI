# Contract: Layout Dirty-Set Anti-Drift Guard

R7 exposes **no external/public contract** (no `.fsi` change). The "contracts"
below are the **internal test-facing seams** the enforcement consumes and the
behavioral guarantees the new tests assert. They are the acceptance surface for
US1–US3.

## C1 — Pure drift report (FR-002, FR-003, FR-007)

`layoutDriftReport (discovered: Set<string>) (covered: Set<string>) :
DriftFinding list`

| Given (discovered, covered) | Then findings |
|---|---|
| `({width;height;orientation}, {width;height;orientation})` | `[]` (US1 scenario 4 — shipping state passes) |
| `({width;height;padding}, {width;height})` | `[Uncovered "padding"]` (US1 scenario 1) |
| `({width}, {width;orientation})` | `[OverBroad "orientation"]` (US1 scenario 2) |
| `({a;b}, {b;c})` | `[Uncovered "a"; OverBroad "c"]` (both directions, sorted) |

- Totality: never throws; defined for all `Set<string>` pairs.
- `formatDrift` names every finding's attribute **and** direction in
  human-legible text (FR-007); empty list → an explicit "no drift" string.

## C2 — Behavioral probe equality gate (FR-001, US1, US2, SC-001/SC-002)

The load-bearing assertion:

```
layoutDriftReport (discoverLayoutDrivingNames size) ControlInternals.layoutAffectingAttrNames = []
```

- `discoverLayoutDrivingNames` uses the **real** `ControlInternals.evaluateLayout`
  over representative fixtures + corpus (data-model + research D2).
- Passes today (discovered = `{width;height;orientation}` = covered).
- **Fails the instant** `toLayout` starts reading a corpus name not in the
  literal (under-coverage) **or** the literal lists a name `toLayout` ignores
  (over-coverage), naming the drift via `formatDrift` — converting a silent
  stale-bounds bug into a fast, explicit, named build/test failure.

## C3 — Category honoring is an independent channel (FR-004)

Asserted directly on the **real** `internal layoutDirtySet prev patch next`:

| Scenario | Expectation |
|---|---|
| `Update` with `AttrSet { Name = "elevation"; Category = Layout }`, `"elevation"` ∉ name set | node id ∈ dirty set (category channel dirties) |
| `Update` with `AttrRemoved "elevation"`, prev node carried `elevation` as `Category = Layout` | node id ∈ dirty set (category-recovered-from-prev path, FR-004 edge case) |
| name-set equality gate (C2) run with a category-only attr present | gate does **not** demand the category-only name appear in `layoutAffectingAttrNames` (channels independent — FR-003↔FR-004 resolution) |
| `Update` with `AttrSet { Name = "background"; Category = Visual }` only | node id ∉ dirty set (content/style change does not dirty measure — SC-004) |

## C4 — R2 invariant preservation (FR-005, FR-006, SC-003, SC-004)

R7 asserts these by **re-running existing tests unchanged** (no new code on the
classifier/lowering path):

| Guarantee | Evidence (unchanged) |
|---|---|
| incremental bounds ≡ full evaluation, ≥1000 randomized edit sequences | `tests/Layout.Tests/Feature097IncrementalTests.fs` |
| content-only / style / state / visual-state edit re-measures the same node count as pre-R7 | `WorkReductionRecord.RemeasuredNodeCount` assertions in `tests/Controls.Tests/Feature097WiringTests.fs` |

If either changes, R7 has perturbed behavior and is a regression — gating US3.

## C5 — Surface & route posture (SC-005, research D7)

- No public/internal `.fsi` signature change; new symbols are private/test-local.
- `./fake.sh build -t Route` routes **inner-loop** (`Dev`); no new FAKE gate,
  no `AgentValidation.knownGates` / `validation.contract.yml` edit.
- `EvidenceGraph` + `EvidenceAudit` run sequentially to emit verdict-token
  artifacts with **no synthetic** work (no `[S]`/`[S*]`).

## C6 — Intrinsic-size memo decision recorded (FR-008, SC-006)

The deferral (research D6) is recorded in the feature artifacts and surfaced in
tasks so R8 can reconcile roadmap §10.4 wording without ambiguity. No memo type
or cache is introduced by R7.
