# Routed gate validation (T024, feature 097, R2)

Ran exactly the gates `Route` printed for this change (escalated controls-public-surface set, because the
internal `.fsi` surfaces moved). FAKE-backed targets run **sequentially** (shared `.fake` state). Aggregate
results below are recorded as **non-authoritative** unless re-confirmed sequentially; each row here is a
sequential single-target run.

| Gate | Result | Note |
|------|--------|------|
| Dev | Ok | full build + all test projects (Layout 28, Controls 277, Governance 573, Parity/Smoke/Scene/...) |
| GeneratedGuidanceCheck | Ok | |
| TemplateDrift | Ok | |
| PackageSurfaceCheck | Ok | |
| PerPackageSurfaceDiff | Ok | per-package baselines regenerated via RefreshSurfaceBaselines |
| FsiTranscripts | Ok | public `evaluateIncremental` FSI transcript captured (fsi-transcript.md) |
| ControlsCatalogCheck | Ok | no catalog control added |
| ControlsCatalogGenerationCheck | Ok | |
| DesignTokenDrift | Ok | no DTCG token touched |
| ContrastCheck | Ok | no theme/contrast change |
| ControlsInteractionCheck | Ok | |
| ControlsRenderingCheck | Ok | byte-identical render (Controls.Tests 277/277) |
| EvidenceGraph | Ok | no cycles, no dangling refs, tasks resolved |
| EvidenceAudit | PASS | 0 blockers, real-tasks=22, diff-scan-hits=0, synthetic=0 |

failure-class=none (product); GeneratedProductCheck is the known non-authoritative local environment
failure (no template feature.json; Map.empty env) — see generated-validation.md.
