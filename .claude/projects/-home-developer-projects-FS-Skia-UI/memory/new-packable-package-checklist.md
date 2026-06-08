---
name: new-packable-package-checklist
description: All the governance touch-points required when adding a new packable FS.Skia.UI.* library (learned adding FS.Skia.UI.Color, feature 083)
metadata:
  type: project
---

Adding a new packable `FS.Skia.UI.*` library touches many governance single-sources beyond the
`src/<Name>/` project itself. Miss one and a FAKE gate fails. Full checklist (from feature 083,
`FS.Skia.UI.Color`):

- **Project**: `src/<Name>/<Name>.fsproj` (IsPackable, PackageId, Version = shared version,
  ProjectReference deps); `dotnet sln add`.
- **Per-package surface** (`build/Governance/PerPackageSurface.fs`): add to `packagesInScope`
  AND `packageSourceDir`; generate the baseline `readiness/per-package-surface/FS.Skia.UI.<Name>.fsi.txt`
  — RefreshSurfaceBaselines does NOT create per-package baselines; generate by applying
  `PerPackageSurface.normalize` to the concatenated sorted `*.fsi`. Update count assertions in
  `tests/Governance.Tests/PerPackageSurfaceTests.fs` (e.g. "ten"→"eleven", 10→11, both tests).
- **Test wiring** (`build/Governance/Front/Helpers.fs`): add the test project to
  `defaultTestProjects` (else the `Dev`/`Test` gate never runs it), and the pack tuple to
  `packProjects` (else `PackLocal`/`TemplateCheck` lacks the `.nupkg`). Bump the count in
  `tests/Governance.Tests/Feature064PublishTests.fs` ("11 libs"→"12", 12→13 plan rows).
- **Template pin**: `template/base/Directory.Packages.props` `<PackageVersion ... $(FsSkiaUiVersion)>`.
  Update the `fs-skia-template-update` skill: the props-pinned bullet list, the "ten/eleven
  packages" + "eleven/twelve projects" counts, AND the step-5 `for pkg in ...; do` feed loop —
  the loop is the authority `TemplateUpdateSkillPackageCheck` diffs against the packable set.
- **A new build gate** also needs: `Targets` union + `.fsi` + `allTargets` + `name` +
  `directPrerequisites` + `failureOwner` arms; `AgentValidation.knownGates`; `Routing.fs` rule(s);
  the `StartTarget` arm in `Engine/Update.fs`; then `RefreshSurfaceBaselines` regenerates
  `validation.contract.yml` from `Routing.fs`.
- **Token-value changes** ripple to frozen oracles: feature-069 parity tests
  (`tests/Controls.Tests/DesignTokenParityTests.fs` `frozenDark`) and
  `tests/Governance.Tests/DesignTokenGenTests.fs` alias test pin the old value — update them.
- See [[generated-product-check-env-failure]], [[build-package-version-drift-gotcha]],
  [[per-package-baseline-not-in-refresh-target]].
