# Generated Validation (feature 116)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Route output

`./fake.sh build -t Route` was run against the working-tree diff. Because the diff changes the public
`FS.Skia.UI.Controls.Elmish` `FrameMetrics` `.fsi` (six new fields `RepaintedNodeCount` /
`DirtyRectCount` / `DirtyArea` / `PictureCacheHitCount` / `PictureCacheMissCount` /
`PictureCacheEntryCount`) and the public `FS.Skia.UI.Controls` `Types` surface (an additive advisory
`ControlDiagnosticCode.OffscreenComposition` case), plus an internal `RetainedRender` /
`WorkReductionRecord` damage-set + picture-cache seam, `Route` escalates to the **controls-public-surface**
tier. Only the gates Route prints were run, **sequentially** (shared `.fake` state). See
[governance-risk-levels.md](./governance-risk-levels.md) for the authoritative gate list.

## Why the generated project is unaffected at this point

Feature 116 adds additive observability (six `FrameMetrics` fields), an additive advisory diagnostic
case, an internal damage-set + a correctly-keyed bounded picture cache, and an offscreen-effect
diagnostic — all additive. At-rest rendered output (the deterministic scene-list goldens), control
geometry, focus/keyboard/pointer routing, and every dispatch outcome are byte-identical (FR-014);
cache-on ≡ cache-off (FR-007). The generated default/minimal project contents and generated `Dev`
behaviour are unchanged; generated projects gain the six additive `FrameMetrics` fields and the additive
diagnostic case transitively (`OnFrameMetrics` default stays `ignore`, the cache is byte-identical at
rest, the diagnostic is advisory). The `dotnet new fs-skia-ui` template pins the currently-published
package versions; those resolve cleanly (`package-resolution=resolved`, `package-mismatch=false`,
`exact-package-match=true`). `generated-tests-exist=false` / `generated-tests-ran=not-applicable` because
116 introduces no new generated-project test; `authoritative=false` because `GeneratedProductCheck` is
not the authoritative signal for this change (the authoritative signal is the controls-public-surface
gate set + `EvidenceAudit verdict=PASS` with 0 synthetic).

## Pin-lag (non-authoritative, merge-resolved)

Any compile gap a generated/product check reports before merge is the template **pin-lag** every
surface-bumping feature carries until merge: `PackLocal` packs the new fields/surface but, under the same
version literal, the NuGet global cache shadows it. This is NOT a version-resolution mismatch — the pins
resolve to real packages. `speckit-merge` packs every packable library with a **bumped** version, pushes,
clears caches, and advances the `template/base/Directory.Packages.props` pin; after that bump the
generated product resolves the post-116 surface. Hence `authoritative=false` for any pre-bump run.
