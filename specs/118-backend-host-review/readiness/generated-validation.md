# Generated Validation (feature 118)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Route output

`./fake.sh build -t Route` was run against the working-tree diff. Because the diff changes the
public `FS.Skia.UI.SkiaViewer` `.fsi` (new `ViewerPresentMode` DU + `ViewerOptions.PresentMode`
field) and the template/generated product construct `ViewerOptions`, Route escalates to the
**package-surface** tier and prints `TemplateCheck` / `GeneratedProductCheck`. Only the printed
gates are run, sequentially (shared `.fake` state). See governance-risk-levels.md for the gate
list.

## Why the generated project is unaffected at default

Feature 118 adds an additive, default-bearing `PresentMode` field (default
`ViewerPresentMode.OffscreenReadback`) and threads it through to the backend; the default value
keeps the present path, screenshots, window diagnostics, and visual output byte-identical to the
pre-feature baseline (FR-001). The generated product's `ViewerOptions` construction sites
(`template/base/src/Product/EvidenceCommands.fs`) gain the defaulted field only; generated `Dev`
/ evidence behaviour is unchanged. No package identity or dependency changes
(`package-resolution=resolved`, `package-mismatch=false`, `exact-package-match=true`).
`generated-tests-exist=false` / `generated-tests-ran=not-applicable` because 118 introduces no
new generated-project test; `authoritative=false` because `GeneratedProductCheck` is not the
authoritative signal for this change (the authoritative signal is the package-surface gate set +
`EvidenceAudit verdict=PASS` with 0 synthetic).

## Pin-lag (non-authoritative, merge-resolved)

Any compile gap a generated/product check reports before merge is the template **pin-lag** every
surface-bumping feature carries until merge: `PackLocal` packs the new field but, under the same
version literal, the NuGet global cache shadows it. This is NOT a version-resolution mismatch.
`speckit-merge` packs every packable library with a **bumped** version, pushes, clears caches,
and advances the `template/base/Directory.Packages.props` pin; after that bump the generated
product resolves the post-118 surface. Hence `authoritative=false` for any pre-bump run.
