# Generated Validation (084)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Scope

Feature 084 changes generated products through the template: the new
`--window-startup windowed-fullscreen` flag value, the windowed-fullscreen no-flag
default, the reclassified honored states, the guarded `runAppWithWindowBehavior` launch
wiring (keeping the durable `Viewer.runApp viewerOptions generatedHost` literal
reachable), and the regenerated `docs/evidence-formats.md` / hand-edited
`docs/scaffold-map.md` / `docs/product.md` / `README.md`.

`authoritative=false` because the local `GeneratedProductCheck` is a known
**non-authoritative environment-failure** (see `aggregate-hang-diagnostics.md`): it
cannot resolve a generated feature locally and is not the authoritative signal for this
change. The authoritative generated-product validation — building a generated project
and launching its windowed-fullscreen default on a display-capable host — runs on that
host. No package identity changes; there is no package mismatch to resolve.
