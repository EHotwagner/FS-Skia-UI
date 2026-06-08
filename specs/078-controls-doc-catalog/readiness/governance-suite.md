# Governance Suite — aggregate (078) — T028

Run sequentially (shared `.fake` state). Aggregate results recorded
**non-authoritatively** here; the per-gate readiness reports are authoritative.

| Step | Gate | Result |
|------|------|--------|
| 1 | `Route` | tier=maintainer-verify; gates include `ControlsCatalogDocsCheck`; matched-rules include `controls-catalog-docs` (the new rule) |
| 2 | `Dev` | PASS — Restore/Build/Test green (all test projects, incl. the 20 Feature-078 `CatalogDocsGenTests`) |
| 3 | `ControlsCatalogDocsCheck` | PASS — index/detail-header current vs `catalogFacts`; 52 detail pages; `previews-present: 52` (decodable/non-1×1/non-trivial); API links resolved |
| 4 | `GeneratedGuidanceCheck` | PASS |
| 5 | `EvidenceGraph` | PASS — acyclic/consistent; 27→29 `[X]`, 0 `[S]`, 0 `[S*]` |
| 6 | `EvidenceAudit` | see `readiness/logs/evidence-audit.txt` (`evidence-audit.md`) |

Also confirmed earlier: `RefreshSurfaceBaselines` (clean regen of the index + 52 detail
headers + `validation.contract.yml`), `TargetMetadataDrift` (PASS — the new target's
metadata is drift-free), and `dotnet fsdocs build --strict --eval` (exit 0, all controls
pages + previews + API links in `output/`).

## Maintainer-verify caveat

`Route` escalates to `maintainer-verify` because `validation.contract.yml` is in the diff
(the build-target-contract rule), adding `AgentReady`, `TargetMetadataDrift`, `Verify`,
`Ci` to the full merge-time set. `TargetMetadataDrift` is PASS. `Verify`/`Ci` bundle
`GeneratedProductCheck`, which **fails locally as a non-authoritative environment failure**
(see the `generated-product-check-env-failure` gotcha: the generated Verify can't resolve a
feature with an empty env) — not a product defect introduced by this feature, which changes
no template/generated-project surface.
