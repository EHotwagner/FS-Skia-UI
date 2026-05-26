# Governance Risk Levels

This feature is a medium governance-contract change for focused validation and
a broad readiness change only when final aggregate verification is required.

- small: copy and wording changes use targeted guidance tests.
- medium: evidence script behavior uses focused governance tests plus fixture
  runs for accepted, late, and non-eligible `[SEH]` cases.
- broad: `Verify` is required only if shared aggregate readiness, package
  outputs, or runtime package surfaces change.

Required evidence is captured in the named readiness files for generated
guidance, evidence graph, evidence audit, classification examples, and audit
fixtures. Broad validation results, when run, are supporting and
non-authoritative aggregate evidence compared to focused gates.
