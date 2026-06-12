# Generated Validation (feature 111)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Route output

`./fake.sh build -t Route` was run against the working-tree diff. Because the diff makes a breaking
public `.fsi` change to `FrameMetrics` (+ the new `FrameCause` type) in `FS.Skia.UI.Controls.Elmish`,
`Route` routes to the **package-surface** tier (it does NOT escalate to controls-public-surface — the
change touches `Controls.Elmish`, not the Controls catalog `.fsi`, same as feature 109). Only the gates
Route prints were run, **sequentially** (shared `.fake` state). See
[governance-risk-levels.md](./governance-risk-levels.md) for the authoritative gate list.

## Why the generated project is unaffected at this point

Feature 111 changes only `FS.Skia.UI.Controls.Elmish` (the `FrameMetrics`/`FrameCause` surface + the
per-frame scheduler) plus the test/evidence corpus + baselines. The `dotnet new fs-skia-ui` template
pins the currently-published package versions; those versions resolve cleanly
(`package-resolution=resolved`, `package-mismatch=false`, `exact-package-match=true`). The template's
only `FrameMetrics` touchpoint is `template/base/src/Product/EvidenceCommands.fs` which sets
`OnFrameMetrics = ignore` — a host *field*, not a `FrameMetrics` *construction* — so it is unaffected by
the additive fields and needs no edit. The 111 public surface reaches generated projects only after the
squash-merge version bump (the separate template-pin track), exactly as every prior surface feature.
`generated-tests-exist=false` / `generated-tests-ran=not-applicable` because 111 introduces no new
generated-project test; `authoritative=false` because `GeneratedProductCheck` is not the authoritative
signal for this change (the authoritative signal is the package-surface gate set + `EvidenceAudit
verdict=PASS` with 0 synthetic).

## Pin-lag (non-authoritative, merge-resolved)

Any compile gap a generated/product check reports before merge is the template **pin-lag** every
surface feature carries until merge: `PackLocal` packs the new API but, under the same version literal,
the NuGet global cache shadows it. This is NOT a version-resolution mismatch — the pins resolve to real
packages. `speckit-merge` packs every packable library with a **bumped** version, pushes, clears
caches, and advances the `template/base/Directory.Packages.props` pin; after that bump the generated
product resolves the post-111 API. Hence `authoritative=false` for any pre-bump run.
