# Governance Risk Levels — Feature 083 (Color Contrast & Palettes)

This feature is an **escalated / broad** consumer-contract change: a new public package
`FS.Skia.UI.Color` (+ `.fsi` surface), a new build gate (`ContrastCheck`), generated
design-token *value* changes, governance routing edits, and a new template pin. It touches
`template/**`, governance routing, and a new public surface, so `Route` **escalates** it to
the serialized maintainer-verify gate set.

| Risk level | Scope | Authoritative validation |
|------------|-------|--------------------------|
| **small**  | routine framework-internal edits within this feature's own pure `src/Color/*.fs` library work (Contrast/Palettes bodies) | focused `./fake.sh build -t Dev` + the `Color.Tests` suite |
| **medium** | the new public `src/Color/*.fsi` surface, the `ContrastGate.fs[i]` gate core, the DTCG token-value edit, and the `Governance.Tests` routing/regression cases | focused `Dev` + the targeted FAKE gates `Route` prints (`ContrastCheck`, `DesignTokenDrift`, `PerPackageSurfaceDiff`) |
| **broad**  | required here because this is a new public package + new gate + `template/**` + governance-path change that `Route` escalates | the full serialized FAKE gate order — see below |

## Required evidence and broad validation

The **required evidence** per risk level is named in the table above. **Broad validation**
(the full serialized FAKE order) is required here because `Route` escalates this change.

The **broad** serialized order: `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` →
`GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`. FAKE-backed targets share `.fake`
state and run **sequentially**; aggregate results are recorded as **non-authoritative**, and any
race-like or environment-flaky gate failure is rerun in focused isolation (the focused rerun is
the authoritative result).

- **Authoritative command**: `./fake.sh build -t Route` (selector), then the serialized order.
- **Artifact path**: this file; per-target results in `readiness/aggregate-hang-diagnostics.md`.
- **Failure class**: governance / product (a sub-threshold shipped token → product).
- **Next action**: run `Route` first; run only the gates it prints; for this escalated change run
  the serialized order sequentially and record outcomes here.

## Serialized-order results (T028)

`Route` confirmed: tier `maintainer-verify`; matched rules include `controls-public-surface` and
`color-contrast`; gate list includes `ContrastCheck`. The escalated order was run sequentially:

| Target | Result |
|--------|--------|
| Route | PASS — lists `ContrastCheck` + `color-contrast` rule |
| Dev | PASS (Test all projects incl. Color.Tests 15/15 + Governance.Tests 511/511; SkillSyncCheck) |
| ContrastCheck | PASS — both themes conformant (SC-001) |
| DesignTokenDrift | PASS — `DesignTokens.fs` current |
| PerPackageSurfaceDiff | PASS — zero drift incl. new `FS.Skia.UI.Color` baseline |
| GeneratedGuidanceCheck | PASS — `.claude` mirrors current |
| TemplateCheck | PASS — new `FS.Skia.UI.Color` pin packed + revalidated |
| GeneratedProductCheck | known local **environment-failure** (no template `feature.json`; `Map.empty` env) — non-authoritative, see `aggregate-hang-diagnostics.md` |
| EvidenceGraph | PASS — see `readiness/evidence-graph.md` |
| EvidenceAudit | see `readiness/evidence-audit.md` |
