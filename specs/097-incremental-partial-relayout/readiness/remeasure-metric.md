# Re-measure metric — `WorkReductionRecord.RemeasuredNodeCount` (US3 / FR-006 / SC-003)

evidence-kind=semantic-test
status=pass
authoritative=true
command=dotnet test tests/Controls.Tests/Controls.Tests.fsproj --filter Feature097
artifact=tests/Controls.Tests/Feature097WiringTests.fs
failure-class=product-defect

## Claim

`WorkReductionRecord` is extended with `RemeasuredNodeCount` (read from the real wired `step`), reporting
partial MEASURE work alongside the existing partial PAINT counts:

- localized update -> RemeasuredNodeCount < BaselineNodeCount (strict reduction) AND a re-paint reduction.
- genuine whole-tree relayout -> RemeasuredNodeCount = BaselineNodeCount (never under-reports real work).
- empty (all-Keep) patch -> RemeasuredNodeCount = 0.

## Evidence rows

case=localized-geometry-edit   remeasured=3  baseline=5  verdict=reduction
case=whole-tree-relayout       remeasured=baseline       verdict=equals-baseline (root orientation change; content-sized chain to root)
case=at-rest-empty-patch       remeasured=0              verdict=zero
case=content-only-change       remeasured=0              verdict=zero (non-layout change does not dirty measure, SC-004)

source=read from `RetainedRender.step(...).WorkReduction`, the live interpreter-edge counter; not assumed.
