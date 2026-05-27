# Governance Risk Levels

## Classification

This feature is broad Tier 1. It changes public runtime behavior, generated templates, package validation, readiness artifacts, and audit governance.

## Levels

- small: isolated documentation or readiness text with no runtime surface change.
- medium: one implementation area, one public contract, or one generated workflow.
- broad: public API, package, generated product, and supported-host runtime behavior changed together.

## Required Evidence

Required evidence includes focused SkiaViewer tests, generated product validation, package restore and exact package evidence, visible-window supported-host evidence, image evidence decodability, and graph/audit readiness records.

## Broad Validation

Broad validation requires `./fake.sh build -t Verify` plus `EvidenceGraph` and `EvidenceAudit`. When `Verify` cannot produce an authoritative aggregate verdict, the failure class and focused rerun evidence must be recorded as non-authoritative aggregate evidence.
