# Generated Validation (feature 121)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Route output

`./fake.sh build -t Route` was run against the working-tree diff. Because the diff changes public
`.fsi` surface (`SkiaViewer` `ViewerOptions.FrameRateCap`, the published `Controls/Pointer.fsi`
api-surface, `Controls` `RetainedRender` internal-but-baselined surface) and the `.agents` skill
tree, it escalates to the **controls-public-surface** tier and prints `Dev, PackageSurfaceCheck,
PerPackageSurfaceDiff, FsiTranscripts, TemplateCheck, GeneratedProductCheck, ControlsCatalogCheck,
ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck, ControlsCatalogDocsCheck,
ControlsDocCoverageCheck, ControlFidelityCheck, ControlsInteractionCheck, ControlsRenderingCheck,
GeneratedGuidanceCheck, SkillSyncCheck, SkillQualityCheck, PhaseHookParityCheck,
SkillContractPathCheck, TemplateUpdateSkillPackageCheck, TemplateDrift, EvidenceGraph,
EvidenceAudit`. Only the printed gates are run, sequentially (shared `.fake` state) — bundled here
via the `Verify` composite plus the few omitted gates. See `governance-risk-levels.md`.

## Why the generated project is unaffected at default

Generated products consume the **source-stable** high-level entry points (`runInteractiveApp` /
`runInteractiveViewer` / `Viewer.runApp`) unchanged. The one consumer-visible change is the
additive, defaulted `ViewerOptions.FrameRateCap` field; the template's own `ViewerOptions`
construction sites (`template/base/src/Product/EvidenceCommands.fs`) were updated to `FrameRateCap =
None`, so the generated product compiles byte-identically in behaviour. No new source files ship
into generated projects.

## Template pin lag (deferred, expected)

The `dotnet new fs-skia-ui` template package pin is a **separate follow-up track**
(`/fs-skia-template-update`), not in this feature's merge scope. Because this feature adds a new
`ViewerOptions` field that the template source now references, the **package-mode** template build
requires the bumped package (post-merge `PackLocal`); `TemplateCheck` / `GeneratedProductCheck` may
show the known pin-lag against the prior published package version until the bumped libs are packed
and the template re-pin follow-up runs. `package-resolution=resolved` for the repo-built (source-
mode / locally-packed) packages, which carry the new field.
</content>
