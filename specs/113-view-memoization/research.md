# Phase 0 Research: View Memoization and Stable Dependency Contracts

All decisions are grounded in the existing retained render path (features 091–112) and
the source report's Phase 5 (`docs/reports/2026-06-12-1422-controls-performance-framework-research.md`).

## R1 — Memo cache key: `ControlId`, stored in the retained per-identity state

- **Decision**: The seam's logical key is the control's stable **`ControlId`** (the
  `Key ?? structural-path` identity unified in feature 098), exactly as the report and
  the 2026-06-12 clarification specify. The cache is a `Map<ControlId, MemoEntry>`
  carried frame-to-frame in the retained structure (`RetainedRender`), alongside the
  existing `StateByIdentity` layout/clock/text state, and threaded through
  `RetainedRender.step` like the `Layout` bounds cache and `WorkReductionRecord`.
- **Rationale**: The spec's Assumptions require reuse keyed by stable identity +
  dependency and the cache to live "the same place `RetainedRender` holds prior
  layout/state." `ControlId` is the seam contract the report names; it is stable at rest
  (`Key ?? path`). The retained `RetainedId` is the diff's per-node identity — correct
  for focus/clock survival across a positional shift, but the memo seam is a
  control-internal pure-transform cache, not a cross-shift state carrier, so keying it on
  the author-facing `ControlId` matches the clarified contract and keeps the seam
  testable in isolation (a `ControlId` is reproducible in a unit test without driving the
  full diff).
- **Safety**: A `ControlId` that is *unstable* across frames (e.g. a positional path that
  shifts) simply produces a **miss** (no entry for the new key) — never staleness. A
  `ControlId` that *collides* (the existing `KeyCollision` diagnostic case) is already
  surfaced by the diff; the memo seam misses-safe on any unknown/unequal key.
- **Alternatives considered**: keying on `RetainedId` (rejected — `RetainedId` is minted
  by the diff and is not available to a control-internal transform before lowering, and
  the clarification explicitly says `ControlId`); a global process-wide memo table
  (rejected — not frame-scoped, would leak across host instances and defeat determinism).

## R2 — Dependency value: a deterministic structural value, equality is the sole reuse condition

- **Decision**: The caller (the control internal) supplies a **deterministic dependency
  value** that captures **every** input that can change the memoized subtree. For the
  DataGrid row/column projection that is the projection's real inputs — the cell/column
  data and the theme/geometry inputs that feed `gridGeom`. Equality (`=`) of the
  dependency value against the prior frame's stored value is the **sole** reuse
  condition; the seam **never** reuses across an unequal or absent dependency.
- **Rationale**: This is the report's "deterministic dependency value supplied by the
  caller" and FR-005. Using structural equality (not object identity) is what makes a
  stable-but-rebuilt input still hit (the React "one always-new prop defeats memoization"
  failure mode is then attributable to the *dependency value*, surfaced by the stability
  diagnostic). A too-coarse dependency value (omitting a real input) is **caught by the
  memo-on/memo-off parity test** (R4), never shipped as a stale frame (FR-007).
- **Alternatives considered**: object-identity dependency (rejected — FR-005 forbids an
  "object-identity accident"; a rebuilt-but-equal list would always miss); hashing the
  rendered subtree (rejected — that defeats the point: you'd recompute to get the hash).

## R3 — Representative memoized site: the DataGrid row/column projection

- **Decision**: Memoize the **DataGrid row/column projection** (`Control.fs` `gridGeom`,
  the `cells → Scene` tabular projection) as the load-bearing representative site.
  `Style.resolve` (the per-kind `theme + baseStyle + classes + state → ResolvedStyle`
  derivation, called at `Control.fs:594/631/667/704/...`) is kept as a **candidate
  secondary** site — the seam is general enough to wrap it — but the byte-identity proof
  rides on the DataGrid projection.
- **Rationale**: The report names "DataGrid column/row transforms and style resolution"
  and says "prefer high-level control-internal memoization first." The DataGrid
  projection (a) is the report's #1, (b) is genuinely expensive (the 109 corpus already
  drives a non-virtualized 10000-row DataGrid), and (c) produces a **subtree** (a `Scene`
  fragment), matching FR-004's "reuses the previously-lowered subtree." A steady-state
  frame whose DataGrid data + theme are unchanged then records a memo **hit** and skips
  the projection entirely — directly observable in the corpus goldens.
- **Alternatives considered**: a synthetic memoizable control (rejected — FR-003 requires
  a *real high-value* site); migrating all 52 controls (explicitly OUT — only a
  representative site this rung).

## R4 — Memo-off parity oracle (the always-miss switch)

