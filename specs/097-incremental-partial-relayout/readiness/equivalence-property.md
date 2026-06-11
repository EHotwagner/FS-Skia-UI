# Equivalence property — `evaluateIncremental` ≡ `evaluate` (US2 / FR-007 / SC-002)

evidence-kind=property-test
status=pass
authoritative=true
command=dotnet test tests/Layout.Tests/Layout.Tests.fsproj --filter Feature097
artifact=tests/Layout.Tests/Feature097IncrementalTests.fs (testList "Feature097 equivalence invariant (FsCheck)")
failure-class=product-defect

## Claim

`Layout.evaluateIncremental`, carrying its previous `LayoutResult` (the measure/bounds cache) forward,
produces `Bounds` **byte-identical** (exact `NodeId -> ComputedBounds` map equality) to a full
`Layout.evaluate` at **every** step of a cumulative random edit sequence — INV-1.

## Method (real generated data, not synthetic fixtures)

- generator=Gen097.treeWithEdits — random `LayoutNode` trees (depth ≤ 3, mixed fixed-size and
  content-sized containers, Row/Column) + a cumulative sequence of 1–5 random size edits.
- per-edit, the new tree is evaluated through BOTH `evaluateIncremental` (cache carried as `previous`)
  and a from-scratch `evaluate`; the `NodeId -> ComputedBounds` maps are compared for exact equality.
- cases=1000 (Config.QuickThrowOnFailure.WithMaxTest 1000); reruns=3 consecutive, all green.
- staleness=covered — the incremental result is carried across the whole sequence, so a long chain
  stresses cache staleness; zero divergences across all steps.

## Key enabling decision

byte-identity-blocker=Yoga internal pixel-grid rounding is absolute-position-dependent, so a re-rooted
boundary subtree rounded flex-distributed fractional sizes a pixel differently than the full tree.
resolution=disable Yoga internal rounding (`YGConfigSetPointScaleFactor 0`); explicit snapping remains
available via the separate `snapBounds`/`PixelSnapPolicy`. With rounding off, flex layout is exact-float
and position-independent, so the re-rooted subtree is exactly byte-identical.
blast-radius=measured: Controls.Tests 277/277 pass; Layout.Tests only required the one test asserting
the old stub's verbatim-echo `Invalidated` updated to the FR-001a honest set. Zero geometry breakage.

result=zero divergences over 3×1000 generated (tree, edit-sequence) cases.
