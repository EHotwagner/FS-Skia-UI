# Generated-Project Validation (089)

exact-package-match=true
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Why

Feature 089 changes **no** package identity, version pin, or
`Directory.Packages.props` entry — it enrolls the existing typed
`src/Controls/Widgets/*.fsi` into the published api-surface, adds a `TypedModule`
catalog token, and edits governance code + the Spec-Kit skill tree. The generated
`dotnet new fs-skia-ui` project's package resolution is therefore unchanged and
exact (`exact-package-match=true`), with no NU1603 downgrade and no package
mismatch.

The change to generated projects is **additive** (the enriched
`docs/api-surface/Controls/` typed `.fsi` and the `TypedModule` token in
`catalog.yml`); it adds no generated-project test, so there is no generated-test
execution to run (`generated-tests-exist=false`). This record is **not** an
authoritative generated-project validation (`authoritative=false`); the
authoritative consumer checks (`TemplateCheck` / `GeneratedProductCheck`) are
exercised by the serialized six-target order and recorded in `logs/`, where a
local `GeneratedProductCheck` feature-resolution failure is a non-authoritative
environment-failure (see `runtime-limitations.md`).
