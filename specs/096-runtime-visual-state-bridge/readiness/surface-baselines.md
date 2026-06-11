# Surface baselines — recaptured after the Tier-1 `.fsi` change (feature 096, T026)

evidence-kind=surface-baselines
status=pass

The single public addition is `val deriveVisualState` on `src/Controls/ControlRuntime.fsi`. The host
bridge `applyRuntimeVisualState` is declared `val internal`, so it is captured in the internal-aware
per-package / api-surface tree but is **not** part of the public contract.

## Recapture

- `./fake.sh build -t RefreshSurfaceBaselines` — regenerated the controls-public-surface api-surface
  tree (`template/base/docs/api-surface/Controls/ControlRuntime.fsi`) and the 11 per-package surface
  baselines (`readiness/per-package-surface/`).
- per-package snapshot (`readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt`) now carries both
  the public `deriveVisualState` and the `internal applyRuntimeVisualState` rows.

## Diffs

```
template/base/docs/api-surface/Controls/ControlRuntime.fsi
+    val deriveVisualState: model: ControlRuntimeModel -> controlId: ControlId -> VisualState
+    val internal applyRuntimeVisualState: model: ControlRuntimeModel -> control: Control<'msg> -> Control<'msg>

readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt
+    val deriveVisualState: model: ControlRuntimeModel -> controlId: ControlId -> VisualState
+    val internal applyRuntimeVisualState: model: ControlRuntimeModel -> control: Control<'msg> -> Control<'msg>
```

## Gate confirmation

- `PackageSurfaceCheck` — Status: Ok
- `PerPackageSurfaceDiff` — Status: Ok
- `FsiTranscripts` — Status: Ok

compatibility=purely additive; no existing signature changed; migration guidance = "none required;
the projection is opt-in for direct callers, automatic on the built-in retained host."
