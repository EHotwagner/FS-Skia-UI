# 092 retained focus path non-regression (SC-007, FR-008)

evidence-kind=non-regression
status=pass
authoritative=true
command=dotnet test tests/Elmish.Tests/Elmish.Tests.fsproj ; dotnet test tests/Controls.Tests/Controls.Tests.fsproj
failure-class=product-defect

## What is out of scope and untouched

The 092 retained focus path is **out of scope** for R3 and must **not** regress (FR-008):

- `ControlsElmish.resolveFocus` (returns a `RetainedId`),
- `RetainedRender.retainedHitTest`,
- the `RetainedId` domain and the `StateByIdentity` focus/text/clock map.

R3 corrects a **separate** seam — `Layout.evaluate` hit id + `nearestAuthored` (a `ControlId option`) +
`EventBindings` dispatch. The `RetainedId` focus domain is a distinct identity space; R3 touches neither
`resolveFocus`, `retainedHitTest`, nor the `RetainedId` allocation. The only `RetainedRender` change is the
**additive** `BoundIds` population at the two `ControlRenderResult` construction sites (via `boundIdsOf`),
which is byte-identical to the full rebuild and does not feed the focus path.

## Demonstration

The existing 092 focus suite stays green:

- tests/Elmish.Tests/Feature092LiveSurvivalTests.fs — RetainedId-keyed focus/text survival across a
  sibling-shifting re-render.
- tests/Elmish.Tests/Feature090DispatchTests.fs — the focus-aware text seam (`resolveFocus` /
  `routeFocusedText`) delivers a keystroke to the focused control's RetainedId-keyed state.
- tests/Controls.Tests/Feature092RetainedRenderTests.fs — retained metric / identity invariants.

result=Elmish.Tests 55/55, Controls.Tests 282/282 — focus resolution behavior identical.
