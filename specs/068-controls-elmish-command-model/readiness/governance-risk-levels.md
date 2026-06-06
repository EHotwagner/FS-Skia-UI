# Governance risk levels

This feature is a **Tier 1 (contracted)** but **additive-only** change confined to
`src/Controls.Elmish/`. It spans the **small** level below; because it edits a public
`src/**/*.fsi`, `Route` escalates it to the consumer-contract **broad validation** path,
but it introduces no new gate, routing, or target-metadata surface (so it does not reach
the routing-class **broad** change tier).

- **small** (this feature's level) — adding **additive** public API to a single shipped
  package (`FS.Skia.UI.Controls.Elmish`): `ControlsElmish.widgetView` /
  `ControlsElmish.programOfWidget` and the `AdapterCmd` bridge module. **Required evidence**
  (focused validation): `Dev` (build + the new/extended `Elmish.Tests`, red→green) and the
  `package-surface` gate set printed by `Route` — `PackageSurfaceCheck`,
  `PerPackageSurfaceDiff`, `FsiTranscripts` — showing the regenerated baselines are
  **additive-only** (SC-006) and the new symbols load from the packed library.
- **medium** — would apply if a second package's surface or a dependency edge changed.
  **None does here**: the base `FS.Skia.UI.Controls` package is byte-unchanged and gains no
  `Fable.Elmish` reference (FR-006/SC-005), and no other package baseline moves (SC-006).
  **Required evidence** would be the additional affected-package gates; not triggered.
- **broad** — a routing/target-metadata change (`Routing.fs`, `Targets.fs`,
  `validation.contract.yml`) or a template-asset change. **This feature includes no broad
  change** — no new gate and no routing edit (`Route` already escalates a public
  `src/**/*.fsi`). The corresponding **broad validation** (the serialized maintainer-verify
  order) is nonetheless run because this is a consumer-contract change.

## Authoritative per-gate verdicts (this run)

The authoritative gate selector is `./fake.sh build -t Route` over the branch diff; it
printed `Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts,
GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit` (matched rules:
`package-surface`, `evidence-governance`, `specify-catchall`, `docs-only`). Each was run
**sequentially** (FAKE `.fake` state is not concurrency-safe).

| Gate | Verdict | Note |
|------|---------|------|
| `Dev` | PASS | build + all default tests, incl. 17 `Elmish.Tests` (US1–US4, AdapterCmd edges, 2 FsCheck ≥1000-case properties) — 17 passed, 0 failed |
| `PackageSurfaceCheck` | PASS | reflection baseline additive-only (`+AdapterCmd`) |
| `PerPackageSurfaceDiff` | PASS | raw `.fsi` snapshot additive-only (`+open Elmish`, `+module AdapterCmd`, `+widgetView`, `+programOfWidget`; 0 removed) |
| `FsiTranscripts` | PASS | `controls-elmish-prelude.fsx` loads `programOfWidget` and `AdapterCmd.toCmd`/`productMessages` from the packed library |
| `GeneratedGuidanceCheck` | PASS | unchanged |
| `TemplateDrift` | PASS | template byte-unchanged (feature ships no template asset) |
| `EvidenceGraph` | PASS | DAG acyclic, no dangling refs, no `[S*]` |
| `EvidenceAudit` | PASS | `verdict=PASS`, `total-blockers=0`, empty Synthetic-Evidence Inventory |

## Authoritative vs aggregate results

FAKE-backed gates run **sequentially**; the **aggregate** umbrella result of a multi-target
run is **non-authoritative**. The authoritative verdict is the **per-gate** result recorded
in this `readiness/` tree (`controls-elmish-command-model.md` for the parity/round-trip
results, `package-surface-expectations.md` for the additive surface delta, and
`evidence-audit.md` for the merge gate). `EvidenceAudit verdict=PASS` is the merge gate.
