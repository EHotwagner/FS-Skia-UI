# Governance Risk Levels

Status: complete for focused guidance implementation.

- Small: single guidance edits with focused scanner evidence.
- Medium: generated-template guidance or scanner changes that require generated
  guidance validation.
- Broad: `Verify` or generated package output evidence, recorded separately
  from focused command-order evidence.

Required evidence: each changed guidance class must have focused scanner or
FAKE target evidence. Broad validation is required when generated package
output or final aggregate readiness is claimed.

Feature scope: Tier 2 governance/docs change. No package identity changes,
public F# API changes, runtime UI/rendering changes, or MVU/effect boundary
changes were introduced.

Applied risk levels:

| Area | Risk | Evidence |
|------|------|----------|
| Repository docs and agent instructions | Small | `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --no-restore` |
| Generated-template guidance and scanner changes | Medium | `SequentialFakeGuidanceTests`, `GeneratedGuidanceCheck` scanner updates in `build.fsx` |
| Broad aggregate `Verify` | Broad | Optional final aggregate only; not a substitute for ordered focused logs |
