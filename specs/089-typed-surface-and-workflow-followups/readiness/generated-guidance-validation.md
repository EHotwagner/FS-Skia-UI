# Generated Guidance Validation — Feature 089

`GeneratedGuidanceCheck` validates the generated guidance/currency artifacts after
the `RefreshSurfaceBaselines` regen. For this feature the regenerated, currency-
checked artifacts are:

- the emitted `template/base/docs/api-surface/Controls/*.fsi` tree (14 typed
  `.fsi` added, byte-identical to `src/Controls/Widgets/*.fsi`);
- `src/Controls/catalog.yml` with the new `typedModule:` token per control;
- the `.claude` mirror of the two edited Spec-Kit skills (byte-identical to
  `.agents` — see `skill-sync-check.md`);
- `docs/skillist-reference.md` / `validation.contract.yml` (unchanged — no new gate
  or routing row).

Authoritative command: `./fake.sh build -t GeneratedGuidanceCheck` (recorded in
`logs/`). Failure class for any drift is a **currency failure** naming the drifted
file and the `./fake.sh build -t RefreshSurfaceBaselines` remedy
(`ApiSurfaceGen.currency` / `CatalogGen.currency` / `SkillSyncCheck`). Next action
on failure: re-run `RefreshSurfaceBaselines` and re-validate.
