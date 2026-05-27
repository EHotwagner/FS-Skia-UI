# Governance Risk Levels

status=ok

Risk policy applied for this feature:

- isolated tests/docs: targeted test or guidance check is enough
- small changes: targeted semantic tests and a focused readiness artifact
- medium changes: generated/template checks plus package-specific tests
- public package surface changes: package surface baseline refresh plus
  `PackageSurfaceCheck`
- generated template changes: `TemplateCheck`, `GeneratedGuidanceCheck`, and
  generated product validation
- required evidence: named readiness files, logs, FSI transcripts, and generated
  product command output for each affected user story
- broad validation: `./fake.sh build -t Verify`
- broad cross-package/generated default changes: `Verify`, `EvidenceGraph`, and
  `EvidenceAudit`

Aggregate logs are non-authoritative unless backed by the named readiness
artifacts in this directory.
