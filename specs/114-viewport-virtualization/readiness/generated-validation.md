# Generated Validation (feature 114)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Route output

`./fake.sh build -t Route` was run against the working-tree diff. Because the diff changes the public
`FS.Skia.UI.Controls.Elmish` `FrameMetrics` `.fsi` (two new fields `VirtualItemsMaterialized` /
`VirtualItemsTotal`) and the public `FS.Skia.UI.Controls` surface (a trailing `overscan` parameter on
`Collections.visibleRange`, an `Overscan` field on `CollectionModel`/`DataGridModel`, a new
`CollectionPosition` type + `AccessibilityMetadata.Collection` field), plus an internal
`WorkReductionRecord` count seam, `Route` escalates to the **controls-public-surface** tier. Only the
gates Route prints were run, **sequentially** (shared `.fake` state). See
[governance-risk-levels.md](./governance-risk-levels.md) for the authoritative gate list.

## Why the generated project is unaffected at this point

Feature 114 adds a virtualization contract (overscan defaulted to 0), two additive public `FrameMetrics`
fields, an additive `Collections`/`DataGrid`/`Types` surface, and offscreen focus/selection
addressability + a11y totals — all additive. With default overscan the realized window, geometry,
focus/keyboard routing for materialized rows, and every dispatch outcome are byte-identical (FR-006/
FR-016). The generated default/minimal project contents and generated `Dev` behaviour are unchanged;
generated projects gain the two additive `FrameMetrics` fields and the additive overscan/a11y surface
transitively (`OnFrameMetrics` default stays `ignore`, overscan defaults to 0, byte-identical at rest).
The `dotnet new fs-skia-ui` template pins the currently-published package versions; those resolve cleanly
(`package-resolution=resolved`, `package-mismatch=false`, `exact-package-match=true`).
`generated-tests-exist=false` / `generated-tests-ran=not-applicable` because 114 introduces no new
generated-project test; `authoritative=false` because `GeneratedProductCheck` is not the authoritative
signal for this change (the authoritative signal is the controls-public-surface gate set +
`EvidenceAudit verdict=PASS` with 0 synthetic).

## Pin-lag (non-authoritative, merge-resolved)

Any compile gap a generated/product check reports before merge is the template **pin-lag** every
surface-bumping feature carries until merge: `PackLocal` packs the new fields/surface but, under the same
version literal, the NuGet global cache shadows it. This is NOT a version-resolution mismatch — the pins
resolve to real packages. `speckit-merge` packs every packable library with a **bumped** version, pushes,
clears caches, and advances the `template/base/Directory.Packages.props` pin; after that bump the
generated product resolves the post-114 surface. Hence `authoritative=false` for any pre-bump run.
