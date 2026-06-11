# Dirty-set derivation (SC-004 / FR-003 / FR-004)

evidence-kind=semantic-test
status=pass
authoritative=true
command=dotnet test tests/Controls.Tests/Controls.Tests.fsproj --filter Feature097 ; dotnet test tests/Layout.Tests/Layout.Tests.fsproj --filter Feature097
artifact=tests/Controls.Tests/Feature097WiringTests.fs ; tests/Layout.Tests/Feature097IncrementalTests.fs
failure-class=product-defect

## Derivation (Controls-side, `RetainedRender.layoutDirtySet`, LayoutNodeId domain)

A node is self-dirty iff its `Update` patch:
- sets/removes an attribute that is geometry-driving (name in single-sourced
  `ControlInternals.layoutAffectingAttrNames` = { width, height, orientation }) OR tagged
  `AttrCategory.Layout`, OR
- carries any `ChildInsert` / `ChildRemove` / `ChildMove`.
A `Keep`, a `Replace` (re-measured fresh), or a content/style/state/visual-state `Update` contributes no
self-dirt.

rule-source=FR-003 says classify by `AttrCategory.Layout` and never a hand-maintained name list. In THIS
codebase NO attribute is tagged `AttrCategory.Layout`; `toLayout` derives geometry from attr NAMES. So the
classifier keys on the SAME names `toLayout` reads, single-sourced in one value so it cannot drift — the
anti-drift INTENT of FR-003. (A future `AttrCategory.Layout`-tagged attr is also honoured.)
deviation=mechanism is name-based not category-based; observable SC-004 behaviour is unchanged. Flagged
for maintainer ratification.

## Propagation (Layout-side, FR-004 / SC-004)

verified-via=tests/Layout.Tests/Feature097IncrementalTests.fs:
- a change under a fixed-`Size` (Width+Height Some) ancestor re-measures up to and INCLUDING that
  ancestor and STOPS — the ancestor's box is content-independent, so its ancestors stay clean.
- a fully content-sized chain (no fixed ancestor) propagates to the ROOT (correct, not degenerate).
- a non-layout (content) change dirties NO measure (RemeasuredNodeCount=0, Feature097WiringTests).

## Child-op cases

childinsert=dirties container (verified, byte-identical, Feature097WiringTests)
childremove/childmove=dirty container (same code path; `ChildKeep` alone is not dirtying).

result=all dirty-derivation and propagation cases pass.
