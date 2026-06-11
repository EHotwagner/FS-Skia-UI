# Generated validation (feature 097, R2)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Why not-applicable

R2 ships **nothing** into the `dotnet new fs-skia-ui` template or generated products (plan §Generated
project impact: None; §Dependency impact: N/A — no new dependency). The incremental evaluator, the measure
cache, the dirty-set derivation, the `RetainedRender.step` wiring, and the extended `WorkReductionRecord`
are all internal to FS.Skia.UI.Layout / FS.Skia.UI.Controls, so a generated project consuming
`runInteractiveApp` gains the partial-measure speedup **automatically** with no scaffold change, no new
selected-Controls guidance, and no placeholder/excluded-history scan delta. The public
`Layout.evaluateIncremental` signature is unchanged (body-only), so there is no package mismatch to resolve
and no generated-project test suite introduced by this feature.

`generated-tests-ran=not-applicable` because this change introduces no new generated-project test;
`authoritative=false` because `GeneratedProductCheck` is the known non-authoritative environment failure
locally (no template feature.json; Map.empty env). The authoritative signal is `EvidenceAudit
verdict=PASS`.
