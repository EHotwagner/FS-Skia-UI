---
title: ADR 0010 — Legacy-sample policy
---

# ADR 0010 — Legacy-sample policy

- **Status**: Accepted
- **Date**: 2026-06-02
- **Decision source**: the V3 modular-distribution implementation plan
  (`docs/reports/2026-06-02-v3-modular-distribution-implementation-plan.md`, §0.4 + Stage 3)
  and feature `048-v3-retirement-baseline` (research.md D8).

## Context

Six samples consume the monolith at the pin (before-baseline §6): `BasicViewer`,
`DemoReel`, `EffectsGallery`, `InteractiveViewer`, `ParityGallery`, `ScreenshotGallery`.
The other galleries (`ChartsGallery`, `ControlsGallery`, `DataGridGallery`,
`KeyboardInputGallery`, `LayoutGraphGallery`) already consume only split packages.
`ParityGallery` exists specifically to drive the historical upstream-Skia parity bridge.

## Decision

**Repoint the monolith samples onto the split packages, or move them into an opt-in
sample-pack.** Concretely: the demonstrative viewer samples (`BasicViewer`,
`EffectsGallery`, `ScreenshotGallery`, `InteractiveViewer`, `DemoReel`) repoint onto
`FS.Skia.UI.SkiaViewer` + `FS.Skia.UI.Scene` (the host package, post-ADR-0007). They may
be grouped under an **opt-in sample-pack** rather than shipped by default.
**`ParityGallery` retires together with the parity bridge** (it has no role once the
upstream-Skia comparison is gone — Stage 4).

## Alternatives considered

- **Keep the samples on the monolith and ship the monolith for samples only (rejected).**
  Keeps the monolith alive purely for demos; contradicts the retirement.
- **Delete all monolith samples (rejected).** The viewer samples are useful documentation
  of the host API; repointing preserves their value at low cost.

## Rationale

Samples must demonstrate the **shipped** split packages, not a retiring monolith. Repointing
(or opt-in packaging) keeps the demonstrations while removing the last non-test monolith
consumers; `ParityGallery` is the one sample with no post-retirement purpose.

## Affected stages

- **Stage 3**: repoint the monolith samples onto split packages / opt-in sample-pack.
- **Stage 4**: `ParityGallery` retires with the parity bridge.
