# Governance risk levels (071)

The implementation tasks split into three governance risk bands. Broad
validation is required only at close-out; aggregate FAKE-backed results are
non-authoritative and recorded under `readiness/logs/` and
`readiness/evidence-audit.md`. FAKE-backed targets share `.fake` state — run them
**sequentially**, never concurrently.

## Small

T006–T008 (the `066` cross-check test edit + the `CatalogGen.catalogFacts`
fact-table extension + the `renderFSharpRow`/YAML evidence special-case
generalization). A focused `./fake.sh build -t Dev` is sufficient evidence for
these.

## Medium

T009–T010 (the regenerated governance artifacts `catalog.yml` / `Catalog.fs`
plus the 41 new parity-fixture pairs). The **required evidence** for this band is
the `ControlsCatalogGenerationCheck` currency proof (T011 — the gate biting on a
deliberate hand-edit, then green after revert) and the per-package surface review
(T018, additive-only or empty).

## Broad

T019–T021. **Broad validation** re-runs `./fake.sh build -t Route` on the full
implementation diff and runs exactly the gates it prints — the serialized
FAKE-backed order (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`,
`GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit` when escalated) —
finishing on `EvidenceGraph` + `EvidenceAudit`. Aggregate results are
non-authoritative and recorded under `readiness/logs/`.
