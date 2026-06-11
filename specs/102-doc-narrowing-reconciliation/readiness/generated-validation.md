# Generated Validation (feature 102, R8)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Why not-applicable

R8 ships **nothing** into the `dotnet new fs-skia-ui` template or generated products: it is a
documentation/internal-comment honesty pass — roadmap report prose (`docs/reports/…-roadmap.md`, repo
docs, not a packaged surface) plus descriptive comments in `src/Controls/ControlRuntime.fs`,
`src/Controls/Focus.fs`, `src/Controls/Control.fs`, and `src/Layout/Layout.fs` — with **no**
`template/**`, sample, command-surface, or generated-content change, and **no** `.fsi` surface move. A
generated project consuming `FS.Skia.UI.Controls` / `FS.Skia.UI.Layout` therefore resolves the **same**
package surface as before — `package-resolution=resolved`, `package-mismatch=false`,
`exact-package-match=true` — and renders and behaves identically (byte-identical output).

`generated-tests-exist=false` / `generated-tests-ran=not-applicable` because R8 introduces no new
generated-project test; `authoritative=false` because `GeneratedProductCheck` is not the authoritative
signal for this framework-internal documentation change (the authoritative signal is `EvidenceAudit
verdict=PASS` plus the unchanged framework suites under `Dev`). Any local `GeneratedProductCheck`
environment failure is recorded as **non-authoritative** environment-class, not a product defect (see
[aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md)).
