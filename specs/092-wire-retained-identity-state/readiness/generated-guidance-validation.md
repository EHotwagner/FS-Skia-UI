# Generated Guidance Validation — Feature 092

`GeneratedGuidanceCheck` validates the generated guidance/currency artifacts after the
`RefreshSurfaceBaselines` regen. For this feature the regenerated, currency-checked artifacts are:

- the emitted `template/base/docs/api-surface/SkiaViewer/SkiaViewer.fsi` (byte-identical to
  `src/SkiaViewer/SkiaViewer.fsi`, now carrying `InteractiveViewerHost.MapKey : 'msg list`);
- the emitted `template/base/docs/api-surface/Controls.Elmish/ControlsElmish.fsi` (the re-keyed
  internal focus seam) and `…/Controls/RetainedRender.fsi` (internal `WorkReductionRecord` +
  `ShiftedNodeCount`, `RetainedRender` + `Theme`, `RetainedInit`, `retainedHitTest`);
- the per-package surface snapshots `readiness/per-package-surface/FS.Skia.UI.SkiaViewer.fsi.txt`,
  `…FS.Skia.UI.Controls.Elmish.fsi.txt`, and `…FS.Skia.UI.Controls.fsi.txt` (recaptured via
  `RefreshSurfaceBaselines` / `PerPackageSurface.captureCurrent`);
- `docs/skillist-reference.md` / `validation.contract.yml` (unchanged — no new gate or routing row).

Authoritative command: `./fake.sh build -t GeneratedGuidanceCheck` (recorded in `logs/`). Failure
class for any drift is a **currency failure** naming the drifted file and the
`./fake.sh build -t RefreshSurfaceBaselines` remedy (`ApiSurfaceGen.currency`). Next action on
failure: re-run `RefreshSurfaceBaselines` (and `PerPackageSurface.captureCurrent` for the per-package
`.fsi.txt` snapshots) and re-validate.
