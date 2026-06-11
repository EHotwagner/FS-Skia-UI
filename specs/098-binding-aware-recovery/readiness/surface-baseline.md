# Surface baseline recapture (SC-006, FR-007)

evidence-kind=surface-baseline
status=pass
authoritative=true
command=./fake.sh build -t RefreshSurfaceBaselines ; dotnet test tests/Governance.Tests/Governance.Tests.fsproj
failure-class=product-defect

## Public Controls surface — MOVED (Tier 1, recaptured)

The public `FS.Skia.UI.Controls` surface changed in exactly two additive/documented ways; the api-surface
tree + per-package `.fsi.txt` baselines were recaptured against the T005 pre-change reference (which lacked
both symbols), and the diff shows **exactly** these and nothing else:

```
+      BoundIds: Set<ControlId>                              (Types.fsi — ControlRenderResult<'msg>)
+    val boundIdsOf: control: Control<'msg> -> Set<ControlId>  (Control.fsi — ControlInternals)
```

Plus the documented runtime canonical-id change (no signature change): the `ControlId` **value** for
**unkeyed** controls in the public `Bounds` list and the `ControlEvent.ControlId` payload changes
`Kind → structural-path` (FR-007). `nearestAuthored`'s signature is **unchanged**
(`result -> hit -> ControlId option`); only its behavior widened.

## No other drift

- `render` / `renderTree` / `collectBoundsWith` / `eventBindingsOf` / `dispatch` / `nearestAuthored`
  signatures are unchanged.
- `RetainedRender` / `Reconcile` stay `module internal` (no public-surface entry).
- No DTCG token, no theme/contrast change, no catalog control added.

## Recapture + verification

action=regenerated via `./fake.sh build -t RefreshSurfaceBaselines` (11 per-package baselines + api-surface
tree). A stray timing-only `elapsed-ms` wobble in an unrelated 011 sample-smoke artifact was reverted to
keep the diff scoped to R3.
verification=Governance.Tests pass (PerPackageSurface zero-drift, ApiSurfaceGen byte-current,
Feature089/060 surface-current all green) — confirmed in `validation-log.md`.
single-scheme-note=`Bounds` / `EventBindings` / `BoundIds` / recovery all report the same `Key ?? path` id
for a node (SC-003), proven by Feature098UnifiedSchemeTests.
