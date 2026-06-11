# Routed gate validation (T021, feature 098, R3)

Ran exactly the gates `Route` printed for this change (escalated controls-public-surface set, because the
public `src/Controls/**/*.fsi` surface moved — `BoundIds` field + `val boundIdsOf` + the documented
unkeyed canonical-id change). FAKE-backed targets run **sequentially** (shared `.fake` state); each row is a
sequential single-target run. Aggregate results are recorded as **non-authoritative** unless re-confirmed
sequentially.

exact-package-match=true
generated-tests-ran=true
authoritative=true
failure-class=none

| Gate | Result | Note |
|------|--------|------|
| Dev | Ok | full build + all test projects (Controls 282/282, Elmish 55/55, Governance/Layout/Scene/...) |
| PackageSurfaceCheck | Ok | |
| PerPackageSurfaceDiff | Ok | per-package baselines regenerated via RefreshSurfaceBaselines |
| FsiTranscripts | Ok | unified `Key ?? path` scheme FSI transcript captured (fsi-transcript.md) |
| TemplateCheck | Ok | |
| GeneratedProductCheck | Ok | |
| ControlsCatalogCheck | Ok | no catalog control added |
| ControlsCatalogGenerationCheck | Ok | |
| DesignTokenDrift | Ok | no DTCG token touched |
| ContrastCheck | Ok | no theme/contrast change |
| ControlsInteractionCheck | Ok | keyed dispatch regression + routing-seam suites green |
| ControlsRenderingCheck | Ok | byte-identical render (Bounds rectangles unchanged; only id labels) |
| GeneratedGuidanceCheck | Ok | |
| SkillContractPathCheck | Ok | |
| TemplateDrift | Ok | api-surface tree recaptured for BoundIds + boundIdsOf |
| EvidenceGraph | Ok | see evidence-graph.md |
| EvidenceAudit | PASS | see evidence-audit.md |

failure-class=none (product). All 15 pre-evidence gates returned `Status: Ok` on sequential single-target
runs. The libdecor-gtk plugin-load line during the native-GUI suites is the known benign dual-display host
warning, not a product defect.
