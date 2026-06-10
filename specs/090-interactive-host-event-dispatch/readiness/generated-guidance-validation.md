# Generated Guidance Validation — Feature 090

`GeneratedGuidanceCheck` validates the generated guidance/currency artifacts after
the `RefreshSurfaceBaselines` regen. For this feature the regenerated,
currency-checked artifacts are:

- the emitted `template/base/docs/api-surface/Controls/Control.fsi` (byte-identical
  to `src/Controls/Control.fsi`, now carrying the `nearestAuthored` recovery);
- the per-package surface snapshots
  `readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt` and
  `…FS.Skia.UI.Controls.Elmish.fsi.txt` (recaptured via `RefreshSurfaceBaselines`);
- the `.claude` mirror of the edited `fs-skia-evidence-mode` skill (byte-identical
  to `.agents` — see `skill-sync-check.md`).
- `docs/skillist-reference.md` / `validation.contract.yml` (unchanged — no new gate
  or routing row).

Authoritative command: `./fake.sh build -t GeneratedGuidanceCheck` (recorded in
`logs/`). Failure class for any drift is a **currency failure** naming the drifted
file and the `./fake.sh build -t RefreshSurfaceBaselines` remedy
(`ApiSurfaceGen.currency` / `SkillSyncCheck`). Next action on failure: re-run
`RefreshSurfaceBaselines` and re-validate.
