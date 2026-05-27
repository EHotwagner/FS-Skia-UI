# Evidence Policy

Task: T003

Risk levels:

- Small checks: isolated documentation, guidance text, and test-only changes.
- Medium checks: package contract, package implementation, generated template, and generated project behavior changes.
- Broad checks: public contracts, generated defaults, package surface, and governance behavior changes.

Required verification:

- Small checks use the narrowest relevant file or governance test plus task graph refresh.
- Medium checks use targeted package or generated-product tests plus `PackageSurfaceCheck`, `TemplateCheck`, or `GeneratedGuidanceCheck` as applicable.
- Broad checks use `./fake.sh build -t Verify` plus `EvidenceGraph` and `EvidenceAudit`.

Aggregate rule:

- Aggregate build output is non-authoritative unless backed by named readiness artifacts, command logs, or package/test evidence files.

