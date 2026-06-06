# Quickstart: Typed-Controls-Migration Follow-Ups (071)

Two independent themes. US1 (catalog single source, P1) and US2 (typed gallery
panel + evidence, P2) share no code and can be implemented in either order; do US1
first since it is the load-bearing currency guarantee.

## Prerequisites

- On branch `071-typed-controls-followups`.
- `070` already shipped the 41 typed modules, their lowering-parity tests, and the
  T036 cross-check — do **not** reauthor them.
- FAKE-backed targets share `.fake` state — run them **sequentially**, never
  concurrently.

## US1 — Catalog single source over all 47 (P1)

1. **Extend the fact table.** In `build/Governance/CatalogGen.fs`, add the 41
   missing controls to `catalogFacts`, copying each row's facts (id, display name,
   category, module, purpose, required attributes, events, accessibility role) from
   the matching hand-maintained row in `src/Controls/Catalog.fs` /
   `src/Controls/catalog.yml`. Set `RequiredAttributes = []` for `custom-control`.
2. **Generalize the evidence special-case.** Change `renderFSharpRow`'s
   `if fact.Id = "data-grid"` to membership in
   `{ "data-grid"; "line-chart"; "bar-chart"; "pie-chart"; "scatter-plot"; "graph-view" }`,
   and extend the YAML chart/data-grid evidence path to the same set.
3. **Regenerate** (single source → both files):
   ```
   ./fake.sh build -t RefreshSurfaceBaselines
   ```
   Confirm `catalog.yml` and `Catalog.fs` now have 47 `BEGIN/END GENERATED:
   typed-catalog/<id>` regions and the row bytes inside the 41 new regions match
   the previously hand-maintained rows (diff is markers-only for those rows).
4. **Capture parity fixtures.** For each of the 41 new ids, write
   `Catalog.fs.<id>.txt` and `catalog.yml.<id>.txt` into
   `specs/066-typed-catalog-generation/readiness/parity-fixtures/`, captured from
   `renderFSharpRow` / `renderYamlRow` output (golden, trailing newline trimmed).
5. **Extend the lockstep map.** In `tests/Controls.Tests/CatalogTests.fs`, extend
   `typedPropsById` to all typed ids (exclude `custom-control` from the Props-field
   assertion).
6. **Verify currency + cross-check.**
   ```
   ./fake.sh build -t Dev                     # Controls.Tests incl. CatalogTests
   ./fake.sh build -t ControlsCatalogGenerationCheck   # if Route prints it
   ```
   Then prove the gate bites: hand-edit one generated region, re-run the currency
   gate, confirm it fails naming the stale `typed-catalog/<id>` region and the
   regen command; revert and confirm green (SC-002).
7. **Write US1 evidence**: `readiness/catalog-single-source.md` (6→47 rationale,
   the six evidence ids, "all 47 generated, zero hand-maintained").

## US2 — Typed gallery panel + evidence (P2)

1. **Extend the panel.** In `samples/ControlsGallery/Program.fs`, grow
   `typedAuthoringPanel` to ≥1 control per mechanic group (display, input, stateful
   input, layout container, navigation/composite, overlay, selection collection,
   charts/graph) using only `FS.Skia.UI.Controls.Typed.*` `view` calls — no `Attr`,
   no `*.create`.
2. **Extend coverage.** Add cases to `tests/Controls.Tests/RenderingTests.fs` and
   `AccessibilityTests.fs` that render/assert the panel at ≥2 viewports (mirror the
   existing "render output covers viewport sizes" + typed-vs-legacy parity tests).
   Confirm they fail before the panel exists, pass after.
   ```
   ./fake.sh build -t Dev
   ```
3. **Capture render evidence**: `readiness/controls-rendering.md` — render-only,
   ≥2 viewports, byte-identical re-capture, **no** `[S]` disclosure. Re-run the
   capture and confirm identical bytes.

## Close out (governance gates)

```
./fake.sh build -t Route          # re-run on the implementation diff; run ONLY what it prints
```
If `Route` escalates, run FAKE-backed gates sequentially in deterministic order
(`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
`EvidenceGraph`, `EvidenceAudit`). Success bar (SC-008): every printed gate passes
and `EvidenceAudit` verdict is **PASS** with no `[S]`/`[S*]` disclosures; the
per-package surface delta is additive-only/empty (SC-007).
