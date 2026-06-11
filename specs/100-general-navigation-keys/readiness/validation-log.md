# Validation log — escalated serialized run (feature 100, R5, T022)

evidence-kind=validation-log
status=pass

`./fake.sh build -t Route` escalated this change to **agent-ready / controls-public-surface**
(`matched-rules=controls-public-surface, evidence-governance, specify-catchall, docs-only,
package-surface`) and printed the 15-gate set. Each FAKE-backed target was run on its **own**
invocation in deterministic order (shared `.fake` state is not safe to run concurrently); no aggregate
was treated as authoritative.

## Run order + verdicts

1. `./fake.sh build -t Dev` → Status: Ok (Controls.Tests 307/307, Elmish.Tests 69/69, all other suites green)
2. `./fake.sh build -t PackageSurfaceCheck` → Status: Ok
3. `./fake.sh build -t PerPackageSurfaceDiff` → Status: Ok
4. `./fake.sh build -t FsiTranscripts` → Status: Ok (after updating the two prelude .fsx scripts for `ControlEvent.Nav`)
5. `./fake.sh build -t ControlsCatalogCheck` → Status: Ok
6. `./fake.sh build -t ControlsCatalogGenerationCheck` → Status: Ok
7. `./fake.sh build -t DesignTokenDrift` → Status: Ok
8. `./fake.sh build -t ContrastCheck` → Status: Ok
9. `./fake.sh build -t ControlsInteractionCheck` → Status: Ok
10. `./fake.sh build -t ControlsRenderingCheck` → Status: Ok
11. `./fake.sh build -t GeneratedGuidanceCheck` → Status: Ok
12. `./fake.sh build -t TemplateDrift` → Status: Ok
13. `./fake.sh build -t GeneratedProductCheck` → Status: Ok
14. `./fake.sh build -t EvidenceGraph` → see [evidence-graph.md](./evidence-graph.md)
15. `./fake.sh build -t EvidenceAudit` → see [evidence-audit.md](./evidence-audit.md)

## Notes

- No race-like FAKE failure was observed once run sequentially; the aggregate-hang contract is in
  [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md).
- The single mid-run test failure was a test-only assertion bug (corrected), not a product regression;
  re-run sequentially confirmed green.
- Surface baselines were recaptured by `RefreshSurfaceBaselines` and validated by PackageSurfaceCheck +
  PerPackageSurfaceDiff (see [surface-baseline.md](./surface-baseline.md)).
