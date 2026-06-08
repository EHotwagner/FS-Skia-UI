# Gate Diagnostics — 079-doc-preview-examples (T022)

Authoritative command: the Route-selected gate set, run sequentially (shared `.fake` state).
Artifact path: this file. Failure class: `gate-failure`.

All Route-selected gates PASS. No remedy required.

| Gate | Status | Notes |
|------|--------|-------|
| `Route` | OK | tier=agent-ready; gates=Dev, ControlsCatalogDocsCheck, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit |
| `Dev` | PASS | Restore 39.8s + Build 1m34s + SampleContractSmoke 8.5s + Test 41.9s; all default test projects green |
| `ControlsCatalogDocsCheck` | PASS | reconciled 51 rendered + 1 unsupported == 52; failure-class: none |
| `GeneratedGuidanceCheck` | PASS | no generated-guidance impact |
| `TemplateDrift` | PASS | no template/metadata/contract drift |
| `EvidenceGraph` | PASS | acyclic, consistent, no `[S*]` (see `evidence-graph.md`) |
| `EvidenceAudit` | PASS | verdict PASS, 0 synthetic (see `evidence-audit.md`) |
| `dotnet fsdocs build --strict --eval` | PASS | exit 0; nav order + previews + links verified (`docs-build.md`) |

## Render-harness tests (render-capable host, off the GPU-free CI path)

`dotnet run --project tests/ControlsPreview.Harness -- --sequenced` → **7 passed, 0 failed**
(totality, catalog-order, explicitness/non-trivial, single-unsupported, idempotence,
committed==fresh, unsupported-has-no-image). These are intentionally not in
`defaultTestProjects` (rendering needs a render host); they are compile-checked by the
solution `Build` and exercised here with this evidence.

## Governance.Tests catalog-docs suite (GPU-free, in `Dev`)

`Feature 078 catalog-docs render + splice` (6 passed) and `Feature 078 catalogDocsCurrency
findings` (17 passed, incl. the new Feature-079 `TrivialPreview` / byte-floor cases and the
committed-tree cleanliness test).
