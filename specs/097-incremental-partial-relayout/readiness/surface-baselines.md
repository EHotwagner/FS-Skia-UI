# Surface baselines (SC-006)

evidence-kind=surface-baseline
status=pass
authoritative=true
command=./fake.sh build -t RefreshSurfaceBaselines ; dotnet test tests/Governance.Tests/Governance.Tests.fsproj
failure-class=product-defect

## Public Layout surface — UNCHANGED (SC-006)

`Layout.evaluateIncremental` keeps its exact signature
(`previous -> changedNodeIds -> available -> root -> LayoutResult`); `LayoutResult` keeps its shape
(`Bounds`/`Diagnostics`/`Invalidated`/`Revision`). Only the BODY changed (stub -> genuine) and the runtime
VALUE of `Invalidated` (FR-001a). No public Layout symbol added or moved.
public-layout-baseline=unchanged.

## Internal surface — moved (escalation; regenerated)

The following are INTERNAL (assembly-internal; reached by tests via `InternalsVisibleTo`):
- `WorkReductionRecord.RemeasuredNodeCount: int` (RetainedRender.fsi)
- `RetainedRender<'msg>.Layout: LayoutResult` (RetainedRender.fsi)
- `ControlInternals.evaluateLayout` now returns the `LayoutResult` too; new
  `ControlInternals.evaluateLayoutIncremental` and `ControlInternals.layoutAffectingAttrNames` (Control.fsi)

Per-package surface captures internal `.fsi`, so the Controls per-package baseline moved.
action=regenerated via `RefreshSurfaceBaselines` (11 per-package baselines) + api-surface tree.
verification=Governance.Tests 573/573 pass (PerPackageSurface zero-drift, ApiSurfaceGen byte-current,
Feature089/060 surface-current all green).
measure-cache=internal (carried `LayoutResult`); metric field=internal. No public baseline delta for them.
