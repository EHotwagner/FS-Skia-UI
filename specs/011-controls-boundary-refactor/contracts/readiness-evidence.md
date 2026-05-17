# Contract: Readiness Evidence

## Purpose

Define the minimum evidence set required before this boundary refactor can be
treated as implementation-ready or merge-ready after implementation.

## Required Files

All readiness files live under
`specs/011-controls-boundary-refactor/readiness/`:

- `public-surface.md`
- `package-boundary.md`
- `elmish-adapter.md`
- `keyboardinput-package.md`
- `control-catalog.md`
- `control-runtime.md`
- `rich-rendering.md`
- `keyboard-input-elmish.md`
- `chart-datagrid-controls.md`
- `generated-product-usage.md`
- `dependency-report.md`
- `template-drift.md`
- `compatibility-impact.md`
- `evidence-graph.md`
- `evidence-audit.md`

## Evidence Requirements

- Public surface evidence includes `.fsi` paths, surface baseline paths, FSI
  transcript paths, and package ids.
- Package boundary evidence records removed Charts package references and
  Controls dependency rationale.
- Elmish adapter evidence demonstrates direct command/subscription/program
  integration outside ordinary Controls declarations.
- KeyboardInput evidence demonstrates runtime state, effects, focus recovery,
  and state display.
- Control runtime evidence demonstrates product-owned transient state and at
  least two stale/cancelled recovery paths.
- Rich rendering evidence demonstrates Skia-specific rich text or rich drawing
  without renderer-neutral claims.
- Chart/DataGrid evidence demonstrates Controls-owned chart, graph, and
  DataGrid usage with DataGrid categorized as data/collection.
- Generated product evidence demonstrates package references, examples, and
  absence of copied framework source.
- Compatibility impact evidence documents the Charts replacement path without
  promising a shim, automated migration, or release automation.

## Synthetic Evidence

Synthetic evidence is not planned as primary proof. If used for clipboard,
IME, GPU, font, or unsupported environment paths, it must be disclosed under
the constitution at task, code, test, spec, and PR surfaces and must remain
visible to `EvidenceAudit`.

## Validation

- `EvidenceGraph` verifies task/evidence dependency shape.
- `EvidenceAudit` blocks unresolved synthetic propagation and diff-scan hits.
- `Verify` and `Ci` include the relevant readiness checks or reference the
  produced evidence paths.
