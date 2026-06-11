# E2 determinism invariants on the incremental-wired path (SC-007)

evidence-kind=semantic-test
status=pass
authoritative=true
command=dotnet test tests/Controls.Tests/Controls.Tests.fsproj
artifact=tests/Controls.Tests/Feature091RetainedRenderTests.fs ; Feature092RetainedRenderTests.fs ; Feature097WiringTests.fs
failure-class=product-defect

## Claim

All E2 determinism invariants continue to hold on the live render seam after R2 wired the incremental
evaluator into `RetainedRender.step`.

## Invariants verified (Controls.Tests 277/277 green)

invariant=RecomputedNodeCount = ChangedSubtreeBound + ShiftedNodeCount   status=holds (Feature092, unchanged paint counters)
invariant=Keep -> reuse (identity-at-rest: zero re-measure, zero paint, same RetainedId)   status=holds (Feature092 + Feature097 at-rest)
invariant=first-frame full paint (init paints once; step diffs from frame 1)   status=holds (Feature091/092)
invariant=KeyCollision diagnostics (duplicate keys -> diagnostic, never throw)   status=holds; evaluateIncremental falls back to full evaluate on duplicate LayoutNodeIds (totality, contract C1)
invariant=determinism (identical (prev,next) -> identical Render + minted RetainedIds)   status=holds

note=the incremental evaluator only changes the SOURCE of `boundsById` (incremental vs full); the
boundsById values are byte-identical, so the paint-reuse walk, the work counters, and the diagnostics are
untouched.

result=all E2 invariants hold on the wired incremental-layout path.
