# Governance risk levels — feature 106 (controls-api-discoverability)

Feature 106 is a **Tier-1 (contracted)** change: `///` documentation across the Controls
public `.fsi` surface (doc-only, no signature shape change), a sample/template contract
rewrite (`View.fs`, `README.md`, a bundled catalog reference), and a new
`ControlsDocCoverageCheck` governance gate routed under the controls-public-surface rule.
`Route` is authoritative and escalates to the controls-public-surface set (triggered by
`src/Controls/**/*.fsi`, `template/**`, `build/**`).

## small

A single `.fsi` file's doc-comment rewrite (e.g. `Collections.fsi`), or a one-line README
pointer edit.
- required evidence: the file compiles; `ControlsDocCoverageCheck` stays green; the diff shows
  only `///` comment-line changes for that file.

## medium

**This feature's level.** The 356-summary rewrite across ~30 `.fsi` files + the starter
migration to the typed front door + the README/catalog-reference bundling + the new routed
gate.
- required evidence: the gate set `Route` prints, run **sequentially** (the escalated
  `controls-public-surface` set incl. `ControlsDocCoverageCheck` + `TargetMetadataDrift`, plus
  `GeneratedProductCheck`, `TemplateCheck`); `ControlsDocCoverageCheck` returns `findings=0`
  over the real surface (SC-002); the Controls + Controls.Elmish suites green incl. the
  `TypedLoweringTests` parity cases (SC-003/FR-003); `git diff -- 'src/Controls/**/*.fsi'`
  shows only `///` lines (zero shape delta); the api-surface bundle current + per-package
  baseline byte-stable; `EvidenceGraph` + `EvidenceAudit` PASS with 0 synthetic.

## broad

Required only when adding the new gate to the routed set changes the aggregate, or a
FAKE-backed failure looks race-like. Then rerun the affected FAKE-backed commands
**sequentially** before any product-regression claim.
- broad validation: the full `Route`-printed gate set executed sequentially (shared `.fake`
  state, never concurrently) in deterministic order; aggregate-suite results obtained outside
  the routed focused set are recorded as a **non-authoritative aggregate** (see
  `aggregate-hang-diagnostics.md`) and the per-suite Expecto outcomes are authoritative.
