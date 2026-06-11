# Generated Validation (099, R4)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Why not-applicable

Feature 099 ships **nothing** into the `dotnet new fs-skia-ui` template or generated products (plan
§Generated project impact: N/A; §Dependency impact: N/A — no new dependency). The animation seam is
internal to the built-in retained host, so a generated project consuming `runInteractiveApp` gains live
animation **automatically** with no scaffold change, no new selected-Controls guidance, and no
placeholder/excluded-history scan delta. The only surface move is the **internal**
`RetainedUiState.Animation` slot type in `src/Controls/RetainedRender.fsi`; the public
`runInteractiveApp` / `InteractiveAppHost` surface is unchanged, so a generated project renders and
behaves identically (more faithfully animated for the same `view`, with no API change).

`generated-tests-ran=not-applicable` because this change introduces no new generated-project test;
`authoritative=false` because `GeneratedProductCheck` is not the authoritative signal for this
framework-internal surface move (the authoritative signal is `EvidenceAudit verdict=PASS`). This run,
`GeneratedProductCheck` nonetheless completed `Status: Ok` — recorded in
[generated-guidance-validation.md](./generated-guidance-validation.md).
