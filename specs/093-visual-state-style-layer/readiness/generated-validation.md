# Generated-Project Validation (093)

exact-package-match=true
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Why

Feature 093 changes **no** package identity, version pin, or
`Directory.Packages.props` entry. The behavioral + surface changes land in the
existing `FS.Skia.UI.Controls` package (new `Style`/`StyleVariant`/`StyleClass`/
`ResolvedStyle` surface, two new DTCG tokens, typed-`Props` `Classes` deltas). It
is purely additive: a generated `dotnet new fs-skia-ui` project that attaches no
class renders identically, so the generated host source and its package
resolution are unchanged and exact (`exact-package-match=true`), with no NU1603
downgrade.

The styling behavior reaches generated projects only via refreshed package pins
after the post-merge version bump (no scaffold edit), so this feature adds no
generated-project test (`generated-tests-exist=false`,
`generated-tests-ran=not-applicable`). This record is **not** itself the
authoritative generated-project validation (`authoritative=false`); the
authoritative consumer check (`GeneratedProductCheck`) ran under the Route gate
list this session and **passed**, recorded in `logs/`. failure-class=none.
