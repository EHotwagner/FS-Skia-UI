# Generated-Project Validation (091)

exact-package-match=true
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Why

Feature 091 changes **no** package identity, version pin, or `Directory.Packages.props` entry. It
wires the existing internal `Reconcile` (067) onto the render path via a new `module internal
RetainedRender` (no public-surface entry — zero baseline delta), adds internal helper vals to
`ControlInternals` (also internal), and adds honest behavioral `.fsi` doc comments to
`Control.fsi` / `ControlsElmish.fsi` / `SkiaViewer.fsi` (signatures unchanged). The generated
`dotnet new fs-skia-ui` project's package resolution is therefore unchanged and exact
(`exact-package-match=true`), with no NU1603 downgrade.

The change to generated projects is **additive and behavior-only** (a consumer's
`view`/`update`/`Init`/`Subscriptions` needs zero changes to inherit the O(changed-subtree),
identity-preserving render path — FR-008). It adds no generated-project test
(`generated-tests-exist=false`), so there is no generated-test execution to run
(`generated-tests-ran=not-applicable`). This record is **not** itself the authoritative
generated-project validation (`authoritative=false`); the authoritative consumer check
(`GeneratedProductCheck`) runs under the Route gate list and is recorded in `logs/`, where a local
`GeneratedProductCheck` feature-resolution failure is a non-authoritative environment-failure (see
`runtime-limitations.md` / `aggregate-hang-diagnostics.md`). failure-class=none for this record.
