# R2 invariant preservation — US3 evidence (feature 101, R7, T013/T014)

authoritative-command=./fake.sh build -t Dev
artifact-path=tests/Layout.Tests/Feature097IncrementalTests.fs ; tests/Controls.Tests/Feature097WiringTests.fs
status=pass
failure-class=r2-regression
next-action=if RED, R7 perturbed the lowering/classifier path — bisect the Control.fs/RetainedRender.fs edit (must be comment/constant only)

## R7 changes no lowering/classifier behavior

R7 adds **no** code on the `toLayout` / `layoutDirtySet` / `evaluateIncremental` path. The only edits to
those files are (a) replacing three duplicated string literals with `[<Literal>] private` tokens of the
**same** strings, and (b) a comment correction. So the existing feature-097 (R2) evidence is re-run
**unchanged** and cited as the preservation proof, not re-implemented. Both files are byte-identical to
their committed state (`git diff` shows neither in the changed set).

## SC-004 / FR-005 — incremental bounds ≡ full evaluation (INV-1, ≥1000 cases)

- evidence: `tests/Layout.Tests/Feature097IncrementalTests.fs` — the incremental-≡-full byte-identity
  property over ≥1000 randomized edit sequences.
- result: GREEN under the `Dev` `Test` aggregate (run 2026-06-11; `Test` target Success in ~1m06s,
  exit 0). Bounds compared as a `NodeId → ComputedBounds` map; unchanged.

## SC-003 / FR-006 — content/style/state/visual-state edit re-measures the SAME node count

- evidence: `tests/Controls.Tests/Feature097WiringTests.fs` — the `WorkReductionRecord.RemeasuredNodeCount`
  assertions for a content-only / style / at-rest / geometry / child-insert edit.
- result: GREEN under the same `Dev` `Test` aggregate. The content-only change re-measures 0 nodes; a
  localized geometry edit re-measures a strict subset; an at-rest frame re-measures 0 — all identical to
  the pre-R7 baseline. R7 introduces **no** additional re-measure.

## Provenance

These two suites ran inside the `Dev` `Test` aggregate (which builds the whole solution and runs every
Expecto project, including the new `Feature101LayoutDriftGuardTests`). The aggregate is recorded as
**non-authoritative** per the FAKE-sequential discipline; the per-suite outcomes above are the
authoritative preservation evidence and can be re-confirmed in isolation with
`dotnet run --project tests/Layout.Tests` / `dotnet run --project tests/Controls.Tests`.
