# Generated Validation (feature 112)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Route output

`./fake.sh build -t Route` was run against the working-tree diff. Because the diff adds an internal
`ControlRuntime.fsi` seam (a new type + vals) to `FS.Skia.UI.Controls`, `Route` escalates to the
**controls-public-surface** tier. Only the gates Route prints were run, **sequentially** (shared `.fake`
state). See [governance-risk-levels.md](./governance-risk-levels.md) for the authoritative gate list.

## Why the generated project is unaffected at this point

Feature 112 changes only internal `FS.Skia.UI.Controls` (`ControlRuntime`) + `FS.Skia.UI.Controls.Elmish`
(the live `renderRetained` seam) plus the Controls.Tests suite. There is **no public API change** (the
new seam is internal). The `dotnet new fs-skia-ui` template pins the currently-published package versions;
those resolve cleanly (`package-resolution=resolved`, `package-mismatch=false`, `exact-package-match=true`).
The template has no touchpoint on the internal `ControlRuntime` stamp. `generated-tests-exist=false` /
`generated-tests-ran=not-applicable` because 112 introduces no new generated-project test;
`authoritative=false` because `GeneratedProductCheck` is not the authoritative signal for this change
(the authoritative signal is the controls-public-surface gate set + `EvidenceAudit verdict=PASS` with 0
synthetic).

## Pin-lag (non-authoritative, merge-resolved)

Any compile gap a generated/product check reports before merge is the template **pin-lag** every
surface-bumping feature carries until merge: `PackLocal` packs the new internals but, under the same
version literal, the NuGet global cache shadows it. This is NOT a version-resolution mismatch — the pins
resolve to real packages. `speckit-merge` packs every packable library with a **bumped** version, pushes,
clears caches, and advances the `template/base/Directory.Packages.props` pin; after that bump the
generated product resolves the post-112 internals. Hence `authoritative=false` for any pre-bump run.
