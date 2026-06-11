# Generated Validation (100, R5)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Why not-applicable

Feature 100 ships **nothing** into the `dotnet new fs-skia-ui` template or generated products (plan
§Generated project impact: N/A; §Dependency impact: N/A — no new dependency). The navigation behavior
is internal to the built-in retained host, so a generated project consuming `runInteractiveApp` gains
general navigation **automatically** with no scaffold change, no new selected-Controls guidance, and no
placeholder/excluded-history scan delta. The only surface moves are the three Controls `.fsi` files
(`Focus`/`Types`/`Accessibility`); the public `runInteractiveApp` / `InteractiveAppHost` surface is
unchanged, so a generated project renders and behaves identically (now with general navigation for the
same `view`, with no consumer API change).

`generated-tests-ran=not-applicable` because this change introduces no new generated-project test;
`authoritative=false` because `GeneratedProductCheck` is not the authoritative signal for this
framework-internal surface move (the authoritative signal is `EvidenceAudit verdict=PASS`). The
`GeneratedProductCheck` run status is recorded in
[generated-guidance-validation.md](./generated-guidance-validation.md).
