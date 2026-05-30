# Governance Risk Levels

Risk level for this feature: medium governance risk.

Small evidence would be a narrow documentation-only check. Medium evidence is required here because the validator script, generated guidance, generated product command labels, and governance tests changed. Broad validation is required after shared task templates or generated product build command surfaces change.

Required evidence for this feature:

- Focused governance tests for validator behavior, guidance coverage, registry diagnostics, and graph-only output labels.
- Direct graph validation for `specs/033-fix-task-validator-feedback`.
- Sequential FAKE-backed `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, and `EvidenceAudit` runs.

Broad validation is recorded as non-authoritative aggregate evidence until the focused readiness files in this directory are refreshed.
