# Governance risk levels

This feature is a Tier 2 **internal** change confined to `src/Controls/**`; it spans
the **small** and **medium** levels below and does **not** reach the **broad** level
(it edits no routing/target-metadata surface).

- **small** — adding assembly-internal framework code that ships no public surface:
  the new `module internal Reconcile` (`Reconcile.fsi`/`Reconcile.fs`) plus the
  test-only `<InternalsVisibleTo>` item and `FsCheck` reference. **Required evidence**
  (focused validation): `Dev` (build + the new `ReconcileTests` red→green) and
  `PackageSurfaceCheck` showing an unchanged public api-surface baseline (SC-005).
- **medium** — because the diff touches `src/Controls/**`, `Route` escalates to the
  `controls-public-surface` rule. **Required evidence**: the Route-printed gate set
  (`PackageSurfaceCheck`, `PerPackageSurfaceDiff`, `FsiTranscripts`,
  `GeneratedProductCheck`, `ControlsCatalogCheck`, `ControlsCatalogGenerationCheck`,
  `ControlsInteractionCheck`, `ControlsRenderingCheck`) — the catalog/interaction/
  rendering gates being the enforcer that render, layout, diagnostics, accessibility,
  and interaction behavior are unchanged (FR-012). The new internal `.fsi` is a raw
  per-package snapshot change, so `readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt`
  is refreshed (PerPackageSurfaceDiff), while the public api-surface contract is
  byte-stable.
- **broad** — a routing/target-metadata change (`Routing.fs`, `Targets.fs`,
  `validation.contract.yml`) or a consumer-contract/template change. **This feature
  does not include any broad change** (no new gate, no routing edit — `Route` already
  escalates `src/Controls/**`). The corresponding **broad validation** (the escalated
  serialized FAKE order: `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`/`TemplateDrift`,
  `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`) is nonetheless run here
  because the routing rule escalates the feature to the maintainer-verify path.

## Authoritative per-gate verdicts (this run)

| Gate | Verdict | Note |
|------|---------|------|
| `Dev` | PASS | build + all default tests, incl. the 12 new `ReconcileTests` (US1–US4, edges, FsCheck round-trip/determinism over 1000 cases each) — 77 passed, 0 failed |
| `PackageSurfaceCheck` | PASS | public api-surface baseline byte-unchanged (SC-005); `Reconcile.fsi` is `module internal`, not in the `contracts:` list |
| `PerPackageSurfaceDiff` | PASS | raw `.fsi` snapshot refreshed for the added internal module (+38 lines, 0 removed) |
| `ControlsCatalogCheck` / `ControlsCatalogGenerationCheck` | PASS | catalog unchanged and current |
| `ControlsInteractionCheck` / `ControlsRenderingCheck` | PASS | render/layout/diagnostics/accessibility/interaction behavior unchanged (FR-012) |
| `FsiTranscripts` | PASS | no public symbol added; internal reach proven by in-assembly tests |
| `GeneratedGuidanceCheck` | PASS | unchanged |
| `TemplateDrift` | PASS | template byte-unchanged (feature ships no template asset) |
| `EvidenceGraph` | PASS | `verdict=ok` (24 tasks, no cycles, no dangling refs, no `[S*]`) |
| `EvidenceAudit` | PASS | `verdict=PASS`, `total-blockers=0`, empty Synthetic-Evidence Inventory |
| `GeneratedProductCheck` | environment-degraded | the generated product's evidence-graph sub-step cannot self-resolve a feature in this headless sandbox (empty generated `.specify/feature.json`) — a pre-existing condition also seen on merged 064/065/066, **not** a regression from this byte-identical, framework-internal change (see `runtime-limitations.md`) |

## Authoritative vs aggregate results

FAKE-backed gates run **sequentially** (shared `.fake` state; never concurrently). The
**aggregate** umbrella result of any multi-target run is **non-authoritative**; the
authoritative verdict is the **per-gate** result recorded in this `readiness/` tree
(`keyed-reconciliation.md` for the algorithm/property results,
`package-surface-expectations.md` for the zero public-surface delta, and
`evidence-audit.md` for the merge gate). `EvidenceAudit verdict=PASS` is the merge gate.