- **Decision**: An internal **always-miss** mode (a switch on the seam / a memo-disabled
  variant of `step`) forces every lookup to a miss, so the rendered scene is produced as
  if memoization did not exist. The memo-on/memo-off scene comparison (FR-006) is the
  authority that the seam changed nothing observable and that the dependency value is not
  too coarse (FR-007).
- **Rationale**: This is FR-008 and the 101 `layoutDriftReport`/097 byte-identity
  precedent — a parity oracle the test drives to prove the optimization is purely
  additive. It is an **internal test switch**, not a public consumer toggle (spec
  Assumptions). When in doubt the seam misses; the oracle proves memo-on ≡ memo-off.
- **Alternatives considered**: comparing against the pre-feature baseline only (kept too —
  SC-002's "and to the pre-feature baseline" — but the memo-on/memo-off switch is the
  *live* same-build oracle that catches a too-coarse dependency, which a frozen baseline
  cannot).

## R5 — Threading `MemoHitCount` / `MemoMissCount` into `FrameMetrics`

- **Decision**: The retained `step` aggregates per-frame memo hits/misses (over all
  memoized sites evaluated that frame) and returns them on its result, mirroring how
  `WorkReductionRecord.RemeasuredNodeCount` is carried. `ControlsElmish.fs` reads them
  from the last step record (the `lastWorkReduction`/`:993` pattern) and populates the new
  public `FrameMetrics.MemoHitCount`/`MemoMissCount` on every construction site — the
  `zero` record (`:1320`, both `0`) and each per-frame branch (pointer-move, tick, key,
  idle, model). An idle frame (no memoizable control evaluated) reports both `0`
  (FR-009).
- **Rationale**: Unlike 112's runtime-state stamp (live-host only → internal count),
  control-internal memoization runs on the **deterministic `Perf.runScript` render
  path**, so the counts are reproducible and golden-assertable — matching how 109/110/111
  added deterministic `FrameMetrics` fields (the 2026-06-12 clarification). The live
  `OnFrameMetrics` sink reports the same fields; the deterministic corpus is the
  authoritative golden evidence.
- **Consequence**: Breaking `ControlsElmish.fsi` `FrameMetrics` change (two new fields) +
  corpus-golden churn (regenerated with `PERF_CORPUS_REGEN=1`), accepted to keep the
  metric observable and regression-proof.

## R6 — Stability-diagnostic algorithm (report-only)

- **Decision**: A pure `Diagnostics` report `val` that, given a control (sub)tree **built
  twice** (the two frames the caller supplies), walks the two trees in parallel and flags
  each attribute/event that compared **unequal** despite the two builds being the *same
  logical tree* — the always-new inputs: a rebuilt `UntypedValue`, a per-frame event
  closure, a rebuilt list, an unstable key. Each finding names the control (`ControlId` +
  `ControlKind`) and the offending input, returned as `ControlDiagnostic`s (reusing the
  existing diagnostic vocabulary). It reports **no** findings when the two builds are
  attribute/event-equal.
- **Rationale**: This is the report's companion deliverable ("identify always-new
  attributes/events that break equality") and FR-011, in the spirit of 101's
  `layoutDriftReport` (a pure report function asserted in tests). It is **report-only,
  NOT an enforced gate** (clarified): consumers legitimately use event closures, so
  failing the build on them would be too aggressive this rung. The diagnostic *reports*
  the instability so an author can decide; an enforced gate may come later.
- **Distinguishing semantic change from instability**: the caller supplies the two builds
  of the *same logical tree* (same model → same `View`), so any unequal attribute/event
  is by construction an always-new input, not a semantic change. (When the model truly
  changed, that is a different tree and out of the report's contract — the report is for
  proving stability across an unchanged model.)
- **Alternatives considered**: an enforced CI gate (rejected this rung per clarification);
  reflection over closures to prove equality (rejected — Principle III; F# closure
  equality is reference-based and that is exactly the signal we report).

## Summary of resolved unknowns

| Unknown | Resolution |
|---|---|
| Memo cache key | `ControlId`; `Map<ControlId, MemoEntry>` carried in `RetainedRender`, threaded through `step` |
| Dependency value | deterministic structural value capturing every subtree input; structural equality is the sole reuse condition |
| Representative site | DataGrid row/column projection (`gridGeom`); `Style.resolve` candidate secondary |
| Parity oracle | internal always-miss switch; memo-on ≡ memo-off authority (FR-006/FR-007/FR-008) |
| Metric threading | step aggregates hits/misses → `FrameMetrics.MemoHitCount`/`MemoMissCount` (deterministic, golden) |
| Stability diagnostic | pure two-build attribute/event-equality report `val`; report-only, asserted in `Controls.Tests` |
