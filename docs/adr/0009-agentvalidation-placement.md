---
title: ADR 0009 — `AgentValidation` placement
category: Design history
categoryindex: 90
---

# ADR 0009 — `AgentValidation` placement

- **Status**: Accepted
- **Date**: 2026-06-02
- **Decision source**: the V3 modular-distribution implementation plan
  (`docs/reports/2026-06-02-v3-modular-distribution-implementation-plan.md`, §0.4 + Stage 2)
  and feature `048-v3-retirement-baseline` (research.md D8).

## Context

`src/Lib/AgentValidation.fs(i)` (835 + 261 LOC) is a **governance/agent-contract parser**
that lives inside the runtime monolith for no runtime reason — it parses and validates
agent-facing contracts, which is build/governance concern, not rendering. The
before-baseline (`docs/reports/_baselines/2026-06-02-v3-before.md` §6) confirms the
build front-end (`build/Governance/Front/Support.fs`) **no longer** consumes it (the
governance library already owns `CapabilityRow`/`ValidationFinding`); the remaining
consumer is `tests/Governance.Tests/AgentValidationFrameworkTests.fs`. So the surface is a
governance artifact stranded in the runtime monolith.

## Decision

**`AgentValidation` moves into the `FS.Skia.UI.Build` governance library.** It leaves the
runtime monolith entirely; its test (`AgentValidationFrameworkTests`) repoints onto the
governance-library home. No runtime package gains or keeps an agent-validation surface.

## Alternatives considered

- **Leave `AgentValidation` in a surviving runtime split package (rejected).** It is not a
  rendering concern; placing it in `Scene`/`SkiaViewer` would smuggle governance code into
  the runtime distribution.
- **Delete it outright (rejected).** It is still exercised by a governance test and is a
  real contract-validation surface; relocation, not deletion, is correct.

## Rationale

Governance code belongs in the governance library (`FS.Skia.UI.Build`, ADR 0001).
Relocating `AgentValidation` removes ~1,096 LOC from the monolith and is independent of the
host move, so it can proceed in parallel after the keystone.

## Affected stages

- **Stage 2**: relocate `AgentValidation` into `FS.Skia.UI.Build`; repoint its test.
