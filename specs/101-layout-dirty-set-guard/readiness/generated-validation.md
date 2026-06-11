# Generated Validation (feature 101, R7)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Why not-applicable

R7 ships **nothing** into the `dotnet new fs-skia-ui` template or generated products: it is a
framework-internal `src/Controls/**` + `tests/**` change (a name-token refactor, a comment correction,
and a new Controls Expecto test) with **no** `template/**`, sample, command-surface, or
generated-content change, and **no** `.fsi` surface move (see `surface-baseline.md`). A generated
project consuming `FS.Skia.UI.Controls` therefore resolves the **same** package surface as before —
`package-resolution=resolved`, `package-mismatch=false`, `exact-package-match=true` — and renders and
behaves identically (R2 INV-1 byte-identical).

`generated-tests-exist=false` / `generated-tests-ran=not-applicable` because R7 introduces no new
generated-project test; `authoritative=false` because `GeneratedProductCheck` is not the authoritative
signal for this framework-internal change (the authoritative signal is `EvidenceAudit verdict=PASS` plus
the framework Expecto guard under `Dev`). The `GeneratedProductCheck` run status is recorded in
[generated-guidance-validation.md](./generated-guidance-validation.md) when run as part of the broad set.
