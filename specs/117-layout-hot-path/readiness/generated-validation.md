# Generated Validation (feature 117)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Route output

`./fake.sh build -t Route` was run against the working-tree diff. Because the diff changes the public
`FS.Skia.UI.Controls.Elmish` `FrameMetrics` `.fsi` (three new fields `TextMeasureCacheHitCount` /
`TextMeasureCacheMissCount` / `LayoutInvalidatedNodeCount`) plus an internal `RetainedRender` /
`WorkReductionRecord` text-measure-cache + dirty-set seam (and the `ControlInternals` `measureText` /
`setMeasureTextHook` internal helpers), `Route` escalates to the **controls-public-surface** tier. Only
the gates Route printed were run, **sequentially** (shared `.fake` state). See
[governance-risk-levels.md](./governance-risk-levels.md) for the authoritative gate list.

## Why the generated project is unaffected at this point

Feature 117 adds additive observability (three `FrameMetrics` fields) and an internal, correctly-keyed
bounded text-measure cache interposed over `Scene.measureText` — all additive. At-rest rendered output
(the deterministic scene-list goldens), control geometry, fitted font sizes, DataGrid geometry, charts,
focus/keyboard/pointer routing, and every dispatch outcome are byte-identical (FR-004); cache-on ≡
cache-off (the `TextCacheEnabled` always-miss oracle). The generated default/minimal project contents and
generated `Dev` behaviour are unchanged; generated projects gain the three additive `FrameMetrics` fields
transitively (`OnFrameMetrics` default stays `ignore`, the cache is byte-identical at rest). The
`dotnet new fs-skia-ui` template pins the currently-published package versions; those resolve cleanly
(`package-resolution=resolved`, `package-mismatch=false`, `exact-package-match=true`).
`generated-tests-exist=false` / `generated-tests-ran=not-applicable` because 117 introduces no new
generated-project test; `authoritative=false` because `GeneratedProductCheck` is not the authoritative
signal for this change (the authoritative signal is the controls-public-surface gate set +
`EvidenceAudit verdict=PASS` with 0 synthetic).

## Pin-lag (non-authoritative, merge-resolved)

Any compile gap a generated/product check reports before merge is the template **pin-lag** every
surface-bumping feature carries until merge: `PackLocal` packs the new fields but, under the same version
literal, the NuGet global cache shadows it. This is NOT a version-resolution mismatch — the pins resolve
to real packages. `speckit-merge` packs every packable library with a **bumped** version, pushes, clears
caches, and advances the `template/base/Directory.Packages.props` pin; after that bump the generated
product resolves the post-117 surface. Hence `authoritative=false` for any pre-bump run.
