# Generated Validation (feature 122)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=true
generated-tests-ran=true
authoritative=true
failure-class=none

## Route output

`./fake.sh build -t Route` was run against the working-tree diff. Because it changes public `.fsi`
surface (`Controls.Elmish.runInteractiveAppWithWindowBehavior`, the `SkiaViewer.Host`
`PresentAction`/`planPresent` test seam), the `dotnet new fs-skia-ui` template
(`template/base/src/Product/Program.fs`, `docs/scaffold-map.md`, `docs/evidence-formats.md`), governance
docs, and the skill tree, it escalates to **agent-ready**. Only the printed gates are run, sequentially
(shared `.fake` state). See `governance-risk-levels.md` for the full gate list.

## Why the generated project is unaffected at default

Generated products consume the **source-stable** `runInteractiveApp` entry point unchanged. The
generated `Program.fs` now ALSO calls the additive `runInteractiveAppWithWindowBehavior` — but only
behind the pre-existing `windowFlagSupplied args` guard (mirroring the game profile), so a no-flag launch
is byte-identical. No new source files ship into generated projects; the present-path fix is internal to
the framework host and improves the live window without any consumer change.

## Template pin lag (deferred, expected)

The `dotnet new fs-skia-ui` template package pin is a **separate follow-up track**
(`/fs-skia-template-update`), not in this feature's merge scope. Because the generated `Program.fs` now
references the new `runInteractiveAppWithWindowBehavior` overload, the **package-mode** template build
requires the bumped package (post-merge `PackLocal`); `TemplateCheck` / `GeneratedProductCheck` may show
the known pin-lag against the prior published package version until the bumped libs are packed and the
template re-pin follow-up runs. `package-resolution=resolved` for the repo-built (source-mode / locally
packed) packages, which carry the new overload.
