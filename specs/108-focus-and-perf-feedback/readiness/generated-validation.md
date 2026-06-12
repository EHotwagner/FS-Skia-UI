# Generated Validation (feature 108)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Route output (T039)

`./fake.sh build -t Route` was run against the working-tree diff (the union of the branch-vs-`main`
merge-base diff and the uncommitted/untracked changes). Because the diff edits public `.fsi` across
Controls / Controls.Elmish / KeyboardInput, adds the new `FS.Skia.UI.Controls.Theming` surface and
`SkillSupport.EvidenceTour`, and touches `template/base/docs/**`, `Route` **escalates** to the
controls-public-surface (maintainer-verify) route. The printed routing was:

```
developer-class=framework-author
tier=agent-ready
gates=Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, TemplateCheck, GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck, ControlsDocCoverageCheck, ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck, SkillContractPathCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
dogfood-forced=false
matched-rules=controls-public-surface, generated-template, evidence-governance, specify-catchall, docs-only, package-surface
```

Only the gates Route prints were run, **sequentially** (shared `.fake` state). See
[governance-risk-levels.md](./governance-risk-levels.md) for the authoritative gate list.

## Why the generated project is unaffected at this point

Feature 108 adds only additive F# types/behaviour within existing packages plus two template **doc**
additions (`scaffold-map.md` host-seam note, `interactive-readiness.md`). The `dotnet new fs-skia-ui`
template pins the currently-published package versions; those versions resolve cleanly
(`package-resolution=resolved`, `package-mismatch=false`, `exact-package-match=true`). The 108 public
surface reaches generated projects only after the squash-merge version bump (the separate template
pin track), exactly as every prior controls feature. `generated-tests-exist=false` /
`generated-tests-ran=not-applicable` because 108 introduces no new generated-project test;
`authoritative=false` because `GeneratedProductCheck` is not the authoritative signal for this change
(the authoritative signal is the escalated controls suite + `EvidenceAudit verdict=PASS` with 0
synthetic). Any local `GeneratedProductCheck` environment failure is recorded as **non-authoritative**
environment-class, not a product defect.

## GeneratedProductCheck pin-lag (non-authoritative, merge-resolved)

`GeneratedProductCheck` currently reports a **compile gap** in the generated product's
`src/Product/EvidenceCommands.fs`: the construction site now sets the two additive
`InteractiveAppHost` fields (`MapKeyChord` / `OnFrameMetrics`) that the **published / cached**
`FS.Skia.UI.Controls.Elmish 0.1.113-preview.1` package does not yet expose. `PackLocal` packs the new
API, but under the SAME version literal the NuGet global cache shadows it (the documented
bump-to-clear-cache condition). This is **not** a NU1603 version-resolution mismatch — the pins resolve
to real packages (`package-resolution=resolved`, `package-mismatch=false`, `exact-package-match=true`)
— it is the template **pin-lag** every controls-public-surface feature carries until merge.

`speckit-merge` packs every packable library with a **bumped** version, pushes, clears caches, and
updates the single `template/base/Directory.Packages.props` `<FsSkiaUiVersion>` pin; after that bump
the generated product resolves the post-108 API and `GeneratedProductCheck` is green. The authoritative
signal for this change is the escalated controls-public-surface suite + `EvidenceAudit verdict=PASS`
(0 synthetic), not this pre-bump `GeneratedProductCheck` run — hence `authoritative=false`.
