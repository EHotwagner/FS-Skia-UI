# Governance Risk Levels

Recorded at: 2026-05-28T10:55:09+02:00

This feature is treated as broad risk because it changes public `.fsi`
contracts, generated template output, generated product package pins, and
readiness evidence semantics.

Risk vocabulary: small, medium, broad. Required evidence for broad validation
includes public surface checks, generated product validation, template
validation, focused tests, graph validation, and audit evidence.

Required focused evidence:

- SkiaViewer public contract tests:
  `readiness/logs/t018-skiaviewer-tests.txt`
- Testing public validator tests:
  `readiness/logs/t024-testing-tests.txt`
- Governance/generated guidance tests:
  `readiness/logs/t029-governance-tests.txt`
- Generated product and template validation:
  `readiness/logs/t031-generated-product-check.txt`,
  `readiness/logs/t031-template-check.txt`,
  `readiness/logs/t031-template-drift.txt`
- Package surface validation:
  `readiness/logs/t030-package-surface-check.txt`
- Evidence graph and audit:
  `readiness/logs/t034-evidence-graph.txt`,
  `readiness/logs/t035-evidence-audit.txt`

Current blocking condition:

- T020 failed because live viewer-window screenshot capture is unavailable in
  the current implementation/host path. Unsupported capture details are
  documented in `screenshot-capability-detail.md`; no screenshot proof is
  claimed.
