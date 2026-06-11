# Generated-guidance validation — escalated gate run (feature 100, R5, T022)

evidence-kind=generated-guidance-validation
status=pass

The controls-public-surface escalation runs the `Route`-printed gate set **sequentially** (shared
`.fake` state); aggregate (batched/concurrent) results are recorded as **non-authoritative** and
re-confirmed sequentially (see [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md)). Each
target below was run on its own FAKE invocation in deterministic order.

exact-package-match=true
generated-tests-ran=true
authoritative=true

| gate | verdict | failure-class |
|------|---------|---------------|
| Dev (build + all product + governance tests) | Status: Ok | none |
| PackageSurfaceCheck | Status: Ok | none |
| PerPackageSurfaceDiff | Status: Ok | none |
| FsiTranscripts | Status: Ok | none |
| ControlsCatalogCheck | Status: Ok | none |
| ControlsCatalogGenerationCheck | Status: Ok | none |
| DesignTokenDrift | Status: Ok | none |
| ContrastCheck | Status: Ok | none |
| ControlsInteractionCheck | Status: Ok | none |
| ControlsRenderingCheck | Status: Ok | none |
| GeneratedGuidanceCheck | Status: Ok | none |
| TemplateDrift | Status: Ok | none |
| GeneratedProductCheck | Status: Ok | none |
| EvidenceGraph | (see evidence-graph.md) | none |
| EvidenceAudit | (see evidence-audit.md) | none |

Controls.Tests 307/307 and Elmish.Tests 69/69 pass (including the Feature100 selection-move /
declared-step / grid-move / closed-model suites). The only mid-run failure was a **test-only** bug (an
assertion pressed `ArrowUp` against a slider keyboard that did not declare it, so `route` correctly
returned `Fallthrough`) — corrected by declaring all four arrows in the value-role test keyboard; the
production source was never at fault. The FsiTranscripts script `controls-prelude.fsx` (and
`typed-controls-prelude.fsx`) were updated to supply the new `ControlEvent.Nav = None` field.
