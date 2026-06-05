# Governance risk levels

This feature's change spans the **small** and **medium** levels below; the routing rule
escalates it to the maintainer-verify path because it also touches a governance target
metadata surface (`Targets.fs` / `validation.contract.yml`).

- **small** — a fact edit in `CatalogGen.catalogFacts` followed by
  `./fake.sh build -t RefreshSurfaceBaselines`. **Required evidence** (focused validation):
  `ControlsCatalogGenerationCheck` + `Dev`. No public-surface or consumer-contract impact:
  the single source changes and both generated files re-splice deterministically.
- **medium** — a change to the generator/render/splice/currency logic itself
  (`build/Governance/CatalogGen.fs`) or to the gate arm. **Required evidence**: add
  `ControlsCatalogCheck`, the extended `CatalogTests` (parity + drift + correspondence), and
  `TargetMetadataDrift` (the regenerated `validation.contract.yml` currency).
- **broad** — a routing/target-metadata change (`Routing.fs`, `Targets.fs`,
  `validation.contract.yml`) — which **this feature includes** (it registers the new gate
  and adds it to the `controls-public-surface` rule). For this level **broad validation**
  is required: the escalated serialized FAKE order (`Dev`, `GeneratedGuidanceCheck`,
  `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`) plus the
  Route-printed gates.

## Authoritative per-gate verdicts (this run)

| Gate | Verdict | Note |
|------|---------|------|
| `Dev` | PASS | build + all default tests, incl. the new `CatalogTests` parity/drift/correspondence and `Governance.Tests` over the new target/routing |
| `ControlsCatalogGenerationCheck` | PASS | 6 controls current in both files; FAILs correctly on a hand-mutated region naming `typed-catalog/button` |
| `Route` | PASS | lists `ControlsCatalogGenerationCheck` under `controls-public-surface` |
| `TargetMetadataDrift` | PASS | regenerated `validation.contract.yml` lists the new gate; no metadata drift |
| `GeneratedGuidanceCheck` | PASS | unchanged |
| `EvidenceGraph` | PASS | `verdict=ok` (no cycles, no dangling refs, no `[S*]`) |
| `EvidenceAudit` | PASS | `verdict=PASS`, `total-blockers=0`, empty Synthetic-Evidence Inventory |
| `TemplateCheck` / `GeneratedProductCheck` | environment-degraded | the generated product's evidence-graph sub-step cannot self-resolve a feature in this headless sandbox (empty generated `.specify/feature.json`) — a pre-existing condition also seen on merged 064/065, **not** a regression from this byte-identical, framework-internal change (see `runtime-limitations.md`) |

## Authoritative vs aggregate results

FAKE-backed gates run **sequentially** (shared `.fake` state; never concurrently). The
**aggregate** umbrella result of any multi-target run is **non-authoritative**; the
authoritative verdict is the **per-gate** result recorded in this `readiness/` tree
(`control-catalog-generation.md` for the drift gate, `control-catalog.md` for
`ControlsCatalogCheck`, `target-metadata-drift.md` for the contract currency, and
`evidence-audit.md` for the merge gate). `EvidenceAudit verdict=PASS` is the merge gate.
