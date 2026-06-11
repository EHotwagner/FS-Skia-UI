# Surface baselines — recaptured after the Tier-1 internal `.fsi` change (feature 099, R4, T021)

evidence-kind=surface-baselines
status=pass

The only surface move is the **internal** carried-slot type in `src/Controls/RetainedRender.fsi`:
`RetainedUiState.Animation` is generalized from `AnimationState<Transform> option` to the new internal
`AnimationClock option`, and five `internal` helper signatures are added to `module internal
RetainedRender`. Everything is `internal` (assembly-internal, reached by the test assemblies via
`InternalsVisibleTo`), so it is captured in the internal-aware per-package surface but is **not** part
of the public contract.

## Recapture

- `./fake.sh build -t RefreshSurfaceBaselines` — regenerated the api-surface tree and the per-package
  surface baselines. **Exactly one** baseline file changed:
  `readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt`.
- The public **api-surface** tree (`template/base/docs/api-surface/**`) is **unchanged** — `RetainedRender`
  is entirely `internal`, so no public-surface delta (confirmed: no api-surface file in the refresh diff).
- The public `src/Controls.Elmish/ControlsElmish.fsi` `runInteractiveApp` / `InteractiveAppHost` surface
  is **unchanged** (the seam is internal host wiring driven by the already-present `Tick` delta).

## Diff (the entire surface change)

```
readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt
+type internal AnimationClock =
+    { Anim: FS.Skia.UI.Scene.Animation
+      Elapsed: System.TimeSpan
+      Target: VisualState }
+
 type internal RetainedUiState =
-    { Animation: FS.Skia.UI.Scene.AnimationState<FS.Skia.UI.Scene.Transform> option
+    { Animation: AnimationClock option
       Text: TextInputModel option }

 module internal RetainedRender =
+    val internal defaultTransitionDuration: System.TimeSpan
+    val internal advance: delta: System.TimeSpan -> clock: AnimationClock -> AnimationClock
+    val internal clockActive: clock: AnimationClock -> bool
+    val internal updateClockForState: desired: VisualState -> carried: AnimationClock option -> AnimationClock option
+    val internal sampleOnPaint: clock: AnimationClock -> ownScene: FS.Skia.UI.Scene.Scene list -> FS.Skia.UI.Scene.Scene list
```

## Gate confirmation

- `PackageSurfaceCheck` — Status: Ok
- `PerPackageSurfaceDiff` — Status: Ok
- `FsiTranscripts` — Status: Ok

compatibility=internal-only; no public signature changed; the carried slot was never a consumer
surface (`internal`). The only external effect is *more faithful* live rendering for the same `view`;
migration guidance = "none required".
