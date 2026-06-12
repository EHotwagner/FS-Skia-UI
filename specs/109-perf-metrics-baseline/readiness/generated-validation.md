# Generated Validation (feature 109)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Route output

`./fake.sh build -t Route` was run against the working-tree diff (the union of the branch-vs-`main`
merge-base diff and the uncommitted/untracked changes). Because the diff makes a **breaking public
`.fsi` change** to `FrameMetrics` in `FS.Skia.UI.Controls.Elmish`, `Route` **escalates** to the
controls-public-surface (maintainer-verify) route. Only the gates Route prints were run, **sequentially**
(shared `.fake` state). See [governance-risk-levels.md](./governance-risk-levels.md) for the
authoritative gate list.

## Why the generated project is unaffected at this point

Feature 109 changes only the `FrameMetrics` field set within `FS.Skia.UI.Controls.Elmish` plus
test/evidence-project corpus + baselines. The `dotnet new fs-skia-ui` template pins the currently-
published package versions; those versions resolve cleanly (`package-resolution=resolved`,
`package-mismatch=false`, `exact-package-match=true`). The template's only `FrameMetrics` touchpoint is
`template/base/src/Product/EvidenceCommands.fs` which sets `OnFrameMetrics = ignore` — a host *field*,
not a `FrameMetrics` *construction* — so it is unaffected by the field rename and needs no edit. The
109 public surface reaches generated projects only after the squash-merge version bump (the separate
template-pin track), exactly as every prior controls-public-surface feature.
`generated-tests-exist=false` / `generated-tests-ran=not-applicable` because 109 introduces no new
generated-project test; `authoritative=false` because `GeneratedProductCheck` is not the authoritative
signal for this change (the authoritative signal is the escalated controls suite +
`EvidenceAudit verdict=PASS` with 0 synthetic).

## GeneratedProductCheck pin-lag (non-authoritative, merge-resolved)

If `GeneratedProductCheck` reports a compile gap, it is the template **pin-lag** every controls-public-
surface feature carries until merge: `PackLocal` packs the new `FrameMetrics` API but, under the same
version literal, the NuGet global cache shadows it (the documented bump-to-clear-cache condition). This
is **not** a version-resolution mismatch — the pins resolve to real packages
(`package-resolution=resolved`, `package-mismatch=false`, `exact-package-match=true`). `speckit-merge`
packs every packable library with a **bumped** version, pushes, clears caches, and advances the single
`template/base/Directory.Packages.props` pin; after that bump the generated product resolves the post-
109 API and `GeneratedProductCheck` is green. Hence `authoritative=false` for any pre-bump run.
