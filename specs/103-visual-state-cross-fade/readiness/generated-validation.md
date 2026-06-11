# Generated Validation (feature 103, R6)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Route output (T017)

`./fake.sh build -t Route` was run against the working-tree diff (the union of the branch-vs-`main`
merge-base diff and the uncommitted/untracked changes). As predicted by features 096–102, the edit to
`src/Controls/RetainedRender.fs(i)` escalates to the **controls-public-surface** gate set (any
`src/Controls/**/*.fs` edit escalates, regardless of whether the public `.fsi` moves). The printed
routing was:

```
developer-class=framework-author
tier=agent-ready
gates=Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck, ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
dogfood-forced=false
matched-rules=controls-public-surface, evidence-governance, specify-catchall, docs-only, package-surface
```

Only the gates Route prints were run, **sequentially** (shared `.fake` state). See
[governance-risk-levels.md](./governance-risk-levels.md) for the authoritative gate list and
[aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md) for the run record.

## Why not-applicable

R6 ships **nothing** into the `dotnet new fs-skia-ui` template or generated products: it is a
framework-internal `FS.Skia.UI.Controls` behavior change (the live visual-state transition becomes a
snapshot-composite cross-fade) plus an **internal** `RetainedRender.fsi` field/doc move. There is **no**
`template/**`, sample, command-surface, or generated-content change, and **no public** `.fsi` surface
move. A generated project consuming `FS.Skia.UI.Controls` therefore resolves the **same public** package
surface as before — `package-resolution=resolved`, `package-mismatch=false`,
`exact-package-match=true`.

`generated-tests-exist=false` / `generated-tests-ran=not-applicable` because R6 introduces no new
generated-project test; `authoritative=false` because `GeneratedProductCheck` is not the authoritative
signal for this framework-internal change (the authoritative signal is the escalated controls suite +
`EvidenceAudit verdict=PASS` with 0 synthetic). Any local `GeneratedProductCheck` environment failure
is recorded as **non-authoritative** environment-class, not a product defect
([[generated-product-check-env-failure]]).
