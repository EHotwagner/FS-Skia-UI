# Generated-Project Validation (092)

exact-package-match=true
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Why

Feature 092 changes **no** package identity, version pin, or `Directory.Packages.props` entry. The
public-surface move is `InteractiveViewerHost.MapKey : 'msg list` (FR-006) plus the re-keyed
internal `Controls.Elmish` focus seam and the internal `RetainedRender` deltas. Generated
`dotnet new fs-skia-ui` projects host through **`GeneratedAppHost`** (the `Viewer.runApp` path),
whose `MapKey : 'msg option` is **deliberately unchanged** (see `governance-risk-levels.md`), so the
generated host source and its package resolution are unchanged and exact
(`exact-package-match=true`), with no NU1603 downgrade.

The behavior reaches generated projects only via refreshed package pins after the post-merge bump
(no scaffold edit), so this feature adds no generated-project test (`generated-tests-exist=false`,
`generated-tests-ran=not-applicable`). This record is **not** itself the authoritative
generated-project validation (`authoritative=false`); the authoritative consumer check
(`GeneratedProductCheck`) ran under the Route gate list this session and **passed** (full template
pack → install → instantiate → consumer validation → smoke), recorded in `logs/`. failure-class=none.
