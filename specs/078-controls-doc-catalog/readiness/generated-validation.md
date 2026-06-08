# Generated-Project Validation (078)

exact-package-match=true
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Why

Feature 078 changes **no** package identity, version pin, or
`Directory.Packages.props` entry — it is a docs-content + governance-generation
feature touching `docs/**`, `build/Governance/**`, and a new currency gate. The
generated `dotnet new fs-skia-ui` project's package resolution is therefore
unchanged and exact (`exact-package-match=true`), with no NU1603 downgrade and no
package mismatch.

This feature ships nothing into the template and adds no generated-project test,
so there is no generated-test execution to run (`generated-tests-exist=false`).
This record is **not** an authoritative generated-project validation
(`authoritative=false`); the authoritative consumer checks (`TemplateCheck` /
`GeneratedProductCheck`) are unchanged by this feature and out of its routed
scope.
