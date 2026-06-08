# Contract: Catalog Docs Generation & Currency

The "interface" this feature exposes is a **generation + currency contract** over the
docs site (there is no public `.fsi` / API change). It binds the single source
`CatalogGen.catalogFacts` to the generated docs artifacts and the new gate.

## C1 — Generation projection (pure)

`renderCatalogIndex (facts) : string` and `renderDetailHeader (fact) : string` are
**pure** functions in `build/Governance/CatalogDocsGen.fs`.

- Deterministic: same `facts` ⇒ byte-identical output (ordering by category then a
  stable within-category order; invariant culture; fixed formatting).
- Index output enumerates every fact, grouped by `Category`, with DisplayName linking
  to `controls/<id>.html`, the one-line `Purpose`, and the total count.
- Detail header output: H1 `DisplayName`, a Category line, the `Purpose`, and an API
  link derived from `Module` (slug verified per research R2).
- Splice: `spliceCatalogDocs (fileText) : string` REPLACES content **only** inside
  existing `BEGIN/END GENERATED: catalog-docs/<key>` markers; it never invents
  markers (memory `catalog-splice-marker-insertion`). Idempotent.

## C2 — Refresh contract

`RefreshSurfaceBaselines` regenerates the index region and every detail-page header
region from `catalogFacts`. After a clean refresh, `ControlsCatalogDocsCheck` MUST
report PASS. Growing the control set requires the new control's marker pair to be
pre-placed (new detail page stub committed) before refresh fills it.

## C3 — Currency check contract (`ControlsCatalogDocsCheck`)

Pure core `catalogDocsCurrency (facts, files) : Finding list`; the `Engine/Update.fs`
handler performs file read/listing and `WriteStructuredReport` / `FailWith` at the
edge. PASS ⇔ findings empty. Fails on any class in
[data-model.md](../data-model.md) (IndexStale, Missing/Stale/OrphanDetailPage,
Missing/Undecodable/OrphanPreview, DeadLink). Report names each finding with an
actionable remedy and the regenerate command. Required output files are asserted via
`RequireFiles`.

## C4 — Preview honesty contract

Each required preview is validated with the existing `Testing` PNG check: decodable,
real (non-1×1) dimensions, non-trivial content. A control with no honest render MUST
carry an explicit unsupported note on its page and MUST NOT have a fabricated/
placeholder asset. Real `RenderingFailure` diagnostics are preserved (not masked as
"unsupported"); benign environment warnings are classified benign per
`fs-skia-evidence-mode`.

## C5 — Link-resolution contract

Every generated link (index→detail, detail→API reference) MUST resolve within the
built `output/` site. The check validates targets exist; a wrong API slug or missing
detail page fails the gate (FR-009, SC-003).

## C6 — Routing contract

`Route` selects `ControlsCatalogDocsCheck` when the diff touches the catalog single
source, `docs/controls/**`, `docs/img/controls/**`, or the generator. The gate is in
`AgentValidation.knownGates`; `validation.contract.yml` is regenerated from
`Routing.fs` (`TargetMetadataDrift`-enforced), never hand-edited.

## Acceptance (maps to spec)

| Contract | Spec |
|---|---|
| C1 index from source, not hand-edited | FR-001, FR-004, FR-010 |
| C1 detail header + API link | FR-002 |
| C3 completeness/currency/orphan | FR-005, SC-002, SC-004 |
| C4 preview honesty | FR-003a, SC-003 |
| C5 link resolution | FR-009, SC-003 |
| C6 routing/gate wiring | Build-target impact |
