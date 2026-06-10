# Surface baselines — recaptured (Tier 1 surface move, T029)

Regenerated via `./fake.sh build -t RefreshSurfaceBaselines` (per-package + cross-package) after the
Tier-1 public surface move. The recapture is the authoritative new baseline for the escalated
`PackageSurfaceCheck` / `PerPackageSurfaceDiff` gates.

## controls-public-surface (cross-package) — `readiness/surface-baselines/FS.Skia.UI.Controls.txt`

New public types + module from `src/Controls/Focus.fsi` (additive):

```text
+FS.Skia.UI.Controls.Focus          (module: order / traverse / route)
+FS.Skia.UI.Controls.FocusStop
+FS.Skia.UI.Controls.TabOrder
+FS.Skia.UI.Controls.FocusMove      (+ Next / Previous)
+FS.Skia.UI.Controls.KeyRouting     (+ Activate / Navigate / Traverse / Fallthrough)
```

No removals from the controls-public-surface — purely additive for consumers; the existing
`view : 'model -> Control<'msg>` contract is unchanged.

## Controls.Elmish package surface — `readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt`

Adds the **internal** `routeFocusedKey` contract (and the updated `runInteractiveApp` doc):

```text
+    val internal routeFocusedKey:
+        retained / focused / order / key / shift -> RetainedRender<'msg> * ControlRuntimeMsg list * 'msg list
```

`internal` accessibility (it takes the internal `RetainedRender`); reached by the adapter tests via
`InternalsVisibleTo`. The per-package snapshot captures the internal `.fsi` line as designed.

## Controls package surface — `readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt`

Captures the new `Focus` public module + types (mirrors the cross-package additions above).

## Accessibility — no signature delta

`Accessibility.fsi` is unchanged — the R1 `defaultFor` / `validate` corrections are behavioral
(`.fs`-only), so no surface delta. The representative typed `Props` (`Widgets/Buttons.fsi`,
`Widgets/Input.fsi`) are unchanged — defaults already supply the metadata.

## Verdict

Baselines recaptured; the diff is **additive** (new `Focus` public surface + internal
`routeFocusedKey`), no removals. Authoritative confirmation is the `PackageSurfaceCheck` /
`PerPackageSurfaceDiff` gate pass recorded in [generated-guidance-validation.md](./generated-guidance-validation.md).
