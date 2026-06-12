# Aggregate-hang diagnostics (feature 108)

aggregate-result=non-authoritative aggregate
focused-rerun=performed

When the full aggregate test run is interrupted or its result is unknown, the affected FAKE-backed
commands are rerun **sequentially** (shared `.fake` state) and the focused rerun is authoritative. The
per-suite Feature108 results below were captured by a focused rerun of each test project in isolation
(`dotnet run --project <suite> -- --filter-test-list "Feature 108"`):

- tests/Controls.Tests (Feature108 Focus + Composition + Theming): 16 passed, 0 failed.
- tests/Elmish.Tests (Feature108 Perf.runScript metrics + coalescing): 7 passed, 0 failed.
- tests/KeyboardInput.Tests (Feature108 modifier boundary): 4 passed, 0 failed.
- tests/SkillSupport.Tests (Feature108 EvidenceTour): 3 passed, 0 failed.

No race-like or concurrent-FAKE failure was observed; the focused reruns are the authoritative signal.

validation_verdict:
  verdict: aggregate pass via focused sequential reruns; no hang observed (any non-authoritative aggregate result is superseded by the focused reruns below)
  stage: Test aggregate
  elapsed duration: Dev completed without hang (Restore ~35s, Build ~50s, Test suites green), exit code 0 on the build+test gate; the routed gates ran sequentially (Status Ok each) — PackageSurfaceCheck, PerPackageSurfaceDiff, ControlsDocCoverageCheck, DesignTokenDrift, ContrastCheck, FsiTranscripts, ControlsCatalogCheck, ControlsCatalogGenerationCheck, ControlsInteractionCheck, ControlsRenderingCheck, SkillContractPathCheck, TemplateDrift, GeneratedGuidanceCheck, TemplateCheck, EvidenceGraph
  last observed command: dotnet test tests/Governance.Tests/Governance.Tests.fsproj (Passed 583/0)
