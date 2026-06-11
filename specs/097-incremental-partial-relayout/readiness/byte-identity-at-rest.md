# Byte-identity at rest and every frame (FR-008 / SC-005)

evidence-kind=semantic-test
status=pass
authoritative=true
command=dotnet test tests/Controls.Tests/Controls.Tests.fsproj ; dotnet test tests/Layout.Tests/Layout.Tests.fsproj
artifact=tests/Controls.Tests/Feature097WiringTests.fs ; whole Controls.Tests suite (Feature091/092/096 byte-identity)
failure-class=product-defect

## Claim

The full per-frame render output is byte-identical to the pre-R2 (full-re-measure) build for EVERY tested
frame — at-rest, localized, content-only, geometry-changing, child-insert, and whole-tree.

## Evidence rows

case=at-rest-empty-patch       scene == Control.renderTree(next)   remeasured=0
case=localized-geometry-edit   scene == Control.renderTree(next)   remeasured=3 of 5
case=content-only-change       scene == Control.renderTree(next)   remeasured=0
case=child-insert              scene == Control.renderTree(next)   remeasured>0
case=whole-tree-relayout       scene == Control.renderTree(next)   remeasured=baseline
regression=Controls.Tests 277/277 pass on the incremental-wired path (Feature091/092 byte-identity +
  Feature096 runtime-bridge parity all green) — the wired path did not alter any computed bound or scene.

## Yoga-rounding-off scope note

R2 disables Yoga's internal pixel rounding so partial relayout is byte-identical. This changes
`Layout.evaluate` output ONLY for fractional/overflow flex layouts; the Controls product path uses integer
geometry, so its output is unchanged (277/277). Explicit snapping is preserved via `snapBounds`. This is a
maintainer-approved widening of R2's "performance-and-metric-only" scope to the layout engine's rounding;
blast radius measured nil across the repo's tests.

result=byte-identical every frame on the wired path.
