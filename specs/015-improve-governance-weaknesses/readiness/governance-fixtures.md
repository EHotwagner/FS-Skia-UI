# Governance Fixture Locations

The feature uses focused parser, audit, and documentation fixtures rather than
runtime product fixtures.

| Area | Location | Purpose |
|------|----------|---------|
| Task skill evidence | `tests/Governance.Tests/GovernanceEvidenceTests.fs` | Validate accepted and rejected skill-loading evidence records |
| Skill-match assessment | `tests/Governance.Tests/GovernanceEvidenceTests.fs` | Validate confidence, matched signals, ambiguity, and reviewer disposition examples |
| Risk-level evidence | `tests/Governance.Tests/GovernanceEvidenceTests.fs` | Validate small, medium, and broad required evidence paths |
| Aggregate timeout verdict | `tests/Governance.Tests/GovernanceEvidenceTests.fs` | Validate timeout, focused rerun, and non-authoritative aggregate classifications |
| Runtime limitation docs | `tests/Governance.Tests/GovernanceEvidenceTests.fs` | Validate platform, renderer, dependency, fallback, and toolchain boundaries |

