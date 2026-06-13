# Generated Validation (feature 115)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Route output

`./fake.sh build -t Route` was run against the working-tree diff. Because the diff edits
`.specify/init-options.json` (a `.specify/**` consumer-contract / governance path), `Route` escalates
beyond the inner loop and prints `Dev`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`,
`EvidenceAudit` (matched rules: evidence-governance, specify-catchall, docs-only). Only the gates Route
prints were run, **sequentially** (shared `.fake` state). See
[governance-risk-levels.md](./governance-risk-levels.md) for the authoritative gate list and
[focused-gates.md](./focused-gates.md) for the live capture.

## Why the generated project is unaffected

Feature 115 is a dependency-version + governance-asset change with **no `src/**` source edit** and **zero
`.fsi` delta**. The safe bumps are an in-line FSharp.Core patch (10.1.300→10.1.301) and a
Microsoft.Extensions.FileSystemGlobbing servicing patch (10.0.8→10.0.9, build-tooling/adopt-set only — not
a shipped-product surface). The `dotnet new fs-skia-ui` template pins the currently-published FS.Skia.UI
package versions; those resolve cleanly (`package-resolution=resolved`, `package-mismatch=false`,
`exact-package-match=true`). `generated-tests-exist=false` / `generated-tests-ran=not-applicable` because
115 introduces no new generated-project test; `authoritative=false` because `GeneratedProductCheck` is not
the authoritative signal for this change (the authoritative signal is the routed gate set green with zero
surface/golden/generated-product diff + `EvidenceAudit verdict=PASS` with 0 synthetic).

## Held bumps and the generated project

Each held major bump (YamlDotNet, Fable.Elmish, the Expecto/Test.Sdk/YoloDev cluster) is a test-tooling or
build/governance dependency, not a shipped-product runtime pin (Fable.Elmish is the one runtime pin among
them — held precisely because of its blast radius). Any held bump is adopted **only** if the full routed
gate set is green with no source change; otherwise it is fully reverted and the generated project is left
on the current resolving pins (`failure-class=none` for the adopted/unchanged set).
