# Governance Risk Levels

Status: PASS

This feature is Tier 1 governance work because it changes validation routing,
agent verdicts, generated evidence policy, controls contracts, and the build
target command surface.

## Risk Levels

- small: docs-only, metadata-only, or single-report wording updates.
- medium: generated template behavior, validation contract parsing, target
  metadata, focused gate reporting, and package surface checks.
- broad: native FAKE target migration, command aggregation, evidence audit
  behavior, and final readiness workflows.

## Required Evidence

- small changes require the focused gate named by `validation.contract.yml`
  plus `EvidenceGraph`.
- medium changes require the focused gate set, `EvidenceGraph`, and
  `EvidenceAudit` when audit-owned evidence changes.
- broad changes require focused evidence plus broad validation through
  `Verify` or `Ci`, unless the aggregate is explicitly classified as
  non-authoritative with focused rerun evidence.

## Broad Validation

Broad validation is triggered by target graph changes, package surface changes,
generated template behavior changes, and evidence policy changes. A broad
aggregate failure is not product evidence unless the verdict classifies it as
authoritative product failure.
