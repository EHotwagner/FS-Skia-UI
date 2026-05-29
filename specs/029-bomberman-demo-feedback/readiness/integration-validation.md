# Integration Validation

Tasks: T050-T054
Captured: 2026-05-29T12:20:00+02:00

## Generated Validation

- `./fake.sh build -t GeneratedProductCheck`: pass, `readiness/generated-product-check.log`
- `./fake.sh build -t GeneratedGuidanceCheck`: pass, `readiness/generated-guidance-check.log`
- `./fake.sh build -t TemplateCheck`: pass, `readiness/template-check-final.log`

## Package Tests

- SkiaViewer.Tests: 48 passed, `readiness/skiaviewer-tests.log`
- Testing.Tests: 36 passed, `readiness/testing-tests.log`
- Elmish.Tests: 4 passed on retry with `--no-build`, `readiness/elmish-tests-retry2.log`
- Scene.Tests: 11 passed, `readiness/scene-tests.log`
- Layout.Tests: 23 passed, `readiness/layout-tests.log`

The first Elmish run aborted with a VSTest host out-of-memory after SkiaViewer and Testing had passed. The immediate `--no-restore --no-build` retry passed all Elmish tests.

## Surface and Drift

- `./fake.sh build -t PackageSurfaceCheck`: pass, `readiness/package-surface-check.log`
- `./fake.sh build -t TemplateDrift`: pass, `readiness/template-drift-check.log`

## Evidence Graph And Audit

- graph-only validation: pass, `readiness/evidence-graph-final.log`
- initial audit: failed readiness wording only, then fixed.
- final audit: pass, `readiness/evidence-audit-final.log`

## Final Verify

- `./fake.sh build -t Verify`: pass, `readiness/verify-final.log`
- Captured: 2026-05-29T14:17:41+02:00
- Total runtime: 00:04:29.0042423
- Reviewer navigation: `verify-final.log` is the aggregate handoff; authoritative focused logs remain the generated validation, package surface, FSI transcript, test, evidence graph, and evidence audit logs named above.
