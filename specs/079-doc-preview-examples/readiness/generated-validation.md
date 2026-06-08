# Generated Validation (079)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Why not-applicable

Feature 079 ships **nothing** into the `dotnet new fs-skia-ui` template or generated products
(plan §Generated project impact / §Dependency impact): no package change, no new dependency,
no generated-project content change. There is therefore no package mismatch to resolve and no
generated-project test suite to run from this feature. `authoritative=false` because the local
`GeneratedProductCheck` is a known non-authoritative environment-failure (see
`aggregate-hang-diagnostics.md`) and is not on this change's Route gate set.
