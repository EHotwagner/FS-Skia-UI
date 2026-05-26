# Governance Risk Levels

Status: scaffolded. This feature is expected to finish as `medium` risk unless
build-orchestration changes require broad validation.

## Scope

- Tier: Tier 1 governance contract change.
- Public F# API impact: none expected.
- Package identity/content/version impact: none expected.
- Product MVU/runtime state impact: not applicable.
- Runtime support expansion: out of scope.
- Real evidence paths: `skill-loading-evidence.md`,
  `skill-detection-calibration.md`, `governance-risk-levels.md`,
  `aggregate-hang-diagnostics.md`, `runtime-limitations.md`,
  `evidence-graph.md`, and `evidence-audit.md`.

## Required Evidence Paths

| Risk level | Minimum evidence |
|------------|------------------|
| small | Relevant documentation or metadata check plus `EvidenceGraph` when tasks change |
| medium | `EvidenceGraph`, `EvidenceAudit`, `GeneratedGuidanceCheck` when guidance changes, and focused governance tests |
| broad | Medium evidence plus `Dev` or documented aggregate timeout verdict with focused rerun evidence |

## Representative Classification

This feature is `medium` for readiness while it changes evidence parser/audit
contracts, implementation guidance, generated governance guidance, and focused
build reporting. Broad validation is required only if final changes alter
runtime behavior, generated product output, package contents, public contracts,
or aggregate build orchestration in a way that cannot be covered by focused
governance checks.

## This Feature Evidence Selection

governance_risk:
  level: medium
  scope_signals:
    - evidence graph parser changed
    - evidence audit runner changed
    - generated guidance changed
    - readiness documentation changed
  required_checks:
    - ./fake.sh build -t EvidenceGraph
    - ./fake.sh build -t EvidenceAudit
    - ./fake.sh build -t GeneratedGuidanceCheck
    - dotnet test tests/Governance.Tests/Governance.Tests.fsproj -m:1 --filter governance
  broad_required: false
  rationale: Focused governance contracts changed, but no runtime product,
    package identity, package contents, or public F# API surface changed.
  non_authoritative_results:
    - Aggregate Dev results are not required for medium risk unless a final
      build-orchestration change makes the feature broad.
