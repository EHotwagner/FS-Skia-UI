# Contract: Readiness Evidence

## Purpose

Define the minimum readiness evidence required before the process reliability
follow-up can be treated as complete.

## Required Files

All readiness files live under
`specs/012-fix-refactor-process/readiness/`:

- `process-health.md`
- `focused-gates.md`
- `governance-scanners.md`
- `stale-boundary-scan.md`
- `generated-product-validation.md`
- `bootstrap-runner.md`
- `verification-verdicts.md`
- `evidence-graph.md`
- `evidence-audit.md`

Logs live under `specs/012-fix-refactor-process/readiness/logs/`.

## Evidence Requirements

- Process-health evidence records snapshot fields, threshold decisions,
  overrides, unsupported signals, and fail-fast outcomes.
- Focused gate evidence records directly invoked gates, prerequisites, logs,
  durations, and verdicts.
- Governance scanner evidence records dependency parsing, profile-aware
  generated validation, inventory coverage, stale scan scope, and fixture
  results.
- Bootstrap evidence records runner dependency restoration, wrapper/tool
  availability, warning classification, and startup smoke results.
- Verification verdict evidence separates product failures from environment
  failures and names whether broad aggregate evidence is authoritative.
- Final readiness states whether broad aggregate evidence passed, failed due
  to environment conditions, or is waiting for fresh-run confirmation.

## Final Readiness Rule

After any broad aggregate `environment-failure`, final readiness remains
blocked until a later healthy broad aggregate pass is recorded. Focused passing
evidence may support diagnosis but must not replace that broad signal.

## Synthetic Evidence

Synthetic scanner fixtures are allowed for rule tests and seeded stale
scenarios. Synthetic final readiness evidence is not planned. If implementation
uses synthetic evidence beyond scanner fixtures, it must follow the
constitution disclosure policy and remain visible to `EvidenceAudit`.

## Validation

- `EvidenceGraph` verifies task/evidence dependency shape.
- `EvidenceAudit` blocks unresolved synthetic propagation and diff-scan hits.
- `Verify` and `Ci` produce verdict evidence that cannot silently convert
  environment failures into success.
