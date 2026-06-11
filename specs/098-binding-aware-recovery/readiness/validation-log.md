# Validation log (T021, feature 098, R3)

evidence-kind=validation-log
status=pass
authoritative=true
failure-class=none

Sequential single-target runs of the gates `Route` printed (escalated controls-public-surface set). FAKE
shared `.fake` state ⇒ never concurrent. Transcript of `Status:` lines:

```
Route                          tier=agent-ready; matched-rules include controls-public-surface
Dev                            Ok   (Controls 282/282, Elmish 55/55, all suites)
PackageSurfaceCheck            Ok
PerPackageSurfaceDiff          Ok
FsiTranscripts                 Ok
TemplateCheck                  Ok
GeneratedProductCheck          Ok
ControlsCatalogCheck           Ok
ControlsCatalogGenerationCheck Ok
DesignTokenDrift               Ok
ContrastCheck                  Ok
ControlsInteractionCheck       Ok
ControlsRenderingCheck         Ok
GeneratedGuidanceCheck         Ok
SkillContractPathCheck         Ok
TemplateDrift                  Ok
EvidenceGraph                  Ok   (see evidence-graph.md)
EvidenceAudit                  PASS (see evidence-audit.md)
```

## Test counts (focused single-project reruns)

- `dotnet test tests/Controls.Tests/Controls.Tests.fsproj` → 282/282 (incl. 5 new Feature098UnifiedScheme
  cases: determinism ≥1000, distinctness ≥1000, sibling routing, single-scheme agreement, render.BoundIds).
- `dotnet test tests/Elmish.Tests/Elmish.Tests.fsproj` → 55/55 (incl. 7 new Feature098Dispatch cases:
  US1 AS1–AS3, US2 AS1–AS3, MapPointer-only invariance).

## Notes

- Surface baselines were regenerated via `RefreshSurfaceBaselines` before the surface gates; the only diff
  is the `BoundIds` field + `val boundIdsOf` (see surface-baseline.md).
- A stray timing-only `elapsed-ms` wobble in an unrelated 011 sample-smoke artifact was reverted.
- Any aggregate FAKE result is treated as non-authoritative unless re-confirmed sequentially (it was).
