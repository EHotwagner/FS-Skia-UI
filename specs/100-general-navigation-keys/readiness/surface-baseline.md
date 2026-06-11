# Surface baselines — recaptured after the Tier-1 `.fsi` change (feature 100, R5, T020)

evidence-kind=surface-baselines
status=pass

The surface moves are confined to the three Controls modules named in the plan; the **public**
`src/Controls.Elmish/ControlsElmish.fsi` `runInteractiveApp` / `InteractiveAppHost` surface is
**unchanged** (the per-intent resolver stayed module-internal), and `Payload : string option` is
**retained** on `ControlEvent`.

## Recapture

- `./fake.sh build -t RefreshSurfaceBaselines` (one operation regenerates the api-surface tree, the
  stable package surface baselines, and the per-package `.fsi.txt` snapshots — feature 087 folded the
  per-package regeneration into this target).
- Exactly four surface artifacts changed, all for `FS.Skia.UI.Controls`:
  - `readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt`
  - `readiness/surface-baselines/FS.Skia.UI.Controls.txt`
  - `template/base/docs/api-surface/Controls/Types.fsi`
  - `template/base/docs/api-surface/Controls/Accessibility.fsi`
- No other package's surface changed; no `ControlsElmish` surface delta.

## The entire surface change (per-package diff)

```
+ [<RequireQualifiedAccess>] type Direction = Previous | Next | First | Last      (new — Focus)
+ type NavIntent = ValueStep of float | SelectionMove of Direction | GridMove of int * int   (new — Focus)
- | Navigate                       ->  + | Navigate of NavIntent                  (KeyRouting case widened)
- val route: keyboard ... -> KeyRouting
+ val route: role: AccessibilityRole -> keyboard -> navRange: NavRange option -> key -> isTab -> shift -> KeyRouting
+ type NavRange = { Step: float; Min: float; Max: float }                          (new — Types)
+ AccessibilityMetadata gains `Navigation: NavRange option`                        (new field)
+ type NavPayload = SteppedValue of float | MovedSelection of int * string option | MovedCell of int * int   (new — Types)
+ ControlEvent gains `Nav: NavPayload option` (Payload: string option RETAINED)    (new field)
+ Accessibility.metadata gains a trailing `navRange: NavRange option` parameter
```

This is exactly the `Direction`/`NavIntent`/widened `route` (Focus), `NavRange`/`NavPayload`/
`ControlEvent.Nav`/`AccessibilityMetadata.Navigation` (Types), and widened `metadata` (Accessibility)
set the plan declared — no other drift. Confirmed by `PackageSurfaceCheck` + `PerPackageSurfaceDiff`
in [generated-guidance-validation.md](./generated-guidance-validation.md).
