# Validation log — escalated controls-public-surface run (feature 099, R4, T022)

`./fake.sh build -t Route` escalated this change to `tier=agent-ready` (matched-rules include
`controls-public-surface`, triggered by the internal `src/Controls/RetainedRender.fsi` slot-type
change). The gates `Route` printed were run **sequentially** (shared `.fake` state — never
concurrently); each verdict below is from its own FAKE invocation, so they are authoritative (no
non-authoritative aggregate).

## Route output

```
tier=agent-ready
gates=Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck, ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
matched-rules=controls-public-surface, evidence-governance, specify-catchall, docs-only, package-surface
```

## Sequential gate transcript

| # | gate | verdict | duration |
|---|------|---------|----------|
| 1 | Dev (Restore + Build + SampleContractSmoke + Test + SkillSyncCheck) | Status: Ok | ~3m29s |
| 2 | PackageSurfaceCheck | Status: Ok | ~6.5s |
| 3 | PerPackageSurfaceDiff | Status: Ok | <1s |
| 4 | FsiTranscripts | Status: Ok | ~10s |
| 5 | DesignTokenDrift | Status: Ok | <1s |
| 6 | ContrastCheck | Status: Ok | <1s |
| 7 | ControlsCatalogCheck | Status: Ok | ~5.7s |
| 8 | ControlsCatalogGenerationCheck | Status: Ok | <1s |
| 9 | ControlsInteractionCheck | Status: Ok | ~5.5s |
| 10 | ControlsRenderingCheck | Status: Ok | ~5.5s |
| 11 | GeneratedGuidanceCheck | Status: Ok | <1s |
| 12 | TemplateDrift | Status: Ok | ~2.1s |
| 13 | GeneratedProductCheck | Status: Ok | <1s |
| 14 | EvidenceGraph | (see evidence-graph.md) | — |
| 15 | EvidenceAudit | (see evidence-audit.md) | — |

## Test detail (Dev → Test target)

- `Controls.Tests`: 294 passed, 0 failed (includes `Feature099AnimationClockTests` — the 1000-case
  determinism property + delta/trigger edges + identity-at-rest byte-identity / zero-recompute).
- `Elmish.Tests`: 59 passed, 0 failed (includes `Feature099AnimationSeamTests` — animates-vs-snaps,
  seam-driven survival, removed-identity GC, scoped-repaint; and the updated `Feature092LiveSurvivalTests`).
- `Parity.Tests`: 21 passed (golden parity unaffected by the reused-fragment / `VisualStateValue`
  equality change).

No race-like FAKE failure was observed; no aggregate hang (see `aggregate-hang-diagnostics.md`).
