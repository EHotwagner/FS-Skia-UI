# Governance Risk Levels — 079-doc-preview-examples (T022)

Authoritative command: `./fake.sh build -t Route` then the printed gate set.
Artifact path: this file. Failure class: `risk-misclassification`.

## Route verdict (authoritative selector)

```
developer-class=framework-author
tier=agent-ready
gates=Dev, ControlsCatalogDocsCheck, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
dogfood-forced=false
matched-rules=evidence-governance, specify-catchall, docs-only, controls-catalog-docs
```

Per CLAUDE.md/AGENTS.md ("run only the gates it prints"), the validated gate set is exactly
the six above — **not** the full maintainer-verify six-target order the plan anticipated as a
worst case. The change is a Tier-2 internal + `docs/**` generation change with **no public
product `.fsi`/API/behavior change**, so `controls-catalog-docs` + `docs-only` routing keeps
it at **agent-ready**.

## Risk bands

The implementation tasks split into three governance risk bands. Broad validation is
required only at close-out; aggregate FAKE-backed results are non-authoritative and recorded
under `readiness/logs/`. FAKE-backed targets share `.fake` state — run them **sequentially**,
never concurrently.

### Small

The additive governance pure-core change (`TrivialPreview` finding + `Bytes` field + pinned
floor) and prose-only readiness edits. The **required evidence** for this band is the
failing-first `catalogDocsCurrency` unit coverage (the `TrivialPreview` / byte-floor cases)
plus a focused `dotnet fsdocs build` link check.

### Medium

Regenerating the 51 preview assets + 1 unsupported declaration, the `Engine/Update.fs`
byte-floor wiring, and the `categoryindex` nav renumber across 58 files. The **required
evidence** for this band is the `ControlsCatalogDocsCheck` currency proof (PASS on the
demonstrative tree; the gate biting on a trivial/missing/orphan preview) plus
`GeneratedGuidanceCheck` and the strict `dotnet fsdocs build --strict --eval`.

### Broad

No routing / target / `validation.contract.yml` / `knownGates` edits were required (the
`controls-catalog-docs` rule already routes the touched paths; `Routing.fs` is unchanged).
**Broad validation** re-ran `./fake.sh build -t Route` on the full implementation diff and
ran exactly the gates it printed — `Dev`, `ControlsCatalogDocsCheck`, `GeneratedGuidanceCheck`,
`TemplateDrift`, `EvidenceGraph`, `EvidenceAudit` — finishing on `EvidenceGraph` +
`EvidenceAudit`. Aggregate results are non-authoritative and recorded under `readiness/logs/`.

This change sits in the **Small–Medium** bands (no Broad routing/target edits).

## Focused validation (all PASS)

| Gate | Result |
|------|--------|
| Dev (Restore + Build + Test) | PASS (~3m05s; all default test projects green) |
| ControlsCatalogDocsCheck | PASS (51 rendered + 1 unsupported == 52; no trivial/missing/orphan) |
| GeneratedGuidanceCheck | PASS (no generated-guidance impact) |
| TemplateDrift | PASS (no template/contract drift; `validation.contract.yml` unchanged — Routing.fs unchanged) |
| EvidenceGraph | PASS (see `evidence-graph.md`) |
| EvidenceAudit | PASS (see `evidence-audit.md`) |
| `dotnet fsdocs build --strict --eval` | PASS exit 0 (see `docs-build.md`) |

`GeneratedProductCheck` is **not** in the Route gate set for this change; the known local
`GeneratedProductCheck` environment-failure (memory `generated-product-check-env-failure`) is
therefore not on the validation path. Aggregate `Dev` diagnostics: see
`aggregate-hang-diagnostics.md` (overwritten by the `Dev` target — aggregate PASS).
