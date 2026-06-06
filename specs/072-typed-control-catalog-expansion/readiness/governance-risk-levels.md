# Governance risk levels — Catalog Breadth Expansion (072)

The implementation tasks split into three governance risk bands. **broad validation**
is required only because public `.fsi` + catalog facts escalate the route; aggregate
FAKE-backed results are non-authoritative and recorded under `readiness/logs/` and
`readiness/evidence-audit.md`. FAKE-backed targets share `.fake` state — run them
**sequentially**, never concurrently. `Route` is authoritative.

## Small

The framework-internal edits: the typed `Buttons`/`Pickers` `.fs` lowering bodies,
the `CatalogGen.catalogFacts` fact-table extension (47 → 52), and the test edits.
A focused `./fake.sh build -t Dev` is sufficient evidence for this band.

## Medium

The contracted additions: the new public `.fsi` surface, the regenerated
governance artifacts (`catalog.yml` / `Catalog.fs`), the per-id parity fixtures, and
the surface baselines. The **required evidence** for this band is the
`ControlsCatalogGenerationCheck` currency proof (T022 — the gate biting on a
deliberate hand-edit, then green after revert), `PackageSurfaceCheck` /
`PerPackageSurfaceDiff` (additive-only / zero drift), and the `controls-public-surface`
gate set (`ControlsCatalogCheck`, `ControlsInteractionCheck`, `ControlsRenderingCheck`,
`FsiTranscripts`, `DesignTokenDrift`).

## Broad

Close-out. **broad validation** re-runs `./fake.sh build -t Route --enforce` on the
full implementation diff and runs exactly the gates it prints — the serialized
FAKE-backed order (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck` / `TemplateDrift`,
`GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`) — finishing on
`EvidenceGraph` + `EvidenceAudit`. Aggregate results (e.g. `GeneratedProductCheck`'s
known local environment failure) are non-authoritative and recorded under
`readiness/logs/`.
