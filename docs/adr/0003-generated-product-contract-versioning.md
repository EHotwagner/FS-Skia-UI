# ADR 0003 — Generated-product contract versioning policy

- **Status**: Accepted (policy recorded; enforcement deferred)
- **Date**: 2026-05-31
- **Decision source**: foundations plan
  (`docs/reports/2026-05-31-1049-foundations-implementation-plan.md`); this ADR
  records the resolved policy.

## Context

Generated consumer products depend on FS.Skia.UI capability contracts (viewer
contracts, Controls guidance, evidence/readiness contracts, template fragments).
As the framework evolves, a generated product pinned to an older contract must be
able to tell, deterministically, whether it is compatible with the framework
version it is validated against. Without an explicit versioning policy, contract
drift surfaces as opaque build/validation failures attributed to application
code rather than to a contract mismatch.

## Decision

Generated-product **contracts are versioned explicitly and independently of the
package marketing version**, using **semantic-versioning intent**:

1. Each consumer-facing contract carries a **contract version** distinct from the
   `Directory.Build.props` package `Version`.
2. **Backward-compatible additions** are **minor** bumps; **breaking changes** to
   an existing contract are **major** bumps and require a migration note.
3. A generated product records the **contract version it was generated against**;
   validation compares that against the framework's current contract version and
   reports a **contract-version mismatch as a first-class diagnostic** — before
   build/input/render failures are attributed to application code (consistent
   with the existing "report setup drift before blaming app code" rule in
   `docs/reports/dependencies.md`).
4. Contracts are **additive-by-default**: removals/renames are avoided in favour
   of deprecate-then-remove across a major boundary.

## Alternatives considered

- **Tie contract version to the package version (rejected):** couples unrelated
  cadences; a docs-only bump would falsely imply a contract change, and a break
  could hide inside a patch bump.
- **No explicit versioning, rely on build failures (rejected):** drift surfaces
  as opaque failures misattributed to application code — the exact failure mode
  this policy prevents.
- **Date-stamped contracts only (rejected):** dates do not encode compatibility
  intent (which break vs which is additive).

## Consequences / rationale

- Deterministic compatibility signalling: a mismatch is a named diagnostic, not a
  mysterious failure.
- Additive-by-default keeps existing generated products working across minor
  framework evolution.
- Decoupling from the package version lets documentation/runtime versions move
  without implying contract churn.

## Stages shaped

- **Stage 2/3** (contract surfacing) attaches explicit contract versions to the
  consumer-facing contracts.
- **Stage 4+** adds the *enforcement* check (mismatch diagnostic) to
  generated-product validation. **Enforcement is explicitly deferred** from
  feature 039 (FR-011); this ADR records the policy only.

## Verification in feature 039

Policy recorded only — no enforcement code added (FR-011). No existing
generated-product validation behaviour is changed.
