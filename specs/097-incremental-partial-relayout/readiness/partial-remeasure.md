# Partial re-measure on the wired path (US1 / SC-001)

evidence-kind=semantic-test
status=pass
authoritative=true
command=dotnet test tests/Controls.Tests/Controls.Tests.fsproj --filter Feature097
artifact=tests/Controls.Tests/Feature097WiringTests.fs ("localized geometry edit re-measures a proper subset ...")
failure-class=product-defect

## Claim

On the live `RetainedRender.step` path, a localized edit re-measures only the affected fixed-size
boundary subtree and reuses cached bounds for everything else, while the produced `Scene` stays
byte-identical to a full `Control.renderTree` of the same frame.

## Method

- tree=root(stack) -> [ panel(width=200,height=100, a fixed-size boundary) -> [leafA, leafB] ; sibling ]
- edit=leafA width 50 -> 70 (one leaf; no child op).
- baseline-node-count=5; remeasured-node-count=3 (panel + leafA + leafB); reused=root + sibling.
- assertion-1=RemeasuredNodeCount < BaselineNodeCount (strict subset) AND > 0 (the change DID re-measure
  — not a stale-bounds reuse).
- assertion-2=step.Render.Scene == Control.renderTree(next).Scene (byte-identical to a full rebuild).

## Note on the dirty classifier

No attribute in this codebase is tagged `AttrCategory.Layout`; `toLayout` derives geometry from attr
NAMES (`width`/`height`/`orientation`). The dirty-set classifier keys on the single-sourced
`ControlInternals.layoutAffectingAttrNames` (the same names `toLayout` reads) plus any
`AttrCategory.Layout` tag — honouring FR-003's anti-drift intent (one source) while remaining correct.

result=localized edit re-measures 3 of 5 nodes, byte-identical Scene.
