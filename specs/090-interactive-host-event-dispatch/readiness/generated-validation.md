# Generated-Project Validation (090)

exact-package-match=true
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Why

Feature 090 changes **no** package identity, version pin, or
`Directory.Packages.props` entry — it adds public `.fsi` surface to the existing
`FS.Skia.UI.Controls` (the `nearestAuthored` recovery) and
`FS.Skia.UI.Controls.Elmish` packages (binding dispatch + text seam +
responds-proof + corrected host doc). The generated `dotnet new fs-skia-ui`
project's package resolution is therefore unchanged and exact
(`exact-package-match=true`), with no NU1603 downgrade.

The change to generated projects is **additive** (a control with no authored
binding behaves exactly as before); it adds no generated-project test
(`generated-tests-exist=false`), so there is no generated-test execution to run
(`generated-tests-ran=not-applicable`). This record is **not** an authoritative
generated-project validation (`authoritative=false`); the authoritative consumer
checks (`TemplateCheck` / `GeneratedProductCheck`) run in the serialized order and
are recorded in `logs/`, where a local `GeneratedProductCheck` feature-resolution
failure is a non-authoritative environment-failure (see `runtime-limitations.md`).
failure-class=none for this record.
