# Generated-guidance validation — escalated gate run (feature 096, T027)

evidence-kind=generated-guidance-validation
status=pass

The controls-public-surface escalation runs the serialized gate set **sequentially** (shared `.fake`
state); aggregate (batched/concurrent) results are recorded as **non-authoritative** and re-confirmed
sequentially. Each target below was run on its own FAKE invocation in deterministic order.

exact-package-match=true
generated-tests-ran=true
authoritative=true

| gate | verdict | failure-class |
|------|---------|---------------|
| Dev (build + all product + governance tests) | Status: Ok | none |
| PackageSurfaceCheck | Status: Ok | none |
| PerPackageSurfaceDiff | Status: Ok | none |
| FsiTranscripts | Status: Ok | none |
| DesignTokenDrift | Status: Ok | none |
| ContrastCheck | Status: Ok | none |
| ControlsCatalogCheck | Status: Ok | none |
| ControlsCatalogGenerationCheck | Status: Ok | none |
| ControlsInteractionCheck | Status: Ok | none |
| ControlsRenderingCheck | Status: Ok | none |
| GeneratedGuidanceCheck | Status: Ok | none |
| GeneratedProductCheck | Status: Ok | none |
| TemplateDrift | Status: Ok | none |
| EvidenceGraph | Status: Ok | none |
| EvidenceAudit | (see evidence-audit.md) | none |

non-authoritative aggregate=none — targets were not batched concurrently; each ran on its own FAKE
invocation, so the verdicts above are authoritative.
authoritative-gate-list=Route-printed controls-public-surface set (run exactly the gates Route prints)
