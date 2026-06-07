---
title: ADR 0011 — Parity-oracle method
category: Design history
categoryindex: 90
---

# ADR 0011 — Parity-oracle method

- **Status**: Accepted
- **Date**: 2026-06-02
- **Decision source**: the V3 modular-distribution implementation plan
  (`docs/reports/2026-06-02-v3-modular-distribution-implementation-plan.md`, §0.4 + Stages 1, 4)
  and feature `048-v3-retirement-baseline` (research.md D4/D8).

## Context

The keystone move (ADR 0007/0008) retypes the host onto the canonical `Scene` vocabulary
and deletes the conversion shim. That move must be provably **behaviour-preserving**, but
the only rendering path is Vulkan, and headless rendering is environment-sensitive (the
known `SkiaViewer.Tests` libdecor-gtk crash). A screenshot-only gate would be flaky and
attribute environment drift to regression.

## Decision

**The host-move merge gate is byte-identical deterministic scene-output (authoritative)
plus reference screenshots (corroboration only).** Scene-output is a fixed, versioned
textual encoding of the host's `Scene` value (`format: scene-output/v1`: ordered element
kinds from `Scene.describe`, `Scene.diagnostics`, and `Scene.renderReadbackEvidence`'s
environment-independent `DeterministicHash`); it must re-derive **byte-identically** across
the move (0-byte diff). Reference screenshots corroborate but never gate. The capture
environment is recorded so a screenshot mismatch is attributable to environment, not
regression.

## Alternatives considered

- **Screenshots as the primary gate (rejected).** Headless/GPU flake makes pixel equality
  unreliable; it cannot distinguish environment drift from a real regression.
- **Re-use the existing `tests/Parity.Tests` upstream-Skia report (rejected).** That harness
  compares against an upstream Skia commit SHA, not host-vs-host; it answers a different
  question and retires in Stage 4.

## Rationale

Deterministic scene-output is reproducible and environment-independent, so it is a sound
merge gate; screenshots add human-visible corroboration without flakiness risk. The Stage-0
goldens (`tests/Parity.Tests/fixtures/v3-host-golden/`) become the comparison oracle the
keystone move is checked against.

## Affected stages

- **Stage 1** (keystone): the captured goldens gate the host move (scene-output must
  re-derive byte-identically after retyping onto `Scene`).
- **Stage 4**: the historical upstream-Skia `Parity.Tests` bridge (and `ParityGallery`) retire,
  leaving this host-vs-host oracle as the parity authority.
