# Governance risk levels (078)

The implementation tasks split into three governance risk bands. Broad
validation is required only at close-out; aggregate FAKE-backed results are
non-authoritative and recorded under `readiness/logs/` and
`readiness/evidence-audit.md`. FAKE-backed targets share `.fake` state — run them
**sequentially**, never concurrently.

## Small

Prose-only edits to an authored region of one `docs/controls/<id>.md` detail page
or the `spec-kit-workflow.md` narrative. A focused local `dotnet fsdocs build`
link check is sufficient evidence for these.

## Medium

Regenerating the catalog index / detail-page header regions or touching the
`CatalogDocsGen` generator. The **required evidence** for this band is the
`ControlsCatalogDocsCheck` currency proof (the gate biting on a deliberate
hand-edit / stale index, then green after `RefreshSurfaceBaselines`) plus
`GeneratedGuidanceCheck`.

## Broad

Routing / target / `validation.contract.yml` / `knownGates` edits. **Broad validation** re-runs `./fake.sh build -t Route` on the full implementation diff
and runs exactly the gates it prints — the serialized FAKE-backed order (`Dev`,
`ControlsCatalogDocsCheck`, `GeneratedGuidanceCheck`, `EvidenceGraph`,
`EvidenceAudit`) — finishing on `EvidenceGraph` + `EvidenceAudit`. Aggregate
results are non-authoritative and recorded under `readiness/logs/`.
