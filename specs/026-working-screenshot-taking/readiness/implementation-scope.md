# Implementation Scope

Status: setup recorded.

- Tier: Tier 1 contracted change.
- Affected layers: `src/SkiaViewer`, `src/Testing`, generated product evidence
  commands, generated guidance, docs, governance checks, package surface
  baselines, FSI transcripts, and readiness evidence.
- Public API impact: additive screenshot capture and validation contracts must
  start in `.fsi`, then semantic tests and FSI transcripts, then implementation,
  then surface baselines.
- MVU/effect applicability: screenshot capture is I/O-bearing and must remain
  modeled through `EvidenceWorkflowModel`, `EvidenceWorkflowMsg`,
  `EvidenceWorkflowEffect`, `initEvidenceWorkflow`, `updateEvidenceWorkflow`,
  and an interpreter at the viewer/generated command edge.
- Synthetic success: forbidden. Screenshot success requires a real PNG produced
  by the working viewer-backed path with readable dimensions and non-blank
  pixels.
- Required real screenshot evidence: at least one supported-host working-code
  PNG artifact, plus honest unsupported-host or failure diagnostics when
  applicable.
